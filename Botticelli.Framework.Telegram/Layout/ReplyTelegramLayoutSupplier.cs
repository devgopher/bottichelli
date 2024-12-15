using Botticelli.Framework.Controls.Exceptions;
using Botticelli.Framework.Controls.Layouts;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botticelli.Framework.Telegram.Layout;

public class ReplyTelegramLayoutSupplier : IReplyTelegramLayoutSupplier
{
    public ReplyKeyboardMarkup GetMarkup(ILayout layout)
    {
        if (layout == default)
            throw new LayoutException("Layout = null!");

        var elems = new List<List<KeyboardButton>>(5);

        foreach (var layoutRow in layout.Rows)
        {
            var keyboardElement = new List<KeyboardButton>();
            keyboardElement.AddRange(layoutRow.Items.Select(item => new KeyboardButton(item?.Control?.Content)));

            elems.Add(keyboardElement);
        }

        return new ReplyKeyboardMarkup(elems)
        {
            ResizeKeyboard = true
        };
    }
}