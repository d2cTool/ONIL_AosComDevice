using System;
using System.Diagnostics;
using System.Threading;
using AosComDevice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComDeviceTest
{
    [TestClass]
    public class ComReaderTest
    {
        //[TestMethod]
        public void TestMethod1()
        {
            ComReader comReader = ComReader.Instance;

            comReader.Start();

            comReader.DataReceived += ComReader_DataReceived;
            comReader.LostConnection += ComReader_LostConnection;
            comReader.GetConnection += ComReader_GetConnection;

            while (true)
            {
                Thread.Sleep(0);
            }
        }

        private void ComReader_GetConnection(object sender, ComDeviceInfo e)
        {
            Debug.WriteLine($"Get connection");
        }

        private void ComReader_LostConnection(object sender, EventArgs e)
        {
            Debug.WriteLine($"Lost connection");
        }

        private void ComReader_DataReceived(object sender, ComMsgEventArgs e)
        {
            Debug.WriteLine($"Get msg: {e.Data}");
        }
    }
}
