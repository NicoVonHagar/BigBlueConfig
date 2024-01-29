using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Xml.Linq;

namespace BigBlueConfig
{
    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return bool.Parse((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return "True";
            }

            return "False";
        }
    }

    /// <summary>
    /// Interaction logic for EditGame.xaml
    /// </summary>
    public partial class EditGame : Window
    {
        private enum launchType
        {
            pre,
            main,
            post
        }

        XmlDataProvider parentProvider;
        //public XmlDataProvider currentProvider;
        XmlNode liNode;
        private const string fileName = "lists.xml";
        string dirToTest;
        string exeToTest;
        string argsToTest;

        string preDirToTest;
        string preExeToTest;
        string preArgsToTest;

        string postDirToTest;
        string postExeToTest;
        string postArgsToTest;
        //bool isList = false;

        public EditGame(XmlDataProvider xdp, XmlNode listItemNode, XmlDocument eDocument)
        {
            InitializeComponent();

            string xp = Utilities.FindXPath(listItemNode);

            XmlDataProvider e = (XmlDataProvider)Resources["EmulatorData"];
            e.Document = eDocument;

            this.PreviewKeyUp += HandlePress;

            Closing += EditGame_Closing;

            liNode = listItemNode;

            parentProvider = xdp;

            XmlDataProvider listData = (XmlDataProvider)Resources["Data"];
            listData.Document = xdp.Document;
            listData.XPath = xp;
            
            displayGame();
        }

        private void HandlePress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && TestGameButton.IsFocused == false)
            {
                this.Close();
            }
        }

        void EditGame_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void displayGame()
        {
            if (liNode != null)
            {
                XmlNode childListNode = liNode.SelectSingleNode("list");

                if (childListNode == null)
                {
                    DefaultListCheckbox.Visibility = Visibility.Collapsed;
                    DefaultListContainer.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ConvertToListItemButton.Visibility = Visibility.Visible;
                    AutoRunLabel.Visibility = Visibility.Collapsed;
                    AutoRunCheckbox.Visibility = Visibility.Collapsed;
                    AutoRunContainer.Visibility = Visibility.Collapsed;

                    //isList = true;
                    //ConvertToListButton.Content = "Convert to List Item";

                    XmlAttribute folderAttribute = childListNode.Attributes["folder"];

                    if (Utilities.IsListItemAFolder(folderAttribute))
                    {
                        this.Title = "Edit Folder";
                        FolderLocationTextBox.Text = folderAttribute.InnerText;
                        ConvertToFolderButton.Visibility = Visibility.Visible;
                        FolderSearchPatternContainer.Visibility = Visibility.Visible;
                        FolderLocationLabel.Visibility = Visibility.Visible;
                        FolderLocationInputs.Visibility = Visibility.Visible;
                        ConvertToFolderButton.Visibility = Visibility.Collapsed;
                        TemplateContainer.Visibility = Visibility.Visible;
                        TemplateLabel.Visibility = Visibility.Visible;
                        MarqueeTemplateLabel.Visibility = Visibility.Visible;
                        MarqueeTemplateContainer.Visibility = Visibility.Visible;
                        InstructionImageTemplateContainer.Visibility = Visibility.Visible;
                        InstructionTemplateLabel.Visibility = Visibility.Visible;
                        FlyerTemplateContainer.Visibility = Visibility.Visible;
                        FlyerTemplateLabel.Visibility = Visibility.Visible;
                        VideoTemplateContainer.Visibility = Visibility.Visible;
                        VideoTemplateLabel.Visibility = Visibility.Visible;
                        SnapshotTemplateLabel.Visibility = Visibility.Visible;
                        SnapshotTemplateContainer.Visibility = Visibility.Visible;

                        XmlAttribute searchPatternAttribute = childListNode.Attributes["searchpattern"];

                        if (searchPatternAttribute != null)
                        {
                            FolderSearchPatternTextBox.Text = searchPatternAttribute.InnerText;
                        }
                    }
                    else
                    {
                        this.Title = "Edit List";
                        ConvertToListButton.Visibility = Visibility.Collapsed;
                        PreLaunchLabel.Visibility = Visibility.Collapsed;
                        PreLaunchArgs.Visibility = Visibility.Collapsed;
                        PreLaunchLocation.Visibility = Visibility.Collapsed;
                        PreArgsLabel.Visibility = Visibility.Collapsed;
                        PreLaunchLocationBrowse.Visibility = Visibility.Collapsed;
                        FolderLocationInputs.Visibility = Visibility.Collapsed;
                        FolderLocationLabel.Visibility = Visibility.Collapsed;
                        PostProgramLabel.Visibility = Visibility.Collapsed;
                        PostLaunchLocation.Visibility = Visibility.Collapsed;
                        PostLaunchLocationBrowse.Visibility = Visibility.Collapsed;
                        PostLaunchArgs.Visibility = Visibility.Collapsed;
                        PostLaunchArgsLabel.Visibility = Visibility.Collapsed;
                        ArgsLabel.Visibility = Visibility.Collapsed;
                        GameArgs.Visibility = Visibility.Collapsed;
                        TestGameButton.Visibility = Visibility.Collapsed;
                        LocationLabel.Visibility = Visibility.Collapsed;
                        GameLocation.Visibility = Visibility.Collapsed;
                        ForceCloseLabel.Visibility = Visibility.Collapsed;
                        KillProcessCheckbox.Visibility = Visibility.Collapsed;
                        ProgramLocationBrowseButton.Visibility = Visibility.Collapsed;
                        CommandlineParameterContainer.Visibility = Visibility.Collapsed;
                        ProgramLocationContainer.Visibility = Visibility.Collapsed;
                        ForceCloseContainer.Visibility = Visibility.Collapsed;
                    }
                }

                XmlNode titleNode = liNode.SelectSingleNode("name");

                if (titleNode != null)
                {
                    GameTitle.Text = titleNode.InnerText;
                }

                XmlAttribute killProcessAttribute = liNode.Attributes["kill"];

                if (killProcessAttribute != null)
                {
                    if (Convert.ToBoolean(killProcessAttribute.Value) == true)
                    {
                        KillProcessCheckbox.IsChecked = true;
                    }
                }

                /*
                XmlAttribute autoRunAttribute = liNode.Attributes["autorun"];

                if (autoRunAttribute != null)
                {
                    if (Convert.ToBoolean(autoRunAttribute.Value) == true)
                    {
                        AutoRunCheckbox.IsChecked = true;
                    }
                }
                */

                SnapshotTemplateTextBox.Text = BigBlueConfig.Utilities.GetNodeValue(liNode, "snaptemplate");
                InstructionTemplateImageTextBox.Text = BigBlueConfig.Utilities.GetNodeValue(liNode, "instructiontemplate");
                MarqueeImageTemplateTextBox.Text = BigBlueConfig.Utilities.GetNodeValue(liNode, "marqueetemplate");
                FlyerTemplateImageTextBox.Text = BigBlueConfig.Utilities.GetNodeValue(liNode, "flyertemplate");

                XmlNode videoNode = liNode.SelectSingleNode("video");

                if (videoNode != null)
                {
                    VideoPreview.Text = videoNode.InnerText;
                }

                XmlNode videoTemplateNode = liNode.SelectSingleNode("videotemplate");

                if (videoTemplateNode != null)
                {
                    VideoTemplatePreview.Text = videoTemplateNode.InnerText;
                }

                XmlNode marqueeNode = liNode.SelectSingleNode("marquee");

                if (marqueeNode != null)
                {
                    MarqueeImage.Text = marqueeNode.InnerText;
                }

                XmlNode flyerNode = liNode.SelectSingleNode("flyer");

                if (flyerNode != null)
                {
                    FlyerImage.Text = flyerNode.InnerText;
                }

                XmlNode instructionNode = liNode.SelectSingleNode("instruct");

                if (instructionNode != null)
                {
                    InstructionImage.Text = instructionNode.InnerText;
                }

                XmlNode thumbnailNode = liNode.SelectSingleNode("snap");

                if (thumbnailNode != null)
                {
                    GameThumbnail.Text = thumbnailNode.InnerText;
                }

                XmlNode dirNode = liNode.SelectSingleNode("dir");
                XmlNode exeNode = liNode.SelectSingleNode("exe");

                if (dirNode != null && exeNode != null)
                {
                    string exePath = dirNode.InnerText + @"\" + exeNode.InnerText;

                    GameLocation.Text = exePath;
                }

                XmlNode argsNode = liNode.SelectSingleNode("args");

                if (argsNode != null)
                {
                    GameArgs.Text = argsNode.InnerText;
                }

                XmlNode preDirNode = liNode.SelectSingleNode("predir");
                XmlNode preExeNode = liNode.SelectSingleNode("preexe");

                if (preDirNode != null && preExeNode != null)
                {
                    // load pre and post launch data
                    if (string.IsNullOrWhiteSpace(preDirNode.InnerText) == false && string.IsNullOrWhiteSpace(preExeNode.InnerText) == false)
                    {
                        string preExePath = preDirNode.InnerText + @"\" + preExeNode.InnerText;
                        PreLaunchLocation.Text = preExePath;
                    }
                }

                XmlNode preArgsNode = liNode.SelectSingleNode("preargs");

                if (preArgsNode != null)
                {
                    PreLaunchArgs.Text = preArgsNode.InnerText;
                }

                XmlNode postDirNode = liNode.SelectSingleNode("postdir");
                XmlNode postExeNode = liNode.SelectSingleNode("postexe");

                if (postDirNode != null && postExeNode != null)
                {
                    if (string.IsNullOrWhiteSpace(postDirNode.InnerText) == false && string.IsNullOrWhiteSpace(postExeNode.InnerText) == false)
                    {
                        string postExePath = postDirNode.InnerText + @"\" + postExeNode.InnerText;
                        PostLaunchLocation.Text = postExePath;
                    }
                }

                XmlNode postArgsNode = liNode.SelectSingleNode("postargs");

                if (postArgsNode != null)
                {
                    PostLaunchArgs.Text = postArgsNode.InnerText;
                }
            }
        }

        

        private void SetListItemCheckboxAttribute(bool isChecked, XmlNode node, string attributeName, string attributeXPath)
        {
            // wipe out this attribute anywhere else it might exist
            foreach (XmlNode xNode in parentProvider.Document.SelectNodes(attributeXPath))
            {
                xNode.Attributes.RemoveNamedItem(attributeName);
            }

            /*
            if (Convert.ToBoolean(attr.Value) == true)
            {
                cb.IsChecked = true;
            }
            */
            
            if (isChecked)
            {
                XmlAttribute newAutoRunAttribute = parentProvider.Document.CreateAttribute(attributeName);
                newAutoRunAttribute.InnerText = true.ToString();

                node.Attributes.SetNamedItem(newAutoRunAttribute);
            }
        }

        private void editGame(string mode)
        {
            SetListItemCheckboxAttribute((bool)AutoRunCheckbox.IsChecked, liNode, "autorun", "//list/item");

            bool isDefaultChecked = (bool)DefaultListCheckbox.IsChecked;

            if (liNode != null)
            {
                XmlAttribute killProcessAttribute = liNode.Attributes["kill"];

                if (killProcessAttribute != null)
                {
                    if (KillProcessCheckbox.IsChecked == true)
                    {
                        killProcessAttribute.InnerText = true.ToString();
                    }
                    else
                    {
                        liNode.Attributes.RemoveNamedItem("kill");
                    }
                }
                else
                {
                    if (KillProcessCheckbox.IsChecked == true)
                    {
                        XmlAttribute newKillProcessAttribute = parentProvider.Document.CreateAttribute("kill");
                        newKillProcessAttribute.InnerText = true.ToString();

                        liNode.Attributes.SetNamedItem(newKillProcessAttribute);
                    }
                }

                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "name", GameTitle.Text);
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "snap", GameThumbnail.Text);
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "video", VideoPreview.Text);
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "marquee", MarqueeImage.Text);
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "flyer", FlyerImage.Text);
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "instruct", InstructionImage.Text);
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "dir", Utilities.GetDirectoryOfProgram(GameLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "exe", Utilities.GetExeOfProgram(GameLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "args", GameArgs.Text);
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "predir", Utilities.GetDirectoryOfProgram(PreLaunchLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "preexe", Utilities.GetExeOfProgram(PreLaunchLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "preargs", PreLaunchArgs.Text);
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "postdir", Utilities.GetDirectoryOfProgram(PostLaunchLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "postexe", Utilities.GetExeOfProgram(PostLaunchLocation.Text));
                BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "postargs", PostLaunchArgs.Text);

                XmlNode listNode = liNode.SelectSingleNode("list");

                if (listNode != null)
                {
                    SetListItemCheckboxAttribute(isDefaultChecked, listNode, "default", "//list");
                }

                if (FolderLocationTextBox.Visibility == Visibility.Visible)
                {
                    BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "videotemplate", VideoTemplatePreview.Text);

                    // image templates
                    BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "snaptemplate", SnapshotTemplateTextBox.Text);
                    BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "marqueetemplate", MarqueeImageTemplateTextBox.Text);
                    BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "flyertemplate", FlyerTemplateImageTextBox.Text);
                    BigBlueConfig.Utilities.EditNodeValue(parentProvider.Document, liNode, "instructtemplate", InstructionTemplateImageTextBox.Text);

                    if (listNode != null)
                    {
                        if (listNode.Attributes["folder"] == null)
                        {
                            XmlAttribute folderAttribute = parentProvider.Document.CreateAttribute("folder");
                            folderAttribute.InnerText = FolderLocationTextBox.Text;
                            listNode.Attributes.SetNamedItem(folderAttribute);
                        }
                        else
                        {
                            listNode.Attributes["folder"].InnerText = FolderLocationTextBox.Text;
                        }

                        if (listNode.Attributes["searchpattern"] == null)
                        {
                            XmlAttribute searchPatternAttribute = parentProvider.Document.CreateAttribute("searchpattern");
                            searchPatternAttribute.InnerText = FolderSearchPatternTextBox.Text;
                            listNode.Attributes.SetNamedItem(searchPatternAttribute);
                        }
                        else
                        {
                            listNode.Attributes["searchpattern"].InnerText = FolderSearchPatternTextBox.Text;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(mode))
                {
                    // if it already has a list, you're going to remove it
                    // otherwise create it
                    switch (mode)
                    {
                        // convert to folder
                        case "folderconvert":
                            // folders can't auto run because there can always be different items in them
                            if (liNode.Attributes["autorun"] != null)
                            {
                                liNode.Attributes.RemoveNamedItem("autorun");
                            }

                            if (listNode != null)
                            {
                                // you're going to want to get rid of any child items that exist under this if you convert it to a folder
                                listNode.RemoveAll();

                                if (isDefaultChecked)
                                {
                                    XmlAttribute defaultAttribute = parentProvider.Document.CreateAttribute("default");
                                    defaultAttribute.InnerText = "True";
                                    listNode.Attributes.SetNamedItem(defaultAttribute);
                                }

                                // add the folder attribute to this list
                                if (listNode.Attributes["folder"] == null)
                                {
                                    XmlAttribute folderAttribute = parentProvider.Document.CreateAttribute("folder");
                                    folderAttribute.InnerText = FolderLocationTextBox.Text;
                                    listNode.Attributes.SetNamedItem(folderAttribute);
                                }
                                else
                                {
                                    listNode.Attributes["folder"].InnerText = FolderLocationTextBox.Text;
                                }

                                if (listNode.Attributes["searchpattern"] == null)
                                {
                                    XmlAttribute searchPatternAttribute = parentProvider.Document.CreateAttribute("searchpattern");
                                    searchPatternAttribute.InnerText = "*.*";
                                    listNode.Attributes.SetNamedItem(searchPatternAttribute);
                                }
                                else
                                {
                                    listNode.Attributes["searchpattern"].InnerText = "*.*";
                                }
                            }
                            else
                            {
                                // create a new list within this list item
                                XmlNode gameNode = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "list", "");

                                // create a new folder attribute on the new list
                                XmlAttribute folderAttribute = parentProvider.Document.CreateAttribute("folder");
                                folderAttribute.InnerText = FolderLocationTextBox.Text;
                                gameNode.Attributes.SetNamedItem(folderAttribute);

                                XmlAttribute searchPatternAttribute = parentProvider.Document.CreateAttribute("searchpattern");
                                searchPatternAttribute.InnerText = "*.*";
                                gameNode.Attributes.SetNamedItem(searchPatternAttribute);

                                // add the new list
                                liNode.AppendChild(gameNode);
                            }
                            break;
                        // convert to list
                        case "listconvert":
                            // lists can't be killed
                            if (liNode.Attributes["kill"] != null)
                            {
                                liNode.Attributes.RemoveNamedItem("kill");
                            }

                            // lists can't auto run
                            if (liNode.Attributes["autorun"] != null)
                            {
                                liNode.Attributes.RemoveNamedItem("autorun");
                            }

                            if (listNode == null)
                            {
                                // create a new XmlNode for the game to add
                                XmlNode gameNode = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "list", "");

                                liNode.AppendChild(gameNode);
                            }
                            else
                            {
                                if (listNode.Attributes["folder"] != null)
                                {
                                    listNode.Attributes.RemoveNamedItem("folder");
                                }

                                if (listNode.Attributes["searchpattern"] != null)
                                {
                                    listNode.Attributes.RemoveNamedItem("searchpattern");
                                }
                            }
                            break;
                        // convert to item
                        case "itemconvert":
                            // items can't be the default list/folder
                            if (liNode.Attributes["default"] != null)
                            {
                                liNode.Attributes.RemoveNamedItem("default");
                            }

                            if (listNode != null)
                            {
                                liNode.RemoveChild(listNode);
                            }
                            break;
                    }
                }
            }

            parentProvider.Document.Save(fileName);

            this.Owner.Activate();
            this.Close();
        }

        private void EditGameButton_Click(object sender, RoutedEventArgs e)
        {
            editGame(string.Empty);
        }

        private void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (liNode != null)
            {
                liNode.ParentNode.RemoveChild(liNode);
            }

            parentProvider.Document.Save(fileName);

            this.Close();
        }

        private void ProgramLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            GameLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void ThumbnailBrowser_Click_1(object sender, RoutedEventArgs e)
        {
            GameThumbnail.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files (.png)|*.png");
        }

        private void CopyGameButton_Click_1(object sender, RoutedEventArgs e)
        {
            List<XmlNode> nodesToMove = new List<XmlNode>();
            nodesToMove.Add(liNode);

            MoveGame moveGameWindow = new MoveGame(fileName, nodesToMove, false);
            moveGameWindow.Owner = this;
            moveGameWindow.Show();
        }

        private void MoveGameButton_Click_1(object sender, RoutedEventArgs e)
        {
            List<XmlNode> nodesToMove = new List<XmlNode>();
            nodesToMove.Add(liNode);

            MoveGame moveGameWindow = new MoveGame(fileName, nodesToMove, true);
            moveGameWindow.Owner = this;
            moveGameWindow.Show();
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

        private void TestGameButton_Click(object sender, RoutedEventArgs e)
        {
            TestGameButton.Focus();

            try
            {
                dirToTest = Utilities.GetDirectoryOfProgram(GameLocation.Text);
                exeToTest = Utilities.GetExeOfProgram(GameLocation.Text);
                argsToTest = GameArgs.Text;

                preDirToTest = Utilities.GetDirectoryOfProgram(PreLaunchLocation.Text);
                preExeToTest = Utilities.GetExeOfProgram(PreLaunchLocation.Text);
                preArgsToTest = PreLaunchArgs.Text;

                postDirToTest = Utilities.GetDirectoryOfProgram(PostLaunchLocation.Text);
                postExeToTest = Utilities.GetExeOfProgram(PostLaunchLocation.Text);
                postArgsToTest = PostLaunchArgs.Text;

                try
                {
                    // if the game has pre-launch settings, we're going to launch it like that
                    if (string.IsNullOrEmpty(preDirToTest) == true || string.IsNullOrEmpty(preExeToTest) == true)
                    {
                        //MessageBox.Show("running game");
                        string fileNameToLaunch = dirToTest + @"\" + exeToTest;

                        launchProgram(dirToTest, fileNameToLaunch, argsToTest, launchType.main);
                    }
                    else
                    {
                        //MessageBox.Show("mounting image");
                        string fileNameToLaunch = preDirToTest + @"\" + preExeToTest;

                        launchProgram(preDirToTest, fileNameToLaunch, preArgsToTest, launchType.pre);
                    }
                }
                catch
                {
                    //returnToFrontEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't run this game successfully: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void launchProgram(string dir, string fileName, string args, launchType type)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = dir;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = fileName;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = args;

            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.EnableRaisingEvents = true;

                switch (type)
                {
                    case launchType.pre:
                        exeProcess.Exited += returnFromPreLaunch;
                        break;
                    case launchType.main:
                        exeProcess.Exited += returnFromGame;
                        break;
                    case launchType.post:
                        break;
                }

                exeProcess.WaitForExit();
            }
        }

        void returnFromPreLaunch(object sender, EventArgs e)
        {
            try
            {
                string fileNameToLaunch = dirToTest + @"\" + exeToTest;

                launchProgram(dirToTest, fileNameToLaunch, argsToTest, launchType.main);
            }
            catch (Exception)
            {
            }
        }

        private void returnFromGame(object sender, EventArgs e)
        {
            try
            {
                // if the game has pre-launch settings, we're going to 
                if (string.IsNullOrEmpty(postDirToTest) == false && string.IsNullOrEmpty(postExeToTest) == false)
                {
                    string fileNameToLaunch = postDirToTest + @"\" + postExeToTest;

                    launchProgram(postDirToTest, fileNameToLaunch, postArgsToTest, launchType.post);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            Application.Current.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                new Action(() => EditGameButton.Focus())
            );
        }

        private void PreLaunchLocationBrowse_Click(object sender, RoutedEventArgs e)
        {
            PreLaunchLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void PostLaunchLocationBrowse_Click(object sender, RoutedEventArgs e)
        {
            PostLaunchLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void CopyGameButton_Click(object sender, RoutedEventArgs e)
        {
            List<XmlNode> nodesToCopy = new List<XmlNode>();
            nodesToCopy.Add(liNode);

            MoveGame moveGameWindow = new MoveGame(fileName, nodesToCopy, false);
            moveGameWindow.Owner = this;
            moveGameWindow.Show();
        }

        private void ConvertToListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                editGame("listconvert");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't add list: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void ConvertToFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderLocationTextBox.Text = dialog.SelectedPath;
                // need to call editGame here
                editGame("folderconvert");
            }
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

        private void ConvertToListItemButton_Click(object sender, RoutedEventArgs e)
        {
            editGame("itemconvert");
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
    }
}
