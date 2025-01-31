using System.Text;
using Botticelli.Bot.Utils.TextUtils;

namespace Botticelli.Framework.Telegram.Utils;

public class TelegramTextTransformer : ITextTransformer
{
    /// <summary>
    ///     Autoescape for special symbols
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public StringBuilder Escape(StringBuilder text) =>
            text
                // .Replace("!", @"\!")
                // .Replace("*", @"\*")
                .Replace("'", @"\'")
                // .Replace(".", @"\.")
                .Replace("+", @"\+")
                .Replace("~", @"\~")
                .Replace("@", @"\@")
                // .Replace("_", @"\_")
                //.Replace("(", @"\(")
                //.Replace(")", @"\)")
                .Replace("-", @"\-")
                // .Replace("`", @"\`")
                .Replace("=", @"\=")
                .Replace(">", @"\\>")
                .Replace("<", @"\\<")
                .Replace("{", @"\{")
                .Replace("}", @"\}")
                .Replace("[", @"\[")
                .Replace("]", @"\]")
                .Replace("|", @"\|")
                .Replace("#", @"\#");
}