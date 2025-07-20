using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VisionLog;

public class CameraRecorder(ILogger<CameraRecorder> logger, string name, string url, int segment) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"[CameraRecorder] ExecuteAsync started for {name}");

        var camera = new IpCameraSource(name, url, segment);
        await camera.StartAsync(stoppingToken);
    }
}