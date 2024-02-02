using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for AddList.xaml
    /// </summary>
    public partial class AddList : Window
    {
        int indexToAddGame;
        string xPath;

        public AddList(int index, string currentXPath)
        {
            InitializeComponent();

            PreviewKeyUp += HandlePress;

            GameTitle.Focus();

            xPath = currentXPath;

            indexToAddGame = index;
        }

        private void HandlePress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void AddGameButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // create a new XmlNode for the game to add
                XmlNode gameNode = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "item", "");

                Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "name", GameTitle.Text);
                Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "snap", GameThumbnail.Text);
                Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "marquee", MarqueeImage.Text);
                Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "flyer", FlyerImage.Text);
                Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "instruct", InstructionImage.Text);
                Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "video", VideoPreview.Text);
                Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "logo", LogoImage.Text);

                XmlNode listNode = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "list", "");

                if (DefaultListCheckbox.IsChecked == true)
                {
                    foreach (XmlNode xNode in ((App)Application.Current).listDocument.SelectNodes("//list"))
                    {
                        xNode.Attributes.RemoveNamedItem("default");
                    }

                    XmlAttribute newAutoRunAttribute = ((App)Application.Current).listDocument.CreateAttribute("default");
                    newAutoRunAttribute.InnerText = true.ToString();

                    listNode.Attributes.SetNamedItem(newAutoRunAttribute);
                }

                gameNode.AppendChild(listNode);

                if (indexToAddGame > -1)
                {
                    // if a game is selected, add it before the selected game
                    XmlNodeList elemList = ((App)Application.Current).listDocument.SelectNodes(xPath + @"/item");

                    XmlNode el = elemList[indexToAddGame];

                    el.ParentNode.InsertBefore(gameNode, el);
                }
                else
                {
                    // if no game is selected, just put it at the end of the list
                    // add the game node to the destination game list
                    ((App)Application.Current).listDocument.SelectSingleNode(xPath).AppendChild(gameNode);
                }

                ((App)Application.Current).listDocument.Save("lists.xml");

                if (Owner.Resources["Data"] != null)
                {
                    XmlDataProvider pXdp = (XmlDataProvider)Owner.Resources["Data"];
                    pXdp.Document = ((App)Application.Current).listDocument;
                    pXdp.Refresh();
                }

                Close();
                Owner.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't add list: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void ThumbnailBrowser_Click_1(object sender, RoutedEventArgs e)
        {
            GameThumbnail.Text = Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void VideoBrowser_Click(object sender, RoutedEventArgs e)
        {
            VideoPreview.Text = Utilities.GetFileNameFromWindowDialog(".avi", "Video files|*.mp4;*.mpg;*.mpeg*.avi;*.avi|All files (*.*)|*.*");
        }

        private void MarqueeBrowser_Click(object sender, RoutedEventArgs e)
        {
            MarqueeImage.Text = Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void FlyerBrowser_Click(object sender, RoutedEventArgs e)
        {
            FlyerImage.Text = Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void InstructionBrowser_Click(object sender, RoutedEventArgs e)
        {
            InstructionImage.Text = Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void LogoBrowser_Click(object sender, RoutedEventArgs e)
        {
            LogoImage.Text = Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Border)
            {
                WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            }
        }
    }
}
