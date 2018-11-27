using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ComDevice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeasuringDeviceTest
{
    [TestClass]
    public class ScaleTest
    {
        #region TestData
        string view_ac_scale = @"
           <item>
			<scale>ac</scale>
			<max_val>30</max_val>
			<canb_urn>true</canb_urn>
			<arws>
				<item>
					<value>-inf</value>
					<angle>-5</angle>
				</item>
				<item>
					<value>0</value>
					<angle>0</angle>
				</item>
				<item>
					<value>5</value>
					<angle>16.3</angle>
				</item>
				<item>
					<value>10</value>
					<angle>30.3</angle>
				</item>
				<item>
					<value>15</value>
					<angle>44.6</angle>
				</item>
				<item>
					<value>20</value>
					<angle>58.8</angle>
				</item>
				<item>
					<value>25</value>
					<angle>73</angle>
				</item>
				<item>
					<value>30</value>
					<angle>89.3</angle>
				</item>
				<item>
					<value>+inf</value>
					<angle>96.5</angle>
				</item>
			</arws>
		</item>";

        string hw_ac_scale = @"
            <item>
              <scale>ac</scale>
              <arws>
                <item>
                  <value>-inf</value>
                  <hw_angle>-10</hw_angle>
                </item>
                <item>
                  <value>0</value>
                  <hw_angle>0</hw_angle>
                </item>
                <item>
                  <value>5</value>
                  <hw_angle>27</hw_angle>
                </item>
                <item>
                  <value>10</value>
                  <hw_angle>56</hw_angle>
                </item>
                <item>
                  <value>15</value>
                  <hw_angle>87</hw_angle>
                </item>
                <item>
                  <value>20</value>
                  <hw_angle>116</hw_angle>
                </item>
                <item>
                  <value>25</value>
                  <hw_angle>146</hw_angle>
                </item>
                <item>
                  <value>30</value>
                  <hw_angle>175</hw_angle>
                </item>
                <item>
                  <value>+inf</value>
                  <hw_angle>190</hw_angle>
                </item>
              </arws>
            </item>";

        string view_ohm_scale = @"
           <item>
			<scale>ohm</scale>
			<max_val>5</max_val>
			<canb_urn>false</canb_urn>
			<arws>
				<item>
					<value>-inf</value>
					<angle>96.5</angle>
				</item>
				<item>
					<value>0</value>
					<angle>89.2</angle>
				</item>
				<item>
					<value>0.01</value>
					<angle>85.5</angle>
				</item>
				<item>
					<value>0.1</value>
					<angle>59.5</angle>
				</item>
				<item>
					<value>0.5</value>
					<angle>25.6</angle>
				</item>
				<item>
					<value>1</value>
					<angle>14.6</angle>
				</item>
				<item>
					<value>5</value>
					<angle>4</angle>
				</item>
				<item>
					<value>+inf</value>
					<angle>0</angle>
				</item>
			</arws>
		</item>";

        string hw_ohm_scale = @"
            <item>
              <scale>ohm</scale>
              <arws>
                <item>
                  <value>-inf</value>
                  <hw_angle>190</hw_angle>
                </item>
                <item>
                  <value>0</value>
                  <hw_angle>175</hw_angle>
                </item>
                <item>
                  <value>0.01</value>
                  <hw_angle>169</hw_angle>
                </item>
                <item>
                  <value>0.1</value>
                  <hw_angle>122</hw_angle>
                </item>
                <item>
                  <value>0.5</value>
                  <hw_angle>53</hw_angle>
                </item>
                <item>
                  <value>1</value>
                  <hw_angle>30</hw_angle>
                </item>
                <item>
                  <value>5</value>
                  <hw_angle>6</hw_angle>
                </item>
                <item>
                  <value>+inf</value>
                  <hw_angle>0</hw_angle>
                </item>
              </arws>
            </item>";
        #endregion

        [TestMethod]
        public void CreationTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en");

            XElement view_scale = XElement.Parse(view_ac_scale);
            XElement hw_scale = XElement.Parse(hw_ac_scale);

            Scale scale = new Scale(view_scale, hw_scale);

            Assert.AreEqual(scale.MaxValue, 30);
            Assert.AreEqual(scale.Type, "ac");
            Assert.AreEqual(scale.CanBurn, true);
            Assert.AreEqual(scale.HwAngles[0], -10);
            Assert.AreEqual(scale.Angles[0], -5);
            Assert.AreEqual(scale.HwAngles[1], 0);
            Assert.AreEqual(scale.Angles[1], 0);
            Assert.IsTrue(double.IsNegativeInfinity(scale.Values[0]));
        }

        [TestMethod]
        public void CalculationAcTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en");

            XElement view_scale = XElement.Parse(view_ac_scale);
            XElement hw_scale = XElement.Parse(hw_ac_scale);

            Scale scale = new Scale(view_scale, hw_scale);

            //int quality = scale.GetArrowAngle(10, 30, out int hwA, out double viewA, out bool isB);
            //Assert.AreEqual(hwA, 56);
            //Assert.AreEqual(viewA, 30.3);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 1);

            //quality = scale.GetArrowAngle(1, 300, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, 1);
            //Assert.AreEqual(viewA, 0.326);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 2);

            //quality = scale.GetArrowAngle(59, 30, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, 190);
            //Assert.AreEqual(viewA, 96.26);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 3);

            //quality = scale.GetArrowAngle(61, 30, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, 0);
            //Assert.AreEqual(viewA, 0);
            //Assert.AreEqual(isB, true);
            //Assert.AreEqual(quality, 5);

            //quality = scale.GetArrowAngle(-5, 30, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, -2);
            //Assert.AreEqual(viewA, -0.83333333333333326);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 4);

            //quality = scale.GetArrowAngle(-35, 30, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, 0);
            //Assert.AreEqual(viewA, 0);
            //Assert.AreEqual(isB, true);
            //Assert.AreEqual(quality, 5);

        }

        [TestMethod]
        public void CalculationOhmTest()
        {
            CultureInfo cultureInfo = new CultureInfo("en");

            XElement view_scale = XElement.Parse(view_ohm_scale);
            XElement hw_scale = XElement.Parse(hw_ohm_scale);

            Scale scale = new Scale(view_scale, hw_scale);

            //int quality = scale.GetArrowAngle(3, 10, out int hwA, out double viewA, out bool isB);
            //Assert.AreEqual(hwA, 27);
            //Assert.AreEqual(viewA, 13.275);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 1);

            //quality = scale.GetArrowAngle(9, 5, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, 1);
            //Assert.AreEqual(viewA, 0.79999999999999982);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 3);

            //quality = scale.GetArrowAngle(25, 10, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, 0);
            //Assert.AreEqual(viewA, 0);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 3);

            //quality = scale.GetArrowAngle(-10, 10, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, 190);
            //Assert.AreEqual(viewA, 96.5);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 4);

            //quality = scale.GetArrowAngle(-50, 5, out hwA, out viewA, out isB);
            //Assert.AreEqual(hwA, 190);
            //Assert.AreEqual(viewA, 96.5);
            //Assert.AreEqual(isB, false);
            //Assert.AreEqual(quality, 4);

        }
    }
}
