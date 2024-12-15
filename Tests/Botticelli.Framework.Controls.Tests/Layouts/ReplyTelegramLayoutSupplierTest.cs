using Botticelli.Framework.Controls.Layouts;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Telegram.Layout;

namespace Botticelli.Framework.Controls.Tests.Layouts;

[TestFixture]
[TestOf(typeof(ReplyTelegramLayoutSupplier))]
public class ReplyTelegramLayoutSupplierTest
{
    private readonly JsonLayoutParser _jsonLayoutParser = new();
    private readonly IReplyTelegramLayoutSupplier _supplier = new ReplyTelegramLayoutSupplier();

    [Test]
    public void GetMarkupTest()
    {
        var jsonText = File.ReadAllText("TestCases/CorrectLayout.json");
        var layout = _jsonLayoutParser.Parse(jsonText);
        var markup = _supplier.GetMarkup(layout);
        
        Assert.That(markup != null);
    }
}