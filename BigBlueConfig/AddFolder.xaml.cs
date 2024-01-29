using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
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
    /// Interaction logic for AddFolder.xaml
    /// </summary>
    public partial class AddFolder : Window
    {
        int indexToAddGame;
        string xPath;
        XmlDataProvider parentProvider;

        public AddFolder(XmlDataProvider xdp, int index, string currentXPath, XmlDocument eDocument)
        {
            InitializeComponent();

            XmlDataProvider e = (XmlDataProvider)Resources["EmulatorData"];
            e.Document = eDocument;

            GameTitle.Focus();

            xPath = currentXPath;

            parentProvider = xdp;

            indexToAddGame = index;
        }

        private void FolderLocationButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderLocationTextBox.Text = dialog.SelectedPath;
            }
        }

        private void ProgramLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            GameLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void ThumbnailBrowser_Click_1(object sender, RoutedEventArgs e)
        {
            GameThumbnail.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files (.png)|*.png");
        }

        private void SnapshotTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            SnapshotTemplateTextBox.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void VideoTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            VideoTemplatePreview.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".avi", "Video files|*.mp4;*.mpg;*.mpeg*.avi;*.avi|All files (*.*)|*.*");
        }

        private void MarqueeTemplateBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            MarqueeImageTemplateTextBox.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void FlyerTemplateBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            FlyerTemplateImageTextBox.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void InstructionTemplateBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            InstructionTemplateImageTextBox.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void TemplateCombobox_Click(object sender, RoutedEventArgs e)
        {
            if (EmulatorList.SelectedIndex != -1)
            {
                XmlElement selectedEmulatorNode = (XmlElement)EmulatorList.SelectedItem;

                string emulatorDir = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "dir");
                string emulatorExe = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "exe");

                if (!string.IsNullOrWhiteSpace(emulatorDir) && !string.IsNullOrWhiteSpace(emulatorExe))
                {
                    GameLocation.Text = emulatorDir + @"\" + emulatorExe;
                }

                string emulatorArgs = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "args");
                GameArgs.Text = emulatorArgs;

                string emulatorThumbnail = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "snap");

                SnapshotTemplateTextBox.Text = emulatorThumbnail;

                string emulatorVideo = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "video");

                VideoTemplatePreview.Text = emulatorVideo;

                string emulatorMarquee = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "marquee");

                MarqueeImageTemplateTextBox.Text = emulatorMarquee;

                string emulatorFlyer = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "flyer");

                FlyerTemplateImageTextBox.Text = emulatorFlyer;

                string emulatorInstructions = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "instruct");

                InstructionTemplateImageTextBox.Text = emulatorInstructions;

                string emulatorPreDir = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "predir");
                string emulatorPreExe = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "preexe");

                if (!string.IsNullOrWhiteSpace(emulatorPreDir) && !string.IsNullOrWhiteSpace(emulatorPreExe))
                {
                    PreLaunchLocation.Text = emulatorPreDir + @"\" + emulatorPreExe;
                }

                string emulatorPreArgs = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "preargs");

                PreLaunchArgs.Text = emulatorPreArgs;

                string emulatorPostDir = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "postdir");
                string emulatorPostExe = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "postexe");

                if (!string.IsNullOrWhiteSpace(emulatorPostDir) && !string.IsNullOrWhiteSpace(emulatorPostExe))
                {
                    PostLaunchLocation.Text = emulatorPostDir + @"\" + emulatorPostExe;
                }

                string emulatorPostArgs = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "postargs");

                PostLaunchArgs.Text = emulatorPostArgs;

                bool emulatorFullRomPath = Convert.ToBoolean(selectedEmulatorNode.SelectSingleNode("fullpath").InnerText);

                bool kill = false;

                XmlNode killNode = selectedEmulatorNode.SelectSingleNode("kill");

                if (killNode != null)
                {
                    kill = Convert.ToBoolean(killNode.InnerText);
                }

                KillProcessCheckbox.IsChecked = kill;
            }
        }

        private void PreLaunchLocationBrowse_Click(object sender, RoutedEventArgs e)
        {
            PreLaunchLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void PostLaunchLocationBrowse_Click(object sender, RoutedEventArgs e)
        {
            PostLaunchLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void VideoBrowser_Click(object sender, RoutedEventArgs e)
        {
            VideoPreview.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".avi", "Video files|*.mp4;*.mpg;*.mpeg*.avi;*.avi|All files (*.*)|*.*");
        }

        private void MarqueeBrowser_Click(object sender, RoutedEventArgs e)
        {
            MarqueeImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void FlyerBrowser_Click(object sender, RoutedEventArgs e)
        {
            FlyerImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void InstructionBrowser_Click(object sender, RoutedEventArgs e)
        {
            InstructionImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // create a new XmlNode for the game to add
                XmlNode gameNode = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "item", "");

                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "name", GameTitle.Text);

                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "snap", GameThumbnail.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "marquee", MarqueeImage.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "flyer", FlyerImage.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "instruct", InstructionImage.Text);

                // image template values
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "snaptemplate", SnapshotTemplateTextBox.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "marqueetemplate", MarqueeImageTemplateTextBox.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "instructiontemplate", InstructionTemplateImageTextBox.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "flyertemplate", FlyerTemplateImageTextBox.Text);

                // video 
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "video", VideoPreview.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "videotemplate", VideoTemplatePreview.Text);

                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "args", GameArgs.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "preargs", PreLaunchArgs.Text);
                BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "postargs", PostLaunchArgs.Text);

                if (!string.IsNullOrWhiteSpace(PreLaunchLocation.Text))
                {
                    // pre launch dir
                    BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "predir", Utilities.GetDirectoryOfProgram(PreLaunchLocation.Text));

                    // pre launch exe
                    BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "preexe", Utilities.GetExeOfProgram(PreLaunchLocation.Text));
                }

                if (!string.IsNullOrWhiteSpace(PostLaunchLocation.Text))
                {
                    // post launch dir
                    BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "postdir", Utilities.GetDirectoryOfProgram(PostLaunchLocation.Text));

                    // post launch exe
                    BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "postexe", Utilities.GetExeOfProgram(PostLaunchLocation.Text));
                }

                if (!string.IsNullOrWhiteSpace(GameLocation.Text))
                {
                    BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "dir", Utilities.GetDirectoryOfProgram(GameLocation.Text));

                    // exe
                    BigBlueConfig.Utilities.AddValueToNode(((App)Application.Current).listDocument, gameNode, "exe", Utilities.GetExeOfProgram(GameLocation.Text));
                }

                if (KillProcessCheckbox.IsChecked == true)
                {
                    XmlAttribute killProcessAttribute = ((App)Application.Current).listDocument.CreateAttribute("kill");
                    killProcessAttribute.InnerText = true.ToString();
                    gameNode.Attributes.SetNamedItem(killProcessAttribute);
                }

                XmlNode listNode = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "list", "");

                if (DefaultListCheckbox.IsChecked == true)
                {
                    foreach (XmlNode xNode in ((App)Application.Current).listDocument.SelectNodes("//list"))
                    {
                        xNode.Attributes.RemoveNamedItem("default");
                    }

                    XmlAttribute newAutoRunAttribute = parentProvider.Document.CreateAttribute("default");
                    newAutoRunAttribute.InnerText = true.ToString();

                    listNode.Attributes.SetNamedItem(newAutoRunAttribute);
                }

                XmlAttribute folderAttribute = ((App)Application.Current).listDocument.CreateAttribute("folder");
                folderAttribute.InnerText = FolderLocationTextBox.Text;
                listNode.Attributes.SetNamedItem(folderAttribute);

                XmlAttribute searchPatternAttribute = ((App)Application.Current).listDocument.CreateAttribute("searchpattern");
                searchPatternAttribute.InnerText = FolderSearchPatternTextBox.Text;
                listNode.Attributes.SetNamedItem(searchPatternAttribute);

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

                if (this.Owner.Resources["Data"] != null)
                {
                    XmlDataProvider pXdp = (XmlDataProvider)this.Owner.Resources["Data"];
                    pXdp.Document = ((App)Application.Current).listDocument;
                    pXdp.Refresh();
                }

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't add list: " + ex.Message, "Big Blue Configuration");
            }
        }
    }
}
