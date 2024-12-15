namespace Botticelli.Framework.Controls.Layouts;

public interface ILayout
{
    public IList<Row>? Rows { get; }
    public void AddRow(Row row);
}