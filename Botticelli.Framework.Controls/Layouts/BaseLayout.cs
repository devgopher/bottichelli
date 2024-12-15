namespace Botticelli.Framework.Controls.Layouts;

public class BaseLayout : ILayout
{
    private readonly List<Row>? _rows = new(5);

    public void AddRow(Row row) => _rows.Add(row);

    public IList<Row>? Rows => _rows;
}