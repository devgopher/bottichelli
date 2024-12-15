using Microsoft.Extensions.Options;

namespace Shared;

public class OptionsMock<T> : IOptions<T> where T : class
{
    public OptionsMock(T value) => Value = value;

    public T Value { get; }
}