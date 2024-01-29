using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for MultiMonitorOptions.xaml
    /// </summary>
    public partial class MultiMonitorOptions : Window
    {
        private List<Screen> GetAllScreens()
        {
            return System.Windows.Forms.Screen.AllScreens.ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

                // secondary displays
                ComboBoxItem selectedMarqueeDisplay = (ComboBoxItem)MarqueeDisplayName.SelectedItem;
                configNode["marqueedisplay"].InnerText = (string)selectedMarqueeDisplay.Content;

                ComboBoxItem selectedFlyerDisplay = (ComboBoxItem)FlyerDisplayName.SelectedItem;
                configNode["flyerdisplay"].InnerText = (string)selectedFlyerDisplay.Content;

                ComboBoxItem selectedInstructionDisplay = (ComboBoxItem)InstructionDisplayName.SelectedItem;
                configNode["instructiondisplay"].InnerText = (string)selectedInstructionDisplay.Content;

                ComboBoxItem selectedSurroundPosition = (ComboBoxItem)SurroundMonitorConfigurationComboBox.SelectedItem;
                configNode["surroundposition"].InnerText = (string)selectedSurroundPosition.Content;

                ComboBoxItem surroundMonitor = (ComboBoxItem)SecondarySurroundMonitorComboBox.SelectedItem;
                configNode["surroundmonitor"].InnerText = (string)surroundMonitor.Content;

                configXmlDocument.Save("config.xml");

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't save config.xml: " + ex.Message, "Big Blue Configuration");
            }
        }

        XmlDocument configXmlDocument;

        public MultiMonitorOptions(XmlDocument configDocument)
        {
            InitializeComponent();

            List<Screen> allScreens = GetAllScreens();

            ComboBoxItem noneItem1 = new ComboBoxItem();
            noneItem1.Content = "None";

            ComboBoxItem noneItem2 = new ComboBoxItem();
            noneItem2.Content = "None";

            ComboBoxItem noneItem3 = new ComboBoxItem();
            noneItem3.Content = "None";

            ComboBoxItem noneItem4 = new ComboBoxItem();
            noneItem4.Content = "None";

            MarqueeDisplayName.Items.Add(noneItem1);
            FlyerDisplayName.Items.Add(noneItem2);
            InstructionDisplayName.Items.Add(noneItem3);
            SecondarySurroundMonitorComboBox.Items.Add(noneItem4);

            Screen primaryScreen = System.Windows.Forms.Screen.PrimaryScreen;

            foreach (Screen s in allScreens)
            {
                ComboBoxItem cbi1 = new ComboBoxItem();
                cbi1.Content = s.DeviceName;
                MarqueeDisplayName.Items.Add(cbi1);

                ComboBoxItem cbi2 = new ComboBoxItem();
                cbi2.Content = s.DeviceName;
                FlyerDisplayName.Items.Add(cbi2);

                ComboBoxItem cbi3 = new ComboBoxItem();
                cbi3.Content = s.DeviceName;
                InstructionDisplayName.Items.Add(cbi3);

                if (s != primaryScreen)
                {
                    ComboBoxItem cbi4 = new ComboBoxItem();
                    cbi4.Content = s.DeviceName;
                    SecondarySurroundMonitorComboBox.Items.Add(cbi4);
                }
            }



            try
            {
                configXmlDocument = configDocument;

                XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

                string marqueeDisplay = configNode["marqueedisplay"].InnerText;
                string surroundConfiguration = configNode["surroundposition"].InnerText;
                string surroundMonitor = configNode["surroundmonitor"].InnerText;

                for (int i = 0; i < SurroundMonitorConfigurationComboBox.Items.Count; i++)
                {
                    ComboBoxItem sc = (ComboBoxItem)SurroundMonitorConfigurationComboBox.Items[i];

                    if (sc.Content.ToString() == surroundConfiguration)
                    {
                        SurroundMonitorConfigurationComboBox.SelectedIndex = i;
                        break;
                    }
                }

                foreach (ComboBoxItem cbi in SecondarySurroundMonitorComboBox.Items)
                {
                    if (surroundMonitor == (string)cbi.Content)
                    {
                        SecondarySurroundMonitorComboBox.SelectedItem = cbi;
                        break;
                    }
                }


                if (!string.IsNullOrWhiteSpace(marqueeDisplay))
                {
                    bool foundDisplay = false;

                    foreach (ComboBoxItem cbi in MarqueeDisplayName.Items)
                    {
                        if (marqueeDisplay == (string)cbi.Content)
                        {
                            MarqueeDisplayName.SelectedItem = cbi;
                            foundDisplay = true;
                        }
                    }

                    if (foundDisplay == false)
                    {
                        System.Windows.MessageBox.Show("It looks like you set a marquee display that is no longer available on this machine. You might want to change it!", "Big Blue Configuration");

                        ComboBoxItem unavailableDisplayOption = new ComboBoxItem();
                        unavailableDisplayOption.Content = marqueeDisplay;
                        MarqueeDisplayName.Items.Add(unavailableDisplayOption);
                        MarqueeDisplayName.SelectedItem = unavailableDisplayOption;
                    }

                }
                else
                {
                    MarqueeDisplayName.SelectedIndex = 0;
                }

                string flyerDisplay = configNode["flyerdisplay"].InnerText;

                if (!string.IsNullOrWhiteSpace(flyerDisplay))
                {
                    bool foundDisplay = false;

                    foreach (ComboBoxItem cbi in FlyerDisplayName.Items)
                    {
                        if (flyerDisplay == (string)cbi.Content)
                        {
                            FlyerDisplayName.SelectedItem = cbi;
                            foundDisplay = true;
                        }
                    }

                    if (foundDisplay == false)
                    {
                        System.Windows.MessageBox.Show("It looks like you set a flyer display that is no longer available on this machine. You might want to change it!", "Big Blue Configuration");
                    }
                }
                else
                {
                    FlyerDisplayName.SelectedItem = 0;
                }

                string instructionDisplay = configNode["instructiondisplay"].InnerText;

                if (!string.IsNullOrWhiteSpace(instructionDisplay))
                {
                    bool foundDisplay = false;

                    foreach (ComboBoxItem cbi in InstructionDisplayName.Items)
                    {
                        if (instructionDisplay == (string)cbi.Content)
                        {
                            InstructionDisplayName.SelectedItem = cbi;
                            foundDisplay = true;
                        }
                    }

                    if (foundDisplay == false)
                    {
                        System.Windows.MessageBox.Show("It looks like you set an instruction display that is no longer available on this machine. You might want to change it!", "Big Blue Configuration");
                    }
                }
                else
                {
                    FlyerDisplayName.SelectedItem = 0;
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't load config.xml data: " + ex.Message, "Big Blue Configuration");
            }

            FlyerDisplayName.SelectionChanged += FlyerDisplayName_SelectionChanged;
            InstructionDisplayName.SelectionChanged += InstructionDisplayName_SelectionChanged;
            MarqueeDisplayName.SelectionChanged += MarqueeDisplayName_SelectionChanged;

            Closing += MultiMonitorOptions_Closing;
        }

        private void MultiMonitorOptions_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void InstructionDisplayName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InstructionDisplayName.SelectedIndex != -1)
            {
                System.Windows.Controls.ComboBoxItem icb = (System.Windows.Controls.ComboBoxItem)InstructionDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem fcb = (System.Windows.Controls.ComboBoxItem)FlyerDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem mcb = (System.Windows.Controls.ComboBoxItem)MarqueeDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem scb = (System.Windows.Controls.ComboBoxItem)SecondarySurroundMonitorComboBox.SelectedItem;

                if (icb.Content.ToString() != "None" && (icb.Content.ToString() == fcb.Content.ToString() || icb.Content.ToString() == mcb.Content.ToString()))
                {
                    System.Windows.MessageBox.Show("The Instruction Card monitor can't be the same as the Flyer or Marquee monitor!", "Big Blue Configuration");
                    InstructionDisplayName.SelectedIndex = 0;
                }

                if (icb.Content.ToString() == scb.Content.ToString() && icb.Content.ToString() != "None" && scb.Content.ToString() != "None")
                {
                    System.Windows.MessageBox.Show("The Instruction Card monitor can't be the same as the surround monitor!", "Blue Blue Configuration");
                    InstructionDisplayName.SelectedIndex = 0;
                }
            }


        }

        private void FlyerDisplayName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlyerDisplayName.SelectedIndex != -1)
            {
                System.Windows.Controls.ComboBoxItem icb = (System.Windows.Controls.ComboBoxItem)InstructionDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem fcb = (System.Windows.Controls.ComboBoxItem)FlyerDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem mcb = (System.Windows.Controls.ComboBoxItem)MarqueeDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem scb = (System.Windows.Controls.ComboBoxItem)SecondarySurroundMonitorComboBox.SelectedItem;

                if (fcb.Content.ToString() != "None" && (fcb.Content.ToString() == icb.Content.ToString() || fcb.Content.ToString() == mcb.Content.ToString()))
                {
                    System.Windows.MessageBox.Show("The Flyer monitor can't be the same as the Instruction Card or Marquee monitor!", "Big Blue Configuration");
                    FlyerDisplayName.SelectedIndex = 0;
                }

                if (fcb.Content.ToString() == scb.Content.ToString() && fcb.Content.ToString() != "None" && scb.Content.ToString() != "None")
                {
                    System.Windows.MessageBox.Show("The Flyer monitor can't be the same as the surround monitor!", "Blue Blue Configuration");
                    FlyerDisplayName.SelectedIndex = 0;
                }
            }


        }

        private void MarqueeDisplayName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MarqueeDisplayName.SelectedIndex != -1)
            {
                System.Windows.Controls.ComboBoxItem icb = (System.Windows.Controls.ComboBoxItem)InstructionDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem fcb = (System.Windows.Controls.ComboBoxItem)FlyerDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem mcb = (System.Windows.Controls.ComboBoxItem)MarqueeDisplayName.SelectedItem;
                System.Windows.Controls.ComboBoxItem scb = (System.Windows.Controls.ComboBoxItem)SecondarySurroundMonitorComboBox.SelectedItem;

                if (mcb.Content.ToString() != "None" && (mcb.Content.ToString() == icb.Content.ToString() || mcb.Content.ToString() == fcb.Content.ToString()))
                {
                    System.Windows.MessageBox.Show("The Marquee monitor can't be the same as the Flyer or Instruction Card monitor!", "Big Blue Configuration");
                    MarqueeDisplayName.SelectedIndex = 0;
                }

                if (mcb.Content.ToString() == scb.Content.ToString() && mcb.Content.ToString() != "None" && scb.Content.ToString() != "None")
                {
                    System.Windows.MessageBox.Show("The Marquee monitor can't be the same as the surround monitor!", "Blue Blue Configuration");
                    MarqueeDisplayName.SelectedIndex = 0;
                }
            }


        }
    }
}
