using Botticelli.Framework.Controls.Exceptions;
using Botticelli.Framework.Controls.Parsers;

namespace Botticelli.Framework.Controls.Tests.Layouts;

[TestFixture]
[TestOf(typeof(JsonLayoutParser))]
public class JsonLayoutParserTest
{
    private readonly JsonLayoutParser _jsonLayoutParser = new();

    [Test]
    [TestCase]
    public void ValidParseJsonTest()
    {
        var jsonText = File.ReadAllText("TestCases/CorrectLayout.json");

        var layout = _jsonLayoutParser.Parse(jsonText);

        Assert.That(layout, Is.Not.Null);
    }

    [Test]
    [TestCase]
    public void InvalidParseJsonTest()
    {
        var jsonText = File.ReadAllText("TestCases/InvalidLayout.json");

        Assert.Throws<LayoutException>(() => _jsonLayoutParser.Parse(jsonText));
    }
}