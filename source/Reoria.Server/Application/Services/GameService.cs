using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Application.Enumerations;
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
    private ApplicationServiceState state;

    public ApplicationServiceState State => this.state;

    public GameService(ILogger<GameService> logger, IConfiguration configuration, IApplicationCore app)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.app = app;
        this.targetFrameRate = this.configuration.GetValue("Game:Framerate", 20);
        this.targetFrameTime = TimeSpan.FromSeconds(1.0 / targetFrameRate);
        this.state = ApplicationServiceState.Constructed;
    }

    public void Start()
    {
        this.state = ApplicationServiceState.Starting;

        string serverName = this.configuration["Game:Name"] ?? string.Empty;
        int ticks = this.configuration.GetValue("Server:Ticks", 5);

        logger.LogInformation("Starting {serverName} game server...", serverName);

        var stopwatch = Stopwatch.StartNew();
        var previousFrameTime = stopwatch.Elapsed;

        while (this.state != ApplicationServiceState.Stopping)
        {
            switch (this.state)
            {
                case ApplicationServiceState.Starting:
                    this.state = ApplicationServiceState.Running;
                    continue;
                default:
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
                    break;
            }
        }

        logger.LogInformation("Shutting down {serverName} game server...{n}", serverName, Environment.NewLine);

        this.state = ApplicationServiceState.Stopped;
    }

    public void Stop()
    {
        this.state = ApplicationServiceState.Stopping;
    }
}
