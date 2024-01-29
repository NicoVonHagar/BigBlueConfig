using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueConfig
{
    public class Models
    {
        public enum MouseInputType
        {
            Up,
            Down,
            Left,
            Right,
            Button1,
            Button2,
            Button3,
            Button4,
            Button5,
            WheelUp,
            WheelDown,
            MAX_MOUSE
        }

        public enum KeyboardInputType
        {
            Escape,
            Up,
            Down,
            Left,
            Right,
            Zero,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Enter,
            NumpadEnter,
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J,
            K,
            L,
            M,
            N,
            O,
            P,
            Q,
            R,
            S,
            T,
            U,
            V,
            W,
            X,
            Y,
            Z,
            //LeftWindows,
            //RightWindows, 
            //Applications,
            //ComputerSleep,
            NumPadZero,
            NumPadOne,
            NumPadTwo,
            NumPadThree,
            NumPadFour,
            NumPadFive,
            NumPadSix,
            NumPadSeven,
            NumPadEight,
            NumPadNine,
            Multiply,
            Add,
            //Separator,
            Subtract,
            Decimal,
            Divide,
            F1,
            F2,
            F3,
            F4,
            F5,
            F6,
            F7,
            F8,
            F9,
            F10,
            F11,
            F12,
            F13,
            F14,
            F15,
            //F16,
            //F17,
            //F18,
            //F19,
            //F20,
            //F21,
            //F22,
            //F23,
            //F24,
            CapsLock,
            NumLock,
            Scroll,
            LeftShift,
            RightShift,
            LeftCtrl,
            RightCtrl,
            LeftAlt,
            RightAlt,
            //BrowserBack,
            //BrowserForward,
            //BrowserRefresh,
            //BrowserStop,
            //BrowserSearch,
            //BrowserFavorites,
            //BrowserHome,
            //VolumeMute,
            //VolumeDown,
            //VolumeUp,
            //MediaNext,
            //MediaPrevious,
            //MediaStop,
            //MediaPlayPause,
            //MediaLaunchMail,
            //MediaSelect,
            //LaunchApp1,
            //LaunchApp2,
            Colon,
            Equals,
            Comma,
            Hyphen,
            Period,
            ForwardSlash,
            Grave,
            OpenBracket,
            BackSlash,
            CloseBracket,
            Quote,
            PageUp,
            PageDown,
            Home,
            Delete,
            Insert,
            Space,
            Backspace,
            End,
            Tab,
            //Break,
            Pause,
            SystemRequest,
            MAX_KEY
        };

        public enum GameInputLabel
        {
            NoInput,
            RAMPAGE_START,
            RAMPAGE_BACK,
            RAMPAGE_NEXT_PAGE,
            RAMPAGE_PREVIOUS_PAGE,
            RAMPAGE_NEXT_ITEM,
            RAMPAGE_PREVIOUS_ITEM,
            RAMPAGE_FIRST_ITEM,
            RAMPAGE_LAST_ITEM,
            RAMPAGE_RANDOM_ITEM,
            RAMPAGE_PUNCH_LEFT,
            RAMPAGE_PUNCH_RIGHT,
            RAMPAGE_SPECTATE,
            RAMPAGE_EXIT,
            RTYPE_START,
            RTYPE_BACK,
            RTYPE_NEXT_PAGE,
            RTYPE_PREVIOUS_PAGE,
            RTYPE_NEXT_ITEM,
            RTYPE_PREVIOUS_ITEM,
            RTYPE_FIRST_ITEM,
            RTYPE_LAST_ITEM,
            RTYPE_RANDOM_ITEM,
            RTYPE_SHOOT,
            RTYPE_WARP,
            RTYPE_EXIT,
            BIG_BLUE_EXIT,
            BIG_BLUE_VOLUME_UP,
            BIG_BLUE_VOLUME_DOWN,
            BIG_BLUE_MUTE,
            BIG_BLUE_SPEECH,
            QUIT_TO_DESKTOP,
            RESTART,
            SHUTDOWN,
            INPUT_LABEL_MAX = SHUTDOWN
        };

        public enum JoystickInputType
        {
            Button1,
            Button2,
            Button3,
            Button4,
            Button5,
            Button6,
            Button7,
            Button8,
            Button9,
            Button10,
            Button11,
            Button12,
            Button13,
            Button14,
            Button15,
            Button16,
            DPadUp,
            DPadRight,
            DPadDown,
            DPadLeft,
            Analog1Plus,
            Analog1Minus,
            Analog2Plus,
            Analog2Minus,
            Analog3Plus,
            Analog3Minus,
            Analog4Plus,
            Analog4Minus,
            Analog5Plus,
            Analog5Minus,
            Analog6Plus,
            Analog6Minus,
            JOYSTICK_INPUT_TYPE_MAX
        };

        public enum InputState
        {
            NotPressed,
            Released,
            Pressed,
            HeldDown
        };

        public class UsbVendor
        {
            public string VendorID { get; set; }

            public string VendorName { get; set; }
        }

        public class UsbDevice
        {
            public string DeviceName { get; set; }

            public string VendorID { get; set;  }

            public string ProductID { get; set; }
        }

        public class RawInputDevice
        {
            public string DeviceLabel { get; set; }

            public string DeviceName { get; set; }

            public string ProductID { get; set; }

            public string VendorID { get; set; }

            public InputState[] InputStates { get; set; }

            public bool[] InputPressed { get; set; }

            public int DeviceIndex { get; set; }
        }

        public class RawInputJoystick : RawInputDevice
        {
            public Dictionary<Models.JoystickInputType, Models.GameInputLabel> InputMappings = new Dictionary<Models.JoystickInputType, Models.GameInputLabel>();
        }

        public class RawInputMouse : RawInputDevice
        {
            public Dictionary<Models.MouseInputType, Models.GameInputLabel> InputMappings = new Dictionary<MouseInputType, GameInputLabel>();

            public int PreviousWheelUp { get; set; }

            public int PreviousWheelDown { get; set; }

            public int CurrentWheelUp { get; set; }

            public int CurrentWheelDown { get; set; }

            public int UpPixels { get; set; }

            public int DownPixels { get; set; }

            public int LeftPixels { get; set; }

            public int RightPixels { get; set; }
        }

        public class RawInputKeyboard : RawInputDevice
        {
            public Dictionary<byte, Models.GameInputLabel> InputMappings = new Dictionary<byte, GameInputLabel>();
        }
    }
}
