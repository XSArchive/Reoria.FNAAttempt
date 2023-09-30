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
    private IHost? application;
    private bool disposedValue;

    public ApplicationCore() : this(Array.Empty<string>()) { }

    public ApplicationCore(string[] args)
    {
        this.args = args;
        this.config = this.CreateConfiguration().Build();
        this.logger = this.CreateLogger().CreateLogger();
        this.host = this.CreateHostBuilder(this.args).ConfigureServices((context, services) =>
        {
            services.AddSingleton<IApplicationCore>(this);
        }).ConfigureAppConfiguration((context, configuration) =>
        {
            configuration.AddConfiguration(this.config);
        })
        .ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders();
            logging.AddSerilog(this.logger, true);
        });
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

    public virtual IApplicationCore Start<TService>(params object[] parameters) where TService : class, IApplicationService
    {
        if (this.service is not null) { return this; }

        if (this.application is null)
        {
            this.host.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IApplicationService, TService>();
            });

            this.application = this.host.Build();
        }

        (this.service = ActivatorUtilities.CreateInstance<TService>(this.application.Services, parameters)).Start();

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
        this.host.ConfigureServices(configureDelegate);

        return this;
    }

    public virtual IApplicationCore ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        this.host.ConfigureAppConfiguration(configureDelegate);

        return this;
    }

    public virtual IApplicationCore ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configureDelegate)
    {
        this.host.ConfigureLogging(configureDelegate);

        return this;
    }
}
