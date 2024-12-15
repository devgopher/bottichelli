using Botticelli.Framework.Exceptions;
using Botticelli.Interfaces;

namespace Botticelli.Framework.SendOptions;

/// <summary>
///     Additional options for sending messages for partial messenger
///     (for example you can use InlineKeyboardMarkup  as T)
/// </summary>
/// <typeparam name="T"></typeparam>
public class SendOptionsBuilder<T> : ISendOptionsBuilder<T> where T : class
{
    private T? _innerObject;

    private SendOptionsBuilder()
    {
    }

    private SendOptionsBuilder(T? innerObject)
    {
        _innerObject = innerObject;
    }

    public ISendOptionsBuilder<T> Create(params object[]? args)
    {
        if (_innerObject != default) throw new BotException($"You shouldn't use {nameof(Create)}() method twice!");

        var constructors = typeof(T)
            .GetConstructors()
            .Where(c => c.IsPublic)
            .ToArray();

        // no params? ok => let's seek a parameterless constructor!
        if ((args != null && args.Length != 0) || constructors.All(c => c.GetParameters().Length != 0))
            return this;

        _innerObject = Activator.CreateInstance<T>();

        return this;
    }

    public ISendOptionsBuilder<T> Set(Func<T?, T>? func)
    {
        func?.Invoke(_innerObject);

        return this;
    }

    public T? Build() => _innerObject;

    public static SendOptionsBuilder<T> CreateBuilder() => new();

    public static SendOptionsBuilder<T> CreateBuilder(T input) => new(input);
}