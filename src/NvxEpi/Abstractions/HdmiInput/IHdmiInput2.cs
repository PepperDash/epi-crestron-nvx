using Crestron.SimplSharpPro.DM;

namespace NvxEpi.Abstractions.HdmiInput
{
    public interface IHdmiInput2 : IHdmiInput1
    {
        void SetHdmi2HdcpCapability(int capability);
        void SetHdmi2HdcpCapability(eHdcpCapabilityType capability);
    }
}