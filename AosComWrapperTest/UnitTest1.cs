using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using AosComWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComDevice;

namespace AosComWrapperTest
{
    [TestClass]
    public class UnitTest1
    {
        Wrapper wrapper;
        int i = 0;
        string dev = File.ReadAllText("C:\\Users\\optic\\Desktop\\Ц4352-М1.xml");
        string conf = File.ReadAllText("C:\\Users\\optic\\Desktop\\Ц4352-М1 Прибор.xml");


        [TestMethod]
        public void TestMethod1()
        {
            //MeasuringDeviceLogic d = new MeasuringDeviceLogic(dev, conf);

            wrapper = Wrapper.Instance;
            Wrapper.onConnectDelegate = OnConnDelegate;
            Wrapper.onChangedFirmwareDelegate = GetFirmDelegate;
            Wrapper.onDisconnectDelegate = OnDiscDelegate;
            Wrapper.onGetDataDelegate = GetDDelegate;
            Wrapper.onStateChangedDelegate = StateChangedDelegate;
            Wrapper.Init();

            while (true)
            {
                Thread.Sleep(0);
            }
        }

        private void OnDiscDelegate()
        {
            Debug.WriteLine("Disconnected");
        }

        private void OnConnDelegate(string s1, string s2)
        {
            Debug.WriteLine($"Connected dev: {s1}, subdev: {s2}");
            //string dev = File.ReadAllText("C:\\Users\\optic\\Desktop\\Ц4352-М1.xml");
            //string conf = File.ReadAllText("C:\\Users\\optic\\Desktop\\Ц4352-М1 Прибор.xml");
            Wrapper.InitDevice(dev, conf);
            //Debug.WriteLine($"IntPtr: {Wrapper.GetWindowHandle()}");

            //Wrapper.SetAngle(60);
            //Wrapper.SetValue(60);
            //Wrapper.SetLamp(1, 1);
            i = 0;
            Wrapper.ChangeProbeState(1);
        }

        private void GetFirmDelegate(int firmware)
        {
            Debug.WriteLine($"Firmware: {firmware}");
        }

        private void GetDDelegate(string msg)
        {
            if (i == 5)
            {
                //    Wrapper.ChangeProbeState(1);
                //    Wrapper.SetMeasure(1, 100, 10, 100, 1000);
                Wrapper.Stop();
            }
            //Debug.WriteLine($"IntPtr: {Wrapper.GetWindowHandle()}");
            //i++;
            //Debug.WriteLine($"Get msg: {msg}");
        }

        private void StateChangedDelegate(int plus, int minus, byte isDeviceOn)
        {
            Debug.WriteLine($"plus: {plus}, minus: {minus}, isDeviceOn: {isDeviceOn}");
            //Wrapper.SetMeasure(1, 10, 100, 100, 1000);

            Wrapper.SetLamp(i, 1);
            i++;

            Wrapper.SetDemoMode(1, 12, 12, 2);
            //Wrapper.SetAngle(60);
            //Wrapper.SetValue(60);
        }
    }
}
