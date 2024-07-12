﻿using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Botticelli.Bot.Interfaces.Processors;
using Botticelli.Client.Analytics.Extensions;
using Botticelli.Framework.Commands;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddBotticelliFramework(this IServiceCollection services, IConfiguration config) =>
        services.AddSingleton<ClientProcessorFactory>()
                .AddSingleton<CommandProcessorFactory>()
                .AddSharedValidation()
                .AddMetrics(config);

    public static IServiceCollection AddBotCommand<TCommand,
        TCommandProcessor,
        TCommandValidator>(this IServiceCollection services)
        where TCommand : class, ICommand
        where TCommandProcessor : class, ICommandProcessor
        where TCommandValidator : class, ICommandValidator<TCommand>
        => services.AddScoped<TCommand>()
            .AddScoped<TCommandProcessor>()
            .AddScoped<ICommandValidator<TCommand>, TCommandValidator>();

    public static CommandChainProcessorBuilder<TCommand> AddBotChainProcessedCommand<TCommand,
                                                                                     TCommandValidator>(this IServiceCollection services)
            where TCommand : class, ICommand where TCommandValidator : class, ICommandValidator<TCommand>
    {
        var builder = new CommandChainProcessorBuilder<TCommand>(services);
        
        services.AddScoped<TCommand>()
                .AddScoped<ICommandValidator<TCommand>, TCommandValidator>()
                .AddScoped(_ => builder);
        
      return builder;
    }

    public static IServiceProvider RegisterBotChainedCommand<TCommand, TBot>(this IServiceProvider sp)
            where TCommand : class, ICommand
            where TBot : IBot<TBot>
    {
        var commandChainProcessorBuilder = sp.GetRequiredService<CommandChainProcessorBuilder<TCommand>>();
        var processor = commandChainProcessorBuilder.Build();
        
        var clientProcessorFactory = sp.GetRequiredService<ClientProcessorFactory>();
        
        clientProcessorFactory.AddProcessor<TBot>(sp, processor);
      
        return sp;
    }

    public static IServiceProvider RegisterBotCommand<TCommand, TCommandProcessor, TBot>(this IServiceProvider sp)
        where TCommand : class, ICommand
        where TCommandProcessor : class, ICommandProcessor
        where TBot : IBot<TBot>
    {
        var commandProcessorFactory = sp.GetRequiredService<CommandProcessorFactory>();
        var clientProcessorFactory = sp.GetRequiredService<ClientProcessorFactory>();

        commandProcessorFactory.AddCommandType(typeof(TCommand), typeof(TCommandProcessor));
        clientProcessorFactory.AddProcessor<TCommandProcessor, TBot>(sp);

        return sp;
    }
    
    public static IServiceProvider RegisterFluentBotCommand<TCommand, TCommandProcessor, TBot>(this IServiceProvider sp)
        where TCommand : class, ICommand
        where TCommandProcessor : class, ICommandProcessor
        where TBot : IBot<TBot>
    {
        var commandProcessorFactory = sp.GetRequiredService<CommandProcessorFactory>();
        var clientProcessorFactory = sp.GetRequiredService<ClientProcessorFactory>();

        commandProcessorFactory.AddCommandType(typeof(TCommand), typeof(TCommandProcessor));
        clientProcessorFactory.AddProcessor<TCommandProcessor, TBot>(sp);

        return sp;
    }

    public static IHttpClientBuilder AddCertificates(this IHttpClientBuilder builder, BotSettings settings) =>
            builder.ConfigurePrimaryHttpMessageHandler(() =>
            {
                var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);

                var certificate = store.Certificates
                                       .FirstOrDefault(c => c.FriendlyName == settings.BotCertificateName);

                if (certificate == null) throw new NullReferenceException("Can't find a client certificate!");
                
                return new HttpClientHandler
                {
                    ClientCertificates = { certificate },
                    ServerCertificateCustomValidationCallback =
                            (_, _, _, policyErrors) =>
                            {
                                #if DEBUG
                                return true;
                                #endif
                                return policyErrors == SslPolicyErrors.None;
                                // TODO: cert checking
                            }
                };
            });
}