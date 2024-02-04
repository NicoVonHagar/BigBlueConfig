using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BigBlue
{
    public static class XInput
    {
        internal static void ProcessXInputs(XInputDotNetPure.PlayerIndex index, BigBlueWindow bbWindow)
        {
            XInputDotNetPure.GamePadState state = XInputDotNetPure.GamePad.GetState(index);

            if (state.IsConnected == true)
            {
                XInputDotNetPure.GamePadButtons xInputButtons = state.Buttons;
                XInputDotNetPure.GamePadDPad xInputDpad = state.DPad;
                XInputDotNetPure.GamePadTriggers xInputTriggers = state.Triggers;

                if (xInputButtons.LeftShoulder == XInputDotNetPure.ButtonState.Pressed || xInputButtons.LeftShoulder == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputLeftShoulderButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("LEFT_SHOULDER_BUTTON", out xInputLeftShoulderButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.LeftShoulder == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputLeftShoulderButtonAction, pressed);
                    }
                }

                if (xInputButtons.RightShoulder == XInputDotNetPure.ButtonState.Pressed || xInputButtons.RightShoulder == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputRightShoulderButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("RIGHT_SHOULDER_BUTTON", out xInputRightShoulderButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.RightShoulder == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputRightShoulderButtonAction, pressed);
                    }
                }

                if (xInputTriggers.Left == 1)
                {
                    string xInputXButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("LEFT_TRIGGER", out xInputXButtonAction))
                    {
                        bbWindow.ProcessFrontendAction(xInputXButtonAction, true);
                    }
                }

                if (xInputTriggers.Right == 1)
                {
                    string xInputXButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("RIGHT_TRIGGER", out xInputXButtonAction))
                    {
                        bbWindow.ProcessFrontendAction(xInputXButtonAction, true);
                    }
                }


                if (xInputButtons.Guide == XInputDotNetPure.ButtonState.Pressed || xInputButtons.A == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputGuideButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("GUIDE_BUTTON", out xInputGuideButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.Guide == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputGuideButtonAction, pressed);
                    }
                }

                if (xInputButtons.Start == XInputDotNetPure.ButtonState.Pressed || xInputButtons.Start == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputStartButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("START_BUTTON", out xInputStartButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.Start == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputStartButtonAction, pressed);
                    }
                }

                if (xInputButtons.Back == XInputDotNetPure.ButtonState.Pressed || xInputButtons.Back == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputBackButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("BACK_BUTTON", out xInputBackButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.Back == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputBackButtonAction, pressed);
                    }
                }

                if (xInputButtons.A == XInputDotNetPure.ButtonState.Pressed || xInputButtons.A == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputAButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("A_BUTTON", out xInputAButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.A == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputAButtonAction, pressed);
                    }
                }

                if (xInputButtons.B == XInputDotNetPure.ButtonState.Pressed || xInputButtons.B == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputBButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("B_BUTTON", out xInputBButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.B == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputBButtonAction, pressed);
                    }
                }

                if (xInputButtons.X == XInputDotNetPure.ButtonState.Pressed || xInputButtons.X == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputXButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("X_BUTTON", out xInputXButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.X == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputXButtonAction, pressed);
                    }
                }

                if (xInputButtons.Y == XInputDotNetPure.ButtonState.Pressed || xInputButtons.Y == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputYButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("Y_BUTTON", out xInputYButtonAction))
                    {
                        bool pressed = false;
                        if (xInputButtons.Y == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputYButtonAction, pressed);
                    }
                }

                if (xInputDpad.Up == XInputDotNetPure.ButtonState.Pressed || xInputDpad.Up == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputUpButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("DPAD_UP", out xInputUpButtonAction))
                    {
                        bool pressed = false;
                        if (xInputDpad.Up == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputUpButtonAction, pressed);
                    }
                }

                if (xInputDpad.Down == XInputDotNetPure.ButtonState.Pressed || xInputDpad.Down == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputDownButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("DPAD_DOWN", out xInputDownButtonAction))
                    {
                        bool pressed = false;
                        if (xInputDpad.Down == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputDownButtonAction, pressed);
                    }
                }

                if (xInputDpad.Left == XInputDotNetPure.ButtonState.Pressed || xInputDpad.Left == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputLeftButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("DPAD_LEFT", out xInputLeftButtonAction))
                    {
                        bool pressed = false;
                        if (xInputDpad.Left == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputLeftButtonAction, pressed);
                    }
                }

                if (xInputDpad.Right == XInputDotNetPure.ButtonState.Pressed || xInputDpad.Right == XInputDotNetPure.ButtonState.Released)
                {
                    string xInputRightButtonAction = null;
                    if (bbWindow.xInputDevices[index].TryGetValue("DPAD_RIGHT", out xInputRightButtonAction))
                    {
                        bool pressed = false;
                        if (xInputDpad.Right == XInputDotNetPure.ButtonState.Pressed)
                        {
                            pressed = true;
                        }
                        bbWindow.ProcessFrontendAction(xInputRightButtonAction, pressed);
                    }
                }
            }
        }

        internal static void ProvisionXInputDevice(XInputDotNetPure.PlayerIndex index, System.Xml.XmlNodeList controlNodes, Dictionary<XInputDotNetPure.PlayerIndex, Dictionary<string, string>> xInputDevices)
        {
            string labelToCheck = string.Empty;

            switch (index)
            {
                case XInputDotNetPure.PlayerIndex.One:
                    labelToCheck = "X_INPUT_1";
                    break;
                case XInputDotNetPure.PlayerIndex.Two:
                    labelToCheck = "X_INPUT_2";
                    break;
                case XInputDotNetPure.PlayerIndex.Three:
                    labelToCheck = "X_INPUT_3";
                    break;
                case XInputDotNetPure.PlayerIndex.Four:
                    labelToCheck = "X_INPUT_4";
                    break;
            }

            try
            {
                XInputDotNetPure.GamePadState xInput1State = XInputDotNetPure.GamePad.GetState(index);

                if (xInput1State.IsConnected == true)
                {
                    Dictionary<string, string> xInput1Mappings = new Dictionary<string, string>();

                    foreach (System.Xml.XmlNode cNode in controlNodes)
                    {
                        string deviceLabel = cNode.Attributes["device"].InnerText;

                        if (deviceLabel == labelToCheck)
                        {
                            string input = cNode.Attributes["input"].InnerText;
                            string action = cNode.Attributes["action"].InnerText;
                            xInput1Mappings[input] = action;
                        }
                    }

                    if (xInput1Mappings.Count() > 0)
                    {
                        xInputDevices[index] = xInput1Mappings;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
