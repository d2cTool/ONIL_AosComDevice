using System;
using System.Diagnostics;
using System.Threading;
using AosComDevice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComDeviceTest
{
    [TestClass]
    public class ComWorkerTest
    {
        //[TestMethod]
        public void TestMethod1()
        {
            ComWorker comWorker = new ComWorker(ComReader.Instance);

            comWorker.ConnectDevice += ComWorker_ConnectDevice;
            comWorker.DisconnectDevice += ComWorker_DisconnectDevice;
            comWorker.GetMessage += ComWorker_GetMessage;

            comWorker.Start();

            while (true)
            {
                Thread.Sleep(10);
            }
        }

        private void ComWorker_GetMessage(object sender, ComMsgEventArgs e)
        {
            Debug.WriteLine($"Get msg: {e.Data}");
        }

        private void ComWorker_DisconnectDevice(object sender, EventArgs e)
        {
            Debug.WriteLine($"Lost connection");
        }

        private void ComWorker_ConnectDevice(object sender, ConnectDeviceEventArgs e)
        {
            Debug.WriteLine($"Connected dev: {e.DeviceSerial}, subdev: {e.SubDeviceSerial}");
        }
    }
}
