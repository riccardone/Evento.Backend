using System.Text;
using System.Text.Json;

namespace Evento.Cli.Commands
{
    public class HttpHelper
    {
        private readonly string _hostAddress;
        private readonly HttpClient _client = new();

        public HttpHelper(string hostAddress)
        {
            _hostAddress = hostAddress;
        }

        public async Task<string> Post<T>(string path, T data)
        {
            var json = JsonSerializer.Serialize(data);
            var responseMessage = await _client.PostAsync(_hostAddress + path,
                new StringContent(json, Encoding.UTF8, "application/json"));
            var response = await responseMessage.Content.ReadAsStringAsync();
            if (responseMessage.IsSuccessStatusCode)
                return response;
            throw new Exception(response);
        }
    }
}
