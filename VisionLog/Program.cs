// See https://aka.ms/new-console-template for more information

using IpCameraRecorder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VisionLog;


await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<ProcessRecordingFiles>();
        
        var monitorConfigs = context.Configuration.GetSection("MonitorConfigs").Get<List<MonitorConfig>>();

        if (monitorConfigs == null)
        {
            throw new Exception("Missing configuration section 'MonitorConfigs'");
        }

        foreach (var monitor in monitorConfigs)
        {
            services.AddSingleton<IHostedService>(sp => 
                new CameraRecorder(monitor.Name, monitor.RtspUrl, TimeSpan.FromMinutes(monitor.Segment)));
        }
    })
    .RunConsoleAsync();