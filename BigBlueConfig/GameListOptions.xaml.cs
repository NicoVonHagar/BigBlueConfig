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
    /// Interaction logic for GameListOptions.xaml
    /// </summary>
    public partial class GameListOptions : Window
    {
        XmlDocument configXmlDocument;

        void Options_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void HandlePress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

                configNode["listitemhorizontalpadding"].InnerText = ListHorzPadding.Text;

                configNode["gamelistmarginx"].InnerText = GameListXMarginTextBox.Text;
                configNode["gamelistmarginy"].InnerText = GameListYMarginTextBox.Text;

                configNode["gamelistoverscanx"].InnerText = OverScanXTextBox.Text;

                configNode["gamelistoverscany"].InnerText = OverScanYTextBox.Text;


                configNode["unselectedtextsize"].InnerText = UnselectedTextSizeTextBox.Text;

                configNode["selectedtextsize"].InnerText = SelectedTextSizeTextBox.Text;

                configNode["selecteditemverticalpadding"].InnerText = SelectedItemPaddingY.Text;

                configNode["unselecteditemverticalpadding"].InnerText = UnselectedItemPaddingY.Text;

                configNode["mainmenutextsize"].InnerText = MainMenuFontSizeTextBox.Text;

                configNode["mainmenupaddingx"].InnerText = MainMenuPaddingXTextBox.Text;

                configNode["mainmenupaddingy"].InnerText = MainMenuPaddingYTextBox.Text;

                configNode["returnlabel"].InnerText = MainMenuReturnTextBox.Text;

                configNode["exitlabel"].InnerText = MainMenuExitTextBox.Text;

                configNode["shutdownlabel"].InnerText = MainMenuShutdownTextBox.Text;

                configNode["restartlabel"].InnerText = MainMenuRestartTextBox.Text;

                ComboBoxItem selectedFont = (ComboBoxItem)GameListFont.SelectedItem;
                configNode["font"].InnerText = (string)selectedFont.Content;

                int listAlignmentIndex = GameListAlignment.SelectedIndex;

                switch (listAlignmentIndex)
                {
                    case -1:
                    case 0:
                        configNode["listalignment"].InnerText = "Center";
                        break;
                    case 1:
                        configNode["listalignment"].InnerText = "Left";
                        break;
                    case 2:
                        configNode["listalignment"].InnerText = "Right";
                        break;
                    default:
                        break;
                }

                configXmlDocument.Save("config.xml");

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't save config.xml: " + ex.Message, "Big Blue Configuration");
            }
        }

        public GameListOptions(XmlDocument configDocument)
        {
            InitializeComponent();
            configXmlDocument = configDocument;

            XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

            SelectedItemPaddingY.Text = configNode["selecteditemverticalpadding"].InnerText;
            UnselectedItemPaddingY.Text = configNode["unselecteditemverticalpadding"].InnerText;

            ListHorzPadding.Text = configNode["listitemhorizontalpadding"].InnerText;

            string gameListXMarginPixels = configNode["gamelistmarginx"].InnerText;
            GameListXMarginTextBox.Text = gameListXMarginPixels;

            string gameListYMarginPixels = configNode["gamelistmarginy"].InnerText;
            GameListYMarginTextBox.Text = gameListYMarginPixels;

            string gameListHorizontalOverscan = configNode["gamelistoverscanx"].InnerText;
            OverScanXTextBox.Text = gameListHorizontalOverscan;

            string gameListVerticalOverscan = configNode["gamelistoverscany"].InnerText;
            OverScanYTextBox.Text = gameListVerticalOverscan;

            string unselectedFontSize = configNode["unselectedtextsize"].InnerText;
            UnselectedTextSizeTextBox.Text = unselectedFontSize;

            string selectedFontSize = configNode["selectedtextsize"].InnerText;
            SelectedTextSizeTextBox.Text = selectedFontSize;

            string mainMenuFontSize = configNode["mainmenutextsize"].InnerText;
            MainMenuFontSizeTextBox.Text = mainMenuFontSize;

            string mainMenuPaddingX = configNode["mainmenupaddingx"].InnerText;
            MainMenuPaddingXTextBox.Text = mainMenuPaddingX;

            string mainMenuPaddingY = configNode["mainmenupaddingy"].InnerText;
            MainMenuPaddingYTextBox.Text = mainMenuPaddingY;

            string returnLabel = configNode["returnlabel"].InnerText;
            MainMenuReturnTextBox.Text = returnLabel;

            string exitLabel = configNode["exitlabel"].InnerText;
            MainMenuExitTextBox.Text = exitLabel;

            string shutdownLabel = configNode["shutdownlabel"].InnerText;
            MainMenuShutdownTextBox.Text = shutdownLabel;

            string restartLabel = configNode["restartlabel"].InnerText;
            MainMenuRestartTextBox.Text = restartLabel;

            string listAlignment = configNode["listalignment"].InnerText;

            switch (listAlignment)
            {
                case "Center":
                    GameListAlignment.SelectedIndex = 0;
                    break;
                case "Left":
                    GameListAlignment.SelectedIndex = 1;
                    break;
                case "Right":
                    GameListAlignment.SelectedIndex = 2;
                    break;
                default:
                    break;
            }

            // populate the combobox with fonts
            Utilities.PopulateFontComboBoxItems(GameListFont);

            string fontFamily = configNode["font"].InnerText;

            if (!string.IsNullOrWhiteSpace(fontFamily))
            {
                foreach (ComboBoxItem cbi in GameListFont.Items)
                {
                    if (fontFamily == (string)cbi.Content)
                    {
                        GameListFont.SelectedItem = cbi;
                    }
                }
            }

            this.PreviewKeyUp += HandlePress;


            Closing += Options_Closing;
        }

        private void DefaultListButton_Click(object sender, RoutedEventArgs e)
        {
            ListTree defaultListWindow = new ListTree();
            defaultListWindow.Owner = this;

            defaultListWindow.Show();
        }
    }
}
