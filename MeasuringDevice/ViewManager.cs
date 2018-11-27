using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ComDevice
{
    public class ViewManager: IDisposable
    {
        private Window view;
        private Thread viewThread;
        public ViewDeviceModel ViewDeviceModel { get; set; }

        public ViewManager(int devType)
        {
            //StopViewThread();
            ViewDeviceModel = new ViewDeviceModel();
            InitViewThread(devType);
        }

        public IntPtr GetWindowHandle()
        {
            IntPtr handle = IntPtr.Zero;
            if (view != null)
            {
                view.Dispatcher.Invoke(new Action(() => { handle = new WindowInteropHelper(view).Handle; }));
            }
            return handle;
        }

        private void InitViewThread(int devType)
        {
            viewThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                switch (devType)
                {
                    case 2:
                        view = new View();
                        break;
                    case 4:
                        view = new View();
                        break;
                    case 5:
                        view = new NewView();
                        break;
                    default:
                        return;
                }
                
                view.Closed += (s, e) => Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                view.Visibility = Visibility.Visible;
                view.Show();
                view.DataContext = ViewDeviceModel;
                Dispatcher.Run();
            }))
            {
                Name = "AosComView",
                IsBackground = true
            };
            viewThread.SetApartmentState(ApartmentState.STA);
            viewThread.Start();
        }

        public void StopViewThread()
        {
            if (view != null)
            {
                view.Dispatcher.Invoke(new Action(() => { view.Close(); }));
            }
            viewThread?.Abort(); // TODO check
        }

        public void ChangeViewVisibility(byte status)
        {
            if (status == 1)
            {
                view.Dispatcher.Invoke(new Action(() => { view.Visibility = Visibility.Visible; }));
            }

            if (status == 0)
            {
                view.Dispatcher.Invoke(new Action(() => { view.Visibility = Visibility.Collapsed; }));
            }
        }

        public void ChangeDebugInfoVisibility(int state)
        {
            if (state == 1)
            {
                ViewDeviceModel.DebugInfoVisibility = Visibility.Visible;
            }

            if (state == 0)
            {
                ViewDeviceModel.DebugInfoVisibility = Visibility.Hidden;
            }
        }

        public void Dispose()
        {
            viewThread?.Abort();
        }
    }
}
