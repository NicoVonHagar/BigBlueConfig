using System;
using System.Collections.Generic;
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

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for AddGame.xaml
    /// </summary>
    public partial class AddGame : Window
    {
        int indexToAddGame;
        string xPath;
        XmlDataProvider parentProvider;

        public AddGame(XmlDataProvider xdp, int index, string currentXPath)
        {
            InitializeComponent();

            this.PreviewKeyUp += HandlePress;

            parentProvider = xdp;

            GameTitle.Focus();

            xPath = currentXPath;

            indexToAddGame = index;
        }

        private void HandlePress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && TestGameButton.IsFocused == false)
            {
                this.Close();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            GameLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void ThumbnailBrowser_Click_1(object sender, RoutedEventArgs e)
        {
            GameThumbnail.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".png", "Image files)|*.png;*.jpg;*.jpeg");
        }

        private void AddGameButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //XmlDocument document = ((App)Application.Current).listDocument;

                // create a new XmlNode for the game to add
                XmlNode gameNode = parentProvider.Document.CreateNode(XmlNodeType.Element, "item", "");

                if (KillProcessCheckbox.IsChecked == true)
                {
                    XmlAttribute killProcessAttribute = parentProvider.Document.CreateAttribute("kill");
                    killProcessAttribute.InnerText = true.ToString();
                    gameNode.Attributes.SetNamedItem(killProcessAttribute);
                }

                if (AutoRunCheckbox.IsChecked == true)
                {
                    // clear the rest
                    foreach (XmlNode xNode in parentProvider.Document.SelectNodes("//list/item"))
                    {
                        xNode.Attributes.RemoveNamedItem("autorun");
                    }

                    XmlAttribute autoRunAttribute = parentProvider.Document.CreateAttribute("autorun");
                    autoRunAttribute.InnerText = true.ToString();
                    gameNode.Attributes.SetNamedItem(autoRunAttribute);
                }

                if (!string.IsNullOrWhiteSpace(GameTitle.Text))
                {
                    XmlNode gameTitleNode = parentProvider.Document.CreateNode(XmlNodeType.Element, "name", "");
                    gameTitleNode.InnerText = GameTitle.Text;
                    gameNode.AppendChild(gameTitleNode);
                }

                if (!string.IsNullOrWhiteSpace(GameThumbnail.Text))
                {
                    // thumbnail
                    XmlNode gameThumbnail = parentProvider.Document.CreateNode(XmlNodeType.Element, "snap", "");
                    gameThumbnail.InnerText = GameThumbnail.Text;
                    gameNode.AppendChild(gameThumbnail);
                }

                if (!string.IsNullOrWhiteSpace(MarqueeImage.Text))
                {
                    XmlNode marqueeImage = parentProvider.Document.CreateNode(XmlNodeType.Element, "marquee", "");
                    marqueeImage.InnerText = MarqueeImage.Text;
                    gameNode.AppendChild(marqueeImage);
                }

                if (!string.IsNullOrWhiteSpace(FlyerImage.Text))
                {
                    XmlNode flyerImage = parentProvider.Document.CreateNode(XmlNodeType.Element, "flyer", "");
                    flyerImage.InnerText = FlyerImage.Text;
                    gameNode.AppendChild(flyerImage);
                }

                if (!string.IsNullOrWhiteSpace(InstructionImage.Text))
                {
                    XmlNode instructionImage = parentProvider.Document.CreateNode(XmlNodeType.Element, "instruct", "");
                    instructionImage.InnerText = InstructionImage.Text;
                    gameNode.AppendChild(instructionImage);
                }

                if (!string.IsNullOrWhiteSpace(VideoPreview.Text))
                {
                    XmlNode videoUrl = parentProvider.Document.CreateNode(XmlNodeType.Element, "video", "");
                    videoUrl.InnerText = VideoPreview.Text;
                    gameNode.AppendChild(videoUrl);
                }

                if (!string.IsNullOrWhiteSpace(GameLocation.Text))
                {
                    XmlNode gameDir = parentProvider.Document.CreateNode(XmlNodeType.Element, "dir", "");
                    gameDir.InnerText = Utilities.GetDirectoryOfProgram(GameLocation.Text);
                    gameNode.AppendChild(gameDir);

                    // exe
                    XmlNode gameExe = parentProvider.Document.CreateNode(XmlNodeType.Element, "exe", "");
                    gameExe.InnerText = Utilities.GetExeOfProgram(GameLocation.Text);
                    gameNode.AppendChild(gameExe);
                }

                if (!string.IsNullOrWhiteSpace(GameArgs.Text))
                {
                    // args
                    XmlNode gameArgs = parentProvider.Document.CreateNode(XmlNodeType.Element, "args", "");
                    gameArgs.InnerText = GameArgs.Text;
                    gameNode.AppendChild(gameArgs);
                }

                if (!string.IsNullOrWhiteSpace(PreLaunchLocation.Text))
                {
                    // pre launch exe
                    XmlNode preGameDir = parentProvider.Document.CreateNode(XmlNodeType.Element, "predir", "");
                    preGameDir.InnerText = Utilities.GetDirectoryOfProgram(PreLaunchLocation.Text);
                    gameNode.AppendChild(preGameDir);

                    XmlNode preGameExe = parentProvider.Document.CreateNode(XmlNodeType.Element, "preexe", "");
                    preGameExe.InnerText = Utilities.GetExeOfProgram(PreLaunchLocation.Text);
                    gameNode.AppendChild(preGameExe);
                }


                if (!string.IsNullOrWhiteSpace(PreLaunchArgs.Text))
                {
                    XmlNode preGameArgs = parentProvider.Document.CreateNode(XmlNodeType.Element, "preargs", "");
                    preGameArgs.InnerText = PreLaunchArgs.Text;
                    gameNode.AppendChild(preGameArgs);
                }


                if (!string.IsNullOrWhiteSpace(PostLaunchLocation.Text))
                {
                    // post launch exe
                    XmlNode postGameDir = parentProvider.Document.CreateNode(XmlNodeType.Element, "postdir", "");
                    postGameDir.InnerText = Utilities.GetDirectoryOfProgram(PostLaunchLocation.Text);
                    gameNode.AppendChild(postGameDir);

                    XmlNode postGameExe = parentProvider.Document.CreateNode(XmlNodeType.Element, "postexe", "");
                    postGameExe.InnerText = Utilities.GetExeOfProgram(PostLaunchLocation.Text);
                    gameNode.AppendChild(postGameExe);
                }

                if (!string.IsNullOrWhiteSpace(PostLaunchArgs.Text))
                {
                    XmlNode postGameArgs = parentProvider.Document.CreateNode(XmlNodeType.Element, "postargs", "");
                    postGameArgs.InnerText = PostLaunchArgs.Text;
                    gameNode.AppendChild(postGameArgs);
                }

                if (indexToAddGame > -1)
                {
                    // if a game is selected, add it before the selected game
                    XmlNodeList elemList = parentProvider.Document.SelectNodes(xPath + @"/item");

                    XmlNode el = elemList[indexToAddGame];

                    el.ParentNode.InsertBefore(gameNode, el);
                }
                else
                {
                    // if no game is selected, just put it at the end of the list
                    // add the game node to the destination game list
                    parentProvider.Document.SelectSingleNode(xPath).AppendChild(gameNode);
                }

                parentProvider.Document.Save("lists.xml");

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't add game: " + ex.Message, "Big Blue Configuration");
            }
        }

        string fullPathToTest;
        string preFullPathToTest;
        string postFullPathToTest;
        string dirToTest;
        string exeToTest;
        string argsToTest;
        string preDirToTest;
        string preExeToTest;
        string preArgsToTest;
        string postDirToTest;
        string postExeToTest;
        string postArgsToTest;

        private void TestGameButton_Click(object sender, RoutedEventArgs e)
        {
            TestGameButton.Focus();

            try
            {
                fullPathToTest = GameLocation.Text;

                dirToTest = Utilities.GetDirectoryOfProgram(GameLocation.Text);
                exeToTest = Utilities.GetExeOfProgram(GameLocation.Text);

                argsToTest = GameArgs.Text;

                preFullPathToTest = PreLaunchLocation.Text;

                preDirToTest = Utilities.GetDirectoryOfProgram(PreLaunchLocation.Text);
                preExeToTest = Utilities.GetExeOfProgram(PreLaunchLocation.Text);

                postFullPathToTest = PostLaunchLocation.Text;

                postDirToTest = Utilities.GetDirectoryOfProgram(PostLaunchLocation.Text);
                postExeToTest = Utilities.GetExeOfProgram(PostLaunchLocation.Text);

                preArgsToTest = PreLaunchArgs.Text;
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

        private enum launchType
        {
            pre,
            main,
            post
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
                        new Action(() => AddGameButton.Focus())
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
    }
}
