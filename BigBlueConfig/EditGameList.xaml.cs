using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Xml.XPath;

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for EditGameList.xaml
    /// </summary>
    public partial class EditGameList : Window, INotifyPropertyChanged
    {
        //XmlDocument emuDocument;
        //public XmlDocument document;
        private const string fileName = "lists.xml";
        XmlDocument emuDocument;

        // XmlDocument listDocument;

        string listEditing;

        string tempXpath = "";

        XmlDataProvider xdp;

        //XmlDocument listsXml, 
        public EditGameList(string listName, XmlDocument eDocument, string xPath)
        {
            InitializeComponent();

            Closing += EditGameList_Closing;
            this.PreviewKeyUp += HandlePress;

            if (string.IsNullOrWhiteSpace(xPath))
            {
                tempXpath = "list";
            }
            else
            {
                tempXpath = xPath;
            }

            if (string.IsNullOrWhiteSpace(listName))
            {
                listName = "Root";
            }

            listEditing = listName;

            this.Title = "Edit items in " + listName + " list";

            //emuDocument = eDocument;

            //listDocument = listsXml;

            //elemList = document.GetElementsByTagName("game");

            emuDocument = eDocument;
            XmlDataProvider e = (XmlDataProvider)Resources["EmulatorData"];
            e.Document = eDocument;

            xdp = (XmlDataProvider)Resources["Data"];

            xdp.Document = ((App)Application.Current).listDocument;

            if (string.IsNullOrWhiteSpace(xPath))
            {
                xdp.XPath = "list/item";
            }
            else
            {
                //MessageBox.Show(xPath);
                xdp.XPath = xPath + "/item";

                //MessageBox.Show(xpath);   
            }

            //xdp.Source = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + gamesListXmlFile);
            //p.Document = document;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                ((INotifyPropertyChanged)xdp).PropertyChanged += value;
            }

            remove
            {
                ((INotifyPropertyChanged)xdp).PropertyChanged -= value;
            }
        }

        private void HandlePress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        void EditGameList_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void GamesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            XmlNode listItemElement = (XmlNode)GamesList.SelectedItem;

            XmlNode children = listItemElement.SelectSingleNode("list");

            bool isFolder = false;

            if (children != null)
            {
                isFolder = Utilities.IsListItemAFolder(children.Attributes["folder"]);
            }

            if (children == null || isFolder)
            {
                EditGame egWindow = new(xdp, listItemElement, emuDocument)
                {
                    Owner = this
                };

                Action showAction = () => egWindow.Show();
                this.Dispatcher.BeginInvoke(showAction);

            }
            else
            {
                string xPath = BigBlueConfig.Utilities.FindXPath(children);

                XmlNodeList allChildren = children.SelectNodes("item");



                int index = GamesList.SelectedIndex + 1;

                //string baseXPath = "list/listitem";

                //listLevels.Add(index);

                //foreach (int i in listLevels)
                //{
                //  baseXPath = baseXPath + "[" + i.ToString() + "]/list/listitem";
                //}

                //string childXPath = @"list/listitem[" + index.ToString() + "]/list/listitem";

                //string childXPath = "list/listitem[11]/list/listitem[2]/list/listitem";

                //string childXPath = "list/listitem[11]/list/listitem[2]/list/listitem[2]/list/listitem";

                // get all nodes and find out what the index is the current one is


                XmlNode titleNode = listItemElement.SelectSingleNode("name");

                if (titleNode != null)
                {
                    //((App)Application.Current).listDocument,
                    EditGameList newEditGameListsWindow = new(titleNode.InnerText, emuDocument, xPath)
                    {
                        Owner = this
                    };
                    newEditGameListsWindow.Show();
                }
                else
                {
                    MessageBox.Show("You must give this list item a title before editing it.", "Big Blue Configuration");
                }
            }

            //EditGame weow = new EditGame(fileName, xdp.Document, index);
            //weow.Owner = this;
            //weow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //xdp,
            BigBlueConfig.Utilities.SortUp(this, (XmlNode)GamesList.SelectedItem, GamesList, tempXpath, fileName);


            /*
            int index = GamesList.SelectedIndex;
            
            if (index > -1)
            {
                XmlNode el = elemList[index];

                XmlNode elClone = el.Clone();

                if (el.PreviousSibling != null)
                {
                    XmlNode movedItem = el.ParentNode.InsertBefore(elClone, el.PreviousSibling);

                    el.ParentNode.RemoveChild(el);

                    document.Save(fileName);

                    GamesList.SelectedItem = movedItem;
                    GamesList.ScrollIntoView(movedItem);
                }
            }
             */
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BigBlueConfig.Utilities.SortDown(this, xdp, (XmlNode)GamesList.SelectedItem, GamesList, tempXpath, fileName);


            //GamesList.Items.SortDescriptions.Add(new SortDescription("title", ListSortDirection.Ascending));
            //document.Save(fileName);

            /*
            int index = GamesList.SelectedIndex;
            int newIndex = index + 1;

            if (index > -1)
            {
                XmlNode el = elemList[index];

                XmlNode elClone = el.Clone();

                if (el.NextSibling != null)
                {
                    XmlNode movedItem = el.ParentNode.InsertAfter(elClone, el.NextSibling);

                    el.ParentNode.RemoveChild(el);

                    document.Save(fileName);

                    GamesList.SelectedItem = movedItem;
                    GamesList.ScrollIntoView(movedItem);
                }   
            }
             */
        }

        private void AddGameButton_Click_1(object sender, RoutedEventArgs e)
        {
            AddGame weow = new(xdp, GamesList.SelectedIndex, tempXpath) { Owner = this };
            weow.Show();
        }

        private static string GetROMName(string path)
        {
            int start = path.LastIndexOf(@"\");
            string fileNameWithoutDirectory = path.Substring(start + 1);
            int periodIndex = fileNameWithoutDirectory.LastIndexOf(".");

            return fileNameWithoutDirectory.Substring(0, periodIndex);
        }

        private static XElement BulkAddGame(string path, string dir, string exe, string args, string thumb, string video, string marquee, string flyer, string instruct, string preDir, string preExe, string preArgs, string postDir, string postExe, string postArgs, bool kill, bool fullPath)
        {
            string fileNameWithoutExtension = GetROMName(path);

            XElement gameElement = new("item");

            if (kill)
            {
                XAttribute killAttribute = new("kill", "True");
                gameElement.Add(killAttribute);
            }

            if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                XElement gameTitleElement = new("name", fileNameWithoutExtension);
                gameElement.Add(gameTitleElement);

                if (!string.IsNullOrWhiteSpace(thumb))
                {
                    XElement thumbnailElement = new("snap", BigBlueConfig.Utilities.GenerateBulkMediaValue(thumb, fileNameWithoutExtension));
                    gameElement.Add(thumbnailElement);
                }

                if (!string.IsNullOrWhiteSpace(video))
                {
                    XElement videoElement = new("video", BigBlueConfig.Utilities.GenerateBulkMediaValue(video, fileNameWithoutExtension));
                    gameElement.Add(videoElement);
                }

                if (!string.IsNullOrWhiteSpace(marquee))
                {
                    XElement marqueeElement = new("marquee", BigBlueConfig.Utilities.GenerateBulkMediaValue(marquee, fileNameWithoutExtension));
                    gameElement.Add(marqueeElement);
                }

                if (!string.IsNullOrWhiteSpace(flyer))
                {
                    XElement flyerElement = new("flyer", BigBlueConfig.Utilities.GenerateBulkMediaValue(flyer, fileNameWithoutExtension));
                    gameElement.Add(flyerElement);
                }

                if (!string.IsNullOrWhiteSpace(instruct))
                {
                    XElement instructElement = new("instruct", BigBlueConfig.Utilities.GenerateBulkMediaValue(instruct, fileNameWithoutExtension));
                    gameElement.Add(instructElement);
                }

                if (!string.IsNullOrWhiteSpace(args))
                {
                    XElement argsElement = new("args", BigBlueConfig.Utilities.GenerateBulkArgs(path, args, fileNameWithoutExtension, fullPath));
                    gameElement.Add(argsElement);
                }

                if (!string.IsNullOrWhiteSpace(preArgs))
                {
                    XElement preArgsElement = new("preargs", BigBlueConfig.Utilities.GenerateBulkArgs(path, preArgs, fileNameWithoutExtension, fullPath));
                    gameElement.Add(preArgsElement);
                }

                if (!string.IsNullOrWhiteSpace(postArgs))
                {
                    XElement postArgsElement = new("postargs", BigBlueConfig.Utilities.GenerateBulkArgs(path, postArgs, fileNameWithoutExtension, fullPath));
                    gameElement.Add(postArgsElement);
                }
            }

            if (!string.IsNullOrWhiteSpace(dir))
            {
                XElement gameDirElement = new("dir", dir);
                gameElement.Add(gameDirElement);
            }

            if (!string.IsNullOrWhiteSpace(exe))
            {
                XElement gameExeElement = new("exe", exe);
                gameElement.Add(gameExeElement);
            }

            if (!string.IsNullOrWhiteSpace(preDir))
            {
                XElement preDirElement = new("predir", preDir);
                gameElement.Add(preDirElement);
            }

            if (!string.IsNullOrWhiteSpace(preExe))
            {
                XElement preExeElement = new("preexe", preExe);
                gameElement.Add(preExeElement);
            }

            if (!string.IsNullOrWhiteSpace(postDir))
            {
                XElement postDirElement = new("postdir", postDir);
                gameElement.Add(postDirElement);
            }

            if (!string.IsNullOrWhiteSpace(postExe))
            {
                XElement postExeElement = new("postexe", postExe);
                gameElement.Add(postExeElement);
            }

            return gameElement;
        }

        private void GamesList_Drop(object sender, DragEventArgs e)
        {
            try
            {
                // we're not going to do anything if these conditions aren't met
                if (BulkAddToggle.IsChecked == true && EmulatorList.SelectedIndex > -1 && EmulatorList.Visibility == System.Windows.Visibility.Visible)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        XmlElement selectedEmulatorNode = (XmlElement)EmulatorList.SelectedItem;

                        string emulatorDir = string.Empty;

                        XmlNode dirNode = selectedEmulatorNode.SelectSingleNode("dir");

                        if (dirNode != null)
                        {
                            emulatorDir = dirNode.InnerText;
                        }

                        string emulatorExe = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "exe");
                        string emulatorArgs = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "args");
                        string emulatorThumbnail = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "snap");
                        string emulatorVideo = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "video");
                        string emulatorMarquee = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "marquee");
                        string emulatorFlyer = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "flyer");
                        string emulatorInstructions = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "instruct");
                        string emulatorPreDir = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "predir");
                        string emulatorPreExe = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "preexe");
                        string emulatorPreArgs = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "preargs");
                        string emulatorPostDir = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "postdir");
                        string emulatorPostExe = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "postexe");
                        string emulatorPostArgs = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "postargs");

                        bool emulatorFullRomPath = Convert.ToBoolean(selectedEmulatorNode.SelectSingleNode("fullpath").InnerText);

                        bool kill = false;

                        XmlNode killNode = selectedEmulatorNode.SelectSingleNode("kill");

                        if (killNode != null)
                        {
                            kill = Convert.ToBoolean(killNode.InnerText);
                        }                        

                        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                        XDocument tempListDocument = XDocument.Load(fileName);

                        List<XElement> gamesToAdd = new();

                        foreach (string file in files)
                        {
                            gamesToAdd.Add(BulkAddGame(file, emulatorDir, emulatorExe, emulatorArgs, emulatorThumbnail, emulatorVideo, emulatorMarquee, emulatorFlyer, emulatorInstructions, emulatorPreDir, emulatorPreExe, emulatorPreArgs, emulatorPostDir, emulatorPostExe, emulatorPostArgs, kill, emulatorFullRomPath));
                        }

                        tempListDocument.XPathSelectElement(tempXpath).Add(gamesToAdd);

                        tempListDocument.Save(fileName);

                        XmlDocument xmlDoc = new();
                        xmlDoc.Load(fileName);

                        ((App)Application.Current).listDocument = xmlDoc;

                        BigBlueConfig.Utilities.RefreshListDataForAllWindows();

                        // reset this thing so people don't bungle it out?
                        //BulkAddToggle.IsChecked = false;
                        //EmulatorList.Visibility = System.Windows.Visibility.Hidden;
                        //EmulatorList.SelectedIndex = -1;
                        MessageBox.Show("Games added! Make sure you add some snapshots for them and give them proper titles!", "Big Blue Configuration");
                    }
                }
                else
                {
                    MessageBox.Show("You choose an emulator template for the ROM(s) that you're going to add.", "Big Blue Configuration");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't add games using emulator template: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void BulkAddToggle_Checked(object sender, RoutedEventArgs e)
        {
            EmulatorList.Visibility = System.Windows.Visibility.Visible;
        }

        private void BulkAddToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            EmulatorList.Visibility = System.Windows.Visibility.Hidden;
            EmulatorList.SelectedIndex = -1;
        }

        private void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            int selectedGame = GamesList.SelectedIndex;
            XmlNodeList elemList = document.GetElementsByTagName("game");
            XmlNode el = elemList[selectedGame];

            if (el != null)
            {
                el.ParentNode.RemoveChild(el);
            }

            document.Save(fileName);

            GamesList.SelectedIndex = selectedGame;
            GamesList.Focus();
             */

            BigBlueConfig.Utilities.DeleteFromGameList(this, GamesList, xdp.XPath);
        }

        private void AddMame_Click(object sender, RoutedEventArgs e)
        {
            WindowCollection children = this.OwnedWindows;

            foreach (Window win in children)
            {
                if (win.Title == "Add Games from M.A.M.E.")
                {
                    MessageBox.Show("The Add Games from M.A.M.E. window is already open!", "Big Blue Configuration");
                    return;
                }
            }

            MameAdd newMameAddWindow = new(emuDocument, xdp, tempXpath, listEditing)
            {
                Owner = this
            };
            newMameAddWindow.Show();
        }

        private void GamesList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                BigBlueConfig.Utilities.DeleteFromGameList(this, GamesList, xdp.XPath);
            }
        }

        private void AddListButton_Click(object sender, RoutedEventArgs e)
        {
            AddList weow = new(GamesList.SelectedIndex, tempXpath)
            {
                Owner = this
            };
            weow.Show();
        }

        private void EditGameButton_Click(object sender, RoutedEventArgs e)
        {
            WindowCollection windowsToClose = this.OwnedWindows;

            foreach (Window w in windowsToClose)
            {
                w.Close();
            }

            if (GamesList.SelectedItem != null)
            {
                XmlNode listItemElement = (XmlNode)GamesList.SelectedItem;

                EditGame egWindow = new(xdp, listItemElement, emuDocument)
                {
                    Owner = this
                };
                egWindow.Show();
            }
        }

        private void MoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            List<XmlNode> nodesToCopy = new();

            if (GamesList.SelectedItems != null)
            {
                foreach (object nodeObject in GamesList.SelectedItems)
                {
                    XmlNode n = (XmlNode)nodeObject;

                    nodesToCopy.Add(n);
                }
            }

            if (nodesToCopy.Count > 0)
            {
                MoveGame moveGameWindow = new(fileName, nodesToCopy, true)
                {
                    Owner = this
                };
                moveGameWindow.Show();
            }
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            AddFolder afWindow = new(xdp, GamesList.SelectedIndex, tempXpath, emuDocument) { Owner = this };

            Action showAction = () => afWindow.Show();
            this.Dispatcher.BeginInvoke(showAction);
        }
    }
}
