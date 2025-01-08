using PlatformService.DTOs;

namespace PlatformService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        Task PublishNewPlatformAsync(PlatformPublishedDto platformPublishedDto);
    }
}
