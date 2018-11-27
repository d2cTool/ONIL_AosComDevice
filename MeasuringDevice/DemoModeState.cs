using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComDevice
{
    public class DemoModeState
    {
        public double Arrow { get; set; }
        public double Limit { get; set; }
        public int Ac { get; set; }
        public int Dc { get; set; }
        public int Ohm { get; set; }
        public DemoModeState(ViewDeviceModel model)
        {
            Arrow = model.Arrow;
            Limit = model.Selector;
            Ac = model.AcBtn;
            Dc = model.DcBtn;
            Ohm = model.OhmBtn;
        }

        public void SetToModel(ViewDeviceModel model)
        {
            model.Arrow = Arrow;
            model.Selector = Limit;
            model.AcBtn = Ac;
            model.DcBtn = Dc;
            model.OhmBtn = Ohm;
        }
    }
}
