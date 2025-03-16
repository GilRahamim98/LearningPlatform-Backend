namespace Talent;

public class LogCleanupService : BackgroundService
{
    private readonly ILogger<LogCleanupService> _logger;

    // Currently deleting from the solution directory, can be changed to AppDomain.CurrentDomain.BaseDirectory for the build output directory
    private readonly string _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

    public LogCleanupService(ILogger<LogCleanupService> logger)
    {
        _logger = logger;
    }

    // Main execution method for the background service
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Perform log cleanup
                CleanupLogs();
            }
            catch (Exception ex)
            {
                // Log any errors that occur during log cleanup
                _logger.LogError(ex, "An error occurred while deleting old logs.");
            }

            // Wait for 30 days before running again
            await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
        }
    }

    // Method to clean up old log files
    private void CleanupLogs()
    {
        if (!Directory.Exists(_logDirectory))
        {
            _logger.LogInformation("Log directory does not exist. Skipping cleanup.");
            return;
        }

        // Get all log files in the directory
        string[] files = Directory.GetFiles(_logDirectory, "*.txt");
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            // Delete logs older than 30 days
            if (fileInfo.CreationTime < DateTime.Now.AddDays(-30))
            {
                fileInfo.Delete();
                _logger.LogInformation($"Deleted log file: {fileInfo.Name}");
            }
        }
    }
}
