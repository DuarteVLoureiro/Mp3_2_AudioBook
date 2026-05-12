using System;
using NAudio.Wave;
using NAudio.MediaFoundation;
using System.IO;
using Mp3_2_AudioBook;

internal class AudioConverter
{

    private static readonly int[] validBitrateVal =
        {
    32000, 48000, 64000, 80000, 96000, 112000,
    128000, 160000, 192000, 256000, 320000
        };

    public void Convert(string inputFilePath, string outputFilePath, int bitrate = 128000, IProgress<int> conversionProgress = null, CancellationToken cancellation = default, MetadataHandler.AudioMetadata metadata = null)
	{
		if (!File.Exists(inputFilePath)) 
		{
			throw new FileNotFoundException("Input file not found at: " + inputFilePath);
		}
		if (!inputFilePath.EndsWith(".mp3",StringComparison.OrdinalIgnoreCase))
		{
			throw new FileFormatException(Path.GetExtension(inputFilePath) + " is not an accepted file format");
		}
		if (!validBitrateVal.Contains(bitrate)) 
		{
			throw new ArgumentException("Invalid bitrate value: " + bitrate);
		}
		try
		{
            using (var byteReader = new MediaFoundationReader(inputFilePath))
            {
				long totalFileBytes = byteReader.Length;
				long bytesRead = 0;
				int lastReported = -1;
				// check if resample is needed ( audio is not in standard format )
				bool needsResample = byteReader.WaveFormat.SampleRate != 44100 || byteReader.WaveFormat.Channels != 2;
				IWaveProvider sourceStream;
				if (needsResample)
				{
					var targetFormat = new WaveFormat(44100, 2);
					sourceStream = new MediaFoundationResampler(byteReader, targetFormat);
				}
				else
				{
					sourceStream = byteReader;
				}
                var mediaType = MediaFoundationEncoder.SelectMediaType(
					AudioSubtypes.MFAudioFormat_AAC,
					sourceStream.WaveFormat,
					bitrate);
				if (mediaType == null)
				{
					throw new InvalidOperationException("No ACC encoder is available on this device");
				}
				using (var byteEncoder = new MediaFoundationEncoder(mediaType))
				{
                    ProgressBarStream progressBarStream = new ProgressBarStream(sourceStream, byteReader, conversionProgress, cancellation);
                    byteEncoder.Encode(outputFilePath, progressBarStream);
                    cancellation.ThrowIfCancellationRequested();
                }
				if (needsResample && sourceStream is IDisposable disposable)
				{
					disposable.Dispose();
				}
                //Write metadata
                if (metadata != null)
                {
                    MetadataHandler.WriteMetadata(outputFilePath, metadata);
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch ( Exception ex)
		{
            throw new InvalidOperationException($"Conversion failed: "+ ex.Message, ex);
        }
    }
    public static string[] GetAvailableEncoders()
    {
        var encoders = new System.Collections.Generic.List<string>();
        var formats = new[]
        {
        ("AAC", AudioSubtypes.MFAudioFormat_AAC),
        ("MP3", AudioSubtypes.MFAudioFormat_MP3),
        ("WMA", AudioSubtypes.MFAudioFormat_WMAudioV8)
    };
        var testFormat = new WaveFormat(44100, 2);
        foreach (var (name, guid) in formats)
        {
            var mediaType = MediaFoundationEncoder.SelectMediaType(guid, testFormat, 128000);
            if (mediaType != null)
            {
                encoders.Add(name);
            }
        }
        return encoders.ToArray();
    }

    public static int SnapToValidBitrate(int requestedBitrate)
    {
        return validBitrateVal.OrderBy(b => Math.Abs(b - requestedBitrate)).First();
    }
}
internal class ProgressBarStream : WaveStream
{
    private readonly IWaveProvider _sourceStream;
    private readonly WaveStream _lengthSource;
    private readonly IProgress<int> _progress;
    private readonly CancellationToken _cancellationToken;

    public ProgressBarStream(IWaveProvider sourceStream, WaveStream lengthSource, IProgress<int> progress, CancellationToken cancellationToken)
    {
        _sourceStream = sourceStream;
        _lengthSource = lengthSource;
        _progress = progress;
        _cancellationToken = cancellationToken;
    }

    public override WaveFormat WaveFormat => _sourceStream.WaveFormat;
    public override long Length => _lengthSource.Length;

    public override long Position
    {
        get => _lengthSource.Position;
        set { /* intentionally ignored — encoder can seek but we don't relay it */ }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        _cancellationToken.ThrowIfCancellationRequested();
        int bytesRead = _sourceStream.Read(buffer, offset, count);

        if (_progress != null && _lengthSource.Length > 0)
        {
            int percent = (int)((_lengthSource.Position * 100L) / _lengthSource.Length);
            _progress.Report(percent);
        }

        return bytesRead;
    }
}
