using Botticelli.Framework.Monads.Commands.Processors.Multichain;

namespace Botticelli.Framework.Monads.Commands.Context;

public interface IChoiseResolver
{
    public bool Resolve(IChoise choise);
}