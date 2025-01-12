
using FluentResults;

namespace PlatformService.CustomeResponse
{
    public static class ApiResponseExtensions
    {
        public static ApiResponse<T> ToApiResponse<T>(this Result<T> result)
        {
            var apiResponse = new ApiResponse<T>
            {
                Data = result.IsSuccess ? result.Value : default,
                IsSuccess = result.IsSuccess,
                Errors = result.Errors.Select(e => e.Message).ToList()
            };
           
            return apiResponse;
        }

    }
}
