using AosComDevice;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ComDevice
{
    public class ComDeviceModel : INotifyPropertyChanged, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ComWorker comWorker;
        private string lastMsg = string.Empty;

        private int arrow = 0;
        private int selector = 0;
        private int type = 0;
        private int raw_guard = 0;
        private int plus = 0;
        private int minus = 0;

        private readonly List<byte> lamps = new List<byte>(new byte[16]);
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ChangedState;

        public ComDeviceModel(ComWorker comWorker)
        {
            this.comWorker = comWorker;
            this.comWorker.GetMessage += ComWorker_GetMessage;
        }

        public void SetArrow(int angle)
        {
            comWorker.SetValue(angle);
        }

        public void SetProbes(byte state)
        {
            if (state == 1)
            {
                comWorker.Send("h13");
            }
            else
            {
                comWorker.Send("l13");
            }
        }

        public void SetBurn()
        {
            comWorker.Send("h14");
        }

        #region props
        public int Arrow
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

        public int Selector
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

        public int Type
        {
            get { return type; }
            set
            {
                if (value != type)
                {
                    type = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
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

        public int Raw_guard
        {
            get { return raw_guard; }
            set
            {
                if (value != raw_guard)
                {
                    raw_guard = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Raw_guard"));
                }
            }
        }
        #endregion

        private bool Parse(string msg)
        {
            if (lastMsg == msg)
                return false;

            try
            {
                lastMsg = msg;
                bool flag = false;

                //m0 p0 s0 d48 b0 v0 i0000000000000000 ok
                Regex state = new Regex(@"^m(\d+) p(\d+) s(\d+) d(\d+) b(\d+) v(\d+) i(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d) ok$");
                
                Match match = state.Match(msg);
                if (match.Success)
                {
                    var minus = int.Parse(match.Groups[1].Value);
                    var plus = int.Parse(match.Groups[2].Value);

                    var type = int.Parse(match.Groups[3].Value);
                    var selector = int.Parse(match.Groups[4].Value);
                    var raw_guard = int.Parse(match.Groups[5].Value);
                    var arrow = int.Parse(match.Groups[6].Value);

                    if (Plus != plus || Minus != minus || Type != type || Selector != selector || Raw_guard != raw_guard)
                        flag = true;

                    Minus = minus;
                    Plus = plus;
                    Type = type;
                    Selector = selector;
                    Raw_guard = raw_guard;

                    lamps[0] = byte.Parse(match.Groups[7].Value);
                    lamps[1] = byte.Parse(match.Groups[8].Value);
                    lamps[2] = byte.Parse(match.Groups[9].Value);
                    lamps[3] = byte.Parse(match.Groups[10].Value);
                    lamps[4] = byte.Parse(match.Groups[11].Value);
                    lamps[5] = byte.Parse(match.Groups[12].Value);
                    lamps[6] = byte.Parse(match.Groups[13].Value);
                    lamps[7] = byte.Parse(match.Groups[14].Value);
                    lamps[8] = byte.Parse(match.Groups[15].Value);
                    lamps[9] = byte.Parse(match.Groups[16].Value);
                    lamps[10] = byte.Parse(match.Groups[17].Value);
                    lamps[11] = byte.Parse(match.Groups[18].Value);
                    lamps[12] = byte.Parse(match.Groups[19].Value);
                    lamps[13] = byte.Parse(match.Groups[20].Value);
                    lamps[14] = byte.Parse(match.Groups[21].Value);
                    lamps[15] = byte.Parse(match.Groups[22].Value);

                    return flag;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Msg: {msg}");
                return false;
            }
        }

        private void ComWorker_GetMessage(object sender, ComMsgEventArgs e)
        {
            if (Parse(e.Data))
            {
                ChangedState?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            comWorker.GetMessage -= ComWorker_GetMessage;
        }
    }
}
