using NLog;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace ComDevice
{
    [Flags]
    public enum MeasuringType
    {
        none,
        dcI,
        acI,
        dcU,
        acU,
        ohm
    }

    [Flags]
    public enum MeasuringQuality : byte
    {
        none,
        high_quality,
        low_quality,
        pos_overbound,
        neg_overbound,
        guard_on
    }

    public class DeviceLogic : IDisposable
    {
        private double acI;
        private double dcI;
        private double acU;
        private double dcU;
        private double ohm;

        public LimitSelector TLimit { get; private set; }
        public TypePicker TType { get; private set; }
        public List<Scale> Scales { get; private set; }

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DeviceLogic(string dev_xml, string conf_xml)
        {
            try
            {
                XElement devXml = XElement.Parse(dev_xml);
                XElement confXml = XElement.Parse(conf_xml);

                XElement limitXElmnt = devXml.Element("limits");
                XElement typeXElmnt = devXml.Element("btns");

                //DeviceType = device.Attribute("type").Value;
                TLimit = new LimitSelector(limitXElmnt);
                TType = new TypePicker(typeXElmnt);

                Scales = new List<Scale>();

                var ss = devXml.Element("scls").Elements("item");
                foreach (XElement item in ss)
                {
                    string type = item.Element("scale").Value;
                    XElement sc = confXml.Element("scls").Elements("item").FirstOrDefault(el => el.Element("scale")?.Value == type);
                    Scales.Add(new Scale(item, sc));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateModel(ComDeviceModel comDeviceModel, ViewDeviceModel veiwDeviceModel)
        {
            try
            {
                string t = TType.GetValue(comDeviceModel.Type.ToString());
                double limit = TLimit.GetValue(comDeviceModel.Selector.ToString());
                string group = TLimit.GetGroup(comDeviceModel.Selector.ToString());

                if (t == "off" || comDeviceModel.Raw_guard != 1)
                {
                    Scales[0].GetZeroAngle(out int hwAngle, out double angle);
                    comDeviceModel.SetArrow(hwAngle);
                    veiwDeviceModel.Arrow = angle;
                    veiwDeviceModel.Selector = double.Parse(TLimit.GetAngle(comDeviceModel.Selector.ToString()), CultureInfo.InvariantCulture);
                    veiwDeviceModel.MQuality = MeasuringQuality.none;
                    veiwDeviceModel.MType = MeasuringType.none;
                    return;
                }

                var currentScale = Scales.Find(sc => sc.Type == t);
                MeasuringQuality quality = currentScale.GetArrowAngle(GetMesuringValue(group, t), limit, out int _hwAngle, out double _angle, out bool isBurn);

                if(isBurn)
                {
                    currentScale.GetMaxAngle(out int maxHwAngle, out double maxAngle);
                    comDeviceModel.SetArrow(maxHwAngle);
                    veiwDeviceModel.Arrow = maxAngle;
                    comDeviceModel.SetBurn();
                    Thread.Sleep(1000);
                    comDeviceModel.SetArrow(_hwAngle);
                    veiwDeviceModel.Arrow = _angle;
                    comDeviceModel.SetProbes(1);
                }
                else
                {
                    comDeviceModel.SetArrow(_hwAngle);
                    veiwDeviceModel.Arrow = _angle;
                }
                veiwDeviceModel.Selector = double.Parse(TLimit.GetAngle(comDeviceModel.Selector.ToString()), CultureInfo.InvariantCulture);
                veiwDeviceModel.MQuality = quality;
                veiwDeviceModel.MType = GetMesuringType(group, t);
            }
            catch (Exception ex)
            {
                comDeviceModel.Arrow = 0;
                veiwDeviceModel.Arrow = 0;
                veiwDeviceModel.MQuality = MeasuringQuality.none;
                veiwDeviceModel.MType = MeasuringType.none;
            }
        }

        public void SelectorNext(ViewDeviceModel veiwDeviceModel)
        {
            veiwDeviceModel.Selector = double.Parse(TLimit.GetNextAngle(), CultureInfo.InvariantCulture);
        }

        public void SelectorPrev(ViewDeviceModel veiwDeviceModel)
        {
            veiwDeviceModel.Selector = double.Parse(TLimit.GetPrevAngle(), CultureInfo.InvariantCulture);
        }

        public void Update(double acI, double dcI, double acU, double dcU, double ohm)
        {
            this.acI = acI;
            this.dcI = dcI;
            this.acU = acU;
            this.dcU = dcU;
            this.ohm = ohm;
        }

        private MeasuringType GetMesuringType(string group, string type)
        {
            if (group == "ohm" && type == "ohm")
                return MeasuringType.ohm;

            if (group == "voltage" && type == "ac")
                return MeasuringType.acU;

            if (group == "voltage" && type == "dc")
                return MeasuringType.dcU;

            if (group == "current" && type == "ac")
                return MeasuringType.acI;

            if (group == "current" && type == "dc")
                return MeasuringType.dcI;

            return MeasuringType.none;
        }

        private double GetMesuringValue(string group, string type)
        {
            if (group == "ohm" && type == "ohm")
                return ohm;

            if (group == "voltage" && type == "ac")
                return acU;

            if (group == "voltage" && type == "dc")
                return dcU;

            if (group == "current" && type == "ac")
                return acI;

            if (group == "current" && type == "dc")
                return dcI;

            return 0;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
