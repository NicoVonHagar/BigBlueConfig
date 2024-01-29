using System;
using System.Collections.Generic;
using System.Linq;
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

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for ControlOptions.xaml
    /// </summary>
    public partial class ControlOptions : Window
    {
        XmlDocument configXmlDocument;

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

                configNode["freeforall"].InnerText = FFACheckbox.IsChecked.ToString();
                configNode["inputdelayonlaunch"].InnerText = InputDelay.Text;
                configNode["exitoption"].InnerText = ExitToDesktopCheckBox.IsChecked.ToString();
                configNode["globalinputs"].InnerText = GlobalInputs.IsChecked.ToString();
                configNode["disablemenu"].InnerText = MenuCheckBox.IsChecked.ToString();
                configNode["trapcursor"].InnerText = TrapMouseCursorCheckbox.IsChecked.ToString();

                configXmlDocument.Save("config.xml");

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't save config.xml: " + ex.Message, "Big Blue Configuration");
            }
        }

        public ControlOptions(XmlDocument configDocument)
        {
            InitializeComponent();

            configXmlDocument = configDocument;

            XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

            bool trapMouseCursor = Convert.ToBoolean(configNode["trapcursor"].InnerText);
            TrapMouseCursorCheckbox.IsChecked = trapMouseCursor;

            int inputDelayValue = Convert.ToInt32(configNode["inputdelayonlaunch"].InnerText);
            InputDelay.Text = inputDelayValue.ToString();

            bool ffaOption = Convert.ToBoolean(configNode["freeforall"].InnerText);
            FFACheckbox.IsChecked = ffaOption;

            bool globalInputs = Convert.ToBoolean(configNode["globalinputs"].InnerText);
            GlobalInputs.IsChecked = globalInputs;

            bool exitOption = Convert.ToBoolean(configNode["exitoption"].InnerText);
            ExitToDesktopCheckBox.IsChecked = exitOption;

            bool disableMenuOption = Convert.ToBoolean(configNode["disablemenu"].InnerText);
            MenuCheckBox.IsChecked = disableMenuOption;

            Closing += ControlOptions_Closing;

        }

        private void ControlOptions_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }
    }
}
