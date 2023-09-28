using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reoria.Application.Interfaces;

namespace Reoria.Application;

public abstract class ApplicationCore : IApplicationCore
{
    protected readonly string[] args;
    protected readonly IHostBuilder host;
    private IApplicationService? service;
    private bool disposedValue;

    public ApplicationCore()
    {
        this.args = Array.Empty<string>();
        this.host = CreateHostBuilder();
    }

    public ApplicationCore(string[] args)
    {
        this.args = args;
        this.host = CreateHostBuilder();
    }

    ~ApplicationCore()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {

            }

            disposedValue = true;
        }
    }

    public virtual IApplicationCore Start<TService>(params object[] parameters) where TService : IApplicationService
    {
        if (this.service is not null) { return this; }

        var host = this.host.Build();

        (this.service = ActivatorUtilities.CreateInstance<TService>(host.Services, parameters)).Start();

        return this;
    }

    public virtual IApplicationCore Stop()
    {
        if(this.service is null) { return this; }

        this.service?.Stop();
        this.service = null;

        return this;
    }

    protected virtual IHostBuilder CreateHostBuilder(string[]? args = null)
    {
        if (args is null) { return Host.CreateDefaultBuilder(); }

        return Host.CreateDefaultBuilder(args);
    }

    public virtual IApplicationCore ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        if (this.service is not null) { return this; }

        this.host.ConfigureServices(configureDelegate);

        return this;
    }

    public virtual IApplicationCore ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        if (this.service is not null) { return this; }

        this.host.ConfigureAppConfiguration(configureDelegate);

        return this;
    }

    public virtual IApplicationCore ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configureDelegate)
    {
        if (this.service is not null) { return this; }

        this.host.ConfigureLogging(configureDelegate);

        return this;
    }
}
