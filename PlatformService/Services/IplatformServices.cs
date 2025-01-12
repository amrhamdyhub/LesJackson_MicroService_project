using FluentResults;
using PlatformService.DTOs;
//using PlatformService.Result;

namespace PlatformService.Services
{
    public interface IplatformServices
    {
        public Result<List<PlatformReadDto>> GetAllPlatforms();

        public Result<PlatformReadDto> GetPlatformById(int id);

        public Result<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto);
    }
}
