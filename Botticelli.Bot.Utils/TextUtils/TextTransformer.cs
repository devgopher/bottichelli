using System.Text;

namespace Botticelli.Bot.Utils.TextUtils;

public interface ITextTransformer
{
    public StringBuilder Escape(StringBuilder text);
}
