using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BigBlue
{
    public static class NativeMethods
    {
        public const uint WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        public const uint WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);
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

        [DllImport("User32.dll")]
        internal extern static uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

        [DllImport("User32.dll")]
        internal extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

        [DllImport("User32.dll")]
        internal extern static uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("User32.dll")]
        extern static uint GetRawInputBuffer();

        internal static void ProcessRawKeyboardInputs(BigBlue.NativeMethods.RAWINPUT raw, BigBlueWindow bbWindow)
        {
            IntPtr deviceId = (IntPtr)raw.header.hDevice;

            if (bbWindow.keyboardDictionary.ContainsKey(deviceId) == true)
            {
                BigBlue.NativeMethods.RAWKEYBOARD kb = raw.Keyboard;
                ushort virtualKeyCode = kb.VKey;
                ushort flags = kb.Flags;

                bool keyDown = BigBlue.NativeMethods.GetKeyboardInputState(virtualKeyCode, flags);

                if (keyDown == false && bbWindow.screenSaverTimeInMinutes >= 1)
                {
                    bbWindow.screenSaverTimer.Start();
                }

                string bbAction = null;
                if (bbWindow.keyboardDictionary[deviceId].TryGetValue(virtualKeyCode, out bbAction))
                {
                    bbWindow.ProcessFrontendAction(bbAction, keyDown);
                }
            }
        }

        internal static void ProcessRawMouseInputs(BigBlue.NativeMethods.RAWINPUT raw, BigBlueWindow bbWindow)
        {
            IntPtr deviceId = (IntPtr)raw.header.hDevice;

            if (bbWindow.mouseDevices.ContainsKey(deviceId) == true)
            {
                BigBlue.NativeMethods.RAWMOUSE m = raw.mouse;
                uint mouseButtons = m.ulButtons;

                // mouse button 1 down
                if (mouseButtons == 0x0001)
                {
                    string mouseButton1DownAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_1", out mouseButton1DownAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton1DownAction, true);
                    }
                }

                // mouse button 1 up
                if (mouseButtons == 0x0002)
                {
                    string mouseButton1UpAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_1", out mouseButton1UpAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton1UpAction, false);
                    }
                }

                // mouse buton 2 down
                if (mouseButtons == 0x0004)
                {
                    string mouseButton2DownAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_2", out mouseButton2DownAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton2DownAction, true);
                    }
                }

                string mouseButton2UpAction = null;
                // mouse button 2 up
                if (m.ulButtons == 0x0008)
                {
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_2", out mouseButton2UpAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton2UpAction, false);
                    }
                }

                // mouse button 3 down
                if (m.ulButtons == 0x0010)
                {
                    string mouseButton3DownAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_3", out mouseButton3DownAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton3DownAction, true);
                    }
                }

                // mouse button 3 up
                if (m.ulButtons == 0x0020)
                {
                    string mouseButton3UpAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_3", out mouseButton3UpAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton3UpAction, false);
                    }
                }

                // mouse button 4 down
                if (m.ulButtons == 0x0040)
                {
                    string mouseButton4DownAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_4", out mouseButton4DownAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton4DownAction, true);
                    }
                }

                // mouse button 4 up
                if (m.ulButtons == 0x0080)
                {
                    string mouseButton4UpAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_4", out mouseButton4UpAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton4UpAction, false);
                    }
                }

                // mouse button 5 down
                if (mouseButtons == 0x0100)
                {
                    string mouseButton5DownAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_5", out mouseButton5DownAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton5DownAction, true);
                    }
                }

                // mouse button 5 up
                if (mouseButtons == 0x0200)
                {
                    string mouseButton5UpAction = null;
                    if (bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_BUTTON_5", out mouseButton5UpAction))
                    {
                        bbWindow.ProcessFrontendAction(mouseButton5UpAction, false);
                    }
                }

                short mouseWheelDirection = (short)m.buttonsStr.usButtonData;

                //if (mouseStopWatch.ElapsedMilliseconds > mouseMovementSpeed)
                //{
                if (mouseWheelDirection == 0)
                {
                    string mouseXLeft = null;
                    if (m.lLastX < 0 && bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_X_LEFT", out mouseXLeft))
                    {
                        bbWindow.ProcessFrontendAction(mouseXLeft, null);
                    }

                    string mouseXRight = null;
                    if (m.lLastX > 0 && bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_X_RIGHT", out mouseXRight))
                    {
                        bbWindow.ProcessFrontendAction(mouseXRight, null);
                    }

                    string mouseYUp = null;
                    if (m.lLastY < 0 && bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_Y_UP", out mouseYUp))
                    {
                        bbWindow.ProcessFrontendAction(mouseYUp, null);
                    }

                    string mouseYDown = null;
                    if (m.lLastY > 0 && bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_Y_DOWN", out mouseYDown))
                    {
                        bbWindow.ProcessFrontendAction(mouseYDown, null);
                    }

                    // not even being used
                    //lastRelativeMousePosition = m.lLastX;
                }
                else
                {
                    string mouseWheelDown = null;
                    if (mouseWheelDirection < 0 && bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_WHEEL_DOWN", out mouseWheelDown))
                    {
                        bbWindow.ProcessFrontendAction(mouseWheelDown, null);
                    }

                    string mouseWheelUp = null;
                    if (mouseWheelDirection > 0 && bbWindow.mouseDevices[deviceId].TryGetValue("MOUSE_WHEEL_UP", out mouseWheelUp))
                    {
                        bbWindow.ProcessFrontendAction(mouseWheelUp, null);
                    }
                }

                // this should be moved into the mouse methods and you should return after getting one
                //mouseStopWatch.Restart();

                //}
            }
        }

        internal static bool GetKeyboardInputState(ushort virtualKeyCode, ushort flags)
        {
            bool keyDown = false;

            // if it's any of the arrow keys, we figure out key up/down differently
            if (virtualKeyCode == 0x25 || virtualKeyCode == 0x26 || virtualKeyCode == 0x27 || virtualKeyCode == 0x28)
            {
                if (flags == 3)
                {
                    keyDown = false;
                }

                if (flags == 2)
                {
                    keyDown = true;
                }
            }
            else
            {
                // the key is down
                if (flags == 0)
                {
                    keyDown = true;
                }

                if (flags == 1)
                {
                    keyDown = false;
                }
            }

            return keyDown;
        }

        /// <summary>
        /// Filters Windows messages for WM_INPUT messages and calls
        /// ProcessInputCommand if necessary.
        /// </summary>
        /// <param name="message">The Windows message.</param>
        internal static void ProcessMessage(int message, IntPtr lParam, IntPtr wParam, BigBlueWindow bbWindow)
        {
            switch (message)
            {
                case BigBlue.NativeMethods.WM_INPUT:
                    // you don't want to bother processing raw input unless no program is running OR a program's running and global inputs are enabled
                    if (!bbWindow.itsGoTime || (bbWindow.itsGoTime == true && bbWindow.globalInputs))
                    {
                        ProcessInputCommand(lParam, bbWindow);
                    }

                    break;
                case BigBlue.NativeMethods.WM_DEVICECHANGE:
                    // this will clear out and repopulate all the controls
                    bbWindow.awaitingAsync = true;

                    // should release everything so it doesn't get stuck
                    bbWindow.ReleaseAllInputs();

                    BigBlue.NativeMethods.ProvisionRawInputs(bbWindow, true);

                    bbWindow.awaitingAsync = false;

                    
                    switch ((int)wParam)
                    {
                        case BigBlue.USBNotification.DbtDeviceremovecomplete:
                            //MessageBox.Show("removed");
                            break;
                        case BigBlue.USBNotification.DbtDevicearrival:
                            //MessageBox.Show("added");
                            break;
                    }
                    
                    break;
                //case BigBlue.NativeMethods.WM_DISPLAYCHANGE:
                    // this is what happens when you turn off a home theater receiver via hdmi
                   // break;
            }
        }

        static readonly uint rawInputHeaderSize = (uint)Marshal.SizeOf(typeof(BigBlue.NativeMethods.RAWINPUTHEADER));
        
        /// <summary>
        /// Processes WM_INPUT messages to retrieve information about any
        /// keyboard events that occur.
        /// </summary>
        /// <param name="message">The WM_INPUT message to process.</param>
        internal static void ProcessInputCommand(IntPtr lParam, BigBlueWindow bbWindow)
        {
            uint dwSize = 0;
            
            // First call to GetRawInputData sets the value of dwSize dwSize can then be used to allocate the appropriate amount of memory, storing the pointer in "buffer"
            BigBlue.NativeMethods.GetRawInputData(lParam, BigBlue.NativeMethods.RID_INPUT, IntPtr.Zero, ref dwSize, rawInputHeaderSize);
            
            IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
            
            if (BigBlue.NativeMethods.GetRawInputData(lParam, BigBlue.NativeMethods.RID_INPUT, buffer, ref dwSize, rawInputHeaderSize) == dwSize)
            {
                BigBlue.NativeMethods.RAWINPUT raw = (BigBlue.NativeMethods.RAWINPUT)Marshal.PtrToStructure(buffer, typeof(BigBlue.NativeMethods.RAWINPUT));

                switch (raw.header.dwType)
                {
                    case BigBlue.NativeMethods.RIM_TYPEKEYBOARD:
                        BigBlue.NativeMethods.ProcessRawKeyboardInputs(raw, bbWindow);
                        break;
                    case BigBlue.NativeMethods.RIM_TYPEMOUSE:
                        BigBlue.NativeMethods.ProcessRawMouseInputs(raw, bbWindow);
                        break;
                    case BigBlue.NativeMethods.RIM_TYPEHID:
                        foreach (XInputDotNetPure.PlayerIndex xInputDevice in bbWindow.xInputDevices.Keys)
                        {
                            BigBlue.XInput.ProcessXInputs(xInputDevice, bbWindow);
                        }

                        BigBlue.DirectInput.ProcessDirectInputs(bbWindow);

                        break;
                }

                bbWindow.ProcessScreenSaverInput();
            }
            
            Marshal.FreeHGlobal(buffer);
        }

        /// <summary>
        /// InputDevice constructor; registers the raw input devices
        /// for the calling window.
        /// </summary>
        /// <param name="hwnd">Handle of the window listening for key presses</param>
        internal static void RegisterInputDevices(BigBlueWindow bbWindow)
        {
            bbWindow.registeredInputDevices = new BigBlue.NativeMethods.RAWINPUTDEVICE[4];

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
                bbWindow.registeredInputDevices[0].dwFlags = BigBlue.NativeMethods.RIDEV_INPUTSINK;
                bbWindow.registeredInputDevices[1].dwFlags = BigBlue.NativeMethods.RIDEV_INPUTSINK;
                bbWindow.registeredInputDevices[2].dwFlags = BigBlue.NativeMethods.RIDEV_INPUTSINK;
                bbWindow.registeredInputDevices[3].dwFlags = BigBlue.NativeMethods.RIDEV_INPUTSINK;
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

            if (!BigBlue.NativeMethods.RegisterRawInputDevices(bbWindow.registeredInputDevices, (uint)bbWindow.registeredInputDevices.Length, (uint)Marshal.SizeOf(bbWindow.registeredInputDevices[0])))
            {
                throw new ApplicationException("Failed to register raw input device(s).");
            }
        }

        internal static void UnRegisterInputDevices(BigBlueWindow bbWindow)
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

            if (!BigBlue.NativeMethods.RegisterRawInputDevices(bbWindow.registeredInputDevices, (uint)bbWindow.registeredInputDevices.Length, (uint)Marshal.SizeOf(bbWindow.registeredInputDevices[0])))
            {
                throw new ApplicationException("Failed to register raw input device(s).");
            }
        }

        internal static int ProvisionRawInputs(BigBlue.BigBlueWindow window, bool refresh)
        {
            if (refresh)
            {
                window.directInputDevices.Clear();
                window.xInputDevices.Clear();
                window.keyboardDictionary.Clear();
                window.mouseDevices.Clear();
            }

            System.Xml.XmlNodeList controlNodes = window.ConfigNode.SelectSingleNode("controls").ChildNodes;

            BigBlue.XInput.ProvisionXInputDevice(XInputDotNetPure.PlayerIndex.One, controlNodes, window.xInputDevices);
            BigBlue.XInput.ProvisionXInputDevice(XInputDotNetPure.PlayerIndex.Two, controlNodes, window.xInputDevices);
            BigBlue.XInput.ProvisionXInputDevice(XInputDotNetPure.PlayerIndex.Three, controlNodes, window.xInputDevices);
            BigBlue.XInput.ProvisionXInputDevice(XInputDotNetPure.PlayerIndex.Four, controlNodes, window.xInputDevices);

            try
            {
                BigBlue.DirectInput.ProvisionDirectInputDevices(window, controlNodes);
            }
            catch (Exception)
            {
            }

            int numberOfKeyboards = 0;
            int numberOfMice = 0;
            uint deviceCount = 0;
            int dwSize = (Marshal.SizeOf(typeof(BigBlue.NativeMethods.RAWINPUTDEVICELIST)));

            if (BigBlue.NativeMethods.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
            {
                IntPtr pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                BigBlue.NativeMethods.GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                for (int i = 0; i < deviceCount; i++)
                {
                    uint pcbSize = 0;

                    BigBlue.NativeMethods.RAWINPUTDEVICELIST rid = (BigBlue.NativeMethods.RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                                               new IntPtr((pRawInputDeviceList.ToInt64() + (dwSize * i))),
                                               typeof(BigBlue.NativeMethods.RAWINPUTDEVICELIST));

                    BigBlue.NativeMethods.GetRawInputDeviceInfo(rid.hDevice, BigBlue.NativeMethods.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                    if (pcbSize > 0)
                    {
                        IntPtr pData = Marshal.AllocHGlobal((int)pcbSize);
                        BigBlue.NativeMethods.GetRawInputDeviceInfo(rid.hDevice, BigBlue.NativeMethods.RIDI_DEVICENAME, pData, ref pcbSize);
                        string deviceName = Marshal.PtrToStringAnsi(pData);

                        //The list will include the "root" keyboard and mouse devices
                        //which appear to be the remote access devices used by Terminal
                        //Services or the Remote Desktop - we're not interested in these
                        //so the following code with drop into the next loop iteration
                        if (deviceName.ToUpper().Contains("ROOT"))
                        {
                            continue;
                        }

                        //string deviceId = rid.hDevice.ToString();

                        //If the device is identified as a keyboard or HID device,
                        //create a DeviceInfo object to store information about it
                        if (rid.dwType == BigBlue.NativeMethods.RIM_TYPEKEYBOARD)
                        {
                            numberOfKeyboards = numberOfKeyboards + 1;

                            // get the name of the mouse to match in the config file eg. MOUSE1
                            string currentKeyboardLabel = "KEYBOARD_" + numberOfKeyboards.ToString();

                            Dictionary<ushort, string> kbMappings = new Dictionary<ushort, string>();

                            foreach (System.Xml.XmlNode cNode in controlNodes)
                            {
                                string deviceLabel = cNode.Attributes["device"].InnerText;

                                if ((deviceLabel == "ANY_KEYBOARD" || deviceLabel == currentKeyboardLabel) && deviceLabel.Contains("KEYBOARD"))
                                {
                                    string input = cNode.Attributes["input"].InnerText;
                                    string action = cNode.Attributes["action"].InnerText;

                                    System.Windows.Input.Key k = (System.Windows.Input.Key)Enum.Parse(typeof(System.Windows.Input.Key), input);

                                    if (k == System.Windows.Input.Key.LeftCtrl || k == System.Windows.Input.Key.RightCtrl)
                                    {
                                        kbMappings[0x11] = action;
                                    }
                                    else if (k == System.Windows.Input.Key.Enter || k == System.Windows.Input.Key.Return)
                                    {
                                        kbMappings[0x0d] = action;
                                    }
                                    else if (k == System.Windows.Input.Key.LeftShift || k == System.Windows.Input.Key.RightShift)
                                    {
                                        kbMappings[0x10] = action;
                                    }
                                    else if (k == System.Windows.Input.Key.LeftAlt || k == System.Windows.Input.Key.RightAlt)
                                    {
                                        kbMappings[0x12] = action;
                                    }
                                    else
                                    {
                                        ushort kuShort = (ushort)System.Windows.Input.KeyInterop.VirtualKeyFromKey(k);
                                        kbMappings[kuShort] = action;
                                    }
                                }


                                if (kbMappings.Count > 0)
                                {
                                    window.keyboardDictionary[rid.hDevice] = kbMappings;
                                }
                            }
                        }

                        if (rid.dwType == BigBlue.NativeMethods.RIM_TYPEMOUSE)
                        {

                            numberOfMice = numberOfMice + 1;

                            // get the name of the mouse to match in the config file eg. MOUSE1
                            string currentMouseLabel = "MOUSE_" + numberOfMice.ToString();

                            Dictionary<string, string> mouseMappings = new Dictionary<string, string>();

                            foreach (System.Xml.XmlNode cNode in controlNodes)
                            {
                                string deviceLabel = cNode.Attributes["device"].InnerText;

                                if ((deviceLabel == "ANY_MOUSE" || deviceLabel == currentMouseLabel) && deviceLabel.Contains("MOUSE"))
                                {
                                    string input = cNode.Attributes["input"].InnerText;
                                    string action = cNode.Attributes["action"].InnerText;

                                    mouseMappings[input] = action;
                                }
                            }

                            // add these mouse mappings
                            if (mouseMappings.Count() > 0)
                            {
                                window.mouseDevices[rid.hDevice] = mouseMappings;
                            }

                            // here you need to get the mouse mappings based on NumberOfDevices; that will let you get the right value from the config file
                            // this is just some kind of XML node select where we get all the input nodes for a device that matches the current mouse label 
                        }

                        Marshal.FreeHGlobal(pData);
                    }
                }

                Marshal.FreeHGlobal(pRawInputDeviceList);

                return numberOfKeyboards + numberOfMice;
            }
            else
            {
                throw new ApplicationException("Error!");
            }
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
            if (nCode >= 0 && 
                    (
                        wParam == (IntPtr)BigBlue.NativeMethods.WM_KEYDOWN 
                        || wParam == (IntPtr)BigBlue.NativeMethods.WM_KEYUP 
                        || wParam == (IntPtr)BigBlue.NativeMethods.WM_SYSKEYDOWN 
                        || wParam == (IntPtr)BigBlue.NativeMethods.WM_SYSKEYUP
                    )
                )
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // was nice knowing you
                return (IntPtr)1;
            }

            return BigBlue.NativeMethods.CallNextHookEx(BigBlue.NativeMethods._hookID, nCode, wParam, lParam);
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
