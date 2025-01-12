using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using FluentResults;

namespace PlatformService.Services
{
    public class PlatformServices : IplatformServices
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public PlatformServices(IPlatformRepository platformRepository, IMapper mapper)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;

        }

        public Result<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _platformRepository.CreatePlatform(platformModel);
            _platformRepository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            //return Result<PlatformReadDto>.Success(platformReadDto);
            
            return FluentResults.Result.Ok(platformReadDto);
            
        }

        public Result<List<PlatformReadDto>> GetAllPlatforms()
        {
            var platforms = _platformRepository.GetAllPlatforms();
            var platformReadDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            //return Result<PlatformReadDto>.Success(platformReadDtos.ToList());
            return FluentResults.Result.Ok(platformReadDtos.ToList());
        }

        public Result<PlatformReadDto> GetPlatformById(int id)
        {
            var platform = _platformRepository.GetPlatformById(id);
            if (platform != null)
            {
                //return Result.Result.Success(_mapper.Map<PlatformReadDto>(platform));
                return FluentResults.Result.Ok(_mapper.Map<PlatformReadDto>(platform));
            }
            //return Result.Result.Failure<PlatformReadDto>($"Provided PlatformId not found: {id}");
            return FluentResults.Result.Fail<PlatformReadDto>($"Provided PlatformId not found: {id}");
        }
    }
}
