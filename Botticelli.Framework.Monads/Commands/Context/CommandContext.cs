using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Botticelli.Framework.Monads.Commands.Context;

/// <summary>
///     Command context for transmitting data over command context chain
/// </summary>
public class CommandContext : ICommandContext
{
    private readonly Dictionary<string, string> _parameters = new();

    public T? Get<T>(string name)
    {
        if (_parameters.TryGetValue(name, out var parameter))
        {
            var type = typeof(T);
            if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
            {
                var chunk = parameter.Replace('.', ',');

                if (type == typeof(double))
                    return (T?)(object)double.Parse(chunk);
                if (type == typeof(float))
                    return (T?)(object)float.Parse(chunk);
                if (type == typeof(decimal))
                    return (T?)(object)decimal.Parse(chunk);
            }
            else
            {
                return JsonSerializer.Deserialize<T>(parameter);
            }
        }

        return default;
    }

    public string Get(string name) => _parameters[name];

    public T Set<T>(string name, T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        if (name == Names.Args) // args are always string
            _parameters[name] = Stringify(value);
        else
            _parameters[name] = JsonSerializer.Serialize(value);

        return value;
    }

    public string Set(string name, string value)
    {
        _parameters[name] = Stringify(value);
        return value;
    }


    public T Transform<T>(string name, Func<T, T> func)
    {
        if (!_parameters.TryGetValue(name, out var parameter))
            throw new KeyNotFoundException(name);

        var deserialized = JsonSerializer.Deserialize<T>(parameter);

        _parameters[name] = JsonSerializer.Serialize(func(deserialized));

        return deserialized;
    }

    private static string Stringify<T>([DisallowNull] T value) =>
        value switch
        {
            double dbl => dbl.ToString("G").Replace(',', '.'),
            float flt => flt.ToString("G").Replace(',', '.'),
            decimal dcm => dcm.ToString("G").Replace(',', '.'),
            _ => value.ToString()!
        };
}