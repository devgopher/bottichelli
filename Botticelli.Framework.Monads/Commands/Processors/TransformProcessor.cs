using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

/// <summary>
///     Func transform processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public class TransformProcessor<TCommand> : ChainProcessor<TCommand>
    where TCommand : IChainCommand
{
    /// <summary>
    ///     Func transform processor processor
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public TransformProcessor(ILogger<TransformProcessor<TCommand>> logger)
        : base(logger)
    {
    }

    public Func<SuccessResult<TCommand>, SuccessResult<TCommand>> SuccessFunc { get; set; } = t => t;
    public Func<FailResult<TCommand>, FailResult<TCommand>> FailFunc { get; set; } = t => t;

    public override Task<EitherAsync<FailResult<TCommand>, SuccessResult<TCommand>>> Process(
        EitherAsync<FailResult<TCommand>, SuccessResult<TCommand>> stepResult, CancellationToken token = default)
    {
        try
        {
            return Task.FromResult(stepResult.BiMap(SuccessFunc, FailFunc));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return Task.FromResult(stepResult.MapLeft(FailFunc));
        }
    }

    protected override Task InnerProcessAsync(IResult<TCommand> stepResult, CancellationToken token) =>
        Task.CompletedTask;

    protected override Task InnerErrorProcessAsync(FailResult<TCommand> stepResult, CancellationToken token) =>
        Task.CompletedTask;
}