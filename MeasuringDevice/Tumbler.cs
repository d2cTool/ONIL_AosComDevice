using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace ComDevice
{
    public class TypePicker
    {
        private List<string> Angles;
        private List<string> Values;
        private List<string> Msgs;
        private int CurrentIndex;

        public TypePicker(XElement type)
        {
            Angles = new List<string>();
            Values = new List<string>();
            Msgs = new List<string>();

            var items = type.Descendants("item");

            foreach (var it in items)
            {
                Angles.Add(it.Descendants("btn").First().Value);
                Values.Add(it.Descendants("value").First().Value);
                Msgs.Add(it.Descendants("msg").First().Value);
            }
        }

        public string GetValue(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index != -1 && index < Values.Count)
                CurrentIndex = index;
            return Values[CurrentIndex];
        }

        public string GetAngle(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index != -1 && index < Values.Count)
                CurrentIndex = index;
            return Angles[CurrentIndex];
        }

        public string GetNextAngle()
        {
            if (CurrentIndex < Values.Count - 1)
                ++CurrentIndex;
            else
                CurrentIndex = 0;

            return Angles[CurrentIndex];
        }

        public string GetPrevAngle()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
            else
                CurrentIndex = Values.Count - 1;

            return Angles[CurrentIndex];
        }

        public string GetNextValue()
        {
            if (CurrentIndex < Values.Count - 1)
                ++CurrentIndex;
            else
                CurrentIndex = 0;

            return Values[CurrentIndex];
        }

        public string GetPrevValue()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
            else
                CurrentIndex = Values.Count - 1;

            return Values[CurrentIndex];
        }
    }

    public class LimitSelector
    {
        public List<string> Angles { get; private set; }
        public List<double> Values { get; private set; }
        public List<string> Msgs { get; private set; }
        public List<string> Groups { get; private set; }
        private int CurrentIndex;
        public LimitSelector(XElement limits)
        {
            Angles = new List<string>();
            Values = new List<double>();
            Msgs = new List<string>();
            Groups = new List<string>();

            CultureInfo cultureInfo = new CultureInfo("en");

            var items = limits.Elements();

            foreach (var it in items)
            {
                string str_group = it.Descendants("group").First().Value;
                var lims = it.Descendants("item");
                foreach (var lim in lims)
                {
                    Values.Add(double.Parse(lim.Descendants("limit").First().Value, cultureInfo));
                    Angles.Add(lim.Descendants("angle").First().Value);
                    Msgs.Add(lim.Descendants("msg").First().Value);
                    Groups.Add(str_group);
                }
            }
        }

        public double GetValue(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index != -1 && index < Values.Count)
                CurrentIndex = index;
            return Values[CurrentIndex];
        }

        public string GetAngle(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index != -1 && index < Values.Count)
                CurrentIndex = index;
            return Angles[CurrentIndex];
        }

        public string GetGroup(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index != -1 && index < Values.Count)
                CurrentIndex = index;
            return Groups[CurrentIndex];
        }

        public string GetNextAngle()
        {
            if (CurrentIndex < Values.Count - 1)
                ++CurrentIndex;
            else
                CurrentIndex = 0;

            return Angles[CurrentIndex];
        }

        public string GetPrevAngle()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
            else
                CurrentIndex = Values.Count - 1;

            return Angles[CurrentIndex];
        }

        public double GetNextValue()
        {
            if (CurrentIndex < Values.Count - 1)
                ++CurrentIndex;
            else
                CurrentIndex = 0;

            return Values[CurrentIndex];
        }

        public double GetPrevValue()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
            else
                CurrentIndex = Values.Count - 1;

            return Values[CurrentIndex];
        }
    }
}
