namespace NvxEpi.Abstractions.Stream;

/// <summary>
/// A receiver that turns its video output off while no stream is routed to it, so a downstream
/// device (e.g. a codec content input) sees no sync instead of black.
/// </summary>
public interface IDisableOutputOnNoStream
{
    bool DisableOutputOnNoStream { get; }
}
