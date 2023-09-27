using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reoria.Application.Interfaces;
using Reoria.Server.Application;
using Reoria.Server.Application.Services;

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
.Start<GameService>();