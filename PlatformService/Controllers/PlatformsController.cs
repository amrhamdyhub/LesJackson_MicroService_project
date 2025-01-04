﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServcies.Http;

namespace PlatformService.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class PlatformsController : Controller
    {
        private readonly IPlatformRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet()]
        // GET /api/platforms
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            var platforms = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {

            var platform = _repository.GetPlatformById(id);
            if (platform != null)
            {

                return Ok(_mapper.Map<PlatformReadDto>(platform));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                var response = await _commandDataClient.SendPlatformToCommand(platformReadDto);
                var responseMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"--> Command Service Response: {responseMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send platform to Command service: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }

    }
}
