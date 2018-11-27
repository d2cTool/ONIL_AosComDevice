using AosComDevice;
using NLog;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ComDevice
{
    [Serializable]
    public class ViewDeviceModel : INotifyPropertyChanged
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler SNextPosition;
        public event EventHandler SPrevPosition;

        private bool isActive = false;
        private Visibility debugInfoVisibility = Visibility.Hidden;

        private double arrow = 0;
        private double selector = 0;

        private int acBtn = 1;
        private int dcBtn = 1;
        private int ohmBtn = 1;

        private int guardOnBtn = 1;
        private int guardOffBtn = 1;

        private int plus = 0;
        private int minus = 0;

        private string deviceSerial = string.Empty;
        private string subDeviceSerial = string.Empty;
        private int firmwareVersion = 0;

        public ICommand DcBtnClick { get; set; }
        public ICommand AcBtnClick { get; set; }
        public ICommand OhmBtnClick { get; set; }
        public ICommand GuardOnBtnClick { get; set; }
        public ICommand GuardOffBtnClick { get; set; }
        public ICommand SelectorNextPosition { get; set; }
        public ICommand SelectorPrevPosition { get; set; }

        private double _acI = 0;
        private double _dcI = 0;
        private double _acU = 0;
        private double _dcU = 0;
        private double _ohm = 0;

        private MeasuringType mType;
        private MeasuringQuality mQuality;

        public ViewDeviceModel()
        {
            DcBtnClick = new SimpleCommand(obj => isActive, obj => { DcBtn = 0; AcBtn = 1; OhmBtn = 1; });
            AcBtnClick = new SimpleCommand(obj => isActive, obj => { DcBtn = 1; AcBtn = 0; OhmBtn = 1; });
            OhmBtnClick = new SimpleCommand(obj => isActive, obj => { DcBtn = 1; AcBtn = 1; OhmBtn = 0; });
            GuardOnBtnClick = new SimpleCommand(obj => isActive, obj => { GuardOnBtn = 0; GuardOffBtn = 1; });
            GuardOffBtnClick = new SimpleCommand(obj => isActive, obj => { GuardOnBtn = 1; GuardOffBtn = 0; });
            SelectorNextPosition = new SimpleCommand(obj => isActive, obj => { SNextPosition?.Invoke(this, EventArgs.Empty); });
            SelectorPrevPosition = new SimpleCommand(obj => isActive, obj => { SPrevPosition?.Invoke(this, EventArgs.Empty); });
        }

        public void Update(ComDeviceModel comDeviceModel)
        {
            Plus = comDeviceModel.Plus;
            Minus = comDeviceModel.Minus;

            switch (comDeviceModel.Type)
            {
                case 0:
                    AcBtn = 1;
                    DcBtn = 1;
                    OhmBtn = 1;
                    break;
                case 1:
                    AcBtn = 1;
                    DcBtn = 0;
                    OhmBtn = 1;
                    break;
                case 2:
                    AcBtn = 1;
                    DcBtn = 1;
                    OhmBtn = 0;
                    break;
                case 3:
                    AcBtn = 0;
                    DcBtn = 1;
                    OhmBtn = 1;
                    break;
                default:
                    break;
            }

            switch (comDeviceModel.Raw_guard)
            {
                case 0:
                    GuardOnBtn = 1;
                    GuardOffBtn = 1;
                    break;
                case 1:
                    GuardOnBtn = 1;
                    GuardOffBtn = 0;
                    break;
                case 2:
                    GuardOnBtn = 0;
                    GuardOffBtn = 1;
                    break;
                default:
                    break;
            }
        }

        public void Update(string deviceSerial, string subDeviceSerial, int firmwareVersion)
        {
            DeviceSerial = deviceSerial;
            SubDeviceSerial = subDeviceSerial;
            FirmwareVersion = firmwareVersion;
        }
        
        public void Update(double acI, double dcI, double acU, double dcU, double ohm)
        {
            this.acI = acI;
            this.dcI = dcI;
            this.acU = acU;
            this.dcU = dcU;
            this.ohm = ohm;
        }

        public void Update(MeasuringType mType, MeasuringQuality mQuality, double arrow, double selector)
        {
            MType = mType;
            MQuality = mQuality;
            Arrow = arrow;
            Selector = selector;
        }

        #region accessors

        public Visibility DebugInfoVisibility
        {
            get { return debugInfoVisibility; }
            set
            {
                if (value != debugInfoVisibility)
                {
                    debugInfoVisibility = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DebugInfoVisibility"));
                }
            }
        }

        public double Arrow
        {
            get { return arrow; }
            set
            {
                if (value != arrow)
                {
                    arrow = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Arrow"));
                }
            }
        }

        public double Selector
        {
            get { return selector; }
            set
            {
                if (value != selector)
                {
                    selector = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Selector"));
                }
            }
        }

        public int Plus
        {
            get { return plus; }
            set
            {
                if (value != plus)
                {
                    plus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Plus"));
                }
            }
        }

        public int Minus
        {
            get { return minus; }
            set
            {
                if (value != minus)
                {
                    minus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Minus"));
                }
            }
        }

        public string DeviceSerial
        {
            get { return deviceSerial; }
            set
            {
                if (value != deviceSerial)
                {
                    deviceSerial = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeviceSerial"));
                }
            }
        }

        public string SubDeviceSerial
        {
            get { return subDeviceSerial; }
            set
            {
                if (value != subDeviceSerial)
                {
                    subDeviceSerial = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SubDeviceSerial"));
                }
            }
        }

        public int FirmwareVersion
        {
            get { return firmwareVersion; }
            set
            {
                if (value != firmwareVersion)
                {
                    firmwareVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FirmwareVersion"));
                }
            }
        }

        public double acI
        {
            get { return _acI; }
            set
            {
                _acI = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("acI"));
            }
        }

        public double dcI
        {
            get { return _dcI; }
            set
            {
                _dcI = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dcI"));
            }
        }

        public double acU
        {
            get { return _acU; }
            set
            {
                _acU = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("acU"));
            }
        }

        public double dcU
        {
            get { return _dcU; }
            set
            {
                _dcU = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dcU"));
            }
        }

        public double ohm
        {
            get { return _ohm; }
            set
            {
                _ohm = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ohm"));
            }
        }

        public MeasuringType MType
        {
            get { return mType; }
            set
            {
                mType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MType"));
            }
        }

        public MeasuringQuality MQuality
        {
            get { return mQuality; }
            set
            {
                mQuality = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MQuality"));
            }
        }

        public int AcBtn
        {
            get { return acBtn; }
            set
            {
                acBtn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AcBtn"));
            }
        }

        public int DcBtn
        {
            get { return dcBtn; }
            set
            {
                dcBtn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DcBtn"));
            }
        }

        public int OhmBtn
        {
            get { return ohmBtn; }
            set
            {
                ohmBtn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OhmBtn"));
            }
        }

        public int GuardOnBtn
        {
            get { return guardOnBtn; }
            set
            {
                guardOnBtn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuardOnBtn"));
            }
        }

        public int GuardOffBtn
        {
            get { return guardOffBtn; }
            set
            {
                guardOffBtn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuardOffBtn"));
            }
        }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsActive"));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
