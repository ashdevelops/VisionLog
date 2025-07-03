using System.Globalization;
using CliWrap;
using Microsoft.Extensions.Hosting;

namespace IpCameraRecorder;

public class IpCameraSource(
    string name,
    string url,
    TimeSpan segment) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var saveDirectory = $"/mnt/backups/recordings/{name}";
        Directory.CreateDirectory(saveDirectory);
        
        await Cli.Wrap("ffmpeg")
            .WithWorkingDirectory(saveDirectory)
            .WithArguments([
                "-i", url,
                "-c:v", "copy",
                "-c:a", "aac",
                "-f", "segment",
                "-segment_time", segment.TotalSeconds.ToString(CultureInfo.InvariantCulture),
                "-strftime", "1",
                "%Y-%m-%d_%H-%M-%S.mkv"
            ])
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();
    }
}