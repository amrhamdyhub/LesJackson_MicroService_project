namespace PlatformService.CustomeResponse
{
    public class ApiResponse<T>
    {
        public T Data { get; set; } = default!;

        public bool IsSuccess { get; set; }

        public List<string> Errors { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }
    }
}
