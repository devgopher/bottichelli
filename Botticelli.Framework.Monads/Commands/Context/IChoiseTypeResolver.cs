using Botticelli.Framework.Monads.Commands.Processors.Multichain;

namespace Botticelli.Framework.Monads.Commands.Context;

public interface IChoiseResolver<in TNeededType>
{
    public bool Resolve(IChoise choice);
}