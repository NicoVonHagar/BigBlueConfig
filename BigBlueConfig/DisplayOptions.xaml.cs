using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DisplayOptions.xaml
    /// </summary>
    public partial class DisplayOptions : Window
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
            try
            {
                XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

                configNode["windowborder"].InnerText = WindowBorderCheckBox.IsChecked.ToString();
                configNode["hidemousecursor"].InnerText = HideMouseCursor.IsChecked.ToString();
                configNode["loopvideo"].InnerText = LoopVideoCheckbox.IsChecked.ToString();
                configNode["interface"].InnerText = UserInterfaceComboBox.SelectedValue.ToString();
                configNode["theme"].InnerText = CustomThemeComboBox.SelectedValue.ToString();

                configXmlDocument.Save("config.xml");

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't save config.xml: " + ex.Message, "Big Blue Configuration");
            }
        }

        public DisplayOptions(XmlDocument configDocument)
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

            CustomThemeComboBox.ItemsSource = customThemes;

            XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

            string userInterface = configNode["interface"]?.InnerText;

            UserInterfaceComboBox.SelectedValue = userInterface;

            string customTheme = configNode["theme"]?.InnerText;

            CustomThemeComboBox.SelectedValue = customTheme;

            bool hideMouseCursor = Convert.ToBoolean(configNode["hidemousecursor"].InnerText);
            HideMouseCursor.IsChecked = hideMouseCursor;

            bool loopVideos;
            bool.TryParse(configNode["loopvideo"].InnerText, out loopVideos);
            LoopVideoCheckbox.IsChecked = loopVideos;

            bool winBorder = Convert.ToBoolean(configNode["windowborder"].InnerText);
            WindowBorderCheckBox.IsChecked = winBorder;

            Closing += DisplayOptions_Closing;
        }

        private void DisplayOptions_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }
    }
}
