using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace GHDY.Core.AudioPlayer.Interops.DirectShow
{
    static class Extensions
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public static IBaseFilter AddFilterFromClsid(this IGraphBuilder graphBuilder, Guid filterClsid, string filterName)
        {
            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            IBaseFilter result = null;

            try
            {
                Type type = Type.GetTypeFromCLSID(filterClsid);
                result = (IBaseFilter)Activator.CreateInstance(type);

                graphBuilder.AddFilter(result, filterName);
            }
            catch
            {
                if (result != null)
                {
                    Marshal.ReleaseComObject(result);
                    result = null;
                }
            }

            return result;
        }

        public static IEnumerable<IPin> Pins(this IBaseFilter filter)
        {
            IPin[] pins = new IPin[1];
            var enumPins = filter.EnumPins();

            while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
            {
                yield return pins[0];
            }
        }

        public static IEnumerable<IPin> OutputPins(this IBaseFilter filter)
        {
            return filter.Pins().Where(pin => pin.QueryDirection() == PinDirection.Output);
        }

        public static IEnumerable<IPin> InputPins(this IBaseFilter filter)
        {
            return filter.Pins().Where(pin => pin.QueryDirection() == PinDirection.Input);
        }
    }

    class Pin
    {
        public readonly IPin RawPin;

        public Pin(IPin pin)
        {
            this.RawPin = pin;
        }

        public PinInfo Info
        {
            get { return RawPin.QueryPinInfo(); } // should be freed
        }

        public PinDirection Direction { get { return Info.dir; } }

        public string Name { get { return Info.name; } }
    }

    class Filter
    {
        IBaseFilter filter;

        public Filter(IBaseFilter filter)
        {
            if (filter == null) throw new NullReferenceException("filter is null");

            this.filter = filter;
        }

        private IEnumerable<Pin> Pins
        {
            get
            {
                var pinEnumerator = filter.EnumPins();
                IPin[] pin = new IPin[1];

                while (pinEnumerator.Next(1, pin, IntPtr.Zero) == 0)
                {
                    yield return new Pin(pin[0]);
                }
            }
        }

        public Pin DefaultOutputPin
        {
            get
            {
                return OutputPins.FirstOrDefault();
            }
        }

        public Pin DefaultInputPin
        {
            get
            {
                return InputPins.FirstOrDefault();
            }
        }

        public IEnumerable<Pin> InputPins
        {
            get
            {
                return from pin in Pins where pin.Direction == PinDirection.Input select pin;
            }
        }

        public IEnumerable<Pin> OutputPins
        {
            get
            {
                return from pin in Pins where pin.Direction == PinDirection.Output select pin;
            }
        }
    }
}
