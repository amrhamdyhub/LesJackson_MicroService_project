using AutoMapper;
using CommandsServices.Data;
using CommandsServices.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsServices.Controllers
{
    [Route("api/commands/[controller]")]
    [ApiController]
    public class PlatformsController : Controller
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms from CommandsService");

            var platformItems = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # Command Service");
            return Ok("The platform controller is working from Commands");
        }

    }
}
