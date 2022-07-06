namespace Walle.WinUsb
{
    public delegate void WinUsbDeviceEventHandler(object sender, WinUsbDeviceEventArgs e);
    public class WinUsbDeviceEventArgs
    {
        public string Id { get; private set; } = string.Empty;
        public string Guid { get; private set; } = string.Empty;
        public string Pid { get; private set; } = string.Empty;
        public string Vid { get; private set; } = string.Empty;
        public string Path { get; private set; } = string.Empty;

        public WinUsbDeviceEventType EventType { get; private set; } = WinUsbDeviceEventType.Insert;

        public WinUsbDeviceEventArgs(string path, WinUsbDeviceEventType eventType = WinUsbDeviceEventType.Insert)
        {
            this.Path = path;
            this.Id = path.Split('#')[2];
            this.Guid = path.Split('#')[3];
            this.Vid = path.Split('#')[1].Split('&')[0].Replace("vid_", "");
            this.Pid = path.Split('#')[1].Split('&')[1].Replace("pid_", "");
            this.EventType = eventType;
        }
    }
}
