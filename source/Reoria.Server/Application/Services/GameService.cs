using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Application.Interfaces;
using System.Diagnostics;

namespace Reoria.Server.Application.Services;

public class GameService : IApplicationCoreService
{
    private readonly ILogger<GameService> logger;
    private readonly IConfiguration configuration;
    private readonly IApplicationCore app;
    private readonly int targetFrameRate;
    private readonly TimeSpan targetFrameTime;
    private bool isRunning = false;

    public GameService(ILogger<GameService> logger, IConfiguration configuration, IApplicationCore app)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.app = app;
        this.targetFrameRate = this.configuration.GetValue("Game:Framerate", 20);
        this.targetFrameTime = TimeSpan.FromSeconds(1.0 / targetFrameRate);
    }

    public void Start()
    {
        string serverName = this.configuration["Game:Name"] ?? string.Empty;
        int ticks = this.configuration.GetValue("Server:Ticks", 5);

        logger.LogInformation("Starting {serverName} game server...", serverName);

        isRunning = true;

        var stopwatch = Stopwatch.StartNew();
        var previousFrameTime = stopwatch.Elapsed;

        while (isRunning)
        {
            var currentTime = stopwatch.Elapsed;
            var elapsedTime = currentTime - previousFrameTime;

            if (elapsedTime >= targetFrameTime)
            {
                logger.LogInformation("The server has ticked, the time elapsed was {delta} milliseconds, expected {expectedDelta} milliseconds...",
                    (currentTime - previousFrameTime).TotalMilliseconds,
                    targetFrameTime.TotalMilliseconds);
                previousFrameTime = currentTime;
                ticks--;

                if (ticks <= 0) { app.Stop(); }
            }
            else
            {
                Thread.Sleep(targetFrameTime - elapsedTime);
            }
        }

        logger.LogInformation("Shutting down {serverName} game server...{n}", serverName, Environment.NewLine);
    }

    public void Stop()
    {
        isRunning = false;
    }
}
