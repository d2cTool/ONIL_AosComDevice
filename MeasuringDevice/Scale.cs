using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;
using System;

namespace ComDevice
{
    public class Scale
    {
        public string Type { get; private set; }
        public List<double> Values { get; private set; }
        public double MaxValue { get; private set; }
        public bool CanBurn { get; private set; }
        public List<string> StrValues { get; private set; }
        public List<double> Angles { get; private set; }
        public List<int> HwAngles { get; private set; }
        public Scale(XElement view_scale, XElement hw_scale)
        {
            StrValues = new List<string>();
            Values = new List<double>();
            Angles = new List<double>();
            HwAngles = new List<int>();

            CultureInfo cultureInfo = new CultureInfo("en");

            Type = view_scale.Element("scale").Value;
            MaxValue = double.Parse(view_scale.Descendants("max_val").First().Value, cultureInfo);
            CanBurn = bool.Parse(view_scale.Descendants("canb_urn").First().Value);
            var view_arws = view_scale.Descendants("arws").Elements();

            foreach (var it in view_arws)
            {
                string str_value = it.Descendants("value").First().Value;
                double double_value;
                switch (str_value)
                {
                    case "+inf":
                        double_value = double.PositiveInfinity;
                        break;
                    case "-inf":
                        double_value = double.NegativeInfinity;
                        break;
                    default:
                        double_value = double.Parse(str_value, CultureInfo.InvariantCulture);
                        break;
                }

                double view_angle = double.Parse(it.Descendants("angle").First().Value, cultureInfo);

                var rez = hw_scale.Descendants("arws").Elements().FirstOrDefault(item => item.Descendants("value").First().Value == str_value);
                int hw_angle = int.Parse(rez.Descendants("hw_angle").First().Value);

                StrValues.Add(str_value);
                Values.Add(double_value);
                Angles.Add(view_angle);
                HwAngles.Add(hw_angle);
            }
        }

        public MeasuringQuality GetArrowAngle(double value, double limit, out int hwAngle, out double angle, out bool isBurn)
        {
            double relativeValue = value * MaxValue / limit;
            isBurn = false;

            GetNearest(relativeValue, out int prevIndex, out int nextIndex);

            double prevValue = Values[prevIndex];
            double nextValue = Values[nextIndex];

            double prevAngle = Angles[prevIndex];
            double nextAngle = Angles[nextIndex];

            int prevHwAngle = HwAngles[prevIndex];
            int nextHwAngle = HwAngles[nextIndex];

            #region Inf
            if (double.IsPositiveInfinity(prevValue))
            {
                double rel = Math.Abs(nextValue - relativeValue);

                if (rel > MaxValue && CanBurn)
                {
                    GetZeroAngle(out hwAngle, out angle);
                    isBurn = true;
                    return MeasuringQuality.guard_on;
                }

                if (rel > MaxValue && !CanBurn)
                {
                    angle = nextAngle;
                    hwAngle = nextHwAngle;
                    return MeasuringQuality.pos_overbound;
                }

                double dif = rel / (MaxValue - nextValue);
                angle = nextAngle - (nextAngle - prevAngle) * dif; 
                hwAngle = Convert.ToInt32(Math.Round(nextHwAngle - (nextHwAngle - prevHwAngle) * dif));

                return MeasuringQuality.pos_overbound; 
            }

            if (double.IsNegativeInfinity(prevValue))
            {
                double rel = Math.Abs(nextValue - relativeValue);

                if (rel > MaxValue && CanBurn)
                {
                    GetZeroAngle(out hwAngle, out angle);
                    isBurn = true;
                    return MeasuringQuality.guard_on;
                }

                if (rel > MaxValue && !CanBurn)
                {
                    angle = prevAngle;
                    hwAngle = prevHwAngle;
                    return MeasuringQuality.neg_overbound;
                }

                double dif = rel / (MaxValue - nextValue);
                angle = nextAngle - (nextAngle - prevAngle) * dif; 
                hwAngle = Convert.ToInt32(Math.Round(nextHwAngle - (nextHwAngle - prevHwAngle) * dif));

                return MeasuringQuality.neg_overbound; 
            }

            if (double.IsPositiveInfinity(nextValue))
            {
                double rel = Math.Abs(prevValue - relativeValue);

                if (rel > MaxValue && CanBurn)
                {
                    GetZeroAngle(out hwAngle, out angle);
                    isBurn = true;
                    return MeasuringQuality.guard_on;
                }

                if (rel > MaxValue && !CanBurn)
                {
                    angle = nextAngle;
                    hwAngle = nextHwAngle;
                    return MeasuringQuality.pos_overbound;
                }

                double dif = rel / (2 * MaxValue - prevValue);
                angle = prevAngle + (nextAngle - prevAngle) * dif; 
                hwAngle = Convert.ToInt32(Math.Round(prevHwAngle + (nextHwAngle - prevHwAngle) * dif));

                return MeasuringQuality.pos_overbound;
            }

            if (double.IsNegativeInfinity(nextValue))
            {
                double rel = Math.Abs(prevValue - relativeValue);

                if (rel > MaxValue && CanBurn)
                {
                    GetZeroAngle(out hwAngle, out angle);
                    isBurn = true;
                    return MeasuringQuality.guard_on;
                }

                if (rel > MaxValue && !CanBurn)
                {
                    angle = nextAngle;
                    hwAngle = nextHwAngle;
                    return MeasuringQuality.pos_overbound;
                }

                double dif = rel / (2 * MaxValue - prevValue);
                angle = prevAngle + (nextAngle - prevAngle) * dif; 
                hwAngle = Convert.ToInt32(Math.Round(prevHwAngle + (nextHwAngle - prevHwAngle) * dif));

                return MeasuringQuality.pos_overbound;
            }
            #endregion

            hwAngle = HwAngleInter(relativeValue, prevIndex, nextIndex);
            angle = AngleInter(relativeValue, prevIndex, nextIndex);

            if (prevIndex == Values.Count - 1 || nextIndex == Values.Count - 1 || prevIndex == 1 || nextIndex == 1)
                return MeasuringQuality.low_quality;
            else
                return MeasuringQuality.high_quality; 
        }

        public void GetZeroAngle(out int hwAngle, out double angle)
        {
            var zeroIndex = Values.FindIndex(el => el == 0);
            hwAngle = HwAngles[zeroIndex];
            angle = Angles[zeroIndex];
        }

        public void GetMaxAngle(out int hwAngle, out double angle)
        {
            var index = Values.FindIndex(el => el == MaxValue);
            hwAngle = HwAngles[index];
            angle = Angles[index];
        }

        public void GetNearest(double value, out int prevIndex, out int nextIndex)
        {
            double closest = Values.OrderBy(item => Math.Abs(value - item)).First();

            if (value == closest)
            {
                nextIndex = prevIndex = Values.FindIndex(el => el == closest);
                return;
            }

            if (value < closest)
            {
                nextIndex = Values.FindIndex(el => el == closest);
                prevIndex = nextIndex - 1;
            }
            else
            {
                prevIndex = Values.FindIndex(el => el == closest);
                nextIndex = prevIndex + 1;
            }
        }

        private int HwAngleInter(double value, int prevIndex, int nextIndex)
        {
            double prevValue = Values[prevIndex];
            double nextValue = Values[nextIndex];

            int prevAngle = HwAngles[prevIndex];
            int nextAngle = HwAngles[nextIndex];

            if (prevIndex == nextIndex)
                return prevAngle;

            double dif = (value - prevValue) / (nextValue - prevValue);

            return Convert.ToInt32(Math.Round(prevAngle + (nextAngle - prevAngle) * dif));
        }

        private double AngleInter(double value, int prevIndex, int nextIndex)
        {
            double prevValue = Values[prevIndex];
            double nextValue = Values[nextIndex];

            double prevAngle = Angles[prevIndex];
            double nextAngle = Angles[nextIndex];

            if (prevIndex == nextIndex)
                return prevAngle;

            double dif = (value - prevValue) / (nextValue - prevValue);
            return prevAngle + (nextAngle - prevAngle) * dif;
        }
    }
}
