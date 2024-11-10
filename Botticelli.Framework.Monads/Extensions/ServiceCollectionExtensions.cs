using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Monads.Extensions;

public static class ServiceCollectionExtensions
{
    public static CommandAddServices<TCommand> AddMonadsChain<TCommand, TValidator>(
        this CommandAddServices<TCommand> commandAddServices,
        IServiceCollection services,
        Action<ChainBuilder<TCommand>> func)
        where TCommand : class, IChainCommand, new()
        where TValidator : class, ICommandValidator<TCommand>
    {
        var chainBuilder = new ChainBuilder<TCommand>(services);        
        func(chainBuilder);
       
        var runner = chainBuilder.Build();

        services.AddScoped(_ => runner);

        return commandAddServices.AddProcessor<ChainRunProcessor<TCommand>>()
            .AddValidator<TValidator>();
    }
}