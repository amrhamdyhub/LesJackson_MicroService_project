namespace PlatformService.CustomeResponse
{
    public class Error
    {
        public string Message { get; }
        public Error(string message)
        {
            Message = message;
        }


        public static Error None => new(string.Empty);

        public static implicit operator Error(string message) => new(message);

        public static implicit operator string(Error error) => error.Message;
    }
}
