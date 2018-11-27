using System.Threading;
using AosComDevice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComDeviceTest
{
    [TestClass]
    public class ComDeviceTest
    {
        private ComDeviceInfoManager deviceManager;

        //[TestMethod]
        public void TestMethod1()
        {
            deviceManager = new ComDeviceInfoManager();
            deviceManager.PropertyChanged += DeviceManager_PropertyChanged;

            while (true)
            {
                Thread.Sleep(10);
            }
        }

        private void DeviceManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //var dict = deviceManager.COMDeviceDict;
        }
    }
}
