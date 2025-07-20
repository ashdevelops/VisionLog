using CliWrap;
using Microsoft.Extensions.Hosting;

namespace VisionLog;

public class IpCameraSource(
    string name,
    string url,
    int segment) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var saveDirectory = $"/mnt/backups/recordings/{name}";
            Directory.CreateDirectory(saveDirectory);

            var cmd = Cli.Wrap("ffmpeg")
                .WithWorkingDirectory(saveDirectory)
                .WithArguments([
                    "-i", url,
                    "-c:v", "copy",
                    "-c:a", "aac",
                    "-f", "segment",
                    "-segment_time", $"{segment}",
                    "-strftime", "1",
                    "%Y-%m-%d_%H-%M-%S.mkv"
                ])
                .WithValidation(CommandResultValidation.None);


            Console.WriteLine(cmd);
            await cmd.ExecuteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}