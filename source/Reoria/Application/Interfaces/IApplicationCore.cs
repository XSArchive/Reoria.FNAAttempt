﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reoria.Application.Enumerations;

namespace Reoria.Application.Interfaces;

public interface IApplicationCore : IDisposable
{
    ApplicationState State { get; }

    IApplicationCore ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate);
    IApplicationCore ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configureDelegate);
    IApplicationCore ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);
    IApplicationCore Start<TService>(params object[] parameters) where TService : class, IApplicationCoreService;
    IApplicationCore Stop();
}
