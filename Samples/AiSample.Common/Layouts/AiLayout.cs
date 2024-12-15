using Botticelli.Framework.Controls.BasicControls;
using Botticelli.Framework.Controls.Layouts;

namespace AiSample.Common.Layouts;

public class AiLayout : BaseLayout
{
    public AiLayout()
    {
        var row1 = new Row();
        var row2 = new Row();

        var item1 = new Item
        {
            Control = new Button
            {
                Content = "/ai Hello!"
            }
        };

        var item2 = new Item
        {
            Control = new Button
            {
                Content = "/ai Tell me smth!"
            }
        };

        var item3 = new Item
        {
            Control = new Button
            {
                Content = "/ai Good bye!"
            }
        };

        row1.AddItem(item1);
        row1.AddItem(item2);
        row2.AddItem(item3);

        AddRow(row1);
        AddRow(row2);
    }
}