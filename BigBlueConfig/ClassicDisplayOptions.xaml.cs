using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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
    /// Interaction logic for ClassicDisplayOptions.xaml
    /// </summary>
    public partial class ClassicDisplayOptions : Window
    {
        XmlDocument configXmlDocument;

        ObservableCollection<ComboBoxItem> customThemes = new ObservableCollection<ComboBoxItem>();

        public class ThemeDirectory
        {
            public string Label { get; set; }

            public string Value { get; set; }
        }

        string currentDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string rotation = "0";

            ComboBoxItem rotationItem = (ComboBoxItem)RotateComboBox.SelectedItem;

            if (rotationItem != null)
            {
                rotation = rotationItem.Content.ToString().Replace("°", "");
            }

            try
            {
                XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

                configNode["width"].InnerText = ResolutionX.Text;
                configNode["height"].InnerText = ResolutionY.Text;

                int correctedAspectRatioIndex = PortraitModeCheckBox.SelectedIndex - 1;
                configNode["aspectratio"].InnerText = correctedAspectRatioIndex.ToString();

                configNode["clock"].InnerText = ClockCheckBox.IsChecked.ToString();

                configNode["stretch"].InnerText = StretchCheckBox.IsChecked.ToString();

                configNode["keepaspect"].InnerText = LetterboxCheckBox.IsChecked.ToString();

                configNode["snapshotflicker"].InnerText = SnapshotFlickerEffect.IsChecked.ToString();

                configNode["screensavertime"].InnerText = ScreenSaverTime.Text;
                configNode["stretchsnapshot"].InnerText = StretchSnapshotCheckBox.IsChecked.ToString();
                configNode["cleanstretch"].InnerText = CleanStretchCheckBox.IsChecked.ToString();
                configNode["antialiastext"].InnerText = AntiAliasTextCheckBox.IsChecked.ToString();

                int timeOfDayIndex = TimeOfDayComboBox.SelectedIndex;

                switch (timeOfDayIndex)
                {
                    case -1:
                    case 0:
                        configNode["timeofday"].InnerText = "-1";
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        int newIndex = TimeOfDayComboBox.SelectedIndex - 1;
                        configNode["timeofday"].InnerText = newIndex.ToString();
                        break;
                    default:
                        configNode["timeofday"].InnerText = "-1";
                        break;
                }

                int sf2artIndex = SF2Art.SelectedIndex;

                switch (sf2artIndex)
                {
                    case -1:
                        configNode["sf2art"].InnerText = "ce";
                        break;
                    case 0:
                        configNode["sf2art"].InnerText = "ww";
                        break;
                    case 1:
                        configNode["sf2art"].InnerText = "ce";
                        break;
                    case 2:
                        configNode["sf2art"].InnerText = "hf";
                        break;
                    default:
                        break;
                }

                configNode["rotate"].InnerText = rotation;

                configXmlDocument.Save("config.xml");

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't save config.xml: " + ex.Message, "Big Blue Configuration");
            }
        }

        public ClassicDisplayOptions(XmlDocument configDocument)
        {
            InitializeComponent();

            configXmlDocument = configDocument;

            System.IO.DirectoryInfo root = null;

            System.IO.DirectoryInfo[] themeFolders = null;

            if (root == null)
            {
                if (System.IO.Directory.Exists(currentDirectoryPath + @"themes"))
                {
                    root = new System.IO.DirectoryInfo(currentDirectoryPath + @"themes");
                }
            }

            if (root != null)
            {
                try
                {
                    themeFolders = root.GetDirectories("*.*");
                }
                catch
                {
                }
            }

            if (themeFolders != null)
            {
                foreach (System.IO.DirectoryInfo themeDir in themeFolders)
                {
                    ComboBoxItem cbi = new ComboBoxItem();

                    cbi.Content = themeDir.Name;


                    customThemes.Add(cbi);
                }
            }

            XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

            int aspectRatioOption = Convert.ToInt32(configNode["aspectratio"].InnerText) + 1;

            PortraitModeCheckBox.SelectedIndex = aspectRatioOption;

            int timeOfDaySelection;

            int.TryParse(configNode["timeofday"].InnerText, out timeOfDaySelection);

            TimeOfDayComboBox.SelectedIndex = timeOfDaySelection + 1;

            string sf2art = configNode["sf2art"].InnerText;

            switch (sf2art)
            {
                case "ww":
                    SF2Art.SelectedIndex = 0;
                    break;
                case "ce":
                    SF2Art.SelectedIndex = 1;
                    break;
                case "hf":
                    SF2Art.SelectedIndex = 2;
                    break;
                default:
                    break;
            }

            bool showClock = Convert.ToBoolean(configNode["clock"].InnerText);
            ClockCheckBox.IsChecked = showClock;

            bool stretchSnapshots;
            bool.TryParse(configNode["stretchsnapshot"].InnerText, out stretchSnapshots);
            StretchSnapshotCheckBox.IsChecked = stretchSnapshots;

            bool antiAliasText;
            bool.TryParse(configNode["antialiastext"].InnerText, out antiAliasText);
            AntiAliasTextCheckBox.IsChecked = antiAliasText;

            CleanStretchCheckBox.IsChecked = Convert.ToBoolean(configNode["cleanstretch"].InnerText);

            int screenSaver = Convert.ToInt32(configNode["screensavertime"].InnerText);
            ScreenSaverTime.Text = screenSaver.ToString();

            int rotate = Convert.ToInt32(configNode["rotate"].InnerText);
            int selectedIndex = rotate / 90;

            RotateComboBox.SelectedIndex = selectedIndex;

            bool flicker = Convert.ToBoolean(configNode["snapshotflicker"].InnerText);
            SnapshotFlickerEffect.IsChecked = flicker;

            bool stretch = Convert.ToBoolean(configNode["stretch"].InnerText);
            StretchCheckBox.IsChecked = stretch;

            if (!string.IsNullOrEmpty(configNode["width"].InnerText))
            {
                int rX = Convert.ToInt32(configNode["width"].InnerText);
                ResolutionX.Text = rX.ToString();
            }
            else
            {
                ResolutionX.Text = "0";
            }

            if (!string.IsNullOrEmpty(configNode["height"].InnerText))
            {
                int rY = Convert.ToInt32(configNode["height"].InnerText);
                ResolutionY.Text = rY.ToString();
            }
            else
            {
                ResolutionY.Text = "0";
            }
            bool keepAspectRatio = Convert.ToBoolean(configNode["keepaspect"].InnerText);
            LetterboxCheckBox.IsChecked = keepAspectRatio;

            Closing += DisplayOptions_Closing;
        }

        private void DisplayOptions_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }
    }
}
