using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Reoria.Application.Interfaces;

public interface IApplicationCore : IDisposable
{
    IApplicationCore ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate);
    IApplicationCore ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);
    IApplicationCore Start<TService>(params object[] parameters) where TService : IApplicationService;
    IApplicationCore Stop();
}
