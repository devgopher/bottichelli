using System.Text.Json;
using Botticelli.Framework.Controls.BasicControls;
using Botticelli.Framework.Controls.Exceptions;
using Botticelli.Framework.Controls.Layouts;

namespace Botticelli.Framework.Controls.Parsers;

public class JsonLayoutParser : ILayoutParser
{
    public ILayout Parse(string jsonText)
    {
        var jsonDoc = JsonSerializer.Deserialize<JsonElement>(jsonText);
        var layout = new BaseLayout();

        if (!jsonDoc.TryGetProperty("Name", out var layoutNameElement))
            throw new LayoutException("'Name' property wasn't found");

        if (!jsonDoc.TryGetProperty("Layout", out var layoutParams))
            throw new LayoutException("'Layout' property wasn't found");

        // rows
        var subElements = layoutParams.EnumerateArray().ToList();

        foreach (var rowElement in subElements)
        {
            // cells
            var itemElements = rowElement.EnumerateArray().ToList();
            var row = new Row();
            layout.AddRow(row);

            foreach (var itemElement in itemElements)
            {
                var item = new Item();

                ResolveControlType(itemElement, item);

                if (itemElement.TryGetProperty("Params", out var cellParams))
                    item.Params = new ItemParams
                    {
                        Align = cellParams.TryGetProperty("Align", out var alignElem) &&
                                alignElem.TryGetInt32(out var align)
                            ? (CellAlign)align
                            : CellAlign.Left,
                        Stretch = cellParams.TryGetProperty("Stretch", out var stretchElem) &&
                                  stretchElem.TryGetInt32(out var stretch)
                            ? stretch
                            : 1
                    };

                if (itemElement.TryGetProperty("Specials", out var messengerSpecific))
                    item.Control.MessengerSpecificParams =
                        messengerSpecific.Deserialize<Dictionary<string, Dictionary<string, object>>>();

                row.AddItem(item);
            }
        }

        return layout;
    }

    public ILayout ParseFromFile(string jsonFile)
    {
        var json = File.ReadAllText(jsonFile);

        return Parse(json);
    }

    private static void ResolveControlType(JsonElement itemElement, Item item)
    {
        if (itemElement.TryGetProperty("Button", out var buttonElement))
        {
            var button = new Button
            {
                Content = buttonElement.GetProperty("Content").GetString()
            };

            item.Control = button;
        }
        else if (itemElement.TryGetProperty("Text", out var textElement))
        {
            var text = new Text
            {
                Content = buttonElement.GetProperty("Content").GetString()
            };

            item.Control = text;
        }
    }
}