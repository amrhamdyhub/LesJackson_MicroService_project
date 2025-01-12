using AutoMapper;
using FluentResults;
using PlatformService.CustomeResponse;
using PlatformService.DTOs;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishedDto>();

            //Map between Result and ApiResponse
            //CreateMap(typeof(Result<>), typeof(ApiResponse<>))
            //.ForMember("Data", opt => opt.MapFrom((src, dest, destMember, context) =>
            //{
            //    var isSuccess = (bool)context.Items["IsSuccess"];
            //    return isSuccess ? context.Items["Value"] : default;
            //}))
            //.ForMember("IsSuccess", opt => opt.MapFrom((src, dest, destMember, context) =>
            //{
            //    return (bool)context.Items["IsSuccess"];
            //}))
            //.ForMember("Errors", opt => opt.MapFrom((src, dest, destMember, context) =>
            //{
            //    var isSuccess = (bool)context.Items["IsSuccess"];
            //    return isSuccess ? null : ((Result)context.Items["Errors"]).Errors.Select(e => e.Message).ToList();
            //}));
        }
    }
}
