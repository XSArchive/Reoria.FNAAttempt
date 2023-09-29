using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reoria.Application.Interfaces;
using Serilog;
using Serilog.Core;

namespace Reoria.Application;

public abstract class ApplicationCore : IApplicationCore
{
    protected readonly string[] args;
    protected readonly IHostBuilder host;
    private readonly IConfigurationRoot config;
    private readonly Logger logger;
    private IApplicationService? service;
    private bool disposedValue;

    public Logger GetLogger() => this.logger;

    public ApplicationCore() : this(Array.Empty<string>()) { }

    public ApplicationCore(string[] args)
    {
        this.args = args;
        this.host = this.CreateHostBuilder(this.args);
        this.config = this.CreateConfiguration().Build();
        this.logger = this.CreateLogger().CreateLogger();
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

    protected virtual IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args);
    }

    protected virtual IConfigurationBuilder CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddCommandLine(this.args)
            .AddEnvironmentVariables();
    }

    protected virtual LoggerConfiguration CreateLogger()
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(this.config);
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
