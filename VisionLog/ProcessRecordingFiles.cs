using Microsoft.Extensions.Hosting;

namespace IpCameraRecorder;

public class ProcessRecordingFiles : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ProcessFiles("/mnt/backups", 90, 3);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private static void ProcessFiles(string path, int deleteOlderThanDays, int archiveOlderThanDays)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.mkv", SearchOption.AllDirectories))
        {
            var age = DateTime.Now - File.GetLastWriteTime(file);

            if (age.TotalDays > deleteOlderThanDays)
            {
                try
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted: {file}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete {file}: {ex.Message}");
                }
            }
            else if (age.TotalHours > archiveOlderThanDays * 24)
            {
                var archiveDate = File.GetLastWriteTime(file).ToString("dd-MM-yyyy");
                var archiveDir = Path.Combine(path, "archived", archiveDate);
                    
                Directory.CreateDirectory(archiveDir);

                var destFile = Path.Combine(archiveDir, Path.GetFileName(file));

                if (!File.Exists(destFile))
                {
                    File.Move(file, destFile);
                }
            }
        }
    }
}