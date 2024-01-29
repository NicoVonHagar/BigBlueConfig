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
    /// Interaction logic for DesktopDisplayOptions.xaml
    /// </summary>
    public partial class DesktopDisplayOptions : Window
    {
        XmlDocument configXmlDocument;

        public DesktopDisplayOptions(XmlDocument configDocument)
        {
            InitializeComponent();

            configXmlDocument = configDocument;

            XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

            WindowTitle.Text = configNode["title"].InnerText;
            DesktopBackgroundImage.Text = configNode["desktopbg"].InnerText;
            ListTextSizeTextBox.Text = configNode["desktoplistfontsize"].InnerText;
            ItemTextSizeTextBox.Text = configNode["desktopitemfontsize"].InnerText;
            SearchTextSizeTextBox.Text = configNode["desktopsearchfontsize"].InnerText;
            DesktopIcon.Text = configNode["desktopicon"].InnerText;
            ResolutionX.Text = configNode["desktopwidth"].InnerText;
            ResolutionY.Text = configNode["desktopheight"].InnerText;

            Utilities.PopulateFontComboBoxItems(ItemFont);
            Utilities.SelectFontComboBoxChoice(ItemFont, configNode["desktopitemfont"]);
            Utilities.PopulateFontComboBoxItems(ListFont);
            Utilities.SelectFontComboBoxChoice(ListFont, configNode["desktoplistfont"]);
            Utilities.PopulateFontComboBoxItems(SearchFont);
            Utilities.SelectFontComboBoxChoice(SearchFont, configNode["desktopsearchfont"]);

            Closing += DesktopDisplayOptions_Closing;
        }

        private void DesktopDisplayOptions_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void BackgroundImageBrowser_Click(object sender, RoutedEventArgs e)
        {
            DesktopBackgroundImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files (.png)|*.png");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

                configNode["title"].InnerText = WindowTitle.Text;
                configNode["desktopicon"].InnerText = DesktopIcon.Text;
                configNode["desktopwidth"].InnerText = ResolutionX.Text;
                configNode["desktopheight"].InnerText = ResolutionY.Text;
                configNode["desktopbg"].InnerText = DesktopBackgroundImage.Text;
                configNode["desktoplistfontsize"].InnerText = ListTextSizeTextBox.Text;
                configNode["desktopitemfontsize"].InnerText = ItemTextSizeTextBox.Text;
                configNode["desktopsearchfontsize"].InnerText = SearchTextSizeTextBox.Text;

                ComboBoxItem selectedSearchFont = (ComboBoxItem)SearchFont.SelectedItem;

                configNode["desktopsearchfont"].InnerText = (string)selectedSearchFont.Content;

                ComboBoxItem selectedItemFont = (ComboBoxItem)ItemFont.SelectedItem;

                configNode["desktopitemfont"].InnerText = (string)selectedItemFont.Content;

                ComboBoxItem selectedListFont = (ComboBoxItem)ListFont.SelectedItem;

                configNode["desktoplistfont"].InnerText = (string)selectedItemFont.Content;

                configXmlDocument.Save("config.xml");

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't save config.xml: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void DesktopIconBrowser_Click(object sender, RoutedEventArgs e)
        {
            DesktopIcon.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".ico", "Windows icon files (.ico)|*.ico");
        }
    }
}
