using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using AosComDevice;
using ComDevice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeasuringDeviceTest
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        public void TestMethod1()
        {
            string settings = File.ReadAllText("D:\\Dropbox\\Onil\\Projects\\Measurer Driver\\AosComMeasurer\\MDeviceTest\\base\\base_.xml");
            //IDevice device = new Device_c4352_m1(settings);
        }

        //[TestMethod]
        public void TestMethod2()
        {
            string settings = File.ReadAllText("D:\\Dropbox\\Onil\\Projects\\Measurer Driver\\AosComMeasurer\\MDeviceTest\\base\\base_.xml");
            //Device_c4352_m1 device = new Device_c4352_m1(settings);

            while (true)
            {
                Thread.Sleep(10);
            }
        }

        //[TestMethod]
        public void ParseXmlConfigStr()
        {
            string str = File.ReadAllText(@"C:\Users\optic\Desktop\Ц4352-М1.xml");
            //Device.ParseXmlConfigStr(settings);

            XElement scales = XElement.Parse(str);
            var ss = scales.Descendants("scls").Elements();

            foreach (XElement item in ss)
            {
                string type = item.Descendants("scale").First().Value;
                string max_val = item.Descendants("max_val").First().Value;
                bool can_burn = bool.Parse(item.Descendants("canb_urn").First().Value);

                var arws = item.Descendants("arws").Elements();

                foreach (var it in arws)
                {
                    string value = item.Descendants("value").First().Value;
                    string angle = item.Descendants("angle").First().Value;
                }

                //XElement sc = device.Descendants("scale").FirstOrDefault(el => el.Attribute("type")?.Value == item.Attribute("type")?.Value);
                //Scales.Add(new Scale(sc, item));
            }
        }
    }
}
