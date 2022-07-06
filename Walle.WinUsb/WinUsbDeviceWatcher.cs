using System.Threading;

namespace Walle.WinUsb
{
    public class WinUsbDeviceWatcher
    {
        private readonly List<string> paths = new();
        private CancellationTokenSource? CancellationTokenSource = new();
        public event WinUsbDeviceEventHandler? DeviceConnectEventHandler;
        public event WinUsbDeviceEventHandler? DeviceDisconnectEventHandler;
        public Task StartAsync()
        {
            return Task.Factory.StartNew(async () =>
               {
                   CancellationTokenSource = null;
                   CancellationTokenSource = new CancellationTokenSource();
                   while (!CancellationTokenSource.IsCancellationRequested)
                   {
                    
                       var path_snapshots = WinUsbDevice.GetAll();
                       path_snapshots.Where(p => !paths.Any(pa => pa == p)).ToList().ForEach(path =>
                       {
                           DeviceConnectEventHandler?.Invoke(this, new WinUsbDeviceEventArgs(path, WinUsbDeviceEventType.Insert));
                       });
                       paths.AddRange(path_snapshots.Where(p => !paths.Any(pa => pa == p)));
                       paths.Where(p => !path_snapshots.Any(pa => p == pa)).ToList().ForEach(path =>
                       {
                           DeviceDisconnectEventHandler?.Invoke(this, new WinUsbDeviceEventArgs(path, WinUsbDeviceEventType.Remove));
                       });
                       paths.RemoveAll(p => !path_snapshots.Any(pa => p == pa));
                       await Task.Delay(TimeSpan.FromMilliseconds(1000));
                   }
               });
        }
        public void Stop()
        {
            this.CancellationTokenSource?.Cancel();
        }
        ~WinUsbDeviceWatcher()
        {
            this.DeviceConnectEventHandler = null;
            this.DeviceDisconnectEventHandler = null;
            this.CancellationTokenSource = null;
        }
    }
}
