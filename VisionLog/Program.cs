// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        var i = 0;
        
        foreach (var monitor in monitorConfigs)
        {
            Console.WriteLine($"Registering CameraRecorder #{++i} for {monitor.Name}");
            var m = monitor;
            
            services.AddHostedService(sp => 
                new CameraRecorder(
                    sp.GetRequiredService<ILogger<CameraRecorder>>(), 
                    m.Name, 
                    m.RtspUrl, 
                    m.Segment));
        }
    })
    .RunConsoleAsync();