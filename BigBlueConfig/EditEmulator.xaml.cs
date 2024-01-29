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
    /// Interaction logic for EditEmulator.xaml
    /// </summary>
    public partial class EditEmulator : Window
    {
        int emulatorId;
        XmlDocument document;

        public EditEmulator(XmlDocument emulatorXmlDocument, int index)
        {
            InitializeComponent();

            this.PreviewKeyUp += HandlePress;

            Closing += EditEmulator_Closing;
            document = emulatorXmlDocument;
            emulatorId = index;

            displayEmulator();
        }

        private void HandlePress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void CreateNodeIfMissing(XmlNode eNode, string nodeName, string defaultValue)
        {
            if (eNode[nodeName] == null)
            {
                XmlNode emulatorNode = document.CreateNode(XmlNodeType.Element, nodeName, "");
                emulatorNode.InnerText = defaultValue;
                eNode.AppendChild(emulatorNode);

                //configXmlFile = new XmlDocument();
            }
        }

        private void VideoBrowser_Click(object sender, RoutedEventArgs e)
        {
            VideoPreview.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".avi", "Video files|*.mp4;*.mpg;*.mpeg*.avi;*.avi|All files (*.*)|*.*");
        }

        private void ThumbnailBrowser_Click(object sender, RoutedEventArgs e)
        {
            EmulatorThumbnail.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files (.png)|*.png");
        }

        private void MarqueeBrowser_Click(object sender, RoutedEventArgs e)
        {
            MarqueeImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            EmulatorLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        void EditEmulator_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void displayEmulator()
        {
            XmlNodeList elemList = document.GetElementsByTagName("program");
            XmlNode el = elemList[emulatorId];

            if (el != null)
            {
                XmlNode titleNode = el.SelectSingleNode("name");

                if (titleNode != null)
                {
                    EmulatorTitle.Text = titleNode.InnerText;
                }

                XmlNode exeNode = el.SelectSingleNode("exe");
                XmlNode dirNode = el.SelectSingleNode("dir");

                if (exeNode != null && dirNode != null)
                {
                    if (!string.IsNullOrWhiteSpace(exeNode.InnerText) && !string.IsNullOrWhiteSpace(dirNode.InnerText))
                    {
                        EmulatorLocation.Text = dirNode.InnerText + @"\" + exeNode.InnerText;
                    }
                }

                XmlNode argsNode = el.SelectSingleNode("args");

                if (argsNode != null)
                {
                    EmulatorArgs.Text = argsNode.InnerText;
                }

                // load pre and post launch data
                XmlNode preDirNode = el.SelectSingleNode("predir");
                XmlNode preExeNode = el.SelectSingleNode("preexe");

                if (preDirNode != null && preExeNode != null)
                {
                    if (string.IsNullOrEmpty(preDirNode.InnerText) == false && string.IsNullOrEmpty(preExeNode.InnerText) == false)
                    {
                        string preExePath =
                        PreLaunchLocation.Text = preDirNode.InnerText + @"\" + preExeNode.InnerText;
                    }
                }

                XmlNode preArgsNode = el.SelectSingleNode("preargs");

                if (preArgsNode != null)
                {
                    PreLaunchArgs.Text = preArgsNode.InnerText;
                }

                XmlNode postDirNode = el.SelectSingleNode("postdir");
                XmlNode postExeNode = el.SelectSingleNode("postexe");

                if (postDirNode != null && postExeNode != null)
                {
                    if (string.IsNullOrEmpty(postDirNode.InnerText) == false && string.IsNullOrEmpty(postExeNode.InnerText) == false)
                    {
                        PostLaunchLocation.Text = postDirNode.InnerText + @"\" + postExeNode.InnerText;
                    }
                }

                XmlNode postArgsNode = el.SelectSingleNode("postargs");

                if (postArgsNode != null)
                {
                    PostLaunchArgs.Text = postArgsNode.InnerText;
                }

                XmlNode fullRomPathNode = el.SelectSingleNode("fullpath");

                if (fullRomPathNode != null)
                {
                    bool romPathValue;
                    bool.TryParse(fullRomPathNode.InnerText, out romPathValue);

                    FullRomPathCheckbox.IsChecked = romPathValue;
                }

                XmlNode killNode = el.SelectSingleNode("kill");

                if (killNode != null)
                {
                    bool killValue;
                    bool.TryParse(killNode.InnerText, out killValue);
                    KillProcessCheckbox.IsChecked = killValue;
                }

                XmlNode thumbnailNode = el.SelectSingleNode("snap");

                if (thumbnailNode != null)
                {
                    EmulatorThumbnail.Text = thumbnailNode.InnerText;
                }

                XmlNode marqueeNode = el.SelectSingleNode("marquee");

                if (marqueeNode != null)
                {
                    MarqueeImage.Text = marqueeNode.InnerText;
                }

                XmlNode videoPreviewNode = el.SelectSingleNode("video");

                if (videoPreviewNode != null)
                {
                    VideoPreview.Text = videoPreviewNode.InnerText;
                }

                XmlNode instructionNode = el.SelectSingleNode("instruct");

                if (instructionNode != null)
                {
                    InstructionImage.Text = instructionNode.InnerText;
                }

                XmlNode flyerNode = el.SelectSingleNode("flyer");

                if (flyerNode != null)
                {
                    FlyerImage.Text = flyerNode.InnerText;
                }
            }
        }

        private void editEmulator()
        {
            XmlNodeList elemList = document.GetElementsByTagName("program");
            XmlNode el = elemList[emulatorId];

            if (el != null)
            {
                BigBlueConfig.Utilities.EditNodeValue(document, el, "name", EmulatorTitle.Text);
                BigBlueConfig.Utilities.EditNodeValue(document, el, "dir", Utilities.GetDirectoryOfProgram(EmulatorLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(document, el, "exe", Utilities.GetExeOfProgram(EmulatorLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(document, el, "args", EmulatorArgs.Text);
                BigBlueConfig.Utilities.EditNodeValue(document, el, "predir", Utilities.GetDirectoryOfProgram(PreLaunchLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(document, el, "preexe", Utilities.GetExeOfProgram(PreLaunchLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(document, el, "preargs", PreLaunchArgs.Text);
                BigBlueConfig.Utilities.EditNodeValue(document, el, "postdir", Utilities.GetDirectoryOfProgram(PostLaunchLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(document, el, "postexe", Utilities.GetExeOfProgram(PostLaunchLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(document, el, "postargs", PostLaunchArgs.Text);
                BigBlueConfig.Utilities.EditNodeValue(document, el, "fullpath", FullRomPathCheckbox.IsChecked.ToString());
                BigBlueConfig.Utilities.EditNodeValue(document, el, "kill", KillProcessCheckbox.IsChecked.ToString());
                BigBlueConfig.Utilities.EditNodeValue(document, el, "snap", EmulatorThumbnail.Text);
                BigBlueConfig.Utilities.EditNodeValue(document, el, "marquee", MarqueeImage.Text);
                BigBlueConfig.Utilities.EditNodeValue(document, el, "flyer", FlyerImage.Text);
                BigBlueConfig.Utilities.EditNodeValue(document, el, "instruct", InstructionImage.Text);
                BigBlueConfig.Utilities.EditNodeValue(document, el, "video", VideoPreview.Text);

                document.Save("programs.xml");
            }
        }

        private void EditEmulatorButton_Click(object sender, RoutedEventArgs e)
        {
            editEmulator();
            this.Close();
        }

        private void DeleteEmulatorButton_Click(object sender, RoutedEventArgs e)
        {
            XmlNodeList elemList = document.GetElementsByTagName("program");
            XmlNode el = elemList[emulatorId];

            if (el != null)
            {
                el.ParentNode.RemoveChild(el);
            }

            document.Save("programs.xml");

            this.Close();
        }

        private void FlyerBrowser_Click(object sender, RoutedEventArgs e)
        {
            FlyerImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void InstructionBrowser_Click(object sender, RoutedEventArgs e)
        {
            InstructionImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void PreLaunchLocationBrowse_Click(object sender, RoutedEventArgs e)
        {
            PreLaunchLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void PostLaunchLocationBrowse_Click(object sender, RoutedEventArgs e)
        {
            PostLaunchLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }
    }
}
