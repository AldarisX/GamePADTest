using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 外设检测
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class iFrame : Window
    {
        IntPtr wpfHwnd;
        Thread lis;
        Thread lib;
        List<Device> myJoys = new List<Device>();
        List<string> joysNameList = new List<string>();
        Device myJoy = null;
        int contrlsel = 0;
        int btnNum = 0;
        int[] axis = null;

        public iFrame()
        {
            InitializeComponent();

            lis = new Thread(rsListen);
            lib = new Thread(btnListen);
            reflashDevices();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            wpfHwnd = wndHelper.Handle;
        }

        private void sure_Click(object sender, RoutedEventArgs e)
        {
            if (lis.IsAlive)
            {
                lis.Suspend();
                lib.Suspend();
            }
            searchDevice();
            xinputBtn.Visibility = Visibility.Visible;
        }

        private void reflash_Click(object sender, RoutedEventArgs e)
        {
            reflashDevices();
        }

        public void searchDevice()
        {
            if (joysNameList.Count > 0)
            {
                myJoy = null;
                contrlsel = comboBox.SelectedIndex;
                myJoy = myJoys[contrlsel];
                try
                {
                    //myJoy.SetCooperativeLevel(wpfHwnd, Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Exclusive | Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Background);
                }
                catch (AcquiredException)
                {

                }

                try
                {
                    myJoy.Properties.AxisModeAbsolute = true;
                }
                catch (Exception)
                {

                }
                myJoy.Acquire();

                foreach (DeviceObjectInstance doi in myJoy.Objects)
                {
                    if ((doi.ObjectId & (int)DeviceObjectTypeFlags.Axis) != 0)
                    {
                        myJoy.Properties.SetRange(ParameterHow.ById,
                        doi.ObjectId, new InputRange(-256, 256));
                    }

                    int[] temp;

                    if ((doi.Flags & (int)ObjectInstanceFlags.Actuator) != 0)
                    {
                        if (axis != null)
                        {
                            temp = new int[axis.Length + 1];
                            axis.CopyTo(temp, 0);
                            axis = temp;
                        }
                        else
                        {
                            axis = new int[1];
                        }

                        axis[axis.Length - 1] = doi.Offset;
                        if (axis.Length == 2)
                        {
                            break;
                        }
                    }
                }

                if (lis.IsAlive)
                {
                    lis.Resume();
                    lib.Resume();
                }
                else
                {
                    lis.Start();
                    lib.Start();
                }

                btnNum = myJoy.Caps.NumberButtons;

                supportLab.Content = myJoy.Caps;
                infLab.Content = myJoy.DeviceInformation;
            }
        }

        public void reflashDevices()
        {
            this.Dispatcher.Invoke(new System.Action(delegate
            {
                myJoys = new List<Device>();
                joysNameList = new List<string>();
                foreach (DeviceInstance info in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly))
                {
                    myJoys.Add(new Device(info.InstanceGuid));
                    joysNameList.Add(new Device(info.InstanceGuid).DeviceInformation.ProductName);
                }
                SetDirComboBox();
            }));
        }

        public void SetDirComboBox()
        {
            comboBox.ItemsSource = joysNameList;
            if (joysNameList.Count == 0)
            {
                try
                {
                    lis.Suspend();
                    lib.Suspend();
                }
                catch (Exception)
                {

                }
            }
            comboBox.SelectedIndex = contrlsel;
        }

        public void rsListen()
        {
            while (true)
            {
                this.forceLab.Dispatcher.Invoke(new System.Action(delegate
                {
                    try
                    {
                        forceLab.Content = myJoy.CurrentJoystickState.ToString();
                    }
                    catch (InputLostException)
                    {
                        contrlsel = 0;
                        reflashDevices();
                        searchDevice();
                    }catch(NullReferenceException e)
                    {
                        MessageBox.Show("与手柄链接丢失");
                    }
                }));
                Thread.Sleep(50);
            }
        }

        public void btnListen()
        {
            while (true)
            {
                string str = "";
                byte[] btnState = null;
                try
                {
                    str = myJoy.CurrentJoystickState.GetPointOfView()[0] / 100 + "\n";
                    btnState = myJoy.CurrentJoystickState.GetButtons();
                }
                catch (InputLostException)
                {
                    contrlsel = 0;
                    reflashDevices();
                    searchDevice();
                }
                for (int i = 0; i < btnNum; i++)
                {
                    str += btnState.ElementAt(i) / 128 + "\n";
                }
                this.btnLab.Dispatcher.Invoke(new System.Action(delegate
                {
                    try
                    {
                        btnLab.Content = str;
                    }
                    catch (InputLostException)
                    {
                        contrlsel = 0;
                        reflashDevices();
                        searchDevice();
                    }
                }));
                Thread.Sleep(10);
            }
        }

        private void xinputBtn_Click(object sender, RoutedEventArgs e)
        {
            XinputFrame xf = new XinputFrame(contrlsel);
            xf.Show();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            xinputBtn.Visibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
