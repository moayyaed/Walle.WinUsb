using System.Runtime.InteropServices;

namespace Walle.WinUsb
{
    internal class Win32Calls_x64
    {
        #region Win32_api
        public enum DIGCF
        {
            DIGCF_DEFAULT = 0x00000001,         //只返回与系统默认设备相关的设备。
            DIGCF_PRESENT = 0x00000002,         //只返回当前存在的设备。
            DIGCF_ALLCLASSES = 0x00000004,      //返回所有已安装的设备。如果这个标志设置了，ClassGuid参数将被忽略
            DIGCF_PROFILE = 0x00000008,         //只返回当前硬件配置文件中的设备。
            DIGCF_DEVICEINTERFACE = 0x00000010  //返回所有支持的设备。
        }
        
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid interfaceClassGuid;
            public int flags;
            public IntPtr reserved;
        }

        public class SP_DEVINFO_DATA
        {
            public int cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
            public Guid classGuid = Guid.Empty; 
            public int devInst = 0; 
            public IntPtr reserved ;
        }

        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            internal int cbSize;
            internal short devicePath;
        }

        /// <summary>
        /// 获取USB-HID设备的设备接口类GUID，即{4D1E55B2-F16F-11CF-88CB-001111000030}
        /// </summary>
        /// <param name="HidGuid"></param>
        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(ref Guid HidGuid);

        /// <summary>
        /// 获取对应GUID的设备信息集(句柄)
        /// </summary>
        /// <param name="ClassGuid">设备设置类或设备接口类的guid</param>
        /// <param name="Enumerator">指向以空结尾的字符串的指针，该字符串提供PNP枚举器或PNP设备实例标识符的名称</param>
        /// <param name="HwndParent">用于用户界面的顶级窗口的句柄</param>
        /// <param name="Flags">一个变量，指定用于筛选添加到设备信息集中的设备信息元素的控制选项。</param>
        /// <returns>设备信息集的句柄</returns>
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, uint Enumerator, IntPtr HwndParent, DIGCF Flags);

        /// <summary>
        /// 根据句柄，枚举设备信息集中包含的设备接口。
        /// </summary>
        /// <param name="deviceInfoSet"></param>
        /// <param name="deviceInfoData"></param>
        /// <param name="interfaceClassGuid"></param>
        /// <param name="memberIndex"></param>
        /// <param name="deviceInterfaceData"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        /// <summary>
        /// 获取接口详细信息，在第一次主要是读取缓存信息,第二次获取详细信息(必须调用两次)
        /// </summary>
        /// <param name="deviceInfoSet">指向设备信息集的指针，它包含了所要接收信息的接口。该句柄通常由SetupDiGetClassDevs函数返回。</param>
        /// <param name="deviceInterfaceData">返回数据</param>
        /// <param name="deviceInterfaceDetailData"></param>
        /// <param name="deviceInterfaceDetailDataSize"></param>
        /// <param name="requiredSize"></param>
        /// <param name="deviceInfoData"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, SP_DEVINFO_DATA deviceInfoData);

        /// <summary>
        /// 删除设备信息并释放内存。
        /// </summary>
        /// <param name="HIDInfoSet"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);
        #endregion

        #region Open_Device
        /// <summary>
        /// 访问权限
        /// </summary>
        public static class DESIREDACCESS
        {
            public const uint GENERIC_READ = 0x80000000;
            public const uint GENERIC_WRITE = 0x40000000;
            public const uint GENERIC_EXECUTE = 0x20000000;
            public const uint GENERIC_ALL = 0x10000000;
        }
        /// <summary>
        /// 如何创建
        /// </summary>
        public static class CREATIONDISPOSITION
        {
            public const uint CREATE_NEW = 1;
            public const uint CREATE_ALWAYS = 2;
            public const uint OPEN_EXISTING = 3;
            public const uint OPEN_ALWAYS = 4;
            public const uint TRUNCATE_EXISTING = 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpFileName">普通文件名或设备文件名</param>
        /// <param name="desiredAccess">访问模式（写/读） GENERIC_READ、GENERIC_WRITE </param>
        /// <param name="shareMode">共享模式</param>
        /// <param name="securityAttributes">指向安全属性的指针</param>
        /// <param name="creationDisposition">如何创建</param>
        /// <param name="flagsAndAttributes">文件属性</param>
        /// <param name="templateFile">用于复制文件句柄</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint desiredAccess, uint shareMode, uint securityAttributes, uint creationDisposition, uint flagsAndAttributes, uint templateFile);

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="hObject">Handle to an open object</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);

        #endregion
    }
}
