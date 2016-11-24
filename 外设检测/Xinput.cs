using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace 外设检测
{
    class Xinput
    {
        /// <summary>
        /// 设备的按键信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_GAMEPAD
        {
            [MarshalAs(UnmanagedType.U2)]
            public XInputButtonKind wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        /// <summary>
        /// 当前的设备状态
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_STATE
        {
            public int dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        /// <summary>
        /// 设备的震动 0-65535
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_VIBRATION
        {
            public ushort wLeftMotorSpeed;
            public ushort wRightMotorSpeed;
        }

        /// <summary>
        /// 设备的状态（CAPABILITIES）
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_CAPABILITIES
        {
            public byte Type;
            [MarshalAs(UnmanagedType.U1)]
            public XInputSubTypeKind SubType;
            public int Flags;
            public XINPUT_GAMEPAD Gamepad;
            public XINPUT_VIBRATION Vibration;
        }

        /// <summary>
        /// 设备的电池信息（GAMEPAD 0x00和HEADSET 0x01，手柄和头盔？）
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_BATTERY_INFORMATION
        {
            //public byte BatteryType;
            //public byte BatteryLevel;
            [MarshalAs(UnmanagedType.U1)]
            public BatteryTypeKind BatteryType;
            [MarshalAs(UnmanagedType.U1)]
            public BatteryLevelKind BatteryLevel;
        }

        /// <summary>
        /// keystore?
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XINPUT_KEYSTROKE
        {
            public ushort VirtualKey;
            public char Unicode;
            public ushort Flags;
            public byte UserIndex;
            public byte HidCode;
        }

        /// <summary>
        /// 按键的枚举
        /// </summary>
        [Flags]
        public enum XInputButtonKind : ushort
        {
            //DigitalPadUp = 0x0001,
            //DigitalPadDown = 0x0002,
            //DigitalPadLeft = 0x0004,
            //DigitalPadRight = 0x0008,
            //Start = 0x0010,
            //Back = 0x0020,
            //LeftThumb = 0x0040,
            //RightThumb = 0x0080,
            //LeftShoulder = 0x0100,
            //RightShoulder = 0x0200,
            上 = 0x0001,
            下 = 0x0002,
            左 = 0x0004,
            右 = 0x0008,
            Start = 0x0010,
            Back = 0x0020,
            LT = 0x0040,
            RT = 0x0080,
            L = 0x0100,
            R = 0x0200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000,
        }

        /// <summary>
        /// 设备类型的枚举
        /// </summary>
        public enum XInputSubTypeKind : byte
        {
            Gamepad = 0x01,
            Wheel = 0x02,
            ArcadeStick = 0x03,
            FlightStick = 0x04,
            DancePad = 0x05,
            Guitar = 0x06,
            DrumKit = 0x07
        }

        /// <summary>
        /// 电池状态的枚举
        /// </summary>
        public enum BatteryTypeKind : byte
        {
            //BATTERY_TYPE_DISCONNECTED = 0x00,
            //BATTERY_TYPE_WIRED = 0x01,
            //BATTERY_TYPE_ALKALINE = 0x02,
            //BATTERY_TYPE_NIMH = 0x03,
            //BATTERY_TYPE_UNKNOWN = 0xFF,
            未连接 = 0x00,
            有线_无电池 = 0x01,
            碱性电池 = 0x02,
            镍氢电池 = 0x03,
            未知 = 0x04,
        }

        /// <summary>
        /// 电池电量的枚举
        /// </summary>
        public enum BatteryLevelKind : byte
        {
            //BATTERY_LEVEL_EMPTY = 0x00,
            //BATTERY_LEVEL_LOW = 0x01,
            //BATTERY_LEVEL_MEDIUM = 0x02,
            //BATTERY_LEVEL_FULL = 0x03,
            电量空 = 0x00,
            电量低 = 0x01,
            电量中 = 0x02,
            电量满 = 0x03,
        }

        /// <summary>
        /// 设置震动
        /// </summary>
        /// <param name="dwUserIndex">设备编号</param>
        /// <param name="pVibration">设备震动类型</param>
        /// <returns></returns>
        [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.Winapi)]
        static extern uint XInputSetState(int dwUserIndex, ref XINPUT_VIBRATION pVibration);

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="dwUserIndex">设备编号</param>
        /// <param name="pState">设备状态</param>
        /// <returns></returns>
        [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.Winapi)]
        static extern uint XInputGetState(int dwUserIndex, out XINPUT_STATE pState);

        /// <summary>
        /// 得到设备状态（Capabilities）
        /// </summary>
        /// <param name="dwUserIndex">设备编号</param>
        /// <param name="dwFlags">Flags,输入0</param>
        /// <param name="pCapabilities">Capabilities</param>
        /// <returns></returns>
        [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.Winapi)]
        static extern uint XInputGetCapabilities(int dwUserIndex, int dwFlags, out XINPUT_CAPABILITIES pCapabilities);

        /// <summary>
        /// 设备开启？
        /// </summary>
        /// <param name="enable">是否开启</param>
        [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.Winapi)]
        static extern void XInputEnable([MarshalAs(UnmanagedType.Bool)]bool enable);

        /// <summary>
        /// 得到DSound音频设备
        /// </summary>
        /// <param name="dwUserIndex">设备编号</param>
        /// <param name="pDSoundRenderGuid">DSound渲染GUID</param>
        /// <param name="pDSoundCaptureGuid">DSound捕获GUID</param>
        /// <returns></returns>
        [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.Winapi)]
        static extern uint XInputGetDSoundAudioDeviceGuids(int dwUserIndex, out Guid pDSoundRenderGuid, out Guid pDSoundCaptureGuid);

        /// <summary>
        /// 得到电池信息
        /// </summary>
        /// <param name="dwUserIndex">设备编号</param>
        /// <param name="devType">设备类型</param>
        /// <param name="pBatteryInformation">电池信息</param>
        /// <returns></returns>
        [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.Winapi)]
        static extern uint XInputGetBatteryInformation(int dwUserIndex, byte devType, out XINPUT_BATTERY_INFORMATION pBatteryInformation);

        /// <summary>
        /// 获取keystore?
        /// </summary>
        /// <param name="dwUserIndex">设备编号</param>
        /// <param name="dwReserved"></param>
        /// <param name="pKeystroke"></param>
        /// <returns></returns>
        [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.Winapi)]
        static extern uint XInputGetKeystroke(int dwUserIndex, int dwReserved, out XINPUT_KEYSTROKE pKeystroke);

        /// <summary>
        /// 设置震动
        /// </summary>
        /// <param name="index">设备编号</param>
        /// <param name="leftMotorSpeed">左马达速度</param>
        /// <param name="rightMotorSpeed">右马达速度</param>
        public void setVibration(int index, double leftMotorSpeed, double rightMotorSpeed)
        {
            XINPUT_VIBRATION vib = new XINPUT_VIBRATION();
            vib.wLeftMotorSpeed = (ushort)leftMotorSpeed;
            vib.wRightMotorSpeed = (ushort)rightMotorSpeed;
            XInputSetState(index, ref vib);
        }

        Thread vibThr;
        public void setVibration(int index, int leftMotorSpeed, int rightMotorSpeed, double time)
        {
            setVibration(index, 0, 0);
            XINPUT_VIBRATION_TIME vib = new XINPUT_VIBRATION_TIME();
            vib.index = index;
            vib.wLeftMotorSpeed = leftMotorSpeed;
            vib.wRightMotorSpeed = rightMotorSpeed;
            vib.time = time;
            vibThr = new Thread(new ParameterizedThreadStart(vibTime));
            vibThr.Start(vib);
        }

        private struct XINPUT_VIBRATION_TIME
        {
            public int index;
            public int wLeftMotorSpeed;
            public int wRightMotorSpeed;
            public double time;
        }

        private void vibTime(object obj)
        {
            XINPUT_VIBRATION_TIME vib = (XINPUT_VIBRATION_TIME)obj;
            //for(int i = 0; i < vib.time * 25; i++)
            //{
            //    setVibration(vib.index, vib.wLeftMotorSpeed, vib.wRightMotorSpeed);
            //    Thread.Sleep(20);
            //}
            int counNum = (int)Math.Floor(vib.time);

            if (counNum != 0)
            {
                for (int i = 0; i < counNum; i++)
                {
                    setVibration(vib.index, vib.wLeftMotorSpeed, vib.wRightMotorSpeed);
                    Thread.Sleep(1000);
                }
            }
            else
            {
                setVibration(vib.index, vib.wLeftMotorSpeed, vib.wRightMotorSpeed);
                Thread.Sleep((int)(vib.time * 1000));
            }
            setVibration(vib.index, 0, 0);
        }

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <param name="index">设备编号</param>
        /// <returns></returns>
        public XINPUT_STATE XinputState(int index)
        {
            XINPUT_STATE state = new XINPUT_STATE();
            XInputGetState(index, out state);
            return state;
        }

        /// <summary>
        /// 设备是否连接
        /// </summary>
        /// <param name="index">设备编号</param>
        /// <returns></returns>
        public bool IsConected(int index)
        {
            XINPUT_STATE state = new XINPUT_STATE();
            if (XInputGetState(index, out state) == 0x48F)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <param name="index">设备编号</param>
        /// <returns></returns>
        public XINPUT_CAPABILITIES GetCapabilities(int index)
        {
            XINPUT_CAPABILITIES cap = new XINPUT_CAPABILITIES();
            XInputGetCapabilities(index, 0, out cap);
            return cap;
        }

        /// <summary>
        /// 得到电池信息
        /// </summary>
        /// <param name="index">设备编号</param>
        /// <returns></returns>
        public XINPUT_BATTERY_INFORMATION GetBatteryInformation(int index)
        {
            XINPUT_BATTERY_INFORMATION battery = new XINPUT_BATTERY_INFORMATION();
            XInputGetBatteryInformation(index, (byte)GetCapabilities(index).SubType, out battery);
            return battery;
        }
    }
}
