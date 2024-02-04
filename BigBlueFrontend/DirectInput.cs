using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BigBlue
{
    public class DirectInput
    {
        internal static void ProcessDirectInputs(BigBlueWindow bbWindow)
        {
            foreach (SharpDX.DirectInput.Joystick j in bbWindow.directInputDevices.Keys)
            {
                j.Poll();

                SharpDX.DirectInput.JoystickState state = j.GetCurrentState();

                /*
                X 65535  = right
X 0 = left
Y 65535 = down
Y = up
                */


                int yState = state.Y;

                switch (yState)
                {
                    case 0:
                        string dInputActionLeft = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Y_UP", out dInputActionLeft))
                        {
                            bbWindow.ProcessFrontendAction(dInputActionLeft, true);
                        }

                        break;
                    case 65535:
                        string dInputActionRight = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Y_DOWN", out dInputActionRight))
                        {
                            bbWindow.ProcessFrontendAction(dInputActionRight, true);
                        }

                        break;
                    default:
                        string dInputAction1 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Y_UP", out dInputAction1))
                        {
                            // we don't want to do this unless it was actually pressed
                            if (bbWindow.frontendInputs[dInputAction1].wasPressed || bbWindow.frontendInputs[dInputAction1].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction1, false);
                            }
                        }

                        string dInputAction2 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Y_DOWN", out dInputAction2))
                        {
                            if (bbWindow.frontendInputs[dInputAction2].wasPressed || bbWindow.frontendInputs[dInputAction2].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction2, false);
                            }
                        }
                        break;

                }

                int xState = state.X;

                switch (xState)
                {
                    case 0:
                        string dInputActionLeft = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_X_LEFT", out dInputActionLeft))
                        {
                            bbWindow.ProcessFrontendAction(dInputActionLeft, true);
                        }
                        break;
                    case 65535:
                        string dInputActionRight = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_X_RIGHT", out dInputActionRight))
                        {
                            bbWindow.ProcessFrontendAction(dInputActionRight, true);
                        }
                        break;
                    default:
                        string dInputAction1 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_X_LEFT", out dInputAction1))
                        {
                            if (bbWindow.frontendInputs[dInputAction1].wasPressed || bbWindow.frontendInputs[dInputAction1].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction1, false);
                            }
                        }

                        string dInputAction2 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_X_RIGHT", out dInputAction2))
                        {
                            if (bbWindow.frontendInputs[dInputAction2].wasPressed || bbWindow.frontendInputs[dInputAction2].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction2, false);
                            }
                        }

                        break;
                }


                int zState = state.Z;

                switch (zState)
                {
                    case 0:
                        string dInputActionLeft = null;

                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Z_LEFT", out dInputActionLeft))
                        {
                            bbWindow.ProcessFrontendAction(dInputActionLeft, true);
                        }
                        break;
                    case 65535:
                        string dInputActionRight = null;

                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Z_RIGHT", out dInputActionRight))
                        {
                            bbWindow.ProcessFrontendAction(dInputActionRight, true);
                        }
                        break;
                    default:
                        string dInputAction1 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Z_LEFT", out dInputAction1))
                        {
                            if (bbWindow.frontendInputs[dInputAction1].wasPressed || bbWindow.frontendInputs[dInputAction1].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction1, false);
                            }
                        }

                        string dInputAction2 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Z_RIGHT", out dInputAction2))
                        {
                            if (bbWindow.frontendInputs[dInputAction2].wasPressed || bbWindow.frontendInputs[dInputAction2].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction2, false);
                            }
                        }
                        break;
                }

                int zRotationState = state.RotationZ;

                switch (zRotationState)
                {
                    case 0:
                        string dInputActionUp = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Z_UP", out dInputActionUp))
                        {
                            bbWindow.ProcessFrontendAction(dInputActionUp, true);
                        }
                        break;
                    case 65535:
                        string dInputActionDown = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Z_DOWN", out dInputActionUp))
                        {
                            bbWindow.ProcessFrontendAction(dInputActionDown, true);
                        }
                        break;
                    default:
                        string dInputAction1 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Z_DOWN", out dInputAction1))
                        {
                            if (bbWindow.frontendInputs[dInputAction1].wasPressed || bbWindow.frontendInputs[dInputAction1].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction1, false);
                            }
                        }

                        string dInputAction2 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("JOY_Z_DOWN", out dInputAction2))
                        {
                            if (bbWindow.frontendInputs[dInputAction2].wasPressed || bbWindow.frontendInputs[dInputAction2].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction2, false);
                            }
                        }
                        break;
                }

                int pov1State = state.PointOfViewControllers[0];

                switch (pov1State)
                {
                    case 0:
                        string dInputUpAction = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("POV_UP", out dInputUpAction))
                        {
                            bbWindow.ProcessFrontendAction(dInputUpAction, true);
                        }
                        break;
                    case 9000:
                        string dInputRightAction = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("POV_RIGHT", out dInputRightAction))
                        {
                            bbWindow.ProcessFrontendAction(dInputRightAction, true);
                        }

                        break;
                    case 18000:
                        string dInputDownAction = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("POV_DOWN", out dInputDownAction))
                        {
                            bbWindow.ProcessFrontendAction(dInputDownAction, true);
                        }
                        break;
                    case 27000:
                        string dInputLeftAction = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("POV_LEFT", out dInputLeftAction))
                        {
                            bbWindow.ProcessFrontendAction(dInputLeftAction, true);
                        }
                        break;
                    case -1:
                        string dInputAction1 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("POV_UP", out dInputAction1))
                        {
                            if (bbWindow.frontendInputs[dInputAction1].wasPressed || bbWindow.frontendInputs[dInputAction1].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction1, false);
                            }
                        }

                        string dInputAction2 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("POV_RIGHT", out dInputAction2))
                        {
                            if (bbWindow.frontendInputs[dInputAction2].wasPressed || bbWindow.frontendInputs[dInputAction2].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction2, false);
                            }
                        }

                        string dInputAction3 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("POV_DOWN", out dInputAction3))
                        {
                            if (bbWindow.frontendInputs[dInputAction3].wasPressed || bbWindow.frontendInputs[dInputAction3].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction3, false);
                            }
                        }

                        string dInputAction4 = null;
                        if (bbWindow.directInputDevices[j].TryGetValue("POV_LEFT", out dInputAction4))
                        {
                            if (bbWindow.frontendInputs[dInputAction4].wasPressed || bbWindow.frontendInputs[dInputAction4].isRepeating)
                            {
                                bbWindow.ProcessFrontendAction(dInputAction4, false);
                            }
                        }
                        break;
                }

                bool[] buttonsState = state.Buttons;

                for (int i = 0; i <= 12; i++)
                {
                    string dInputButtonAction = null;
                    if (bbWindow.directInputDevices[j].TryGetValue("BUTTON_" + i.ToString(), out dInputButtonAction))
                    {
                        bool pressed = false;
                        if (buttonsState[i] == true)
                        {
                            pressed = true;
                        }

                        bbWindow.ProcessFrontendAction(dInputButtonAction, pressed);
                    }
                }
            }
        }

        internal static void ProvisionDirectInputDevices(BigBlue.BigBlueWindow window, System.Xml.XmlNodeList controlNodes)
        {
            int dInputId = 1;

            IList<SharpDX.DirectInput.DeviceInstance> allDinputDevices = window.directInput.GetDevices();

            foreach (SharpDX.DirectInput.DeviceInstance deviceInstance in allDinputDevices)
            {
                if (deviceInstance.Type == SharpDX.DirectInput.DeviceType.Joystick || deviceInstance.Type == SharpDX.DirectInput.DeviceType.Gamepad || deviceInstance.Type == SharpDX.DirectInput.DeviceType.Supplemental)
                {
                    string dInputIdString = "D_INPUT_" + dInputId.ToString();

                    Dictionary<string, string> dInput1Mappings = new Dictionary<string, string>();

                    foreach (System.Xml.XmlNode cNode in controlNodes)
                    {
                        string deviceLabel = cNode.Attributes["device"].InnerText;

                        if (deviceLabel == dInputIdString)
                        {
                            string input = cNode.Attributes["input"].InnerText;
                            string action = cNode.Attributes["action"].InnerText;
                            dInput1Mappings[input] = action;
                        }
                    }

                    if (dInput1Mappings.Count() > 0)
                    {

                        SharpDX.DirectInput.Joystick joy = new SharpDX.DirectInput.Joystick(window.directInput, deviceInstance.InstanceGuid);

                        // Set BufferSize in order to use buffered data.
                        joy.Properties.BufferSize = 128;
                        // Acquire the joystick
                        joy.Acquire();

                        window.directInputDevices.Add(joy, dInput1Mappings);
                    }

                    dInputId = dInputId + 1;
                }
            }
        }
    }
}
