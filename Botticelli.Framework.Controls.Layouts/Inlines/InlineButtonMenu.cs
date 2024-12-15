using Botticelli.Framework.Controls.BasicControls;

namespace Botticelli.Framework.Controls.Layouts.Inlines;

public class InlineButtonMenu : ILayout
{
    private readonly int _columns;
    private readonly int _rows;
    private string? _header;

    public InlineButtonMenu(int rows, int columns)
    {
        if (rows < 1) throw new InvalidDataException("rows count should be  > 1!");
        if (columns < 1) throw new InvalidDataException("columns count should be  > 1!");

        _rows = rows;
        _columns = columns;
        Rows = new List<Row>();
    }

    public string? Header
    {
        get => _header;
        set
        {
            var row = Rows.Any() ? Rows.First() : new Row();
            if (!Rows.Any()) Rows.Add(row);

            var item = row.Items.Count != 0 ?
                    row.Items.First() :
                    new Item
                    {
                        Control = new Text
                        {
                            Content = value,
                            CallbackData = string.Empty,
                            Params = null,
                            MessengerSpecificParams = null
                        },
                        Params = null
                    };

            if (Rows[0].Items.Count == 0) Rows[0].Items.Add(item);

            _header = value;
        }
    }

    public void AddRow(Row row) => Rows?.Add(row);


    public IList<Row>? Rows { get; }

    /// <summary>
    ///     Add a control into a free place
    /// </summary>
    /// <param name="control"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddControl(IControl control)
    {
        if (!CheckControlsCount()) throw new InvalidOperationException("Limit of controls has been exceeded!");

        var row = Rows?.Skip(Header != null ? 1 : 0).FirstOrDefault(r => r.Items.Count < _columns) ?? new Row();
        if (!Rows.Any()) Rows.Add(row);
        
        row?.AddItem(new Item
        {
            Control = control,
            Params = null
        });
    }

    private bool CheckControlsCount()
        => Rows?.Skip(Header != null ? 1 : 0).Select(r => r.Items?.Count ?? 0).Sum(cnt => cnt) < _rows * _columns;
}