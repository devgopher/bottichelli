using Botticelli.Framework.Controls.Layouts;

namespace Botticelli.Framework.Controls.Parsers;

/// <summary>
///     Supplier is responsible for conversion of Layout into messenger-specific controls (for example, ReplyMarkup in
///     Telegram)
/// </summary>
public interface ILayoutSupplier<out TReplyOptions>
{
    public TReplyOptions GetMarkup(ILayout layout);
}