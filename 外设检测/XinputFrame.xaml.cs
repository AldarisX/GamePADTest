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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace 外设检测
{
    /// <summary>
    /// XinputFrame.xaml 的交互逻辑
    /// </summary>
    public partial class XinputFrame : Window
    {
        Xinput xp = new Xinput();
        int devNum = -1;
        Thread btnThr;
        Thread rsThr;
        bool isMoroLock = false;
        double LTLock = 0;
        double RTLock = 0;

        public XinputFrame(int devNum)
        {
            InitializeComponent();

            this.devNum = devNum;
            xp.setVibration(devNum, 8000, 8000, 0.5);
            btnThr = new Thread(btnLis);
            btnThr.Start();

            rsThr = new Thread(rsLis);
            rsThr.Start();
        }

        private void robBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int left = Convert.ToInt32(leftmoto.Text);
                int right = Convert.ToInt32(rightmoto.Text);
                if (left > 65535 || right > 65535 || left < 0 || right < 0)
                {
                    MessageBox.Show("只能输入0-65535");
                }
                else
                {
                    xp.setVibration(devNum, left, right, 2);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("输入的值好像不是数字");
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            xp.setVibration(devNum, 0, 0);
        }

        int btnCount = 0;
        public static double sqrt2 = 1.4142135623731;
        private void btnLis()
        {
            while (true)
            {
                int way = (ushort)xp.XinputState(devNum).Gamepad.wButtons;
                if (!isMoroLock)
                {
                    if (btnCount > 100)
                    {
                        if (way == 256 || way == 512)
                        {
                            LTLock = xp.XinputState(devNum).Gamepad.bLeftTrigger;
                            RTLock = xp.XinputState(devNum).Gamepad.bRightTrigger;
                            isMoroLock = true;
                            motoLock.Dispatcher.Invoke(new Action(delegate
                            {
                                motoLock.Content = "锁定震动:" + isMoroLock;
                            }));
                            btnCount = 0;
                        }
                    }
                    btnCount++;
                }
                else
                {
                    if (btnCount > 100)
                    {
                        if (way == 256 || way == 512)
                        {
                            isMoroLock = false;
                            LTLock = 0;
                            RTLock = 0;
                            motoLock.Dispatcher.Invoke(new Action(delegate
                            {
                                motoLock.Content = "锁定震动:" + isMoroLock;
                            }));
                            xp.setVibration(devNum, 0, 0);
                            btnCount = 0;
                        }
                    }
                    btnCount++;
                }
                double lineX = 0;
                double lineY = 0;
                switch (way)
                {
                    case 1:
                        lineX = 60;
                        lineY = 0;
                        break;
                    case 2:
                        lineX = 60;
                        lineY = 128;
                        break;
                    case 4:
                        lineX = 0;
                        lineY = 60;
                        break;
                    case 5:
                        lineX = 60 - 64 / sqrt2;
                        lineY = 60 - 64 / sqrt2;
                        break;
                    case 6:
                        lineX = 60 - 64 / sqrt2;
                        lineY = 60 + 64 / sqrt2;
                        break;
                    case 8:
                        lineX = 128;
                        lineY = 60;
                        break;
                    case 9:
                        lineX = 60 + 64 / sqrt2;
                        lineY = 60 - 64 / sqrt2;
                        break;
                    case 10:
                        lineX = 60 + 64 / sqrt2;
                        lineY = 60 + 64 / sqrt2;
                        break;
                    default:
                        lineX = 60;
                        lineY = 60;
                        break;
                }
                btnLab.Dispatcher.Invoke(new Action(delegate
                {
                    btnLab.Content = "按键: " + xp.XinputState(devNum).Gamepad.wButtons;
                    dLine.SetValue(Canvas.LeftProperty, lineX);
                    dLine.SetValue(Canvas.TopProperty, lineY);
                }));
                Thread.Sleep(10);
            }
        }

        bool LRTC = false;
        private void rsLis()
        {
            while (true)
            {
                double lx = xp.XinputState(devNum).Gamepad.sThumbLX / 512;
                double ly = xp.XinputState(devNum).Gamepad.sThumbLY / 512;
                double rx = xp.XinputState(devNum).Gamepad.sThumbRX / 512;
                double ry = xp.XinputState(devNum).Gamepad.sThumbRY / 512;
                double lt = xp.XinputState(devNum).Gamepad.bLeftTrigger;
                double rt = xp.XinputState(devNum).Gamepad.bRightTrigger;

                if (isMoroLock)
                {
                    xp.setVibration(devNum, LTLock * 257, RTLock * 257);
                }
                else
                {
                    lMotoLab.Dispatcher.Invoke(new Action(delegate
                    {
                        lMotoLab.Content = lt;
                        rMotoLab.Content = rt;
                    }));
                    if (lt != 0 || rt != 0)
                    {
                        LRTC = true;
                        xp.setVibration(devNum, lt * 257, rt * 257);
                    }
                    else
                    {
                        if (LRTC)
                        {
                            xp.setVibration(devNum, 0, 0);
                            LRTC = false;
                        }
                    }
                }
                lPoint.Dispatcher.Invoke(new Action(delegate
                {
                    laxixLab.Content = rx + " " + ry;
                    raxixLab.Content = lx + " " + ly;
                    lPoint.SetValue(Canvas.LeftProperty, lx + 62);
                    lPoint.SetValue(Canvas.TopProperty, -ly + 62);
                    rPoint.SetValue(Canvas.LeftProperty, rx + 62);
                    rPoint.SetValue(Canvas.TopProperty, -ry + 62);
                }));
                Thread.Sleep(25);
            }
        }
    }
}
