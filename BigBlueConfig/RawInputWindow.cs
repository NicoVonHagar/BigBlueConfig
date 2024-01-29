using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace BigBlueConfig
{
    public class RawInputWindow : System.Windows.Window
    {
        internal HwndSource win32Window;

        internal IntPtr windowHandle;

        internal Dictionary<byte, Models.KeyboardInputType> byteToKeyMap = new Dictionary<byte, Models.KeyboardInputType>();

        internal Dictionary<Models.KeyboardInputType, byte> keyToByteMap = new Dictionary<Models.KeyboardInputType, byte>();

        internal Dictionary<IntPtr, Models.RawInputMouse> mouseDevices = new Dictionary<IntPtr, Models.RawInputMouse>();
        internal Dictionary<IntPtr, Models.RawInputKeyboard> keyboardDevices = new Dictionary<IntPtr, Models.RawInputKeyboard>();
        internal Dictionary<IntPtr, Models.RawInputJoystick> joystickDevices = new Dictionary<IntPtr, Models.RawInputJoystick>();

        internal BigBlueConfig.NativeMethods.RAWINPUTDEVICE[] registeredInputDevices = null;

        internal bool globalInputs = false;

        internal bool awaitingAsync = false;

        internal void StartFrontend()
        {
            // limit animation framerate
            //Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 120 });

            InitializeFrontEnd();
        }

        internal virtual void InitializeFrontEnd()
        {
        }

        internal virtual void InitializeSound()
        {

        }

        internal void ProvisionKeyboardMap()
        {
            byteToKeyMap[0x01] = Models.KeyboardInputType.Escape;
            byteToKeyMap[0x02] = Models.KeyboardInputType.One;
            byteToKeyMap[0x03] = Models.KeyboardInputType.Two;
            byteToKeyMap[0x04] = Models.KeyboardInputType.Three;
            byteToKeyMap[0x05] = Models.KeyboardInputType.Four;
            byteToKeyMap[0x06] = Models.KeyboardInputType.Five;
            byteToKeyMap[0x07] = Models.KeyboardInputType.Six;
            byteToKeyMap[0x08] = Models.KeyboardInputType.Seven;
            byteToKeyMap[0x09] = Models.KeyboardInputType.Eight;
            byteToKeyMap[0x0A] = Models.KeyboardInputType.Nine;
            byteToKeyMap[0x0B] = Models.KeyboardInputType.Zero;

            byteToKeyMap[0x0C] = Models.KeyboardInputType.Hyphen;
            byteToKeyMap[0x0D] = Models.KeyboardInputType.Equals;
            byteToKeyMap[0x0E] = Models.KeyboardInputType.Backspace;
            byteToKeyMap[0x0F] = Models.KeyboardInputType.Tab;
            byteToKeyMap[0x10] = Models.KeyboardInputType.Q;
            byteToKeyMap[0x11] = Models.KeyboardInputType.W;
            byteToKeyMap[0x12] = Models.KeyboardInputType.E;
            byteToKeyMap[0x13] = Models.KeyboardInputType.R;
            byteToKeyMap[0x14] = Models.KeyboardInputType.T;
            byteToKeyMap[0x15] = Models.KeyboardInputType.Y;
            byteToKeyMap[0x16] = Models.KeyboardInputType.U;
            byteToKeyMap[0x17] = Models.KeyboardInputType.I;
            byteToKeyMap[0x18] = Models.KeyboardInputType.O;
            byteToKeyMap[0x19] = Models.KeyboardInputType.P;
            byteToKeyMap[0x1A] = Models.KeyboardInputType.OpenBracket;
            byteToKeyMap[0x1B] = Models.KeyboardInputType.CloseBracket;
            byteToKeyMap[0x1C] = Models.KeyboardInputType.Enter;
            byteToKeyMap[0x1D] = Models.KeyboardInputType.LeftCtrl;
            byteToKeyMap[0x1E] = Models.KeyboardInputType.A;
            byteToKeyMap[0x1F] = Models.KeyboardInputType.S;
            byteToKeyMap[0x20] = Models.KeyboardInputType.D;
            byteToKeyMap[0x21] = Models.KeyboardInputType.F;
            byteToKeyMap[0x22] = Models.KeyboardInputType.G;
            byteToKeyMap[0x23] = Models.KeyboardInputType.H;
            byteToKeyMap[0x24] = Models.KeyboardInputType.J;
            byteToKeyMap[0x25] = Models.KeyboardInputType.K;
            byteToKeyMap[0x26] = Models.KeyboardInputType.L;
            byteToKeyMap[0x27] = Models.KeyboardInputType.Colon;
            byteToKeyMap[0x28] = Models.KeyboardInputType.Quote;
            byteToKeyMap[0x29] = Models.KeyboardInputType.Grave;
            byteToKeyMap[0x2A] = Models.KeyboardInputType.LeftShift;
            byteToKeyMap[0x2B] = Models.KeyboardInputType.BackSlash;
            byteToKeyMap[0x2C] = Models.KeyboardInputType.Z;
            byteToKeyMap[0x2D] = Models.KeyboardInputType.X;
            byteToKeyMap[0x2E] = Models.KeyboardInputType.C;
            byteToKeyMap[0x2F] = Models.KeyboardInputType.V;
            byteToKeyMap[0x30] = Models.KeyboardInputType.B;
            byteToKeyMap[0x31] = Models.KeyboardInputType.N;
            byteToKeyMap[0x32] = Models.KeyboardInputType.M;
            byteToKeyMap[0x33] = Models.KeyboardInputType.Comma;
            byteToKeyMap[0x34] = Models.KeyboardInputType.Period;
            byteToKeyMap[0x35] = Models.KeyboardInputType.ForwardSlash;
            byteToKeyMap[0x36] = Models.KeyboardInputType.RightShift;
            byteToKeyMap[0x37] = Models.KeyboardInputType.Multiply;
            byteToKeyMap[0x38] = Models.KeyboardInputType.LeftAlt;
            byteToKeyMap[0x39] = Models.KeyboardInputType.Space;
            byteToKeyMap[0x3A] = Models.KeyboardInputType.CapsLock;
            byteToKeyMap[0x3B] = Models.KeyboardInputType.F1;
            byteToKeyMap[0x3C] = Models.KeyboardInputType.F2;
            byteToKeyMap[0x3D] = Models.KeyboardInputType.F3;
            byteToKeyMap[0x3E] = Models.KeyboardInputType.F4;
            byteToKeyMap[0x3F] = Models.KeyboardInputType.F5;
            byteToKeyMap[0x40] = Models.KeyboardInputType.F6;
            byteToKeyMap[0x41] = Models.KeyboardInputType.F7;
            byteToKeyMap[0x42] = Models.KeyboardInputType.F8;
            byteToKeyMap[0x43] = Models.KeyboardInputType.F9;
            byteToKeyMap[0x44] = Models.KeyboardInputType.F10;
            byteToKeyMap[0x45] = Models.KeyboardInputType.NumLock;
            byteToKeyMap[0x46] = Models.KeyboardInputType.Scroll;

            byteToKeyMap[0x47] = Models.KeyboardInputType.NumPadSeven;
            byteToKeyMap[0x48] = Models.KeyboardInputType.NumPadEight;
            byteToKeyMap[0x49] = Models.KeyboardInputType.NumPadNine;
            byteToKeyMap[0x4A] = Models.KeyboardInputType.Subtract;
            byteToKeyMap[0x4B] = Models.KeyboardInputType.NumPadFour;
            byteToKeyMap[0x4C] = Models.KeyboardInputType.NumPadFive;
            byteToKeyMap[0x4D] = Models.KeyboardInputType.NumPadSix;
            byteToKeyMap[0x4E] = Models.KeyboardInputType.Add;
            byteToKeyMap[0x4F] = Models.KeyboardInputType.NumPadOne;
            byteToKeyMap[0x50] = Models.KeyboardInputType.NumPadTwo;
            byteToKeyMap[0x51] = Models.KeyboardInputType.NumPadThree;
            byteToKeyMap[0x52] = Models.KeyboardInputType.NumPadZero;
            byteToKeyMap[0x53] = Models.KeyboardInputType.Decimal;
            byteToKeyMap[0x57] = Models.KeyboardInputType.F11;
            byteToKeyMap[0x58] = Models.KeyboardInputType.F12;
            byteToKeyMap[0x64] = Models.KeyboardInputType.F13;
            byteToKeyMap[0x65] = Models.KeyboardInputType.F14;
            byteToKeyMap[0x66] = Models.KeyboardInputType.F15;

            //keyboardMap[0x70]	DIK_KANA	Kana	Japenese Keyboard
            //keyboardMap[0x79]	DIK_CONVERT	Convert	Japenese Keyboard
            //keyboardMap[0x7B]	DIK_NOCONVERT	No Convert	Japenese Keyboard
            //keyboardMap[0x7D]	DIK_YEN	¥	Japenese Keyboard
            //keyboardMap[0x8D]	DIK_NUMPADEQUALS = NEC PC - 98
            //keyboardMap[0x90]	DIK_CIRCUMFLEX ^ Japenese Keyboard
            //keyboardMap[0x91]	DIK_AT	@	NEC PC - 98
            //keyboardMap[0x92]	DIK_COLON:NEC PC - 98
            //keyboardMap[0x93]	DIK_UNDERLINE	_	NEC PC - 98
            //keyboardMap[0x94]	DIK_KANJI	Kanji	Japenese Keyboard
            //keyboardMap[0x95]	DIK_STOP	Stop	NEC PC - 98
            //keyboardMap[0x96]	DIK_AX(Japan AX)
            //keyboardMap[0x97]	DIK_UNLABELED(J3100)
            byteToKeyMap[0x9C] = Models.KeyboardInputType.NumpadEnter;
            byteToKeyMap[0x9D] = Models.KeyboardInputType.RightCtrl;
            //keyboardMap[0xB3]	DIK_NUMPADCOMMA, (Numpad)NEC PC - 98
            byteToKeyMap[0xB5] = Models.KeyboardInputType.Divide;
            byteToKeyMap[0xB7] = Models.KeyboardInputType.SystemRequest;
            byteToKeyMap[0xB8] = Models.KeyboardInputType.RightAlt;
            byteToKeyMap[0xC5] = Models.KeyboardInputType.Pause;
            byteToKeyMap[0xC7] = Models.KeyboardInputType.Home;
            byteToKeyMap[0xC8] = Models.KeyboardInputType.Up;
            byteToKeyMap[0xC9] = Models.KeyboardInputType.PageUp;
            byteToKeyMap[0xCB] = Models.KeyboardInputType.Left;
            byteToKeyMap[0xCD] = Models.KeyboardInputType.Right;
            byteToKeyMap[0xCF] = Models.KeyboardInputType.End;
            byteToKeyMap[0xD0] = Models.KeyboardInputType.Down;
            byteToKeyMap[0xD1] = Models.KeyboardInputType.PageDown;
            byteToKeyMap[0xD2] = Models.KeyboardInputType.Insert;
            byteToKeyMap[0xD3] = Models.KeyboardInputType.Delete;
            //keyboardMap[0xDB] = Models.KeyboardInputType.LeftWindows;
            //keyboardMap[0xDC] = Models.KeyboardInputType.RightWindows;
            //keyboardMap[0xDD] = Models.KeyboardInputType.Applications; //	DIK_APPS	Menu
            //keyboardMap[0xDE] = Models.KeyboardInputType.Power;//	DIK_POWER	Power
            //keyboardMap[0xDF] = Models.KeyboardInputType.ComputerSleep;

            byteToKeyMap[0x1C] = Models.KeyboardInputType.Enter;
            byteToKeyMap[0x9C] = Models.KeyboardInputType.NumpadEnter;


            // second map
            keyToByteMap[Models.KeyboardInputType.Escape] = 0x01;
            keyToByteMap[Models.KeyboardInputType.One] = 0x02;
            keyToByteMap[Models.KeyboardInputType.Two] = 0x03;
            keyToByteMap[Models.KeyboardInputType.Three] = 0x04;
            keyToByteMap[Models.KeyboardInputType.Four] = 0x05;
            keyToByteMap[Models.KeyboardInputType.Five] = 0x06;
            keyToByteMap[Models.KeyboardInputType.Six] = 0x07;
            keyToByteMap[Models.KeyboardInputType.Seven] = 0x08;
            keyToByteMap[Models.KeyboardInputType.Eight] = 0x09;
            keyToByteMap[Models.KeyboardInputType.Nine] = 0x0A;
            keyToByteMap[Models.KeyboardInputType.Zero] = 0x0B;
            keyToByteMap[Models.KeyboardInputType.Hyphen] = 0x0C;
            keyToByteMap[Models.KeyboardInputType.Equals] = 0x0D;
            keyToByteMap[Models.KeyboardInputType.Backspace] = 0x0E;
            keyToByteMap[Models.KeyboardInputType.Tab] = 0x0F;
            keyToByteMap[Models.KeyboardInputType.Q] = 0x10;
            keyToByteMap[Models.KeyboardInputType.W] = 0x11;
            keyToByteMap[Models.KeyboardInputType.E] = 0x12;
            keyToByteMap[Models.KeyboardInputType.R] = 0x13;
            keyToByteMap[Models.KeyboardInputType.T] = 0x14;
            keyToByteMap[Models.KeyboardInputType.Y] = 0x15;
            keyToByteMap[Models.KeyboardInputType.U] = 0x16;
            keyToByteMap[Models.KeyboardInputType.I] = 0x17;
            keyToByteMap[Models.KeyboardInputType.O] = 0x18;
            keyToByteMap[Models.KeyboardInputType.P] = 0x19;
            keyToByteMap[Models.KeyboardInputType.OpenBracket] = 0x1A;
            keyToByteMap[Models.KeyboardInputType.CloseBracket] = 0x1B;
            keyToByteMap[Models.KeyboardInputType.Enter] = 0x1C;
            keyToByteMap[Models.KeyboardInputType.LeftCtrl] = 0x1D;
            keyToByteMap[Models.KeyboardInputType.A] = 0x1E;
            keyToByteMap[Models.KeyboardInputType.S] = 0x1F;
            keyToByteMap[Models.KeyboardInputType.D] = 0x20;
            keyToByteMap[Models.KeyboardInputType.F] = 0x21;
            keyToByteMap[Models.KeyboardInputType.G] = 0x22;
            keyToByteMap[Models.KeyboardInputType.H] = 0x23;
            keyToByteMap[Models.KeyboardInputType.J] = 0x24;
            keyToByteMap[Models.KeyboardInputType.K] = 0x25;
            keyToByteMap[Models.KeyboardInputType.L] = 0x26;
            keyToByteMap[Models.KeyboardInputType.Colon] = 0x27;
            keyToByteMap[Models.KeyboardInputType.Quote] = 0x28;
            keyToByteMap[Models.KeyboardInputType.Grave] = 0x29;
            keyToByteMap[Models.KeyboardInputType.LeftShift] = 0x2A;
            keyToByteMap[Models.KeyboardInputType.BackSlash] = 0x2B;
            keyToByteMap[Models.KeyboardInputType.Z] = 0x2C;
            keyToByteMap[Models.KeyboardInputType.X] = 0x2D;
            keyToByteMap[Models.KeyboardInputType.C] = 0x2E;
            keyToByteMap[Models.KeyboardInputType.V] = 0x2F;
            keyToByteMap[Models.KeyboardInputType.B] = 0x30;
            keyToByteMap[Models.KeyboardInputType.N] = 0x31;
            keyToByteMap[Models.KeyboardInputType.M] = 0x32;
            keyToByteMap[Models.KeyboardInputType.Comma] = 0x33;
            keyToByteMap[Models.KeyboardInputType.Period] = 0x34;
            keyToByteMap[Models.KeyboardInputType.ForwardSlash] = 0x35;
            keyToByteMap[Models.KeyboardInputType.RightShift] = 0x36;
            keyToByteMap[Models.KeyboardInputType.Multiply] = 0x37;
            keyToByteMap[Models.KeyboardInputType.LeftAlt] = 0x38;
            keyToByteMap[Models.KeyboardInputType.Space] = 0x39;
            keyToByteMap[Models.KeyboardInputType.CapsLock] = 0x3A;
            keyToByteMap[Models.KeyboardInputType.F1] = 0x3B;
            keyToByteMap[Models.KeyboardInputType.F2] = 0x3C;
            keyToByteMap[Models.KeyboardInputType.F3] = 0x3D;
            keyToByteMap[Models.KeyboardInputType.F4] = 0x3E;
            keyToByteMap[Models.KeyboardInputType.F5] = 0x3F;
            keyToByteMap[Models.KeyboardInputType.F6] = 0x40;
            keyToByteMap[Models.KeyboardInputType.F7] = 0x41;
            keyToByteMap[Models.KeyboardInputType.F8] = 0x42;
            keyToByteMap[Models.KeyboardInputType.F9] = 0x43;
            keyToByteMap[Models.KeyboardInputType.F10] = 0x44;
            keyToByteMap[Models.KeyboardInputType.NumLock] = 0x45;
            keyToByteMap[Models.KeyboardInputType.Scroll] = 0x46;

            keyToByteMap[Models.KeyboardInputType.NumPadSeven] = 0x47;
            keyToByteMap[Models.KeyboardInputType.NumPadEight] = 0x48;
            keyToByteMap[Models.KeyboardInputType.NumPadNine] = 0x49;
            keyToByteMap[Models.KeyboardInputType.Subtract] = 0x4A;
            keyToByteMap[Models.KeyboardInputType.NumPadFour] = 0x4B;
            keyToByteMap[Models.KeyboardInputType.NumPadFive] = 0x4C;
            keyToByteMap[Models.KeyboardInputType.NumPadSix] = 0x4D;
            keyToByteMap[Models.KeyboardInputType.Add] = 0x4E;
            keyToByteMap[Models.KeyboardInputType.NumPadOne] = 0x4F;
            keyToByteMap[Models.KeyboardInputType.NumPadTwo] = 0x50;
            keyToByteMap[Models.KeyboardInputType.NumPadThree] = 0x51;
            keyToByteMap[Models.KeyboardInputType.NumPadZero] = 0x52;
            keyToByteMap[Models.KeyboardInputType.Decimal] = 0x53;
            keyToByteMap[Models.KeyboardInputType.F11] = 0x57;
            keyToByteMap[Models.KeyboardInputType.F12] = 0x58;
            keyToByteMap[Models.KeyboardInputType.F13] = 0x64;
            keyToByteMap[Models.KeyboardInputType.F14] = 0x65;
            keyToByteMap[Models.KeyboardInputType.F15] = 0x66;

            //keyboardMap[0x70]	DIK_KANA	Kana	Japenese Keyboard
            //keyboardMap[0x79]	DIK_CONVERT	Convert	Japenese Keyboard
            //keyboardMap[0x7B]	DIK_NOCONVERT	No Convert	Japenese Keyboard
            //keyboardMap[0x7D]	DIK_YEN	¥	Japenese Keyboard
            //keyboardMap[0x8D]	DIK_NUMPADEQUALS = NEC PC - 98
            //keyboardMap[0x90]	DIK_CIRCUMFLEX ^ Japenese Keyboard
            //keyboardMap[0x91]	DIK_AT	@	NEC PC - 98
            //keyboardMap[0x92]	DIK_COLON:NEC PC - 98
            //keyboardMap[0x93]	DIK_UNDERLINE	_	NEC PC - 98
            //keyboardMap[0x94]	DIK_KANJI	Kanji	Japenese Keyboard
            //keyboardMap[0x95]	DIK_STOP	Stop	NEC PC - 98
            //keyboardMap[0x96]	DIK_AX(Japan AX)
            //keyboardMap[0x97]	DIK_UNLABELED(J3100)
            keyToByteMap[Models.KeyboardInputType.NumpadEnter] = 0x9C;
            keyToByteMap[Models.KeyboardInputType.RightCtrl] = 0x9D;
            //keyboardMap[0xB3]	DIK_NUMPADCOMMA, (Numpad)NEC PC - 98
            keyToByteMap[Models.KeyboardInputType.Divide] = 0xB5;
            keyToByteMap[Models.KeyboardInputType.SystemRequest] = 0xB7;
            keyToByteMap[Models.KeyboardInputType.RightAlt] = 0xB8;
            keyToByteMap[Models.KeyboardInputType.Pause] = 0xC5;
            keyToByteMap[Models.KeyboardInputType.Home] = 0xC7;
            keyToByteMap[Models.KeyboardInputType.Up] = 0xC8;
            keyToByteMap[Models.KeyboardInputType.PageUp] = 0xC9;
            keyToByteMap[Models.KeyboardInputType.Left] = 0xCB;
            keyToByteMap[Models.KeyboardInputType.Right] = 0xCD;
            keyToByteMap[Models.KeyboardInputType.End] = 0xCF;
            keyToByteMap[Models.KeyboardInputType.Down] = 0xD0;
            keyToByteMap[Models.KeyboardInputType.PageDown] = 0xD1;
            keyToByteMap[Models.KeyboardInputType.Insert] = 0xD2;
            keyToByteMap[Models.KeyboardInputType.Delete] = 0xD3;
            //keyboardMap[Models.KeyboardInputType.LeftWindows] = 0xDB;
            //keyboardMap[Models.KeyboardInputType.RightWindows] = 0xDC;
            //keyboardMap[Models.KeyboardInputType.Applications] = 0xDD; //	DIK_APPS	Menu
            //keyboardMap[Models.KeyboardInputType.Power] = 0xDE;//	DIK_POWER	Power
            //keyboardMap[Models.KeyboardInputType.ComputerSleep] = 0xDF;

            keyToByteMap[Models.KeyboardInputType.Enter] = 0x1C;
            keyToByteMap[Models.KeyboardInputType.NumpadEnter] = 0x9C;
        }

        private void SetInputStates(Models.RawInputDevice rid, int inputId)
        {
            if (rid.InputPressed[inputId])
            {
                if (rid.InputStates[inputId] >= Models.InputState.Pressed)
                {
                    rid.InputStates[inputId] = Models.InputState.HeldDown;
                }
                else
                {
                    rid.InputStates[inputId] = Models.InputState.Pressed;
                }
            }
            else
            {
                if (rid.InputStates[inputId] >= Models.InputState.Pressed)
                {
                    rid.InputStates[inputId] = Models.InputState.Released;
                }
                else
                {
                    rid.InputStates[inputId] = Models.InputState.NotPressed;
                }
            }
        }

        internal void UpdateRawInputStates()
        {
            foreach (KeyValuePair<IntPtr, Models.RawInputKeyboard> kd in keyboardDevices)
            {
                Models.RawInputKeyboard rid = kd.Value;

                for (int jitIndex = 0; jitIndex != (int)Models.KeyboardInputType.MAX_KEY; ++jitIndex)
                {
                    SetInputStates(rid, jitIndex);
                }
            }

            foreach (KeyValuePair<IntPtr, Models.RawInputJoystick> jd in joystickDevices)
            {
                Models.RawInputJoystick rid = jd.Value;

                for (int jitIndex = 0; jitIndex != (int)Models.JoystickInputType.JOYSTICK_INPUT_TYPE_MAX; ++jitIndex)
                {
                    SetInputStates(rid, jitIndex);
                }
            }

            foreach (KeyValuePair<IntPtr, Models.RawInputMouse> md in mouseDevices)
            {
                Models.RawInputMouse rid = md.Value;

                for (int mitIndex = 0; mitIndex != (int)Models.MouseInputType.MAX_MOUSE; ++mitIndex)
                {
                    switch ((Models.MouseInputType)mitIndex)
                    {
                        case Models.MouseInputType.WheelDown:
                            {
                                if (rid.PreviousWheelDown == rid.CurrentWheelDown)
                                {
                                    rid.InputPressed[mitIndex] = false;
                                }

                                rid.PreviousWheelDown = rid.CurrentWheelDown;

                                break;
                            }
                        case Models.MouseInputType.WheelUp:
                            {
                                if (rid.PreviousWheelUp == rid.CurrentWheelUp)
                                {
                                    rid.InputPressed[mitIndex] = false;
                                }

                                rid.PreviousWheelUp = rid.CurrentWheelUp;

                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                    SetInputStates(rid, mitIndex);
                }

                // do something with entry.Value or entry.Key
            }
        }

        internal void ReleaseRawInputStates()
        {
            foreach (KeyValuePair<IntPtr, Models.RawInputKeyboard> kd in keyboardDevices)
            {
                Models.RawInputKeyboard rid = kd.Value;

                for (int jitIndex = 0; jitIndex != (int)Models.KeyboardInputType.MAX_KEY; ++jitIndex)
                {
                    rid.InputStates[jitIndex] = Models.InputState.NotPressed;
                }
            }

            foreach (KeyValuePair<IntPtr, Models.RawInputJoystick> jd in joystickDevices)
            {
                Models.RawInputJoystick rid = jd.Value;

                for (int jitIndex = 0; jitIndex != (int)Models.JoystickInputType.JOYSTICK_INPUT_TYPE_MAX; ++jitIndex)
                {
                    rid.InputStates[jitIndex] = Models.InputState.NotPressed;
                }
            }

            foreach (KeyValuePair<IntPtr, Models.RawInputMouse> md in mouseDevices)
            {
                Models.RawInputMouse rid = md.Value;

                for (int mitIndex = 0; mitIndex != (int)Models.MouseInputType.MAX_MOUSE; ++mitIndex)
                {
                    rid.InputStates[mitIndex] = Models.InputState.NotPressed;
                }

                // do something with entry.Value or entry.Key
            }
        }

        internal bool IsInteger(double d)
        {
            return unchecked(d == (int)d);
        }

        public virtual void OnFrame(object sender, EventArgs e)
        {
        }

        internal void RegisterHotKey(int hotKeyId, uint hotKey)
        {
            //var helper = new WindowInteropHelper(this);

            if (!BigBlueConfig.NativeMethods.RegisterHotKey(windowHandle, hotKeyId, BigBlueConfig.NativeMethods.MOD_CTRL, hotKey))
            {
                // handle error
            }
        }

        internal void UnregisterHotKey(int hotkeyId)
        {
            //var helper = new WindowInteropHelper(this);
            BigBlueConfig.NativeMethods.UnregisterHotKey(windowHandle, hotkeyId);
        }

        protected override void OnClosed(EventArgs e)
        {
            // this unregisters the global hotkey
            //win32Window.RemoveHook(HwndHook);
            //win32Window = null;
            //UnregisterHotKey(ESC_OVERRIDE_HOTKEY_ID);

            // unregister the volume controls
            //UnregisterHotKey(VOLUME_DOWN_HOTKEY_ID);
            //UnregisterHotKey(VOLUME_UP_HOTKEY_ID);
            //UnregisterHotKey(VOLUME_MUTE_HOTKEY_ID);

            base.OnClosed(e);
        }

        private static IntPtr SetHook(BigBlueConfig.NativeMethods.LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return BigBlueConfig.NativeMethods.SetWindowsHookEx(BigBlueConfig.NativeMethods.WH_KEYBOARD_LL, proc, BigBlueConfig.NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        

        internal IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // I could have done one of two things here.
            // 1. Use a Message as it was used before.
            // 2. Changes the ProcessMessage method to handle all of these parameters(more work).
            //    I opted for the easy way.

            //Note: Depending on your application you may or may not want to set the handled param.

            BigBlueConfig.NativeMethods.ProcessMessage(msg, lParam, wParam, this);

            return IntPtr.Zero;
        }



        internal void ProvisionWindowsHooks()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);

            windowHandle = helper.Handle;

            win32Window = HwndSource.FromHwnd(windowHandle);
            win32Window.AddHook(HwndHook);

            //BigBlue.USBNotification.RegisterUsbDeviceNotification(windowHandle);
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // kill video control
            //stopVideo();

            CompositionTarget.Rendering -= OnFrame;

            // remove the hook for raw input
            //win32Window.RemoveHook(HwndHook);

            /*
            if (ptrHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(ptrHook);
                ptrHook = IntPtr.Zero;
            }
             */

            base.OnClosing(e);
        }
    }
}
