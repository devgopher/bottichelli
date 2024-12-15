namespace Botticelli.Framework.Options;

public class ServerSettingsBuilder<T>
    where T : ServerSettings, new()
{
    private T _settings = new();

    public void Set(T settings) => _settings = settings;

    public ServerSettingsBuilder<T> Set(Action<T> func)
    {
        func(_settings);

        return this;
    }

    public T Build() => _settings;
}