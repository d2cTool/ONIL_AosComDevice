using AosComDevice;
using System;
using System.Xml.Linq;

namespace ComDevice
{
    public class StateEventArgs : EventArgs
    {
        public int Plus { get; private set; }
        public int Minus { get; private set; }
        public byte IsDeviceOn { get; private set; }
        public StateEventArgs(int plus, int minus, byte isDeviceOn) { Plus = plus; Minus = minus; IsDeviceOn = isDeviceOn; }
    }
    public class DeviceManager : IDisposable
    {
        private readonly ComWorker ComWorker;
        private readonly ViewManager viewManager;
        private readonly DeviceLogic deviceLogic;
        private readonly ComDeviceModel comDeviceModel;

        public event EventHandler<StateEventArgs> ChangedState;

        private bool isDemoMode = false;
        private DemoModeState BackupDeviceState;

        public DeviceManager(ComWorker comWorker, string dev_xml, string conf_xml)
        {
            ComWorker = comWorker;

            XElement devXml = XElement.Parse(dev_xml);
            XElement confXml = XElement.Parse(conf_xml);

            string deviceName = devXml.Attribute("name").Value;
            int deviceType = int.Parse(devXml.Element("type").Value);

            comDeviceModel = new ComDeviceModel(comWorker);
            viewManager = new ViewManager(deviceType);
            deviceLogic = new DeviceLogic(dev_xml, conf_xml);

            comDeviceModel.ChangedState += ComDeviceModel_ChangedState;
            ComWorker.DisconnectDevice += ComWorker_DisconnectDevice;
            ComWorker.ConnectDevice += ComWorker_ConnectDevice;

            viewManager.ViewDeviceModel.SNextPosition += ViewDeviceModel_SNextPosition;
            viewManager.ViewDeviceModel.SPrevPosition += ViewDeviceModel_SPrevPosition;
        }

        public void Stop()
        {
            viewManager?.StopViewThread();
        }

        private void ViewDeviceModel_SPrevPosition(object sender, EventArgs e)
        {
            deviceLogic.SelectorPrev(viewManager.ViewDeviceModel);
        }

        private void ViewDeviceModel_SNextPosition(object sender, EventArgs e)
        {
            deviceLogic.SelectorNext(viewManager.ViewDeviceModel);
        }

        private void ComWorker_ConnectDevice(object sender, ConnectDeviceEventArgs e)
        {
            viewManager.ViewDeviceModel.IsActive = false;
        }

        private void ComWorker_DisconnectDevice(object sender, EventArgs e)
        {
            viewManager.ChangeViewVisibility(1);
            viewManager.ViewDeviceModel.IsActive = true;
        }

        private void ComDeviceModel_ChangedState(object sender, EventArgs e)
        {
            viewManager.ViewDeviceModel.Update(comDeviceModel);
            viewManager.ViewDeviceModel.Update(ComWorker.DeviceSerial, ComWorker.SubDeviceSerial, ComWorker.FirmwareVersion);
            deviceLogic.UpdateModel(comDeviceModel, viewManager.ViewDeviceModel);
            byte isDeviceOn = (comDeviceModel.Raw_guard == 1) ? (byte)1 : (byte)0;
            ChangedState?.Invoke(this, new StateEventArgs(comDeviceModel.Plus, comDeviceModel.Minus, isDeviceOn));
        }

        public int SetValues(double acI, double dcI, double acU, double dcU, double ohm)
        {
            viewManager.ViewDeviceModel.Update(acI, dcI, acU, dcU, ohm);
            deviceLogic.Update(acI, dcI, acU, dcU, ohm);
            deviceLogic.UpdateModel(comDeviceModel, viewManager.ViewDeviceModel);

            int rez = (byte)viewManager.ViewDeviceModel.MType << 8 | (byte)viewManager.ViewDeviceModel.MQuality;
            return rez;
        }

        public void ChangeViewVisibility(byte bShow)
        {
            viewManager.ChangeViewVisibility(bShow);
        }

        public void ChangeDebugInfoVisiability(int state)
        {
            viewManager.ChangeDebugInfoVisibility(state);
        }

        public IntPtr GetWindowHandle()
        {
            return viewManager.GetWindowHandle();
        }

        public void ChangeProbeState(byte value)
        {
            comDeviceModel.SetProbes(value);
        }

        public void SetViewArrow(double angle)
        {
            viewManager.ViewDeviceModel.Arrow = angle;
        }

        public void SetValue(int value)
        {
            comDeviceModel.SetArrow(value);
        }

        public void SetDemoMode(byte isDemo, double arrow, double limit, MeasuringType type)
        {
            if (isDemoMode == false && isDemo == 1)
            {
                //BackupDeviceModel = ObjectCopier.Clone(viewManager.ViewDeviceModel);
                BackupDeviceState = new DemoModeState(viewManager.ViewDeviceModel);
            }


            if (isDemoMode == true && isDemo == 0)
                BackupDeviceState.SetToModel(viewManager.ViewDeviceModel);

            if (isDemo == 0)
                isDemoMode = false;

            if (isDemo == 1)
            {
                isDemoMode = true;

                viewManager.ViewDeviceModel.Arrow = arrow;
                viewManager.ViewDeviceModel.Selector = limit;

                switch (type)
                {
                    case MeasuringType.acI:
                    case MeasuringType.acU:
                        viewManager.ViewDeviceModel.OhmBtn = 1;
                        viewManager.ViewDeviceModel.AcBtn = 0;
                        viewManager.ViewDeviceModel.DcBtn = 1;
                        break;
                    case MeasuringType.dcI:
                    case MeasuringType.dcU:
                        viewManager.ViewDeviceModel.OhmBtn = 1;
                        viewManager.ViewDeviceModel.AcBtn = 1;
                        viewManager.ViewDeviceModel.DcBtn = 0;
                        break;
                    case MeasuringType.ohm:
                        viewManager.ViewDeviceModel.OhmBtn = 0;
                        viewManager.ViewDeviceModel.AcBtn = 1;
                        viewManager.ViewDeviceModel.DcBtn = 1;
                        break;
                    case MeasuringType.none:
                    default:
                        viewManager.ViewDeviceModel.OhmBtn = 1;
                        viewManager.ViewDeviceModel.AcBtn = 1;
                        viewManager.ViewDeviceModel.DcBtn = 1;
                        break;
                }
            }
        }

        public void Dispose()
        {
            comDeviceModel.ChangedState -= ComDeviceModel_ChangedState;
            ComWorker.DisconnectDevice -= ComWorker_DisconnectDevice;
            ComWorker.ConnectDevice -= ComWorker_ConnectDevice;

            viewManager.ViewDeviceModel.SNextPosition -= ViewDeviceModel_SNextPosition;
            viewManager.ViewDeviceModel.SPrevPosition -= ViewDeviceModel_SPrevPosition;

            comDeviceModel.Dispose();
            viewManager.Dispose();
            deviceLogic.Dispose();
        }

    }
}
