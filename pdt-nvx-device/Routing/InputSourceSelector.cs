using NvxEpi.Interfaces;

namespace NvxEpi.Routing
{
    public class NvxInputSourceSelector
    {
        public INvxDevice Device { get; set; }
        public int HdmiInput { get; set; }
    }
}