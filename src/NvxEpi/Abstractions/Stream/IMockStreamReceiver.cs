namespace NvxEpi.Abstractions.Stream;

/// <summary>
/// A receiver with no NVX hardware behind it (the bench mock). The primary stream router hands it the stream
/// it should now show, instead of switching hardware, so routes through mocks are tracked like real ones.
/// </summary>
public interface IMockStreamReceiver : IStream
{
    /// <summary>Show <paramref name="tx"/>'s stream, or nothing when null.</summary>
    void ReceiveStream(IStream tx);
}
