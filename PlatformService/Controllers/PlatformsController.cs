using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.Services;
using PlatformService.CustomeResponse;
using PlatformService.SyncDataServcies.Http;
using FluentResults;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : Controller
    {
        private readonly IplatformServices _platformService;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IplatformServices platformRepository, IMapper mapper, ICommandDataClient commandDataClient
            , IMessageBusClient messageBusClient)
        {
            _platformService = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        //[HttpGet()]
        //// GET /api/platforms
        //public IActionResult GetAllPlatforms()
        //{
        //    var platforms = _repository.GetAllPlatforms();
        //    var platformReadDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
        //    return Success(platformReadDtos);
        //    //return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        //}


        [HttpGet()]
        // GET /api/platforms
        public ActionResult<List<PlatformReadDto>> GetAllPlatforms()
        {
            var result = _platformService.GetAllPlatforms();
            //return result.IsSuccess ? Ok(result) : NotFound(result.Error);
            return result.IsSuccess ? Ok(result.ToApiResponse()) : NotFound(result.ToApiResponse());
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type =typeof(Result<PlatformReadDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPlatformById(int id)
        {
            var result = _platformService.GetPlatformById(id);
            return result.IsSuccess ? Ok(result.ToApiResponse()) : NotFound(result.ToApiResponse());

        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PlatformReadDto>>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var result = _platformService.CreatePlatform(platformCreateDto);

            //if (!result.IsSuccess || result.Data == null)
            if (!result.IsSuccess || result.Value == null)
            {
                return BadRequest(result.Errors);
            }
            // send sync message
            try
            {
                var response = await _commandDataClient.SendPlatformToCommand(result.Value);
                var responseMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"--> Command Service Response: {responseMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send platform to Command service: {ex.Message}");
            }

            //Send Async Message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(result.Value);
                platformPublishedDto.Event = "Platform_Published";
                await _messageBusClient.PublishNewPlatformAsync(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = result.Value.Id }, result.ToApiResponse());
        }

    }
}
