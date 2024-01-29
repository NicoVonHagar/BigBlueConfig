using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using static BigBlueConfig.Models;

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for EditMouseBindings.xaml
    /// </summary>
    public partial class EditMouseBindings : RawInputWindow
    {
        internal static Stopwatch inputStopWatch = new Stopwatch();

        private const int RIDI_DEVICENAME = 0x20000007;
        private const int RIM_TYPEMOUSE = 0;
        private const int RIM_TYPEKEYBOARD = 1;
        private const int RIM_TYPEHID = 2;

        public class DeviceInfo
        {
            public string deviceName;
            public string deviceType;
            public IntPtr deviceHandle;
            public string Name;
            public string source;
            public ushort key;
            public string vKey;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
        }

        [DllImport("User32.dll")]
        extern static uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

        
        private Dictionary<int, string> deviceList = new Dictionary<int, string>();

        

        /// <summary>
        /// Reads the Registry to retrieve a friendly description
        /// of the device, and whether it is a keyboard.
        /// </summary>
        /// <param name="item">The device name to search for, as provided by GetRawInputDeviceInfo.</param>
        /// <param name="isKeyboard">Determines whether the device's class is "Keyboard". By reference.</param>
        /// <returns>The device description stored in the Registry entry's DeviceDesc value.</returns>
        private static string ReadReg(string item, ref bool isKeyboard)
        {
            // Example Device Identification string
            // @"\??\ACPI#PNP0303#3&13c0b0c5&0#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";

            // remove the \??\
            item = item.Substring(4);

            string[] split = item.Split('#');

            string id_01 = split[0];    // ACPI (Class code)
            string id_02 = split[1];    // PNP0303 (SubClass code)
            string id_03 = split[2];    // 3&13c0b0c5&0 (Protocol code)
            //The final part is the class GUID and is not needed here

            //Open the appropriate key as read-only so no permissions
            //are needed.
            RegistryKey OurKey = Registry.LocalMachine;

            string findme = string.Format(@"System\CurrentControlSet\Enum\{0}\{1}\{2}", id_01, id_02, id_03);

            OurKey = OurKey.OpenSubKey(findme, false);

            //Retrieve the desired information and set isKeyboard
            string deviceDesc = (string)OurKey.GetValue("DeviceDesc");
            string deviceClass = (string)OurKey.GetValue("Class");

            /*
            if (deviceClass.ToUpper().Equals("KEYBOARD"))
            {
                isKeyboard = true;
            }
            else
            {
                isKeyboard = false;
            }
             */

            isKeyboard = true;
            return deviceDesc;
        }

        

        private bool ValidateInput(int index, string label, string action, string input)
        {
            return true;

            /*
            bool valid = true;

            int w = BigBlueControls.Items.Count;

            for (int i = 0; i < w; i++)
            {
                if (i != index)
                {
                    BigBlueControl c = (BigBlueControl)BigBlueControls.Items[i];

                    // first check to see whether the label is the same or it's a keyboard and there's an entry as any OR it's a mouse and there's an entry as any 
                    if ((c.label == label || (label.Contains("KEYBOARD") && c.label == "ANY_KEYBOARD") || (label.Contains("MOUSE") && c.label == "ANY_MOUSE")))
                    {
                        // now we know it's the same or shared device
                        // once we know it's the same device, we're going to fail if there's another entry with the same action for the same input
                        if (c.action == action && c.input == input)
                        {
                            valid = false;
                            break;
                        }
                    }

                    /*
                    //if (c.input == input)
                    //{
//                        valid = false;
                      //  break;
                    //}
                }
            }

                

            return valid;
             */
        }

        private void Input_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;

            int currentIndex = BigBlueControls.SelectedIndex;

            ItemCollection controlDropDown = BigBlueControls.Items;

            if (controlDropDown != null)
            {
                object selectedControlDropDown = controlDropDown[currentIndex];

                if (selectedControlDropDown != null)
                {
                    BigBlueControl c = new BigBlueControl();

                    Type myType = selectedControlDropDown.GetType();

                    IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                    object deviceNameValue = props[1].GetValue(selectedControlDropDown, null);

                    if (deviceNameValue != null)
                    {
                        c.device = deviceNameValue.ToString();
                    }
                    else
                    {
                        c.device = string.Empty;
                    }

                    object deviceTypeValue = props[2].GetValue(selectedControlDropDown, null);

                    if (deviceTypeValue != null)
                    {
                        c.deviceType = deviceTypeValue.ToString();
                    }
                    else
                    {
                        c.deviceType = string.Empty;
                    }

                    object inputValue = props[3].GetValue(selectedControlDropDown, null);

                    if (inputValue != null)
                    {
                        c.input = inputValue.ToString();
                    }
                    else
                    {
                        c.input = string.Empty;
                    }

                    object actionValue = props[4].GetValue(selectedControlDropDown, null);

                    if (actionValue != null)
                    {
                        c.action = actionValue.ToString();
                    }
                    else
                    {
                        c.action = string.Empty;
                    }

                    if (c.device.Contains("KEYBOARD"))
                    {
                        string newKey = e.Key.ToString();

                        if (e.Key == Key.System)
                        {
                            newKey = e.SystemKey.ToString();
                        }

                        if (ValidateInput(currentIndex, c.device, c.action, newKey) == true)
                        {
                            t.Text = newKey;
                            string name = t.Text;
                            SaveKeyBindingsButton.Focus();
                        }
                        else
                        {
                            t.Text = string.Empty;
                            MessageBox.Show("This key is already bound!", "Big Blue Configuration");
                        }

                    }
                }
                else
                {
                    t.Text = string.Empty;
                }
            }
            else
            {
                t.Text = string.Empty;
            }

            e.Handled = true;
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

        }

        private void Mouse_Up(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        //private System.Data.DataRowView rowBeingEdited = null;

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //TextBox t = e.EditingElement as TextBox;  // Assumes columns are all TextBoxes
            //DataGridColumn dgc = e.Column;

            //System.Data.DataRowView rowView = e.Row.Item as System.Data.DataRowView;
            //rowBeingEdited = rowView;

            //MessageBox.Show(t.Text);
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            BigBlueControls.CommitEdit();
            //BigBlueControls.Items.Refresh();
            //if (rowBeingEdited != null)
            //{
            //  rowBeingEdited.EndEdit();
            //}
        }


       

        

        /*
        Dictionary<XInputDotNetPure.PlayerIndex, Dictionary<string, string>> xInputDevices = new Dictionary<XInputDotNetPure.PlayerIndex, Dictionary<string, string>>();
        
        private void provisionXInputDevice(XInputDotNetPure.PlayerIndex index, XmlNodeList controlNodes)
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

            XInputDotNetPure.GamePadState xInput1State = XInputDotNetPure.GamePad.GetState(index);

            if (xInput1State.IsConnected == true)
            {
                Dictionary<string, string> xInput1Mappings = new Dictionary<string, string>();

                foreach (XmlNode cNode in controlNodes)
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
        */

        Dictionary<int, string> VendorIds = new Dictionary<int, string>();

        Dictionary<int, string> ProductIds = new Dictionary<int, string>();

        private void PopulateEnumeratedDevice(IntPtr key, Models.RawInputDevice rd, List<BigBlueControl> inputDevices)
        {
            int deviceId = Convert.ToInt32(key.ToString());
            VendorIds[deviceId] = rd.VendorID;
            ProductIds[deviceId] = rd.ProductID;

            BigBlueControl bbc = new BigBlueControl()
            {
                labelX = rd.DeviceName,
                device = rd.DeviceLabel
            };

            if (string.IsNullOrWhiteSpace(rd.DeviceName))
            {
                bbc.labelX = rd.DeviceLabel;
            }

            deviceList.Add(deviceId, bbc.labelX);

            inputDevices.Add(bbc);
        }

        public List<BigBlueControl> EnumerateDevices()
        {
            List<BigBlueControl> inputDevices = new List<BigBlueControl>
            {
                new BigBlueControl() { device = "ANY_KEYBOARD", labelX = "ANY_KEYBOARD" },
                new BigBlueControl() { device = "ANY_MOUSE", labelX = "ANY_MOUSE" }
            };

            try
            {
                foreach (KeyValuePair<IntPtr, Models.RawInputKeyboard> kd in keyboardDevices)
                {
                    PopulateEnumeratedDevice(kd.Key, kd.Value, inputDevices);
                }

                foreach (KeyValuePair<IntPtr, Models.RawInputJoystick> jd in joystickDevices)
                {
                    PopulateEnumeratedDevice(jd.Key, jd.Value, inputDevices);
                }

                foreach (KeyValuePair<IntPtr, Models.RawInputMouse> md in mouseDevices)
                {
                    PopulateEnumeratedDevice(md.Key, md.Value, inputDevices);
                }

                Mice.ItemsSource = deviceList;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error when enumerating devices: " + ex.Message);
                throw new ApplicationException("Error!");
            }

            return inputDevices;
        }

        public partial class BigBlueControl
        {
            public BigBlueControl() { }

            public string labelX { get; set; }

            public string device { get; set;  }

            public string deviceType { get; set; }

            public string input { get; set; }

            public string action { get; set; }
        }

        private void CreateInputXmlNode(XmlNode parentConfigNode, string label, string input, string action)
        {
            XmlNode inputNode = configXml.CreateNode(XmlNodeType.Element, "control", "");

            XmlAttribute deviceLabelAttribute = configXml.CreateAttribute("device");
            deviceLabelAttribute.InnerText = label;
            inputNode.Attributes.SetNamedItem(deviceLabelAttribute);
            parentConfigNode.AppendChild(inputNode);

            XmlAttribute inputAttribute = configXml.CreateAttribute("input");
            inputAttribute.InnerText = input;
            inputNode.Attributes.SetNamedItem(inputAttribute);
            parentConfigNode.AppendChild(inputNode);

            XmlAttribute actionAttribute = configXml.CreateAttribute("action");
            actionAttribute.InnerText = action;
            inputNode.Attributes.SetNamedItem(actionAttribute);
            parentConfigNode.AppendChild(inputNode);
        }

        private void SaveControlsXml()
        {
            // we're going to wipe out input entries
            XmlNode parentConfigNode = configXml.SelectSingleNode("/config/controls");
            XmlNodeList elemList = configXml.SelectNodes("/config/controls/control");

            parentConfigNode.RemoveAll();

            // then we're going to write in the current bbc
            foreach (BigBlueControl c in bbcs)
            {
                if (c != null)
                {
                    if (!string.IsNullOrWhiteSpace(c.action) && !string.IsNullOrWhiteSpace(c.device) && !string.IsNullOrWhiteSpace(c.input))
                    {
                        CreateInputXmlNode(parentConfigNode, c.device, c.input, c.action);
                    }
                }

                //<control devicelabel="KEYBOARD_1" devicetype="keyboard" input="Down" action="RAMPAGE_NEXT_ITEM" />
            }


            configXml.Save(@"config.xml");

            this.Close();
            this.Owner.Activate();
        }

        private void CreateDefaultInputBindings()
        {
            XmlNode parentConfigNode = configXml.SelectSingleNode("/config/controls");
            XmlNodeList elemList = configXml.SelectNodes("/config/controls/control");

            parentConfigNode.RemoveAll();

            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "Escape", "BIG_BLUE_EXIT");
            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "Up", "RAMPAGE_PREVIOUS_ITEM");

            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "Left", "RAMPAGE_PREVIOUS_PAGE");

            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "Right", "RAMPAGE_NEXT_PAGE");
            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "Down", "RAMPAGE_NEXT_ITEM");
            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "LeftCtrl", "RAMPAGE_PUNCH_LEFT");
            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "LeftAlt", "RAMPAGE_PUNCH_RIGHT");
            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "Space", "RAMPAGE_SPECTATE");
            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "Enter", "RAMPAGE_START");
            CreateInputXmlNode(parentConfigNode, "ANY_KEYBOARD", "LeftShift", "RAMPAGE_BACK");

            configXml.Save(@"config.xml");

            XmlNodeList l = configXml.SelectNodes("/config/controls/control");

            foreach (XmlNode n in l)
            {
                string label = n.Attributes["device"].Value;
                //string type = n.Attributes["devicetype"].Value;
                string input = n.Attributes["input"].Value;
                string action = n.Attributes["action"].Value;

                BigBlueControl bbc = new BigBlueControl
                {
                    device = label,
                    //bbc.deviceType = type;
                    input = input,
                    action = action
                };

                bbcs.Add(bbc);
            }
        }

        XmlDocument configXml = new XmlDocument();
        List<BigBlueControl> bbcs = new List<BigBlueControl>();

        public EditMouseBindings()
        {
            // initialize raw input
            BigBlueConfig.NativeMethods.RegisterInputDevices(this);

            BigBlueConfig.NativeMethods.ProvisionRawInputs(this, false);

            InitializeComponent();

            try
            {
                configXml.Load(@"config.xml");

                XmlNodeList l = configXml.SelectNodes("/config/controls/control");

                if (l.Count == 0)
                {
                    CreateDefaultInputBindings();
                }
                else
                {
                    foreach (XmlNode n in l)
                    {
                        string deviceName = n.Attributes["device"].Value;
                        //string type = n.Attributes["devicetype"].Value;
                        string input = n.Attributes["input"].Value;
                        string action = n.Attributes["action"].Value;

                        BigBlueControl bbc = new BigBlueControl
                        {
                            device = deviceName,
                            //bbc.deviceType = type;
                            input = input,
                            action = action
                        };

                        bbcs.Add(bbc);
                    }
                }

            }
            catch (Exception)
            {
                CreateDefaultInputBindings();
            }

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            BigBlueControls.ItemsSource = bbcs;

            /*
            XmlDataProvider p = (XmlDataProvider)Resources["Controls"];

                p.Document = configXml;
             */

            //BigBlueControls.DataContext = configXml.SelectSingleNode("/config/controls");

            // string[] deviceTypes = new string[] { "keyboard", "MOUSE" };

            string[] actions = new string[] { "NONE", "RAMPAGE_START", "RAMPAGE_BACK", "RAMPAGE_NEXT_PAGE", "RAMPAGE_PREVIOUS_PAGE", "RAMPAGE_NEXT_ITEM", "RAMPAGE_PREVIOUS_ITEM", "RAMPAGE_FIRST_ITEM", "RAMPAGE_LAST_ITEM", "RAMPAGE_RANDOM_ITEM", "RAMPAGE_PUNCH_LEFT", "RAMPAGE_PUNCH_RIGHT", "RAMPAGE_SPECTATE", "RAMPAGE_EXIT", "RTYPE_NEXT_PAGE", "RTYPE_PREVIOUS_PAGE", "RTYPE_PREVIOUS_ITEM", "RTYPE_NEXT_ITEM", "RTYPE_FIRST_ITEM", "RTYPE_LAST_ITEM", "RTYPE_RANDOM_ITEM", "RTYPE_SHOOT", "RTYPE_WARP", "RTYPE_BACK", "RTYPE_START", "RTYPE_EXIT", "BIG_BLUE_EXIT", "BIG_BLUE_VOLUME_UP", "BIG_BLUE_VOLUME_DOWN", "BIG_BLUE_MUTE", "BIG_BLUE_SPEECH", "QUIT_TO_DESKTOP", "RESTART", "SHUTDOWN" };

            // ObjectDataProvider dt = (ObjectDataProvider)Resources["DeviceTypes"];
            //  dt.ObjectInstance = deviceTypes;

            ObjectDataProvider g = (ObjectDataProvider)Resources["Actions"];

            g.ObjectInstance = actions;

            ObjectDataProvider d = (ObjectDataProvider)Resources["Devices"];
            d.ObjectInstance = EnumerateDevices();

        }

        private int GetMaxEnumValue(Type deviceType)
        {
            if (deviceType == typeof(Models.KeyboardInputType))
            {
                return (int)Models.KeyboardInputType.MAX_KEY;
            }
            else if (deviceType == typeof(Models.JoystickInputType))
            {
                return (int)Models.JoystickInputType.JOYSTICK_INPUT_TYPE_MAX;
            }
            else if (deviceType == typeof(Models.MouseInputType))
            {
                return (int)Models.MouseInputType.MAX_MOUSE;
            }
            else
            {
                return 0;
            }
        }

        private string GetInputName(Type deviceType, int index)
        {
            if (deviceType == typeof(Models.KeyboardInputType))
            {
                return Enum.GetName<Models.KeyboardInputType>((Models.KeyboardInputType)index);
            }
            else if (deviceType == typeof(Models.JoystickInputType))
            {
                return Enum.GetName<Models.JoystickInputType>((Models.JoystickInputType)index);
            }
            else if (deviceType == typeof(Models.MouseInputType))
            {
                return Enum.GetName<Models.MouseInputType>((Models.MouseInputType)index);
            }
            else
            {
                return string.Empty;
            }
        }

        private bool ValidateInput(Models.RawInputDevice rid, string device, TextBox focusedControl, Type deviceType)
        {
            if (rid.DeviceLabel == device 
                || (deviceType == typeof(Models.KeyboardInputType) && device == "ANY_KEYBOARD") 
                || (deviceType == typeof(Models.MouseInputType) && device == "ANY_MOUSE"))
            {
                int maxIndex = GetMaxEnumValue(deviceType);

                for (int jitIndex = 0; jitIndex != maxIndex; ++jitIndex)
                {
                    if (rid.InputStates[jitIndex] == InputState.Pressed)
                    {
                        focusedControl.Text = GetInputName(deviceType, jitIndex);
                        SaveKeyBindingsButton.Focus();

                        return true;
                    }
                }
            }

            return false;
        }


        private void ValidateInputs()
        {
            if (inputStopWatch.ElapsedMilliseconds >= 100)
            {
                inputStopWatch.Stop();

                IInputElement focusedControl = FocusManager.GetFocusedElement(this);

                if (focusedControl is TextBox)
                {
                    TextBox tb = (TextBox)focusedControl;

                    int currentRowIndex = BigBlueControls.Items.IndexOf(BigBlueControls.CurrentItem);
                    BigBlueControl c = (BigBlueControl)BigBlueControls.Items[currentRowIndex];

                    foreach (KeyValuePair<IntPtr, Models.RawInputKeyboard> kd in keyboardDevices)
                    {
                        bool done = ValidateInput(kd.Value, c.device, tb, typeof(Models.KeyboardInputType));

                        if (done)
                        {
                            return;
                        }
                    }

                    foreach (KeyValuePair<IntPtr, Models.RawInputJoystick> jd in joystickDevices)
                    {
                        bool done = ValidateInput(jd.Value, c.device, tb, typeof(Models.JoystickInputType));

                        if (done)
                        {
                            return;
                        }
                    }

                    foreach (KeyValuePair<IntPtr, Models.RawInputMouse> md in mouseDevices)
                    {
                        bool done = ValidateInput(md.Value, c.device, tb, typeof(Models.MouseInputType));

                        if (done)
                        {
                            return;
                        }
                    }
                }
            }
            
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            UpdateRawInputStates();

            ValidateInputs();
            
        }

        private void Mice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedDeviceId = (int)Mice.SelectedValue;
            MouseID.Text = selectedDeviceId.ToString();
            if (ProductIds.ContainsKey(selectedDeviceId))
            {
                PID.Text = ProductIds[(int)Mice.SelectedValue];
            }
            else
            {
                PID.Text = "Unknown";
            }
            if (VendorIds.ContainsKey(selectedDeviceId))
            {
                VID.Text = VendorIds[(int)Mice.SelectedValue];
            }
            else
            {
                VID.Text = "Unknown";
            }
        }

        private void SelectGame1Key_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //RampageStartDeviceID.Text = MouseID.Text;
        }

        private void SaveKeyBindingsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveControlsXml();
        }

        private void BigBlueControls_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the edit on the row
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void ResetSorting()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(BigBlueControls.ItemsSource);
            if (view != null && view.SortDescriptions != null)
            {
                view.SortDescriptions.Clear();
                foreach (var column in BigBlueControls.Columns)
                {
                    column.SortDirection = null;
                }
            }
        }

        private void SaveKeyBindingsButton_Click_1(object sender, RoutedEventArgs e)
        {
            SaveControlsXml();
        }

        private void DeleteKeyBindingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (BigBlueControls.SelectedIndex != -1)
            {
                for (int i = BigBlueControls.SelectedItems.Count - 1; i >= 0; i--)
                {
                    int indexToDelete = BigBlueControls.Items.IndexOf(BigBlueControls.SelectedItems[i]);

                    bbcs.RemoveAt(indexToDelete);
                }

                BigBlueControls.Items.Refresh();
            }
            else
            {
                MessageBox.Show("You must select an input before you can delete it!", "Big Blue Configuration");
            }
        }

        private void AddKeyBindingsButton_Click(object sender, RoutedEventArgs e)
        {
            ResetSorting();

            BigBlueControl newControl = new BigBlueControl();
            //newControl.deviceType = "keyboard";
            //newControl.input = "";
            //newControl.label = "KEYBOARD_1";
            //newControl.action = "NONE";
            bbcs.Add(newControl);
            //BigBlueControls.ItemsSource = bbcs;
            BigBlueControls.Items.Refresh();

            if (BigBlueControls.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(BigBlueControls, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            inputStopWatch.Stop();
            inputStopWatch.Reset();

            BigBlueControls.PreviewMouseWheel -= BigBlueControls_PreviewMouseWheel;
        }

        private void TextBox_Focus(object sender, RoutedEventArgs e)
        {
            inputStopWatch.Stop();
            inputStopWatch.Reset();
            inputStopWatch.Start();

            // set all the inputs to released
            ReleaseRawInputStates();

            BigBlueControls.PreviewMouseWheel += BigBlueControls_PreviewMouseWheel;
        }

        private void BigBlueControls_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }
    }
}
