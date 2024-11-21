using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Botticelli.Bot.Interfaces.Processors;
using Botticelli.Framework.Commands;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.HostedService;
using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Shared.Extensions;
using Botticelli.Shared.Utils;
using EasyCaching.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddBotticelliFramework(this IServiceCollection services) =>
        services.AddSingleton<ClientProcessorFactory>()
            .AddSharedValidation()
            .AddHostedService<BotHostedService>()
            .AddEasyCaching(options =>
            {
                options.UseInMemory(config =>
                {
                    config.DBConfig = new InMemoryCachingOptions
                    {
                        ExpirationScanFrequency = 10,
                        // total count of cache items, default value is 10000
                        SizeLimit = 5000
                    };
                    // the max random second will be added to cache's expiration, default value is 120
                    config.MaxRdSecond = 10;
                    // whether enable logging, default is false
                    config.EnableLogging = false;
                    // mutex key's alive time(ms), default is 5000
                    config.LockMs = 500;
                    // when mutex key alive, it will sleep some time, default is 300
                    config.SleepMs = 30;
                }, "botticelli_wait_for_response");
            });

    public static CommandAddServices<TCommand> AddBotCommand<TCommand>(this IServiceCollection services)
        where TCommand : class, ICommand
    {
        services.AddScoped<TCommand>()
            .AddSingleton<CommandAddServices<TCommand>>(_ => new CommandAddServices<TCommand>(services));

        return services.BuildServiceProvider().GetRequiredService<CommandAddServices<TCommand>>();
    }

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
            .AddSingleton(_ => builder);

        return builder;
    }

    public static IServiceProvider RegisterBotChainedCommand<TCommand, TBot>(this IServiceProvider sp)
        where TCommand : class, ICommand
        where TBot : IBot<TBot>
    {
        var commandChainProcessorBuilder = sp.GetRequiredService<CommandChainProcessorBuilder<TCommand>>();
        var processor = commandChainProcessorBuilder.Build();
        var clientProcessorFactory = sp.GetRequiredService<ClientProcessorFactory>();

        processor.NotNull();
        clientProcessorFactory.AddSingleProcessor<TBot>(sp, processor);
        var nextProcessor = processor?.Next;

        while (nextProcessor != default)
        {
            clientProcessorFactory.AddSingleProcessor<TBot>(sp, nextProcessor);
            nextProcessor = nextProcessor.Next;
        }

        return sp;
    }

    public static IServiceProvider RegisterBotCommand<TCommandProcessor, TBot>(this IServiceProvider sp)
        where TCommandProcessor : class, ICommandProcessor
        where TBot : IBot<TBot>
    {
        sp.GetRequiredService<ClientProcessorFactory>()
            .AddProcessor<TCommandProcessor, TBot>(sp);

        return sp;
    }

    public static CommandRegisterServices<TCommand, TBot> RegisterBotCommand<TCommand, TCommandProcessor, TBot>(
        this IServiceProvider sp)
        where TCommandProcessor : class, ICommandProcessor
        where TBot : IBot<TBot>
        where TCommand : ICommand
    {
        sp.GetRequiredService<ClientProcessorFactory>()
            .AddProcessor<TCommandProcessor, TBot>(sp);

        return new CommandRegisterServices<TCommand, TBot>(sp);;
    }

    public static IServiceProvider RegisterFluentBotCommand<TCommandProcessor, TBot>(this IServiceProvider sp) 
        where TCommandProcessor : class, ICommandProcessor
        where TBot : IBot<TBot>
    {
        sp.GetRequiredService<ClientProcessorFactory>()
            .AddProcessor<TCommandProcessor, TBot>(sp);

        return sp;
    }
    
    public static CommandRegisterServices<TCommand, TBot> RegisterFluentBotCommand<TCommand, TCommandProcessor, TBot>(this IServiceProvider sp) 
        where TCommandProcessor : class, ICommandProcessor
        where TBot : IBot<TBot>
        where TCommand : ICommand
    {
        sp.GetRequiredService<ClientProcessorFactory>()
            .AddProcessor<TCommandProcessor, TBot>(sp);

        return new CommandRegisterServices<TCommand, TBot>(sp);
    }

    public static IHttpClientBuilder AddCertificates(this IHttpClientBuilder builder, BotSettings? settings) =>
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