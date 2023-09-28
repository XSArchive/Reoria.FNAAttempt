using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reoria.Application.Interfaces;
using Reoria.Server.Application;
using Reoria.Server.Application.Services;
using Serilog;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

using var server = new ServerApplication(args);

server.ConfigureServices((context, services) =>
{
    services.AddSingleton<IApplicationService, GameService>();
    services.AddSingleton<IApplicationCore>(server);
})
.ConfigureAppConfiguration((context, configuration) =>
{
    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
})
.ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
    logging.AddSerilog(logger, true);
})
.Start<GameService>();