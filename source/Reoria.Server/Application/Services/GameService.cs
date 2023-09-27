using Microsoft.Extensions.Configuration;
using Reoria.Application.Interfaces;
using System.Diagnostics;

namespace Reoria.Server.Application.Services;

public class GameService : IApplicationService
{
    private readonly IConfiguration configuration;
    private readonly IApplicationCore app;
    private readonly int targetFrameRate;
    private readonly TimeSpan targetFrameTime;
    private bool isRunning = false;

    public GameService(IConfiguration configuration, IApplicationCore app)
    {
        this.configuration = configuration;
        this.app = app;
        this.targetFrameRate = this.configuration.GetValue("Server:Framerate", 20);
        this.targetFrameTime = TimeSpan.FromSeconds(1.0 / targetFrameRate);
    }

    public void Start()
    {
        string serverName = this.configuration["Game:Name"] ?? string.Empty;
        int ticks = this.configuration.GetValue("Server:Ticks", 5);

        Console.WriteLine($"Starting {serverName} server...");

        isRunning = true;

        var stopwatch = Stopwatch.StartNew();
        var previousFrameTime = stopwatch.Elapsed;

        while (isRunning)
        {
            var currentTime = stopwatch.Elapsed;
            var elapsedTime = currentTime - previousFrameTime;

            if (elapsedTime >= targetFrameTime)
            {
                Console.WriteLine($"The server has ticked, the time since the last tick was {(currentTime - previousFrameTime).TotalMilliseconds} milliseconds, expected to be {targetFrameTime.TotalMilliseconds} milliseconds...");
                previousFrameTime = currentTime;
                ticks--;

                if (ticks <= 0) { app.Stop(); }
            }
            else
            {
                Thread.Sleep(targetFrameTime - elapsedTime);
            }
        }

        Console.WriteLine($"Shutting down {serverName} game server...");
    }

    public void Stop()
    {
        isRunning = false;
    }
}
