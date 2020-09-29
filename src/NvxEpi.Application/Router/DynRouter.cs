using System.Linq;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Application.Router
{
    public static class DynRouter
    {
        public static TieLineCollection TieLines
        {
            get { return _ties; }
        }

        private static readonly TieLineCollection _ties = new TieLineCollection();
        private static readonly RouteDescriptorCollection _routes = new RouteDescriptorCollection();

        public static void MakeRoute(this IRoutingInputs dest, IRoutingOutputs source, eRoutingSignalType signalType)
        {
            dest.ReleaseRoutes();
            var descriptor = GetRouteToSource(dest, source, signalType);
            if (descriptor == null)
                return;

            _routes.AddRouteDescriptor(descriptor);

            foreach (var route in descriptor.Routes)
            {
                if (route.OutputPort == null)
                {
                    var sink = route.SwitchingDevice as IRoutingSinkWithSwitching;
                    if (sink == null)
                        continue;

                    sink.ExecuteSwitch(route.InputPort);
                    continue;
                }

                var router = route.SwitchingDevice as IRouting;
                if (router == null)
                    continue;

                router.ExecuteSwitch(route.InputPort.Selector, route.OutputPort.Selector, signalType);
                route.OutputPort.InUseTracker.AddUser(dest, dest.Key);
            }
        }

        public static void ReleaseRoutes(this IRoutingInputs dest)
        {
            var current = _routes.GetRouteDescriptorForDestination(dest);
            if (current == null)
                return;

            Debug.Console(1, dest, "Releasing current route: {0}", current.Source.Key);
            foreach (var route in current.Routes)
            {
                if (route.OutputPort == null)
                    continue;

                route.OutputPort.InUseTracker.RemoveUser(dest, dest.Key);
            }
        }

        private static RouteDescriptor GetRouteToSource(IRoutingInputs dest, IRoutingOutputs source, eRoutingSignalType signalType)
        {
            var descriptor = new RouteDescriptor(source, dest, signalType);
            return dest.InputPorts.Any(input => FindRoute(input, source, signalType, descriptor)) ? descriptor : null;
        }

        private static bool FindRoute(RoutingInputPort port, IRoutingOutputs source, eRoutingSignalType signalType, RouteDescriptor descriptor)
        {
            Debug.Console(1, port.ParentDevice, "Looking for a {0} route to {1} on {2}", signalType.ToString(), source.Key, port.Key);

            var tie = DynRouter.TieLines.FirstOrDefault(t => t.DestinationPort.Key.Equals(port.Key) &&
                (t.Type.Has(signalType)));

            if (tie == null)
            {
                Debug.Console(1, port.ParentDevice, "None on this port:'{0}', trying the next...", port.Key);
                return false;
            }

            var sourceDevice = tie.SourcePort.ParentDevice;
            if (sourceDevice.Key.Equals(source.Key))
            {
                Debug.Console(1, port.ParentDevice, "Device is directly connected, attempting to add sink switch...");
                var device = tie.DestinationPort.ParentDevice as IRoutingSinkWithSwitching;

                if (device != null)
                    descriptor.Routes.Add(new RouteSwitchDescriptor(tie.DestinationPort));

                return true;
            }

            var nextDestToCheck = sourceDevice as IRoutingInputs;
            if (nextDestToCheck == null)
                return false;

            if (CheckTieLineUtil(nextDestToCheck, source, tie, signalType, descriptor))
            {
                var device = tie.DestinationPort.ParentDevice as IRoutingSinkWithSwitching;

                if (device != null)
                    descriptor.Routes.Add(new RouteSwitchDescriptor(tie.DestinationPort));

                return true;
            };

            return false;
        }

        private static bool CheckTieLineUtil(IRoutingInputs dest, IRoutingOutputs source, TieLine lastTie, eRoutingSignalType signalType, RouteDescriptor descriptor)
        {
            foreach (var input in dest.InputPorts)
            {
                Debug.Console(1, input.ParentDevice, "Looking for a {0} route to {1} on {2}", signalType.ToString(), source.Key, input.Key);

                var tie = DynRouter.TieLines.FirstOrDefault(t => t.DestinationPort.Key.Equals(input.Key) &&
                    (t.Type == signalType || (t.Type & (eRoutingSignalType.Audio | eRoutingSignalType.Video)) == (eRoutingSignalType.Audio | eRoutingSignalType.Video)));

                if (tie == null)
                {
                    Debug.Console(1, input.ParentDevice, "None on this port, trying the next...");
                    continue;
                }

                var device = tie.DestinationPort.ParentDevice as IRouting;
                var sourceDevice = tie.SourcePort.ParentDevice;
                if (sourceDevice.Key.Equals(source.Key))
                {
                    Debug.Console(1, input.ParentDevice, "Found the source! Rolling back up...");
                    
                    if (device != null)
                    {
                        Debug.Console(1, input.ParentDevice, "Adding route {0}|{1} : {2}|{3}",
                        tie.DestinationPort.ParentDevice.Key, tie.DestinationPort.Key,
                        lastTie.SourcePort.ParentDevice.Key, lastTie.SourcePort.Key);

                        descriptor.Routes.Add(new RouteSwitchDescriptor(lastTie.SourcePort, tie.DestinationPort));
                    }

                    return true;
                }

                var nextDestToCheck = sourceDevice as IRoutingInputs;
                if (nextDestToCheck == null)
                {
                    Debug.Console(1, sourceDevice, "Connected device isn't it, trying the next...");
                    continue;
                }

                if (CheckTieLineUtil(nextDestToCheck, source, tie, signalType, descriptor))
                {
                    if (device == null)
                        return false;

                    var inputTie = tie;
                    var outputTie = lastTie;

                    Debug.Console(1, device, "Got it! adding route {0}|{1} : {2}|{3}",
                    inputTie.DestinationPort.ParentDevice.Key, inputTie.DestinationPort.Key,
                    outputTie.SourcePort.ParentDevice.Key, outputTie.SourcePort.Key);
                    if (device != null)
                        descriptor.Routes.Add(new RouteSwitchDescriptor(lastTie.SourcePort, tie.DestinationPort));

                    return true;
                }
            }

            Debug.Console(1, dest, "No route to source found!");
            return false;
        }
    }
}