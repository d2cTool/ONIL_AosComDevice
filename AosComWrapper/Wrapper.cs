using NLog;
using System;
using System.Runtime.InteropServices;
using AosComDevice;
using ComDevice;
using System.Xml.Linq;

namespace AosComWrapper
{
    public class Wrapper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static volatile Wrapper instance;

        public delegate void OnConnectDelegate(string deviceSerial, string subdeviceSerial);
        public delegate void OnDisconnectDelegate();
        public delegate void OnStateChangedDelegate(int plus, int minus, byte isDeviceOn);
        public delegate void OnChangedFirmwareDelegate(int version);
        public delegate void OnGetDataDelegate(string msg);

        public static OnConnectDelegate onConnectDelegate;
        public static OnDisconnectDelegate onDisconnectDelegate;
        public static OnStateChangedDelegate onStateChangedDelegate;
        public static OnChangedFirmwareDelegate onChangedFirmwareDelegate;
        public static OnGetDataDelegate onGetDataDelegate;

        public static ComReader comReader;
        public static ComWorker comWorker;
        public static DeviceManager DeviceManager;

        private Wrapper() { }

        public static Wrapper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Wrapper();
                }
                return instance;
            }
        }

        #region events
        private static void Worker_DisconnectDevice(object sender, EventArgs e)
        {
            onDisconnectDelegate?.Invoke();
        }

        private static void Worker_ConnectDevice(object sender, ConnectDeviceEventArgs e)
        {
            onConnectDelegate?.Invoke(e.DeviceSerial, e.SubDeviceSerial);
            comWorker.GetMessage += ComWorker_GetMessage;
        }

        private static void Worker_ChangeFirmwareVersion(object sender, FirmwareVersionEventArgs e)
        {
            onChangedFirmwareDelegate?.Invoke(e.Version);
        }

        private static void ComWorker_GetMessage(object sender, ComMsgEventArgs e)
        {
            onGetDataDelegate?.Invoke(e.Data);
            //device?.Parse(e.Data);
            //onStateChangedDelegate?.Invoke(device.Plus, device.Minus);
        }

        private static void Device_UpdatedComDevice(object sender, EventArgs e)
        {
            //device?.UpdateView();
            //onStateChangedDelegate?.Invoke(device.Plus, device.Minus);
        }
        #endregion events

        [DllExport(CallingConvention.Cdecl)]
        public static void Init()
        {
            comReader = ComReader.Instance;
            comWorker = new ComWorker(comReader);

            comWorker.ChangeFirmwareVersion += Worker_ChangeFirmwareVersion;
            comWorker.ConnectDevice += Worker_ConnectDevice;
            comWorker.DisconnectDevice += Worker_DisconnectDevice;
            comWorker.Start();
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void Stop()
        {
            comWorker.ChangeFirmwareVersion -= Worker_ChangeFirmwareVersion;
            comWorker.ConnectDevice -= Worker_ConnectDevice;
            comWorker.DisconnectDevice -= Worker_DisconnectDevice;
            comWorker.Stop();

            DeviceManager?.Dispose();
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void InitDevice(string dev, string conf)
        {
            //XElement devXml = XElement.Parse(dev);
            //XElement confXml = XElement.Parse(conf);

            //string deviceName = devXml.Attribute("name").Value;
            //int deviceType = int.Parse(devXml.Attribute("type").Value);

            //DeviceManager?.Stop();

            comWorker.ClearDeviceState();

            DeviceManager?.Dispose();
            DeviceManager = new DeviceManager(comWorker, dev, conf);
            DeviceManager.ChangedState += DeviceManager_ChangedState;
        }

        private static void DeviceManager_ChangedState(object sender, StateEventArgs e)
        {
            onStateChangedDelegate?.Invoke(e.Plus, e.Minus, e.IsDeviceOn);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void Send(string str)
        {
            comReader?.Send(str);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static byte ShowDevice(byte bShow)
        {
            DeviceManager?.ChangeViewVisibility(bShow);
            return bShow;
        }

        [DllExport(CallingConvention.Cdecl)]
        public static int IsDebugInfoHidden(int state)
        {
            DeviceManager?.ChangeDebugInfoVisiability(state);
            return state;
        }

        [DllExport(CallingConvention.Cdecl)]
        public static int GetFirmware()
        {
            return comWorker?.FirmwareVersion ?? 0;  
        }

        [DllExport(CallingConvention.Cdecl)]
        public static IntPtr GetWindowHandle()
        {
            return DeviceManager?.GetWindowHandle() ?? IntPtr.Zero;
        }

        [DllExport(CallingConvention.Cdecl)]
        public static int UpdateFirmware(string hexFullName)
        {
            comWorker?.FirmwareUpdate(hexFullName);
            return comWorker?.FirmwareVersion ?? 0; 
        }

        [DllExport(CallingConvention.Cdecl)]
        public static int SetLamp(int index, byte value)
        {
            comWorker?.SetLamp(index, value);
            return value;
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void PlayMSound(int track)
        {
            comWorker?.PlayMsound(track);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void SetAngle(double angle)
        {
            DeviceManager?.SetViewArrow(angle);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void SetValue(int value)
        {
            DeviceManager?.SetValue(value);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static byte ChangeProbeState(byte value)
        {
            DeviceManager?.ChangeProbeState(value);
            return value;
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void SetDemoMode(byte isDemo, double arrow, double limit, int type)
        {
            DeviceManager?.SetDemoMode(isDemo, arrow, limit, (MeasuringType)type);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static int SetMeasure(double acI, double dcI, double acU, double dcU, double ohm)
        {
            //logger.Debug($"acI:{acI}, dcI:{dcI}, acU:{acU}, dcU:{dcU}, ohm:{ohm}");
            return DeviceManager?.SetValues(acI, dcI, acU, dcU, ohm) ?? 0;
        }

        [DllExport("OnGetDataCallbackFunction", CallingConvention.Cdecl)]
        public static bool OnGetDataCallbackFunction(IntPtr callback)
        {
            //logger.Debug($"OnGetDataCallbackFunction");
            onGetDataDelegate = (OnGetDataDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(OnGetDataDelegate));
            return true;
        }

        [DllExport("OnConnectCallbackFunction", CallingConvention.Cdecl)]
        public static bool OnConnectCallbackFunction(IntPtr callback)
        {
            //logger.Debug($"OnConnectCallbackFunction");
            onConnectDelegate = (OnConnectDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(OnConnectDelegate));
            return true;
        }

        [DllExport("OnDisconnectCallbackFunction", CallingConvention.Cdecl)]
        public static bool OnDisconnectCallbackFunction(IntPtr callback)
        {
            //logger.Debug($"OnDisconnectCallbackFunction");
            onDisconnectDelegate = (OnDisconnectDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(OnDisconnectDelegate));
            return true;
        }

        [DllExport("OnStateChangedCallbackFunction", CallingConvention.Cdecl)]
        public static bool OnStateChangedCallbackFunction(IntPtr callback)
        {
            //logger.Debug($"OnStateChangedCallbackFunction");
            onStateChangedDelegate = (OnStateChangedDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(OnStateChangedDelegate));
            return true;
        }

        [DllExport("OnChangedFirmwareCallbackFunction", CallingConvention.Cdecl)]
        public static bool OnChangedFirmwareCallbackFunction(IntPtr callback)
        {
            //logger.Debug($"OnChangedFirmwareCallbackFunction");
            onChangedFirmwareDelegate = (OnChangedFirmwareDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(OnChangedFirmwareDelegate));
            return true;
        }
    }
}
