using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

public class ChainRunner<TCommand>(List<IChainProcessor<TCommand>> chain, ILogger<ChainRunner<TCommand>> logger)
    where TCommand : IChainCommand
{
    public async Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Run(TCommand command)
    {
        var output = new Either<FailResult<TCommand>, SuccessResult<TCommand>>();
        var eitherContext = output.Right(_ => command);

        logger.LogInformation("Chain processing for {TCommand} start...", typeof(TCommand).Name);
        foreach (var tc in chain)
        {
            logger.LogInformation("Chain processor {tc} for {TCommand} start...", tc.GetType().Name,
                typeof(TCommand).Name);
            output = await tc.Process(output);

            if (!output.IsRight)
            {
                logger.LogInformation("Chain processor {tc}  for {TCommand} finished: fail!", tc.GetType().Name,
                    typeof(TCommand).Name);

                return output;
            }

            logger.LogInformation("Chain processor {tc}  for {TCommand} finished: success", tc.GetType().Name,
                typeof(TCommand).Name);
        }

        logger.LogInformation("Chain processing for {TCommand} finished: {IsSuccess}", typeof(TCommand).Name,
            output.IsRight ? "success" : "fail");

        return output;
        ;
    }
}