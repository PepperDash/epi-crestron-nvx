using Crestron.SimplSharpPro.DM;

namespace NvxEpi.Abstractions.HdmiInput
{
    public interface IHdmiInput1 : IHdmiInput
    {
        void SetHdmi1HdcpCapability(int capability);
        void SetHdmi1HdcpCapability(eHdcpCapabilityType capability);
    }
}