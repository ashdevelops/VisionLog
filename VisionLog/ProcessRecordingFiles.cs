using Microsoft.Extensions.Hosting;

namespace VisionLog
{
    public class ProcessRecordingFiles : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var directory in Directory.GetDirectories("/mnt/backups/recordings"))
                {
                    if (directory.EndsWith("/archived"))
                    {
                        continue;
                    }
                    
                    ProcessFiles(directory, 90, 3);
                }
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
                    var parentDirectoryName = Directory.GetParent(file)?.Name;

                    if (parentDirectoryName != null)
                    {
                        var archiveDir = Path.Combine(path, "archived", parentDirectoryName, "video");

                        Directory.CreateDirectory(archiveDir);

                        var destFile = Path.Combine(archiveDir, Path.GetFileName(file));

                        if (!File.Exists(destFile))
                        {
                            File.Move(file, destFile);
                            Console.WriteLine($"Moved {file} to {destFile}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Could not get the parent directory for {file}");
                    }
                }
            }
        }
    }
}