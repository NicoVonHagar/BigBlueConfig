using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static BigBlueConfig.EditMouseBindings;
using static BigBlueConfig.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BigBlueConfig
{
    public static class NativeMethods
    {
        public const uint WS_OVERLAPPEDWINDOW =
            (WS_OVERLAPPED |
              WS_CAPTION |
              WS_SYSMENU |
              WS_THICKFRAME |
              WS_MINIMIZEBOX |
              WS_MAXIMIZEBOX);

        public const uint WS_POPUPWINDOW =
            (WS_POPUP |
              WS_BORDER |
              WS_SYSMENU);

        const int FILE_SHARE_READ = 1;
        const int FILE_SHARE_WRITE = 2;
        private const int OPEN_EXISTING = 2;

        public const int WS_OVERLAPPED = 0x00000000;
        public const uint WS_POPUP = 0x80000000;
        public const int WS_CHILD = 0x40000000;
        public const int WS_MINIMIZE = 0x20000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
        public const int WS_BORDER = 0x00800000;
        public const int WS_DLGFRAME = 0x00400000;
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;



        public const int WS_THICKFRAME = 0x00040000;
        public const uint WS_GROUP = 0x00020000;
        public const uint WS_TABSTOP = 0x00010000;

        public const uint WS_MINIMIZEBOX = 0x00020000;
        public const uint WS_MAXIMIZEBOX = 0x00010000;

        public const uint WS_TILED = WS_OVERLAPPED;
        public const uint WS_ICONIC = WS_MINIMIZE;
        //public const int WS_SIZEBOX = WS_THICKFRAME;
        public const uint WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;

        internal const uint MF_BYCOMMAND = 0x00000000;
        internal const uint MF_GRAYED = 0x00000001;
        internal const uint MF_ENABLED = 0x00000000;
        internal const uint MF_DISABLED = 0x00000002;

        internal const Int32 MF_BYPOSITION = 0x400;
        internal const Int32 MF_REMOVE = 0x1000;

        internal const UInt32 SC_SIZE = 0xF000;
        internal const UInt32 SC_MOVE = 0xF010;
        internal const UInt32 SC_MINIMIZE = 0xF020;
        internal const UInt32 SC_MAXIMIZE = 0xF030;
        internal const UInt32 SC_NEXTWINDOW = 0xF040;
        internal const UInt32 SC_PREVWINDOW = 0xF050;
        internal const UInt32 SC_CLOSE = 0xF060;
        internal const UInt32 SC_VSCROLL = 0xF070;
        internal const UInt32 SC_HSCROLL = 0xF080;
        internal const UInt32 SC_MOUSEMENU = 0xF090;
        internal const UInt32 SC_KEYMENU = 0xF100;
        internal const UInt32 SC_ARRANGE = 0xF110;
        internal const UInt32 SC_RESTORE = 0xF120;
        internal const UInt32 SC_TASKLIST = 0xF130;
        internal const UInt32 SC_SCREENSAVE = 0xF140;
        internal const UInt32 SC_HOTKEY = 0xF150;
        internal const UInt32 SC_DEFAULT = 0xF160;
        internal const UInt32 SC_MONITORPOWER = 0xF170;
        internal const UInt32 SC_CONTEXTHELP = 0xF180;
        internal const UInt32 SC_SEPARATOR = 0xF00F;

        //internal const uint SC_CLOSE = 0xF060;

        internal const int WM_SHOWWINDOW = 0x00000018;
        internal const int WM_CLOSE = 0x10;

        internal const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        internal const int APPCOMMAND_VOLUME_UP = 0xA0000;
        internal const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        internal const int WM_APPCOMMAND = 0x319;
        internal const uint MOD_CTRL = 0x4000;
        internal const int WH_KEYBOARD_LL = 13;
        internal static LowLevelKeyboardProc _proc = HookCallback;
        internal static IntPtr _hookID = IntPtr.Zero;
        internal const uint OCR_NORMAL = 32512;
        internal const uint OCR_WAIT = 32514;
        internal const uint OCR_APPSTARTING = 32650;
        internal const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        internal const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        internal const int SPI_SETCURSORS = 0x0057;

        internal const int RIDEV_INPUTSINK = 0x00000100;
        internal const int RIDEV_EXINPUTSINK = 0x00001000;
        internal const int RID_INPUT = 0x10000003;
        internal const int RIDEV_REMOVE = 0x00000001;

        internal const int FAPPCOMMAND_MASK = 0xF000;
        internal const int FAPPCOMMAND_MOUSE = 0x8000;
        internal const int FAPPCOMMAND_OEM = 0x1000;

        internal const int RIM_TYPEMOUSE = 0;
        internal const int RIM_TYPEKEYBOARD = 1;
        internal const int RIM_TYPEHID = 2;

        internal const int RIDI_DEVICENAME = 0x20000007;
        internal const int RIDI_DEVICEINFO = 0x2000000b;
        internal const int WM_DISPLAYCHANGE = 0x007e;

        internal const int WM_KEYDOWN = 0x0100;
        internal const int WM_KEYUP = 0x0101;
        internal const int WM_SYSKEYDOWN = 0x0104;
        internal const int WM_SYSKEYUP = 0x0105;
        internal const int WM_INPUT = 0x00FF;
        internal const int VK_OEM_CLEAR = 0xFE;
        internal const int WM_DEVICECHANGE = 0x0219;
        internal const int VK_LAST_KEY = VK_OEM_CLEAR; // this is a made up value used as a sentinal

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RID_DEVICE_INFO_HID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwVendorId;
            [MarshalAs(UnmanagedType.U4)]
            public int dwProductId;
            [MarshalAs(UnmanagedType.U4)]
            public int dwVersionNumber;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsage;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RID_DEVICE_INFO_KEYBOARD
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSubType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwKeyboardMode;
            [MarshalAs(UnmanagedType.U4)]
            public int dwNumberOfFunctionKeys;
            [MarshalAs(UnmanagedType.U4)]
            public int dwNumberOfIndicators;
            [MarshalAs(UnmanagedType.U4)]
            public int dwNumberOfKeysTotal;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RID_DEVICE_INFO_MOUSE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwId;
            [MarshalAs(UnmanagedType.U4)]
            public int dwNumberOfButtons;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSampleRate;
            [MarshalAs(UnmanagedType.U4)]
            public int fHasHorizontalWheel;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct RID_DEVICE_INFO
        {
            [FieldOffset(0)]
            public int cbSize;
            [FieldOffset(4)]
            public int dwType;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_MOUSE mouse;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_KEYBOARD keyboard;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_HID hid;
        }

        /// <summary>
        /// Contains the raw input from a device.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        internal struct RAWINPUT
        {
            /// <summary>
            /// Header for the data.
            /// </summary>
            [FieldOffset(0)]
            public RAWINPUTHEADER header;
            /// <summary>
            /// Mouse raw input data.
            /// </summary>
            [FieldOffset(24)]
            public RAWMOUSE mouse;
            /// <summary>
            /// Keyboard raw input data.
            /// </summary>
            [FieldOffset(24)]
            public RAWKEYBOARD Keyboard;
            /// <summary>
            /// HID raw input data.
            /// </summary>
            [FieldOffset(24)]
            public RAWHID HID;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTHEADER
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWHID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizHid;
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BUTTONSSTR
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonFlags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonData;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct RAWMOUSE
        {
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(0)]
            public ushort usFlags;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            public uint ulButtons;
            [FieldOffset(4)]
            public BUTTONSSTR buttonsStr;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            public uint ulRawButtons;
            [FieldOffset(12)]
            public int lLastX;
            [FieldOffset(16)]
            public int lLastY;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(20)]
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWKEYBOARD
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort MakeCode;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Flags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Reserved;
            [MarshalAs(UnmanagedType.U2)]
            public ushort VKey;
            [MarshalAs(UnmanagedType.U4)]
            public uint Message;
            [MarshalAs(UnmanagedType.U4)]
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICE
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsage;
            [MarshalAs(UnmanagedType.U4)]
            public int dwFlags;
            public IntPtr hwndTarget;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HIDD_ATTRIBUTES
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint Size;

            [MarshalAs(UnmanagedType.U2)]
            public ushort VendorID;

            [MarshalAs(UnmanagedType.U2)]
            public ushort ProductID;

            [MarshalAs(UnmanagedType.U2)]
            public ushort VersionNumber;
        }

        [DllImport("BigBlueRawInput.dll")]
        internal extern static bool IsHIDJoystick(IntPtr deviceHandle);


        [DllImport("BigBlueRawInput.dll")]
        internal extern static void ParseRawInputForJoysticks(IntPtr pRawInput, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)][In, Out] /*ref*/ bool[] inputStates);

        [DllImport("BigBlueRawInput.dll")]
        internal extern static byte GetRawKeyboardScanCode(ushort vKey, ushort makeCode, ushort flags);

        [DllImport("BigBlueRawInput.dll")]
        internal extern static bool IsRawKeyboardKeyDown(ushort flags);

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool HidD_GetAttributes(SafeFileHandle HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool HidD_GetProductString(SafeFileHandle HidDeviceObject, StringBuilder Buffer, uint BufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Boolean HidD_GetManufacturerString(
            SafeFileHandle HidDeviceObject,
            StringBuilder Buffer,
            Int32 BufferLength);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto,
    CallingConvention = CallingConvention.StdCall,
    SetLastError = true)]
        static extern SafeFileHandle CreateFileW(
    string lpFileName,
    uint dwDesiredAccess,
    uint dwShareMode,
    IntPtr SecurityAttributes,
    uint dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile
);

        [DllImport("user32.dll")]
        internal extern static IntPtr DestroyMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal extern static IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal extern static int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        internal extern static bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal extern static bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll")]
        internal extern static int DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll")]
        internal extern static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32.dll", SetLastError = true)]
        internal extern static uint EnableMenuItem(IntPtr hMenu, uint itemId, uint uEnable);

        [DllImport("User32.dll")]
        internal extern static uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetRawInputDeviceInfoW", CharSet = CharSet.Unicode)]
        internal extern static uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, StringBuilder pData, ref uint pcbSize);

        [DllImport("User32.dll")]
        internal extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

        [DllImport("User32.dll")]
        internal extern static uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("User32.dll")]
        extern static uint GetRawInputBuffer();

        internal static void ProcessRawKeyboardInputs(BigBlueConfig.NativeMethods.RAWINPUT raw, RawInputWindow bbWindow)
        {
            IntPtr deviceId = (IntPtr)raw.header.hDevice;

            if (bbWindow.keyboardDevices.ContainsKey(deviceId) == true)
            {
                BigBlueConfig.NativeMethods.RAWKEYBOARD kb = raw.Keyboard;
                ushort virtualKeyCode = kb.VKey;
                ushort flags = kb.Flags;

                if (virtualKeyCode == 255)
                {
                    return;
                }

                //uint8_t scanCode = (rkb.MakeCode & 0x7f) | ((rkb.Flags & RI_KEY_E0) ? 0x80 : 0x00);
                byte scanCode = GetRawKeyboardScanCode(virtualKeyCode, kb.MakeCode, flags);

                // scancode 0xaa is a special shift code we need to ignore
                if (scanCode == 0xaa)
                {
                    return;
                }

                bool keyDown = IsRawKeyboardKeyDown(flags);

                if (bbWindow.byteToKeyMap.ContainsKey(scanCode))
                {
                    Models.KeyboardInputType kit = bbWindow.byteToKeyMap[scanCode];

                    bbWindow.keyboardDevices[deviceId].InputPressed[(int)kit] = keyDown;
                }
            }
        }

        internal static void ProcessRawMouseInputs(BigBlueConfig.NativeMethods.RAWINPUT raw, RawInputWindow bbWindow)
        {
            IntPtr deviceId = (IntPtr)raw.header.hDevice;

            //if (raw.header.hDevice == (IntPtr)65597)
            if (bbWindow.mouseDevices.ContainsKey(deviceId) == true)
            {
                for (int mouseIndex = 0; mouseIndex != (int)Models.MouseInputType.MAX_MOUSE; ++mouseIndex)
                {
                    bbWindow.mouseDevices[deviceId].InputPressed[mouseIndex] = false;
                }

                BigBlueConfig.NativeMethods.RAWMOUSE m = raw.mouse;
                uint mouseButtons = m.ulButtons;

                // mouse button 1 down
                if (mouseButtons == 0x0001)
                {
                    
                     bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Button1] = true;
                    
                }

                // mouse button 1 up
                //if (mouseButtons == 0x0002)
                //{
                    //if (bbWindow.mouseDevices[deviceId].InputMappings.TryGetValue(Models.MouseInputType.Button1, out Models.GameInputLabel mouseButton1UpAction))
                    //{
                        //Console.WriteLine("");
                    //}
                //}

                // mouse buton 2 down
                if (mouseButtons == 0x0004)
                {
                    
                    bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Button2] = true;
                    
                }

                //Models.GameInputLabel mouseButton2UpAction;
                // mouse button 2 up
                //if (m.ulButtons == 0x0008)
                //{
                //if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_2", out mouseButton2UpAction))
                //{
                //bbWindow.ProcessFrontendAction(mouseButton2UpAction, false);
                //}
                //}

                // mouse button 3 down
                if (m.ulButtons == 0x0010)
                {
                    
                    bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Button3] = true;
                    
                }

                // mouse button 3 up
                //if (m.ulButtons == 0x0020)
                //{
                //Models.GameInputLabel mouseButton3UpAction;
                //if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_3", out mouseButton3UpAction))
                //{
                //bbWindow.ProcessFrontendAction(mouseButton3UpAction, false);
                //}
                //}

                // mouse button 4 down
                if (m.ulButtons == 0x0040)
                {
                    
                    bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Button4] = true;
                    
                }

                // mouse button 4 up
                //if (m.ulButtons == 0x0080)
                //{
                //Models.GameInputLabel mouseButton4UpAction;
                //if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_4", out mouseButton4UpAction))
                //{
                //bbWindow.ProcessFrontendAction(mouseButton4UpAction, false);
                //}
                //}

                // mouse button 5 down
                if (mouseButtons == 0x0100)
                {
                    bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Button5] = true;   
                }

                // mouse button 5 up
                //if (mouseButtons == 0x0200)
                //{
                //Models.GameInputLabel mouseButton5UpAction;
                //if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_5", out mouseButton5UpAction))
                //{
                //bbWindow.ProcessFrontendAction(mouseButton5UpAction, false);
                //}
                //}

                short mouseWheelDirection = (short)m.buttonsStr.usButtonData;


                if (m.lLastX < 0)
                {
                    bbWindow.mouseDevices[deviceId].LeftPixels = bbWindow.mouseDevices[deviceId].LeftPixels - 1;

                    if (bbWindow.mouseDevices[deviceId].LeftPixels == -16)
                    {
                        bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Left] = true;
                        bbWindow.mouseDevices[deviceId].LeftPixels = 0;
                    }
                }

                if (m.lLastX > 0)
                {
                    bbWindow.mouseDevices[deviceId].RightPixels = bbWindow.mouseDevices[deviceId].RightPixels + 1;

                    if (bbWindow.mouseDevices[deviceId].RightPixels == 16)
                    {
                        bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Right] = true;
                        bbWindow.mouseDevices[deviceId].RightPixels = 0;
                    }
                }

                if (m.lLastY < 0)
                {
                    bbWindow.mouseDevices[deviceId].UpPixels = bbWindow.mouseDevices[deviceId].UpPixels - 1;

                    if (bbWindow.mouseDevices[deviceId].UpPixels == -16)
                    {
                        bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Up] = true;
                        bbWindow.mouseDevices[deviceId].UpPixels = 0;
                    }
                }

                if (m.lLastY > 0)
                {
                    bbWindow.mouseDevices[deviceId].DownPixels = bbWindow.mouseDevices[deviceId].DownPixels + 1;

                    if (bbWindow.mouseDevices[deviceId].DownPixels == 16)
                    {
                        bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.Down] = true;
                        bbWindow.mouseDevices[deviceId].DownPixels = 0;
                    }
                }

                if (mouseWheelDirection < 0)
                {
                    bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.WheelDown] = true;

                    bbWindow.mouseDevices[deviceId].PreviousWheelDown = 0;
                    bbWindow.mouseDevices[deviceId].CurrentWheelDown = mouseWheelDirection;
                }

                if (mouseWheelDirection > 0)
                {
                    bbWindow.mouseDevices[deviceId].InputPressed[(int)Models.MouseInputType.WheelUp] = true;

                    bbWindow.mouseDevices[deviceId].PreviousWheelUp = 0;
                    bbWindow.mouseDevices[deviceId].CurrentWheelUp = mouseWheelDirection;
                }
            }
        }


        /// <summary>
        /// Filters Windows messages for WM_INPUT messages and calls
        /// ProcessInputCommand if necessary.
        /// </summary>
        /// <param name="message">The Windows message.</param>
        internal static void ProcessMessage(int message, IntPtr lParam, IntPtr wParam, RawInputWindow bbWindow)
        {
            switch (message)
            {
                case BigBlueConfig.NativeMethods.WM_INPUT:
                    
                    ProcessInputCommand(lParam, bbWindow);
                    
                    break;
                case BigBlueConfig.NativeMethods.WM_DEVICECHANGE:
                    // this will clear out and repopulate all the controls
                    bbWindow.awaitingAsync = true;

                    BigBlueConfig.NativeMethods.ProvisionRawInputs(bbWindow, true);

                    bbWindow.awaitingAsync = false;

                    switch ((int)wParam)
                    {
                        case BigBlueConfig.USBNotification.DbtDeviceremovecomplete:
                            //MessageBox.Show("removed");
                            break;
                        case BigBlueConfig.USBNotification.DbtDevicearrival:
                            //MessageBox.Show("added");
                            break;
                    }

                    break;
                    //case BigBlueConfig.NativeMethods.WM_DISPLAYCHANGE:
                    // this is what happens when you turn off a home theater receiver via hdmi
                    // break;
            }
        }

        static readonly uint rawInputHeaderSize = (uint)Marshal.SizeOf(typeof(BigBlueConfig.NativeMethods.RAWINPUTHEADER));




        /// <summary>
        /// Processes WM_INPUT messages to retrieve information about any
        /// keyboard events that occur.
        /// </summary>
        /// <param name="message">The WM_INPUT message to process.</param>
        internal static void ProcessInputCommand(IntPtr lParam, RawInputWindow bbWindow)
        {
            uint dwSize = 0;

            // First call to GetRawInputData sets the value of dwSize dwSize can then be used to allocate the appropriate amount of memory, storing the pointer in "buffer"
            BigBlueConfig.NativeMethods.GetRawInputData(lParam, BigBlueConfig.NativeMethods.RID_INPUT, IntPtr.Zero, ref dwSize, rawInputHeaderSize);

            IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);

            if (BigBlueConfig.NativeMethods.GetRawInputData(lParam, BigBlueConfig.NativeMethods.RID_INPUT, buffer, ref dwSize, rawInputHeaderSize) == dwSize)
            {
                BigBlueConfig.NativeMethods.RAWINPUT raw = (BigBlueConfig.NativeMethods.RAWINPUT)Marshal.PtrToStructure(buffer, typeof(BigBlueConfig.NativeMethods.RAWINPUT));

                switch (raw.header.dwType)
                {
                    case BigBlueConfig.NativeMethods.RIM_TYPEKEYBOARD:
                        {
                            BigBlueConfig.NativeMethods.ProcessRawKeyboardInputs(raw, bbWindow);

                            break;
                        }

                    case BigBlueConfig.NativeMethods.RIM_TYPEMOUSE:
                        {
                            BigBlueConfig.NativeMethods.ProcessRawMouseInputs(raw, bbWindow);
                            break;
                        }

                    case BigBlueConfig.NativeMethods.RIM_TYPEHID:
                        {
                            IntPtr deviceId = (IntPtr)raw.header.hDevice;

                            if (bbWindow.joystickDevices.ContainsKey(deviceId))
                            {
                                Models.RawInputJoystick j = bbWindow.joystickDevices[deviceId];

                                ParseRawInputForJoysticks(buffer, j.InputPressed);
                            }

                            break;
                        }
                }
            }

            Marshal.FreeHGlobal(buffer);
        }

        /// <summary>
        /// InputDevice constructor; registers the raw input devices
        /// for the calling window.
        /// </summary>
        /// <param name="hwnd">Handle of the window listening for key presses</param>
        internal static void RegisterInputDevices(RawInputWindow bbWindow)
        {
            bbWindow.registeredInputDevices = new BigBlueConfig.NativeMethods.RAWINPUTDEVICE[4];

            // dwFlags
            // 0 = NORMAL
            // RIDEV_INPUTSINK = GLOBAL
            // RIDEV_EXINPUTSINK = GLOBAL UNLESS ANOTHER RAW INPUT PROCESS IS IN THE FOREGROUND

            // register keyboard; 0x06 = KEYBOARD
            bbWindow.registeredInputDevices[0].usUsagePage = 0x01;
            bbWindow.registeredInputDevices[0].usUsage = 0x06;

            // if global inputs are true, then we'll register the raw inputs that way;
            // otherwise, the inputs will not be global
            if (bbWindow.globalInputs == true)
            {
                bbWindow.registeredInputDevices[0].dwFlags = BigBlueConfig.NativeMethods.RIDEV_INPUTSINK;
                bbWindow.registeredInputDevices[1].dwFlags = BigBlueConfig.NativeMethods.RIDEV_INPUTSINK;
                bbWindow.registeredInputDevices[2].dwFlags = BigBlueConfig.NativeMethods.RIDEV_INPUTSINK;
                bbWindow.registeredInputDevices[3].dwFlags = BigBlueConfig.NativeMethods.RIDEV_INPUTSINK;
            }
            else
            {
                bbWindow.registeredInputDevices[0].dwFlags = 0;
                bbWindow.registeredInputDevices[1].dwFlags = 0;
                bbWindow.registeredInputDevices[2].dwFlags = 0;
                bbWindow.registeredInputDevices[3].dwFlags = 0;
            }

            bbWindow.registeredInputDevices[0].hwndTarget = bbWindow.windowHandle;

            // register mouse; 0x02 = MOUSE
            bbWindow.registeredInputDevices[1].usUsagePage = 0x01;
            bbWindow.registeredInputDevices[1].usUsage = 0x02;
            bbWindow.registeredInputDevices[1].hwndTarget = bbWindow.windowHandle;

            // register joystick; 0x04 = JOYSTICK
            bbWindow.registeredInputDevices[2].usUsagePage = 0x01;
            bbWindow.registeredInputDevices[2].usUsage = 0x04;
            bbWindow.registeredInputDevices[2].hwndTarget = bbWindow.windowHandle;

            // register gamepad; 0x05 = GAMEPAD
            bbWindow.registeredInputDevices[3].usUsagePage = 0x01;
            bbWindow.registeredInputDevices[3].usUsage = 0x05;
            bbWindow.registeredInputDevices[3].hwndTarget = bbWindow.windowHandle;

            if (!BigBlueConfig.NativeMethods.RegisterRawInputDevices(bbWindow.registeredInputDevices, (uint)bbWindow.registeredInputDevices.Length, (uint)Marshal.SizeOf(bbWindow.registeredInputDevices[0])))
            {
                throw new ApplicationException("Failed to register raw input device(s).");
            }
        }

        internal static void UnRegisterInputDevices(RawInputWindow bbWindow)
        {
            // keyboard
            bbWindow.registeredInputDevices[0].dwFlags = RIDEV_REMOVE;
            bbWindow.registeredInputDevices[0].hwndTarget = IntPtr.Zero;

            // mouse
            bbWindow.registeredInputDevices[1].dwFlags = RIDEV_REMOVE;
            bbWindow.registeredInputDevices[1].hwndTarget = IntPtr.Zero;

            // joystick
            bbWindow.registeredInputDevices[2].dwFlags = RIDEV_REMOVE;
            bbWindow.registeredInputDevices[2].hwndTarget = IntPtr.Zero;

            // gamepad
            bbWindow.registeredInputDevices[3].dwFlags = RIDEV_REMOVE;
            bbWindow.registeredInputDevices[3].hwndTarget = IntPtr.Zero;

            if (!BigBlueConfig.NativeMethods.RegisterRawInputDevices(bbWindow.registeredInputDevices, (uint)bbWindow.registeredInputDevices.Length, (uint)Marshal.SizeOf(bbWindow.registeredInputDevices[0])))
            {
                throw new ApplicationException("Failed to register raw input device(s).");
            }
        }

        private static UsbDevice GetUsbDevice(string deviceName)
        {
            UsbDevice ud = new();

            if (!string.IsNullOrWhiteSpace(deviceName))
            {
                SafeFileHandle hidHandle = CreateFileW(deviceName, 0, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

                if (hidHandle.IsInvalid)
                {
                    return ud;
                }

                StringBuilder sb = new();

                string humanDeviceName = string.Empty;

                if (HidD_GetProductString(hidHandle, sb, 256))
                {
                    humanDeviceName = sb.ToString();
                }

                if (HidD_GetManufacturerString(hidHandle, sb, 256))
                {
                    string manufacturer = sb.ToString();

                    if (!string.IsNullOrWhiteSpace(manufacturer))
                    {
                        humanDeviceName = humanDeviceName + " (" + sb.ToString() + ")";
                    }
                }

                ud.DeviceName = humanDeviceName;

                sb.Clear();

                HIDD_ATTRIBUTES attrib = new()
                {
                    Size = (uint)(Marshal.SizeOf(typeof(HIDD_ATTRIBUTES)))
                };

                if (HidD_GetAttributes(hidHandle, ref attrib))
                {
                    ud.VendorID = attrib.VendorID.ToString("x4");
                    ud.ProductID = attrib.ProductID.ToString("x4");
                }

                hidHandle.Close();
            }

            return ud;
        }

        internal static int ProvisionRawInputs(RawInputWindow window, bool refresh)
        {
            if (refresh)
            {
                window.keyboardDevices.Clear();
                window.mouseDevices.Clear();
                window.joystickDevices.Clear();
            }

            int numberOfKeyboards = 0;
            int numberOfMice = 0;
            int numberOfJoysticks = 0;
            uint deviceCount = 0;
                        
            int dwSize = Marshal.SizeOf(typeof(BigBlueConfig.NativeMethods.RAWINPUTDEVICELIST));

            if (BigBlueConfig.NativeMethods.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) != 0)
            {
                return 0;
            }           

            IntPtr rawInputDeviceListData = Marshal.AllocHGlobal(dwSize * (int)deviceCount);
            BigBlueConfig.NativeMethods.GetRawInputDeviceList(rawInputDeviceListData, ref deviceCount, (uint)dwSize);

            
            int rdiSize = Marshal.SizeOf(typeof(BigBlueConfig.NativeMethods.RID_DEVICE_INFO));
            IntPtr rdiData = Marshal.AllocHGlobal(rdiSize);
            RID_DEVICE_INFO rdi = (RID_DEVICE_INFO)Marshal.PtrToStructure(rdiData, typeof(RID_DEVICE_INFO));
            rdi.cbSize = rdiSize;

            //for (int i = 0; i < deviceCount; i++)
            for (int devnum = (int)deviceCount - 1; devnum >= 0; devnum--)
            {
                BigBlueConfig.NativeMethods.RAWINPUTDEVICELIST ridl = (BigBlueConfig.NativeMethods.RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                    new IntPtr(rawInputDeviceListData.ToInt64() + (devnum * dwSize)), typeof(BigBlueConfig.NativeMethods.RAWINPUTDEVICELIST));

                /*
                uint cbSize = (uint)rdi.cbSize;

                if (BigBlueConfig.NativeMethods.GetRawInputDeviceInfo(ridl.hDevice, BigBlueConfig.NativeMethods.RIDI_DEVICEINFO, rdiData, ref cbSize) < 1)
                {
                    continue;
                }
                */

                

                uint deviceNameLength = 0;
                BigBlueConfig.NativeMethods.GetRawInputDeviceInfo(ridl.hDevice, BigBlueConfig.NativeMethods.RIDI_DEVICENAME, null, ref deviceNameLength);

                if (deviceNameLength <= 1)
                {
                    continue;
                }

                StringBuilder sb = new StringBuilder((int)deviceNameLength);

                //IntPtr pData = Marshal.AllocHGlobal((int)deviceNameLength);
                BigBlueConfig.NativeMethods.GetRawInputDeviceInfo(ridl.hDevice, BigBlueConfig.NativeMethods.RIDI_DEVICENAME, sb, ref deviceNameLength);
                string deviceName = sb.ToString();

                //Marshal.FreeHGlobal(pData);

                

                //The list will include the "root" keyboard and mouse devices
                //which appear to be the remote access devices used by Terminal
                //Services or the Remote Desktop - we're not interested in these
                //so the following code with drop into the next loop iteration
                if (deviceName.ToUpper().Contains("ROOT#RDP_"))
                {
                   // Marshal.FreeHGlobal(pData);
                    continue;
                }

                if (deviceName.ToUpper().Contains("&COL01"))
                {
                    //Marshal.FreeHGlobal(pData);
                    continue;
                }

                UsbDevice ud = GetUsbDevice(deviceName);

                switch (ridl.dwType)
                {
                    case BigBlueConfig.NativeMethods.RIM_TYPEKEYBOARD:
                        {
                            numberOfKeyboards++;

                            string kb = "KEYBOARD_" + numberOfKeyboards.ToString();

                            window.keyboardDevices[ridl.hDevice] = new Models.RawInputKeyboard()
                            {
                                DeviceLabel = kb,
                                DeviceName = ud.DeviceName,
                                VendorID = ud.VendorID,
                                ProductID = ud.ProductID,
                                DeviceIndex = numberOfKeyboards,
                                InputPressed = new bool[(int)Models.KeyboardInputType.MAX_KEY],
                                InputStates = new Models.InputState[(int)Models.KeyboardInputType.MAX_KEY]
                            };

                            break;
                        }
                    case BigBlueConfig.NativeMethods.RIM_TYPEMOUSE:
                        {
                            numberOfMice++;

                            string m = "MOUSE_" + numberOfMice.ToString();

                            window.mouseDevices[ridl.hDevice] = new Models.RawInputMouse()
                            {
                                DeviceLabel = m,
                                DeviceName = ud.DeviceName,
                                VendorID = ud.VendorID,
                                ProductID = ud.ProductID,
                                DeviceIndex = numberOfMice,
                                InputPressed = new bool[(int)Models.MouseInputType.MAX_MOUSE],
                                InputStates = new Models.InputState[(int)Models.MouseInputType.MAX_MOUSE]
                            };

                            // here you need to get the mouse mappings based on NumberOfDevices; that will let you get the right value from the config file
                            // this is just some kind of XML node select where we get all the input nodes for a device that matches the current mouse label 

                            break;
                        }
                    case BigBlueConfig.NativeMethods.RIM_TYPEHID:
                        {
                            if (IsHIDJoystick(ridl.hDevice) == true)
                            {
                                numberOfJoysticks++;

                                string j = "JOYSTICK_" + numberOfJoysticks.ToString();

                                window.joystickDevices[ridl.hDevice] = new Models.RawInputJoystick()
                                {
                                    DeviceLabel = j,
                                    DeviceName = ud.DeviceName,
                                    VendorID = ud.VendorID,
                                    ProductID = ud.ProductID,
                                    DeviceIndex = numberOfJoysticks,
                                    InputPressed = new bool[(int)Models.JoystickInputType.JOYSTICK_INPUT_TYPE_MAX],
                                    InputStates = new Models.InputState[(int)Models.JoystickInputType.JOYSTICK_INPUT_TYPE_MAX]
                                };
                            }

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                //Marshal.FreeHGlobal(pData);
                
            }

            Marshal.FreeHGlobal(rdiData);
            Marshal.FreeHGlobal(rawInputDeviceListData);

            return numberOfKeyboards + numberOfMice;
        }

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        internal static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        internal struct DevBroadcastDeviceinterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }

        [DllImport("user32.dll")]
        internal static extern void ClipCursor(ref System.Drawing.Rectangle rect);

        [DllImport("user32.dll")]
        internal static extern void ClipCursor(IntPtr rect);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll")]
        internal static extern bool SetSystemCursor(IntPtr hcur, uint id);

        [DllImport("user32.dll")]
        static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("User32.dll")]
        internal static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        internal static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int SetErrorMode(int wMode);

        [Flags]
        internal enum ErrorMode : uint
        {
            SEM_DEFAULT = 0x0000,
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }

        [DllImport("Kernel32.dll")]
        internal static extern ErrorMode SetErrorMode(ErrorMode mode);  //available since XP

        [DllImport("Kernel32.dll")]
        internal static extern ErrorMode GetErrorMode();  //available since Vista

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetThreadErrorMode(ErrorMode newMode, out ErrorMode oldMode);    //available since Windows 7

        [DllImport("Kernel32.dll")]
        internal static extern ErrorMode GetThreadErrorMode();    //available since Windows 7

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)BigBlueConfig.NativeMethods.WM_KEYDOWN || wParam == (IntPtr)BigBlueConfig.NativeMethods.WM_KEYUP || wParam == (IntPtr)BigBlueConfig.NativeMethods.WM_SYSKEYDOWN || wParam == (IntPtr)BigBlueConfig.NativeMethods.WM_SYSKEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // was nice knowing you
                return (IntPtr)1;
                /*
                if (vkCode == 0x5B)
                {
                    return (IntPtr)1;
                }
                 */
            }

            return BigBlueConfig.NativeMethods.CallNextHookEx(BigBlueConfig.NativeMethods._hookID, nCode, wParam, lParam);
        }

        private const string SHELL_VALUE_NAME = "Shell";
        private const string EXPLORER_EXE_NAME = "explorer.exe";

        internal static int GetWindowsErrorReportingDontShowUIValue()
        {
            int dontShowUIValue = 0;

            try
            {
                dontShowUIValue = (int)Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\Windows Error Reporting", "DontShowUI", false);
            }
            catch (Exception)
            {
            }

            return dontShowUIValue;
        }

        internal static bool IsDefaultShellEnabled()
        {
            try
            {
                string allUsersShellValue = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", SHELL_VALUE_NAME, EXPLORER_EXE_NAME);
                string currentUserShellValue = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", SHELL_VALUE_NAME, EXPLORER_EXE_NAME);

                // if the current shell value isn't the default or the all users shell value isn't the default, then we know the default isn't on
                if (!currentUserShellValue.Equals(EXPLORER_EXE_NAME, StringComparison.InvariantCultureIgnoreCase) || !allUsersShellValue.Equals(EXPLORER_EXE_NAME, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }

            // if there was some permission error or problem getting the values, let's just assume that the default shell is enabled
            return true;
        }
    }
}
