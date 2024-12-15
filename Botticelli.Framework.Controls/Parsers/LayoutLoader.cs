using Botticelli.Framework.Controls.Exceptions;

namespace Botticelli.Framework.Controls.Parsers;

public class LayoutLoader<TLayoutParser, TLayoutSupplier, TMarkup>(TLayoutParser parser, TLayoutSupplier supplier)
    : ILayoutLoader<TMarkup>
    where TLayoutParser : ILayoutParser
    where TLayoutSupplier : ILayoutSupplier<TMarkup>
{
    public TMarkup GetMarkup(string configPath)
    {
        if (!File.Exists(configPath))
            throw new LayoutException($"Can't find layout config file: {configPath}!");

        try
        {
            var jsonConfig = File.ReadAllText(configPath);
            var layout = parser.Parse(jsonConfig);

            return supplier.GetMarkup(layout);
        }
        catch (Exception ex)
        {
            throw new LayoutException($"Layout loading exception: {ex.Message}!", ex);
        }
    }
}