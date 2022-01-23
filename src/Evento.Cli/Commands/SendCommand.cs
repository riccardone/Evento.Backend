using System.CommandLine;
using System.Text.Json;
using CloudEventData;

namespace Evento.Cli.Commands;

public class SendCommand : Command
{
    private readonly string[] _file = new string[] { "-f", "--file" };

    public SendCommand() : base("send", "Send messages to the remote api")
    {
        Option fileOption = new Option<FileInfo>(_file, "The file containing the json message");
        AddOption(fileOption);
        this.SetHandler(
            (FileInfo fi) =>
            {
                var request = BuilCloudRequest(fi.ToString());
                var httpHelper = new HttpHelper("");
                httpHelper.Post("/api/Messaging", request).Wait();
                Console.WriteLine("Message sent");
            },
            fileOption);
    }

    private static CloudEventRequest BuilCloudRequest(string payloadPath)
    {
        var request = File.ReadAllText(payloadPath);
        var req = JsonSerializer.Deserialize<CloudEventRequest>(request,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        req.Data = req.Data.ToString();
        return req;
    }
}