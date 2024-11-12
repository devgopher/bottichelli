using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

/// <summary>
///     Func transform for command arguments processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TArgs"></typeparam>
public class TransformArgumentsProcessor<TCommand, TArgs> : ChainProcessor<TCommand>
    where TCommand : IChainCommand
{
    /// <summary>
    ///     Func transform processor
    /// </summary>
    public TransformArgumentsProcessor(ILogger<TransformArgumentsProcessor<TCommand, TArgs>> logger)
        : base(logger)
    {
    }

    public Func<TArgs, TArgs> SuccessFunc { get; set; } = t => t;
    public Func<TArgs, TArgs> FailFunc { get; set; } = t => t;

    public override Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Process(
        Either<FailResult<TCommand>, SuccessResult<TCommand>> stepResult, CancellationToken token = default)
    {
        try
        {
            return Task.FromResult(stepResult.IsRight
                ? stepResult.Map(t =>
                {
                    t.Command.Context.Set(Names.Args,
                        SuccessFunc.Invoke(t.Command.Context.Get<TArgs>(Names.Args)!));

                    return t;
                })
                : stepResult.MapLeft(t =>
                {
                    t.Command.Context.Set(Names.Args,
                        FailFunc.Invoke(t.Command.Context.Get<TArgs>(Names.Args)!));

                    return t;
                }));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return Task.FromResult(stepResult.MapLeft(t =>
            {
                t.Command.Context.Set(Names.Args,
                    FailFunc.Invoke(t.Command.Context.Get<TArgs>(Names.Args)!));

                return t;
            }));
        }
    }

    protected override Task InnerProcessAsync(IResult<TCommand> stepResult, CancellationToken token) =>
        Task.CompletedTask;

    protected override Task InnerErrorProcessAsync(IResult<TCommand> command, CancellationToken token) =>
        Task.CompletedTask;
}