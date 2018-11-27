using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ComDevice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeasuringDeviceTest
{
    [TestClass]
    public class TumblerTest
    {
        #region TestData
        string btns = @"
            <btns>
		        <item>
			        <btn>1</btn>
			        <value>dc</value>
			        <msg>1</msg>
		        </item>
		        <item>
			        <btn>3</btn>
			        <value>ac</value>
			        <msg>3</msg>
		        </item>
		        <item>
			        <btn>2</btn>
			        <value>ohm</value>
			        <msg>2</msg>
		        </item>
		        <item>
			        <btn>0</btn>
			        <value>off</value>
			        <msg>0</msg>
		        </item>
	        </btns>
        ";
        string limits = @"
        <limits>
		<item>
			<group>special</group>
			<lims>
				<item>
					<limit>0</limit>
					<angle>0</angle>
					<msg>48</msg>
				</item>
				<item>
					<limit>0.075</limit>
					<angle>136</angle>
					<msg>34</msg>
				</item>
			</lims>
		</item>
		<item>
			<group>current</group>
			<lims>
				<item>
					<limit>6</limit>
					<angle>14</angle>
					<msg>44</msg>
				</item>
				<item>
					<limit>1.5</limit>
					<angle>28</angle>
					<msg>43</msg>
				</item>
				<item>
					<limit>0.6</limit>
					<angle>44</angle>
					<msg>42</msg>
				</item>
				<item>
					<limit>0.15</limit>
					<angle>60</angle>
					<msg>41</msg>
				</item>
				<item>
					<limit>0.006</limit>
					<angle>74</angle>
					<msg>35</msg>
				</item>
				<item>
					<limit>0.0015</limit>
					<angle>90</angle>
					<msg>36</msg>
				</item>
				<item>
					<limit>0.0006</limit>
					<angle>104</angle>
					<msg>37</msg>
				</item>
				<item>
					<limit>0.00015</limit>
					<angle>120</angle>
					<msg>38</msg>
				</item>
			</lims>
		</item>
		<item>
			<group>ohm</group>
			<lims>
				<item>
					<limit>1000000</limit>
					<angle>150</angle>
					<msg>33</msg>
				</item>
				<item>
					<limit>100000</limit>
					<angle>164</angle>
					<msg>32</msg>
				</item>
				<item>
					<limit>10000</limit>
					<angle>180</angle>
					<msg>31</msg>
				</item>
				<item>
					<limit>1000</limit>
					<angle>194</angle>
					<msg>25</msg>
				</item>
				<item>
					<limit>1</limit>
					<angle>210</angle>
					<msg>26</msg>
				</item>
			</lims>
		</item>
		<item>
			<group>voltage</group>
			<lims>
				<item>
					<limit>0.3</limit>
					<angle>222</angle>
					<msg>27</msg>
				</item>
				<item>
					<limit>1.5</limit>
					<angle>238</angle>
					<msg>28</msg>
				</item>
				<item>
					<limit>6</limit>
					<angle>254</angle>
					<msg>24</msg>
				</item>
				<item>
					<limit>15</limit>
					<angle>270</angle>
					<msg>23</msg>
				</item>
				<item>
					<limit>30</limit>
					<angle>284</angle>
					<msg>22</msg>
				</item>
				<item>
					<limit>60</limit>
					<angle>300</angle>
					<msg>21</msg>
				</item>
				<item>
					<limit>150</limit>
					<angle>316</angle>
					<msg>16</msg>
				</item>
				<item>
					<limit>300</limit>
					<angle>330</angle>
					<msg>17</msg>
				</item>
				<item>
					<limit>600</limit>
					<angle>344</angle>
					<msg>18</msg>
				</item>
			</lims>
		</item>
	</limits>
        ";
        #endregion

        [TestMethod]
        public void TypePickerCreation()
        {
            TypePicker typePicker = new TypePicker(XElement.Parse(btns));
            Assert.AreEqual(typePicker.GetAngle("1"), "1");
            Assert.AreEqual(typePicker.GetValue("1"), "dc");
            Assert.AreEqual(typePicker.GetNextAngle(), "3");
            Assert.AreEqual(typePicker.GetNextValue(), "ohm");
        }

        [TestMethod]
        public void LimitSelectorCreation()
        {
            LimitSelector limitSelector = new LimitSelector(XElement.Parse(limits));
            Assert.AreEqual(limitSelector.GetGroup("48"), "special");
            Assert.AreEqual(limitSelector.GetAngle("34"), "136");
            Assert.AreEqual(limitSelector.GetValue("34"), 0.075);
            Assert.AreEqual(limitSelector.Groups.Count, 24);
            Assert.AreEqual(limitSelector.GetNextAngle(), "14");
        }
    }
}
