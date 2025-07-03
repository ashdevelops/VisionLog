using Microsoft.Extensions.Hosting;

namespace IpCameraRecorder;

public class CameraRecorder(string name, string url, TimeSpan segmentDuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var camera = new IpCameraSource(name, url, segmentDuration);
        await camera.StartAsync(stoppingToken);
    }
}