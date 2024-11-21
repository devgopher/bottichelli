using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Botticelli.Audio.Tests;

[TestFixture]
public class UniversalLowQualityConvertorTests
{
    private readonly IConvertor? _convertor;

    public UniversalLowQualityConvertorTests()
    {
        IAnalyzer analyzer = new InputAnalyzer();
        _convertor = new UniversalLowQualityConvertor(analyzer, new NullLogger<UniversalLowQualityConvertor>());
    }

    [Test]
    public void ConvertMp3ToOpusTest()
    {
        Check();
        var outcome = GetOutcome(new AudioInfo
        {
            AudioFormat = AudioFormat.Opus,
            Bitrate = AudioBitrate.LowVoice
        });

        AssertOutcome(outcome);
    }

    private static void AssertOutcome(byte[]? outcome) => Assert.That(outcome != null && outcome.Any());

    private byte[] GetOutcome(AudioInfo audioInfo)
    {
        using var stream = File.OpenRead("voice.mp3");
        return _convertor.Convert(stream, audioInfo);
    }

    private static void Check()
    {
        if (!File.Exists("voice.mp3"))
            Assert.Fail("no voice.mp3!");
    }

    [Test]
    public void ConvertMp3ToOggTest()
    {
        Check();
        var outcome = GetOutcome(new AudioInfo
        {
            AudioFormat = AudioFormat.Ogg,
            Bitrate = AudioBitrate.LowMusic
        });

        AssertOutcome(outcome);
    }

    [Test]
    public void ConvertMp3ToMp3LowTest()
    {
        Check();
        var outcome = GetOutcome(new AudioInfo
        {
            AudioFormat = AudioFormat.Mp3,
            Bitrate = AudioBitrate.LowVoice
        });

        AssertOutcome(outcome);
    }
}