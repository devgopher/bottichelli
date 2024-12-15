namespace Botticelli.Framework.Controls.Parsers;

/// <summary>
///     Gets layout from config file
/// </summary>
/// <typeparam name="TReplyOptions"></typeparam>
public interface ILayoutLoader<out TReplyOptions>
{
    public TReplyOptions GetMarkup(string configPath);
}