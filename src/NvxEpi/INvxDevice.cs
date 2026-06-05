using NvxEpi;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi
{
    public interface INvxDevice : IRouting, IKeyName
    {
        bool IsTransmitter { get; }
        int DeviceId { get; }
        //string StreamUrl { get; }
        StringFeedback StreamUrlFeedback { get; }
        StringFeedback MulticastAddressFeedback { get; }
        BoolFeedback IsStreamingFeedback { get; }
        StringFeedback StreamStatusFeedback { get; }
        //string DmNaxTxAddress { get; }
        StringFeedback DmNaxTxAddressFeedback { get; }
        BoolFeedback IsTransmittingDmNaxFeedback { get; }
        StringFeedback DmNaxTransmitStatusFeedback { get; }
        //string DmNaxRxAddress { get; }
        StringFeedback DmNaxRxAddressFeedback { get; }
        BoolFeedback IsReceivingDmNaxFeedback { get; }
        StringFeedback DmNaxReceiveStatusFeedback { get; }
        void SetIncomingStreamUrl(string streamUrl);
        void SetIncomingDmNaxStreamAddress(string address);
    }
}