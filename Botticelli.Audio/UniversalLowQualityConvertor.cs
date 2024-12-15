using Botticelli.Audio.Exceptions;
using FFMpegCore;
using FFMpegCore.Pipes;
using Microsoft.Extensions.Logging;
using NAudio.Lame;
using NAudio.Vorbis;
using NAudio.Wave;

namespace Botticelli.Audio;

public class UniversalLowQualityConvertor : IConvertor
{
    private readonly IAnalyzer _analyzer;
    private readonly ILogger<UniversalLowQualityConvertor> _logger;

    public UniversalLowQualityConvertor(IAnalyzer analyzer, ILogger<UniversalLowQualityConvertor> logger)
    {
        _analyzer = analyzer;
        _logger = logger;
    }

    public byte[] Convert(Stream input, AudioInfo tgtParams)
    {
        try
        {
            if (tgtParams.AudioFormat is AudioFormat.M4a or AudioFormat.Aac or AudioFormat.Opus or AudioFormat.Ogg)
                return ProcessByStreamEncoder(input, tgtParams);

            var srcParams = _analyzer.Analyze(input);

            using var resultStream = new MemoryStream();
            using var srcStream = GetSourceWaveStream(input, srcParams);
            using var tgtStream = GetTargetWaveStream(srcStream, tgtParams);

            return tgtStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(Convert)} => ({tgtParams.AudioFormat}, {tgtParams.Bitrate}) error", ex);
            throw new AudioConvertorException($"Audio conversion error: {ex.Message}", ex);
        }
    }

    public byte[] Convert(byte[] input, AudioInfo tgtParams)
    {
        using var ms = new MemoryStream(input);

        return Convert(ms, tgtParams);
    }

    private byte[] ProcessByStreamEncoder(Stream input, AudioInfo tgtParams)
    {
        try
        {
            var codec = string.Empty;

            switch (tgtParams.AudioFormat)
            {
                case AudioFormat.Ogg:
                    codec = "ogg";
                    break;
                case AudioFormat.Aac:
                case AudioFormat.M4a:
                    codec = "aac";
                    break;
                case AudioFormat.Opus:
                    codec = "opus";
                    break;
                case AudioFormat.Wav:
                    codec = "wav";
                    break;
                case AudioFormat.Unknown:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            using var output = new MemoryStream();
            FFMpegArguments
                .FromPipeInput(new StreamPipeSource(input))
                .OutputToPipe(new StreamPipeSink(output), options => options
                    .ForceFormat(codec)
                    .WithAudioBitrate(tgtParams.Bitrate))
                .ProcessSynchronously(ffMpegOptions: new FFOptions
                {
                    BinaryFolder = "ffmpeg"
                });

            return output.ToArray();
        }
        catch (IOException ex)
        {
            _logger.LogError($"{nameof(Convert)} => ({tgtParams.AudioFormat}, {tgtParams.Bitrate}) error", ex);
            return Array.Empty<byte>();
        }
    }

    private WaveStream GetSourceWaveStream(Stream input, AudioInfo srcParams)
    {
        return srcParams.AudioFormat switch
        {
            AudioFormat.Mp3 => new Mp3FileReader(input),
            AudioFormat.Ogg => new VorbisWaveReader(input),
            _ => new WaveFileReader(input)
        };
    }

    private MemoryStream GetTargetWaveStream(WaveStream input, AudioInfo srcParams)
    {
        var ms = new MemoryStream();

        Stream writerStream = srcParams.AudioFormat switch
        {
            AudioFormat.Mp3 => new LameMP3FileWriter(ms, input.WaveFormat, LAMEPreset.ABR_16),
            _ => new WaveFileWriter(ms, input.WaveFormat)
        };

        input.CopyTo(writerStream);

        writerStream.Dispose();
        return ms;
    }
}