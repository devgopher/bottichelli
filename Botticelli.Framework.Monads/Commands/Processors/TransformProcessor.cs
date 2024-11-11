using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

/// <summary>
///     Func transform processor processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public abstract class TransformProcessor<TCommand>(ICommandContext commandContext, ILogger<TransformProcessor<TCommand>> logger, 
    Func<SuccessResult<TCommand>, SuccessResult<TCommand>> successFunc,
    Func<FailResult<TCommand>, FailResult<TCommand>> failFunc)
    : ChainProcessor<TCommand>(commandContext, logger)
    where TCommand : IChainCommand
{
    public override Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Process(
        Either<FailResult<TCommand>, SuccessResult<TCommand>> stepResult, CancellationToken token = default)
    {
        try
        {
            return Task.FromResult(stepResult.IsRight ? stepResult.Map(successFunc) : stepResult.MapLeft(failFunc));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return Task.FromResult(stepResult.MapLeft(failFunc));
        }
    }
}