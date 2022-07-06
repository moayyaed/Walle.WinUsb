using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Walle.Components;

namespace Walle.WinUsb
{
    public class WinUsbDevice : Singleton<WinUsbDevice>
    {
        private FileStream? usb_device_io = null;
        private bool is_open = false;

        private const string USB_DEV_GUID = "{A5DCBF10-6530-11D2-901F-00C04FB951ED}";
        private const int MAX_USB_DEVICES = 64;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private IntPtr USB_DEV_HANDLE = new IntPtr(-1);

        public WinUsbDevice()
        {
          
        }

        private static List<string> GetAll_In64BitProcess()
        {
            List<string> result = new List<string>();
            Guid guid = Guid.Parse(USB_DEV_GUID);

            IntPtr deviceInfoSet = Win32Calls_x64.SetupDiGetClassDevs(ref guid, 0, IntPtr.Zero, Win32Calls_x64.DIGCF.DIGCF_PRESENT | Win32Calls_x64.DIGCF.DIGCF_DEVICEINTERFACE);
            if (deviceInfoSet != IntPtr.Zero)
            {
                Win32Calls_x64.SP_DEVICE_INTERFACE_DATA interfaceInfo = new Win32Calls_x64.SP_DEVICE_INTERFACE_DATA();
                interfaceInfo.cbSize = Marshal.SizeOf(interfaceInfo);
                for (uint index = 0; index < MAX_USB_DEVICES; index++)
                {
                    if (Win32Calls_x64.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref guid, index, ref interfaceInfo))
                    {
                        int buffsize = 0;
                        Win32Calls_x64.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref interfaceInfo, IntPtr.Zero, buffsize, ref buffsize, null);
                        IntPtr pDetail = Marshal.AllocHGlobal(buffsize);

                        Win32Calls_x64.SP_DEVICE_INTERFACE_DETAIL_DATA detail = new Win32Calls_x64.SP_DEVICE_INTERFACE_DETAIL_DATA();
                        detail.cbSize = Marshal.SizeOf(typeof(Win32Calls_x64.SP_DEVICE_INTERFACE_DETAIL_DATA));
                        Marshal.StructureToPtr(detail, pDetail, false);

                        if (Win32Calls_x64.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref interfaceInfo, pDetail, buffsize, ref buffsize, null))
                        {
                            var str = Marshal.PtrToStringAuto((IntPtr)((long)pDetail + 4));
                            result.Add(str);
                        }

                        Marshal.FreeHGlobal(pDetail);
                    }
                }
            }
            Win32Calls_x64.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            return result;
        }

        private static List<string> GetAll_In32BitProcess()
        {
            List<string> result = new List<string>();
            Guid guid = Guid.Parse(USB_DEV_GUID);
            IntPtr deviceInfoSet = Win32Calls_x86.SetupDiGetClassDevs(ref guid, 0, IntPtr.Zero, Win32Calls_x86.DIGCF.DIGCF_PRESENT | Win32Calls_x86.DIGCF.DIGCF_DEVICEINTERFACE);
            if (deviceInfoSet != IntPtr.Zero)
            {
                Win32Calls_x86.SP_DEVICE_INTERFACE_DATA interfaceInfo = new Win32Calls_x86.SP_DEVICE_INTERFACE_DATA();
                interfaceInfo.cbSize = Marshal.SizeOf(interfaceInfo);
                for (uint index = 0; index < MAX_USB_DEVICES; index++)
                {
                    if (Win32Calls_x86.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref guid, index, ref interfaceInfo))
                    {
                        int buffsize = 0;
                        Win32Calls_x86.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref interfaceInfo, IntPtr.Zero, buffsize, ref buffsize, null);
                        IntPtr pDetail = Marshal.AllocHGlobal(buffsize);

                        Win32Calls_x86.SP_DEVICE_INTERFACE_DETAIL_DATA detail = new Win32Calls_x86.SP_DEVICE_INTERFACE_DETAIL_DATA();
                        detail.cbSize = Marshal.SizeOf(typeof(Win32Calls_x86.SP_DEVICE_INTERFACE_DETAIL_DATA));
                        Marshal.StructureToPtr(detail, pDetail, false);
                        if (Win32Calls_x86.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref interfaceInfo, pDetail, buffsize, ref buffsize, null))
                        {
                            var str = Marshal.PtrToStringAuto((IntPtr)((int)pDetail + 4));
                            result.Add(str);
                        }
                        Marshal.FreeHGlobal(pDetail);
                    }
                }
            }
            Win32Calls_x64.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            return result;
        }

        public static List<string> GetAll()
        {
            if (Environment.Is64BitProcess)
            {
                return GetAll_In64BitProcess();
            }
            else
            {
                return GetAll_In32BitProcess();
            }
        }

        public bool Open(string vid, string pid)
        {
            List<string> deviceList = GetAll();
            if (deviceList.Count == 0)
                return false;
            var target_item = deviceList.FirstOrDefault(P => P.ToLower().Contains($"vid_{vid}&pid_{pid}"));
            if (target_item != null && is_open == false)
            {
                if (Environment.Is64BitProcess)
                {
                    USB_DEV_HANDLE = Win32Calls_x64.CreateFile(target_item, Win32Calls_x64.DESIREDACCESS.GENERIC_READ | Win32Calls_x64.DESIREDACCESS.GENERIC_WRITE,
                                    0, 0, Win32Calls_x64.CREATIONDISPOSITION.OPEN_EXISTING, 0x40000000, 0);
                }
                else
                {
                    USB_DEV_HANDLE = Win32Calls_x86.CreateFile(target_item, Win32Calls_x86.DESIREDACCESS.GENERIC_READ | Win32Calls_x86.DESIREDACCESS.GENERIC_WRITE,
                                  0, 0, Win32Calls_x86.CREATIONDISPOSITION.OPEN_EXISTING, 0x40000000, 0);
                }
                if (USB_DEV_HANDLE != INVALID_HANDLE_VALUE)
                {
                    usb_device_io = new FileStream(new SafeFileHandle(USB_DEV_HANDLE, false), FileAccess.ReadWrite, 40, true);
                    this.is_open = true;
                    return true;
                }
                Win32Calls_x64.CloseHandle(USB_DEV_HANDLE);
            }
            return false;
        }

        public void Close()
        {
            if (is_open == true)
            {
                is_open = false;
                usb_device_io?.Close();
                if (Environment.Is64BitProcess)
                {
                    Win32Calls_x64.CloseHandle(USB_DEV_HANDLE);
                }
                else
                {
                    Win32Calls_x86.CloseHandle(USB_DEV_HANDLE);
                }

            }
        }

        public void Send(byte[] data)
        {
            try
            {
                if (usb_device_io == null)
                {
                    return;
                }
                usb_device_io.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static bool IsConnected(string vid, string pid)
        {
            List<string> deviceList = GetAll();
            if (deviceList.Count == 0)
                return false;
            var target_item = deviceList.FirstOrDefault(P => P.ToLower().Contains($"vid_{vid}&pid_{pid}"));
            return target_item != null;
        }
    }
}