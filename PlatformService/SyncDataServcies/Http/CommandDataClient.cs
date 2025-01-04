using PlatformService.DTOs;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataServcies.Http
{
    public class CommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpclient;
        private readonly IConfiguration _configuration;

        public CommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpclient = httpClient;
            _configuration = configuration;
            
        }
        public async Task<HttpResponseMessage> SendPlatformToCommand(PlatformReadDto platform)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(platform), Encoding.UTF8, "application/json");
            var response = await _httpclient.PostAsync($"{_configuration["CommandServiceHost"]}/api/commands/platforms/", content);

            if (response.IsSuccessStatusCode) 
            {
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
            }
            return response;
        }
    }
}
