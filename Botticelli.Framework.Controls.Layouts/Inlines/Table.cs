namespace Botticelli.Framework.Controls.Layouts.Inlines;

public class Table(int cols) : ILayout
{
    public void AddItem(Item item)
    {
        var lastRow = Rows.SkipWhile(row => row.Items.Count == cols).FirstOrDefault();

        if (lastRow == default)
        {
            lastRow = new Row();
            AddRow(lastRow);
        }
        
        lastRow.Items.Add(item);
    }
    
    public virtual void AddRow(Row row)
    {
        Rows.Add(row);
    }

    public IList<Row>? Rows { get; } = new List<Row>();
}