using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddEmulator.xaml
    /// </summary>
    public partial class AddEmulator : Window
    {
        int indexToAddEmulator;
        XmlDocument emuDocument;

        public AddEmulator(int index, XmlDocument emulatorsXmlDocument)
        {
            InitializeComponent();

            this.PreviewKeyUp += HandlePress;
            EmulatorTitle.Focus();

            emuDocument = emulatorsXmlDocument;
            indexToAddEmulator = index;
        }

        private void HandlePress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void FlyerBrowser_Click(object sender, RoutedEventArgs e)
        {
            FlyerImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void InstructionBrowser_Click(object sender, RoutedEventArgs e)
        {
            InstructionImage.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            EmulatorLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
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

        private void AddEmulatorButton_Click(object sender, RoutedEventArgs e)
        {
            // create a new XmlNode for the game to add
            XmlNode emuNode = emuDocument.CreateNode(XmlNodeType.Element, "program", "");

            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "name", EmulatorTitle.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "dir", Utilities.GetDirectoryOfProgram(EmulatorLocation.Text));
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "exe", Utilities.GetExeOfProgram(EmulatorLocation.Text));
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "args", EmulatorArgs.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "predir", PreLaunchLocation.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "preexe", Utilities.GetExeOfProgram(PreLaunchLocation.Text));
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "preargs", PreLaunchArgs.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "postdir", Utilities.GetDirectoryOfProgram(PostLaunchLocation.Text));
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "postexe", Utilities.GetExeOfProgram(PostLaunchLocation.Text));
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "postargs", PostLaunchArgs.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "video", VideoPreview.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "snap", EmulatorThumbnail.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "marquee", MarqueeImage.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "flyer", FlyerImage.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "instruct", InstructionImage.Text);
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "fullpath", FullRomPathCheckbox.IsChecked.ToString());
            BigBlueConfig.Utilities.AddValueToNode(emuDocument, emuNode, "kill", KillProcessCheckbox.IsChecked.ToString());

            if (indexToAddEmulator > -1)
            {
                // if a game is selected, add it before the selected game
                XmlNodeList elemList = emuDocument.GetElementsByTagName("program");

                XmlNode el = elemList[indexToAddEmulator];

                el.ParentNode.InsertBefore(emuNode, el);
            }
            else
            {
                // if no game is selected, just put it at the end of the list
                // add the game node to the destination game list
                emuDocument.DocumentElement.AppendChild(emuNode);
            }

            emuDocument.Save("emulators.xml");

            this.Close();
            this.Owner.Activate();
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
