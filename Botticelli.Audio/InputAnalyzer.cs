using Botticelli.Shared.Utils;
using FileSignatures;
using FileSignatures.Formats;
using NAudio.Vorbis;
using NAudio.Wave;

namespace Botticelli.Audio;

public class InputAnalyzer : IAnalyzer
{
    private readonly FileFormatInspector _fileFormatInspector = new(new List<FileFormat>
    {
        new Mpeg3Id3v2(),
        new Mpeg3NoId3_1(),
        new Mpeg3NoId3_2(),
        new Mpeg3NoId3_3(),
        new Mpeg3NoId3_4(),
        new Ogg(),
        new Amr(),
        new M4A()
    });

    public AudioInfo Analyze(string filePath)
    {
        using var fr = new AudioFileReader(filePath);

        var format = Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            "wav" => AudioFormat.Wav,
            "mp3" => AudioFormat.Mp3,
            "m4a" => AudioFormat.M4a,
            "aac" => AudioFormat.Aac,
            "ogg" => AudioFormat.Ogg,
            _ => AudioFormat.Unknown
        };

        return new AudioInfo
        {
            AudioFormat = format,
            SampleRate = fr.WaveFormat.SampleRate,
            BitsPerSample = fr.WaveFormat.BitsPerSample,
            Channels = fr.WaveFormat.Channels
        };
    }

    public AudioInfo Analyze(Stream input)
    {
        var fileFormat = _fileFormatInspector.DetermineFileFormat(input);
      
        AudioFormat format;
        WaveStream reader;

        fileFormat.NotNull();
        switch (fileFormat.Extension.ToLowerInvariant())
        {
            case "wav":
                format = AudioFormat.Wav;
                reader = new WaveFileReader(input);
                break;
            case "mp3":
                format = AudioFormat.Mp3;
                reader = new Mp3FileReader(input);
                break;
            case "m4a":
            case "aac":
                reader = default!;
                format = default;
                break;
            case "ogg":
                format = AudioFormat.Ogg;
                reader = new VorbisWaveReader(input);
                break;
            default:
                format = AudioFormat.Unknown;
                reader = default!;
                break;
        }

        if (format == AudioFormat.Unknown) throw new InvalidOperationException("Invalid format!");

        if (reader == default) throw new InvalidOperationException("Reader is null!");

        return new AudioInfo
        {
            AudioFormat = format,
            SampleRate = reader.WaveFormat.SampleRate,
            BitsPerSample = reader.WaveFormat.BitsPerSample
        };
    }
}