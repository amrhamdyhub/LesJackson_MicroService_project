using PlatformService.DTOs;

namespace PlatformService.SyncDataServcies.Http
{
    public interface ICommandDataClient
    {
        Task<HttpResponseMessage> SendPlatformToCommand(PlatformReadDto platform);
    }
}
