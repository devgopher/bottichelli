namespace Botticelli.Framework.Options;

public class BotSettingsBuilder<T>
    where T : BotSettings, new()
{
    private T _settings = new();
    
    public void Set(T settings) => _settings = settings;
    
    public BotSettingsBuilder<T> Set(Action<T> func)
    {
        func(_settings);

        return this;
    }

    public T Build() => _settings;
}