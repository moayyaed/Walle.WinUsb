# Walle.WinUsb

[![badge](https://github.com/walle-work/Walle.WinUsb/workflows/dotNetStandard/badge.svg)](https://github.com/walle-work/Walle.WinUsb/actions/)
[![NuGet Badge](https://buildstats.info/nuget/Walle.WinUsb)](https://www.nuget.org/packages/Walle.WinUsb/)

```shell
dotnet add package Walle.WinUsb
```

## WinUsbDeviceWatcher Class

- Provider Usb Insert/Remove Events.
- Include Usb device  information such as PID, VID and Path etc.

```csharp
namespace Walle.WinUsb.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WinUsbDeviceWatcher Sample:");

            WinUsbDeviceWatcher watcher = new WinUsbDeviceWatcher();
            watcher.DeviceConnectEventHandler += Watcher_DeviceConnectEventHandler;
            watcher.DeviceDisconnectEventHandler += Watcher_DeviceDisconnectEventHandler;
        }

        private static void Watcher_DeviceDisconnectEventHandler(object sender, WinUsbDeviceEventArgs e)
        {
            Console.WriteLine("Detected Usb Device Event: {event} Path:{path},VID:{vid},PID:{pid}", e.EventType, e.Path, e.Vid, e.Vid);
        }

        private static void Watcher_DeviceConnectEventHandler(object sender, WinUsbDeviceEventArgs e)
        {
            Console.WriteLine("Detected Usb Device Event: {event}, Path:{path},VID:{vid},PID:{pid}", e.EventType, e.Path, e.Vid, e.Vid);
        }
    }
}
```

## WinUsbDevice Class

- Get the inserted USB device list.

```csharp
namespace Walle.WinUsb.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WinUsbDevice Sample:");

            var pathes = WinUsbDevice.GetAll();
            foreach (var path in pathes)
            {
                Console.WriteLine($"{path}");
            }
            Console.ReadLine();

        }
    }
}
```

```shell
WinUsbDevice.GetAll() Result:

\\?\usb#vid_0a12&pid_0001#5&2f03355&0&6#{a5dcbf10-6530-11d2-901f-00c04fb951ed}
\\?\usb#vid_413c&pid_2113#6&1bfb9409&0&4#{a5dcbf10-6530-11d2-901f-00c04fb951ed}
\\?\usb#vid_0b95&pid_772b#596c08#{a5dcbf10-6530-11d2-901f-00c04fb951ed}
\\?\usb#vid_413c&pid_301a#6&1bfb9409&0&3#{a5dcbf10-6530-11d2-901f-00c04fb951ed}
```

- Send ```bytes[]``` to USB device . scene such as be used in the thermal printer control.

```csharp
using System.Text;

namespace Walle.WinUsb.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //if target usb printer path:
            //\\?\usb#vid_413c&pid_2113#6&1bfb9409&0&4#{a5dcbf10-6530-11d2-901f-00c04fb951ed}        
            var connected = WinUsbDevice.Instance.Open("413c", "2113");
            if (connected)
            {
                var bytes = Encoding.UTF8.GetBytes("hello world");
                WinUsbDevice.Instance.Send(bytes);
            }
        }
    }
}
```

