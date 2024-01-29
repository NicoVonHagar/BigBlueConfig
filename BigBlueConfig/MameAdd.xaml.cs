using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Xml.Xsl;

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for MameAdd.xaml
    /// </summary>
    public partial class MameAdd : Window
    {
        XmlDocument doc = null;
        //XmlDocument glDoc;
        private const string glFileName = "lists.xml";

        XmlDataProvider parentProvider;
        public XmlDataProvider currentProvider;
        //public IEnumerable<XElement> GameList;


        private void PopulateManufacturerDropdown()
        {
            List<string> manufacturers = doc.SelectNodes("//manufacturer").Cast<XmlNode>().Select(c => c.InnerText).Distinct().ToList();

            // sort it in alpha
            manufacturers.Sort();

            Manufacturers.Items.Clear();

            ComboBoxItem cbi0 = new ComboBoxItem();
            cbi0.Content = string.Empty;
            Manufacturers.Items.Add(cbi0);

            foreach (string m in manufacturers)
            {
                ComboBoxItem cbi1 = new ComboBoxItem();
                cbi1.Content = m;

                Manufacturers.Items.Add(cbi1);
            }
        }

        string listXPath;

        public MameAdd(XmlDocument eDocument, XmlDataProvider dataProvider, string xPath, string listEditing)
        {
            InitializeComponent();

            parentProvider = dataProvider;

            listXPath = xPath;

            this.Title = this.Title + " to " + listEditing + " list";

            Closing += MameAdd_Closing;

            GameListLabel.Content = listEditing + " list";

            //GamesList.DataContext = glDoc.Element("games").Elements();

            //glDoc = gameListDocument;

            XmlDataProvider e = (XmlDataProvider)Resources["EmulatorData"];
            e.Document = eDocument;

            if (EmulatorList.Items != null)
            {
                if (EmulatorList.Items.Count > 0)
                {
                    EmulatorList.SelectedIndex = 0;

                }
            }

            currentProvider = (XmlDataProvider)Resources["Data"];
            currentProvider.Document = ((App)Application.Current).listDocument;
            currentProvider.XPath = dataProvider.XPath;

            int currentYear = DateTime.Now.Year;

            for (int i = 1972; i <= currentYear; i++)
            {
                To.Items.Add(i);
                From.Items.Add(i);
            }

            this.PreviewKeyUp += HandlePress;

            if (System.IO.File.Exists("mamelist.xml") == true)
            {
                doc = new XmlDocument();
                doc.Load("mamelist.xml");

                PopulateManufacturerDropdown();

                QueryMameXml(false, false, MameLocation.Text);
            }
        }

        private void disableControls()
        {
            MameLocation.IsEnabled = false;
            GameTitle.IsEnabled = false;
            ResetButton.IsEnabled = false;
            SelectMameButton.IsEnabled = false;
            CloneCheckbox.IsEnabled = false;
            Manufacturers.IsEnabled = false;
            From.IsEnabled = false;
            To.IsEnabled = false;
            ScreenTypeComboBox.IsEnabled = false;
            ScreenOrientationComboBox.IsEnabled = false;
            NumberOfPlayers.IsEnabled = false;
            MameQueryResults.IsEnabled = false;
            GamesList.IsEnabled = false;
            UpdateMameList.IsEnabled = false;
            MinRefreshTextBox.IsEnabled = false;
            MaxRefreshTextBox.IsEnabled = false;
        }

        private void enableControls()
        {
            MameLocation.IsEnabled = true;
            GameTitle.IsEnabled = true;
            ResetButton.IsEnabled = true;
            SelectMameButton.IsEnabled = true;
            CloneCheckbox.IsEnabled = true;
            Manufacturers.IsEnabled = true;
            From.IsEnabled = true;
            To.IsEnabled = true;
            ScreenTypeComboBox.IsEnabled = true;
            ScreenOrientationComboBox.IsEnabled = true;
            NumberOfPlayers.IsEnabled = true;
            MameQueryResults.IsEnabled = true;
            GamesList.IsEnabled = true;
            UpdateMameList.IsEnabled = true;
            MinRefreshTextBox.IsEnabled = true;
            MaxRefreshTextBox.IsEnabled = true;
        }

        private void HandlePress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        void MameAdd_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private string BuildXPathQuery()
        {
            string xPathBuilder = string.Empty;

            if (CloneCheckbox.IsChecked == false)
            {
                xPathBuilder += " and not(@cloneof)";
            }

            if (!string.IsNullOrEmpty(From.Text))
            {
                xPathBuilder += " and year >= " + From.Text;
            }

            if (!string.IsNullOrEmpty(To.Text))
            {
                xPathBuilder += " and year <= " + To.Text;
            }

            if (!string.IsNullOrEmpty(GameTitle.Text))
            {
                xPathBuilder += " and contains(translate(description,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'), '" + GameTitle.Text.ToLower() + "')";
            }

            if (!string.IsNullOrEmpty(NumberOfPlayers.Text))
            {
                xPathBuilder += " and input[@players >= " + NumberOfPlayers.Text + "]";
            }

            if (!string.IsNullOrEmpty(ScreenOrientationComboBox.Text))
            {
                xPathBuilder += " and display and display[@rotate = '" + ScreenOrientationComboBox.Text.Replace("°", "") + "']";
            }

            if (!string.IsNullOrEmpty(ScreenTypeComboBox.Text))
            {
                xPathBuilder += " and display[@type = '" + ScreenTypeComboBox.Text.ToLower() + "']";
            }

            if (!string.IsNullOrEmpty(MinRefreshTextBox.Text))
            {
                // check if it's actually a number first
                float f;
                bool isFloat = float.TryParse(MinRefreshTextBox.Text, out f);

                if (isFloat == true)
                {
                    xPathBuilder += " and display[@refresh >= " + MinRefreshTextBox.Text + "]";
                }
            }

            if (!string.IsNullOrEmpty(MaxRefreshTextBox.Text))
            {
                // check if it's actually a number first
                float f;
                bool isFloat = float.TryParse(MaxRefreshTextBox.Text, out f);

                if (isFloat == true)
                {
                    xPathBuilder += " and display[@refresh <= " + MaxRefreshTextBox.Text + "]";
                }
            }

            string xPathStart = "/mame/game[contains(manufacturer, '" + Manufacturers.Text + "')" + xPathBuilder + "]";

            return xPathStart;
        }

        private void ResetButton_Click_1(object sender, RoutedEventArgs e)
        {
            To.SelectedIndex = -1;
            From.SelectedIndex = -1;
            GameTitle.Text = string.Empty;
            Manufacturers.SelectedIndex = -1;
            ScreenTypeComboBox.SelectedIndex = -1;
            NumberOfPlayers.SelectedIndex = -1;
            CloneCheckbox.IsChecked = false;
            ScreenOrientationComboBox.SelectedIndex = -1;
            MinRefreshTextBox.Text = string.Empty;
            MaxRefreshTextBox.Text = string.Empty;

            updateDataProvider(false);
        }

        private static string CleanGameName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return rawName;
            }

            string slashDelimiter = " / ";
            string colonDelimiter = ": ";
            string hyphenDelimiter = " - ";
            string parenDelimiter = " (";

            // take everything before the slash, everything before the hyphen, and everything before the parentheses for the cleanest name possible
            string cleanName = rawName.Split(new string[] { slashDelimiter }, StringSplitOptions.None)[0].Split(new string[] { colonDelimiter }, StringSplitOptions.None)[0].Split(new string[] { hyphenDelimiter }, StringSplitOptions.None)[0].Split(new string[] { parenDelimiter }, StringSplitOptions.None)[0];

            return cleanName;
        }

        private void ListBox_DragLeave_1(object sender, DragEventArgs e)
        {
            MessageBox.Show(MameQueryResults.SelectedIndex.ToString());
        }

        async void QueryMameXml(bool loadFromBinary, bool forceReload, string mameLocation)
        {
            string response = await LoadMameData(loadFromBinary, forceReload, mameLocation);
        }

        private string LoadDataFromMameBinary(string mameLocation, bool forceReload)
        {
            if (doc == null || forceReload == true)
            {
                doc = new XmlDocument();

                string element = string.Empty;
                string bios = null;
                string clone = null;
                string description = null;
                string players = null;
                string year = null;
                string rotation = null;
                string manufacturer = null;
                string romName = null;
                string screen = null;
                string refresh = null;

                // create the root mame element
                XmlNode rootElement = doc.CreateNode("element", "mame", "");

                // add the root element to the document
                doc.AppendChild(rootElement);

                // reference to root document
                XmlElement root = doc.DocumentElement;

                // add the XML declaration to the beginning of the document
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.InsertBefore(xmlDeclaration, root);

                if (string.IsNullOrEmpty(mameLocation))
                {
                    //MessageBox.Show("You must choose a M.A.M.E. executable!", "Big Blue Configuration");
                }
                else
                {
                    using (Process cmd = new Process())
                    {
                        cmd.StartInfo.WorkingDirectory = BigBlueConfig.Utilities.GetDirectoryOfProgram(mameLocation);
                        cmd.StartInfo.FileName = mameLocation;
                        cmd.StartInfo.Arguments = "-listxml";
                        cmd.StartInfo.UseShellExecute = false;
                        cmd.StartInfo.CreateNoWindow = true;
                        cmd.StartInfo.RedirectStandardOutput = true;

                        cmd.Start();

                        using (XmlTextReader reader = new XmlTextReader(cmd.StandardOutput))
                        {
                            //var s = cmd.StandardOutput.ReadToEndAsync();

                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    element = reader.Name;

                                    if (reader.Name.Equals("machine"))
                                    {
                                        bios = reader.GetAttribute("isbios");

                                        if (bios == null)
                                        {
                                            romName = reader.GetAttribute("name");
                                            clone = reader.GetAttribute("cloneof");
                                        }
                                    }
                                    else if (reader.Name.Equals("input") && bios == null)
                                    {
                                        players = reader.GetAttribute("players");
                                    }
                                    else if (reader.Name.Equals("display") && bios == null && players != null)
                                    {
                                        rotation = reader.GetAttribute("rotate");
                                        screen = reader.GetAttribute("type");
                                        refresh = reader.GetAttribute("refresh");
                                    }
                                    else if (reader.Name.Equals("driver") && bios == null && players != null && screen != null)
                                    {
                                        // get the status of the mame driver
                                        string status = reader.GetAttribute("status");

                                        // only continue if the status isn't null
                                        if (status != null)
                                        {
                                            // we only care about games whose emulation is good or imperfect; preliminary drivers are a waste of time
                                            if (status == "good" || status == "imperfect")
                                            {
                                                // since the driver status is acceptable, we'll create a game element for it
                                                XmlNode gameElement = doc.CreateNode("element", "game", "");

                                                // check to see whether the ROM name is null
                                                if (romName != null)
                                                {
                                                    // it's not null, so we're going to create the ROM name attribute on the games node
                                                    XmlAttribute gameAttribute = doc.CreateAttribute("name");

                                                    // we're assigning this attribute the value that we collected earlier in the romName string
                                                    gameAttribute.Value = romName;
                                                    gameElement.Attributes.Append(gameAttribute);
                                                }

                                                if (clone != null)
                                                {
                                                    XmlAttribute cloneAttribute = doc.CreateAttribute("cloneof");
                                                    cloneAttribute.Value = clone;
                                                    gameElement.Attributes.Append(cloneAttribute);
                                                }

                                                doc.DocumentElement.AppendChild(gameElement);

                                                // not even going to bother creating the node if it doesn't have anything useful in there
                                                if (screen != null)
                                                {
                                                    // create the display node
                                                    XmlNode displayElement = doc.CreateNode("element", "display", "");

                                                    XmlAttribute screenTypeAttribute = doc.CreateAttribute("type");

                                                    screenTypeAttribute.Value = screen;

                                                    displayElement.Attributes.Append(screenTypeAttribute);

                                                    if (rotation != null)
                                                    {
                                                        // don't care about ones that aren't rotated
                                                        if (rotation != "0")
                                                        {
                                                            // create the rotate attribute
                                                            XmlAttribute rotationAttribute = doc.CreateAttribute("rotate");

                                                            // set the value of the rotate attribute
                                                            rotationAttribute.Value = rotation;

                                                            // add the rotate attribute to the display node
                                                            displayElement.Attributes.Append(rotationAttribute);
                                                        }
                                                    }

                                                    // add refresh rate
                                                    if (refresh != null)
                                                    {
                                                        XmlAttribute refreshAttribute = doc.CreateAttribute("refresh");

                                                        refreshAttribute.Value = refresh;
                                                        displayElement.Attributes.Append(refreshAttribute);
                                                    }

                                                    // add the display node to the game node
                                                    gameElement.AppendChild(displayElement);
                                                }

                                                if (players != null)
                                                {
                                                    XmlNode inputElement = doc.CreateNode("element", "input", "");

                                                    XmlAttribute playerAttribute = doc.CreateAttribute("players");

                                                    playerAttribute.Value = players;

                                                    inputElement.Attributes.Append(playerAttribute);

                                                    gameElement.AppendChild(inputElement);
                                                }

                                                XmlNode descriptionElement = doc.CreateNode("element", "description", "");
                                                descriptionElement.InnerText = description;

                                                gameElement.AppendChild(descriptionElement);

                                                XmlNode yearElement = doc.CreateNode("element", "year", "");
                                                yearElement.InnerText = year;
                                                gameElement.AppendChild(yearElement);


                                                XmlNode manufacturerElement = doc.CreateNode("element", "manufacturer", "");
                                                manufacturerElement.InnerText = manufacturer;
                                                gameElement.AppendChild(manufacturerElement);
                                            }
                                            // there's really no point in outputting a driver element when the only thing we want from it is to determine whether the driver is good
                                            // we might as well just filter out the bad driver entries from the get-go
                                            //XmlNode driverElement = doc.CreateNode("element", "driver", "");

                                            //XmlAttribute statusAttribute = doc.CreateAttribute("status");

                                            //statusAttribute.Value = status;

                                            //driverElement.Attributes.Append(statusAttribute);
                                            //gameElement.AppendChild(driverElement);
                                        }

                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.Text && bios == null)
                                {
                                    switch (element)
                                    {
                                        case "description":
                                            // get the game title value
                                            description = reader.Value;
                                            break;
                                        case "year":
                                            // get the game year value
                                            year = reader.Value;
                                            break;
                                        case "manufacturer":
                                            // get the manufacturer value
                                            manufacturer = reader.Value;
                                            break;
                                    }
                                }
                            }
                        }

                        cmd.WaitForExit();


                        //doc = new XmlDocument();
                        //doc.LoadXml(s.Result);
                        //System.IO.File.WriteAllText(@"d:\mame\mamelist.xml", s.Result);



                        //doc.Save(@"mamelist.xml");


                    }
                }
            }



            Application.Current.Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Background,
                        new Action(() => updateDataProvider(forceReload))
                    );

            return string.Empty;
        }

        private void updateDataProvider(bool saveFile)
        {
            ResultCount.Text = string.Empty;

            if (saveFile == true)
            {
                doc = SortGameInfo(doc);
                doc.Save("mamelist.xml");

                PopulateManufacturerDropdown();

                System.Windows.Input.Mouse.OverrideCursor = Cursors.Arrow;

                enableControls();

                MameLocation.Text = string.Empty;
                MessageBox.Show("Imported M.A.M.E. data!", "Big Blue Configuration");
            }

            string finalXpath = BuildXPathQuery();

            XmlDataProvider dp = (XmlDataProvider)Resources["MameData"];
            dp.XPath = finalXpath;

            dp.Document = doc;

            try
            {
                MameQueryResults.ScrollIntoView(MameQueryResults.Items[0]);
                ResultCount.Text = MameQueryResults.Items.Count.ToString("N0") + " results";

            }
            catch (Exception)
            {
            }
        }

        private Task<string> LoadMameData(bool loadFromBinary, bool forceReload, string mameLocation)
        {
            return Task.Run<string>(() => LoadDataFromMameBinary(mameLocation, forceReload));
        }

        private static XmlDocument SortGameInfo(XmlDocument UserInfo)
        {
            XmlDocument sortedXml = new XmlDocument();
            XslCompiledTransform proc = new XslCompiledTransform();

            //proc.Load(@"d:\mame\mamesort.xsl");
            XmlReader xsltReader = XmlReader.Create(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("BigBlueConfig.mamesort.xsl"));
            proc.Load(xsltReader);

            using (XmlWriter writer = sortedXml.CreateNavigator().AppendChild())
            {
                proc.Transform(UserInfo, writer);
            }

            xsltReader.Dispose();

            return sortedXml;
        }

        private void QueryButton_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (MameQueryResults.SelectedIndex != -1)
            {

                DragDrop.DoDragDrop(MameQueryResults, MameQueryResults.SelectedItem, DragDropEffects.Move);

            }
        }

        #region GetDataFromListBox(ListBox,Point)
        private static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }

        #endregion

        private void AddGameFromMame()
        {
            try
            {
                // we're not going to do anything if these conditions aren't met
                if (EmulatorList.SelectedIndex > -1)
                {
                    XmlElement selectedEmulatorNode = (XmlElement)EmulatorList.SelectedItem;

                    string emulatorExe = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "exe");
                    string emulatorDir = BigBlueConfig.Utilities.GetNodeValue(selectedEmulatorNode, "dir");
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

                    // you have to sort the items by index, because WPF will automatically sort the items based on the order they were selected
                    IOrderedEnumerable<XmlElement> sortedSelectedMameItems = (from XmlElement item in MameQueryResults.SelectedItems orderby MameQueryResults.Items.IndexOf(item) select item);

                    int count = 0;

                    //XDocument derp = XDocument.Load("");

                    //derp.Element("games").Add()

                    //XmlNode weow = glDoc.DocumentElement.roo

                    string currentXmlString = string.Empty;

                    using (var stringWriter = new StringWriter())
                    {
                        using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                        {
                            ((App)Application.Current).listDocument.WriteTo(xmlTextWriter);
                            xmlTextWriter.Flush();
                            currentXmlString = stringWriter.GetStringBuilder().ToString();
                        }
                    }

                    //XDocument tempListDocument = XDocument.Load(glFileName);

                    XDocument tempListDocument = XDocument.Parse(currentXmlString);

                    List<XElement> gamesToAdd = new List<XElement>();

                    foreach (XmlElement data in sortedSelectedMameItems)
                    {
                        count = count + 1;
                        string romName = data.GetAttribute("name");
                        string gameTitle = data.SelectSingleNode("description").InnerText;

                        gamesToAdd.Add(bulkAddGame(romName, gameTitle, emulatorDir, emulatorExe, emulatorArgs, emulatorThumbnail, emulatorVideo, emulatorMarquee, emulatorFlyer, emulatorInstructions, emulatorPreDir, emulatorPreExe, emulatorPreArgs, emulatorPostDir, emulatorPostExe, emulatorPostArgs, kill, false));

                    }


                    tempListDocument.XPathSelectElement(listXPath).Add(gamesToAdd);

                    //tempListDocument.Descendants("games").FirstOrDefault().Add(gamesToAdd);

                    tempListDocument.Save(glFileName);


                    XmlDocument wow = new XmlDocument();
                    wow.Load(glFileName);

                    ((App)Application.Current).listDocument = wow;

                    BigBlueConfig.Utilities.RefreshListDataForAllWindows();

                    /*
                    XmlDataProvider weow = Application.Current.Resources["Data"] as XmlDataProvider;
                    weow.Document = wow;
                    weow.Refresh();
                    */


                    /*
                    var parent = VisualTreeHelper.GetParent(this);
                    while (!(parent is Page))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
(parent as Page).DoStuff();
*/



                    //glDoc.Descendants("games").FirstOrDefault().Add(gamesToAdd);

                    // after going through each selected item
                    //glDoc.Save(glFileName);

                    //glDoc.Save(glFileName);

                    //EditGameList eglWindow = this.Owner as EditGameList;


                    //glDoc.SelectSingleNode(parentProvider.XPath).a

                    //ObjectDataProvider odp = GamesList.DataContext as ObjectDataProvider;

                    //GamesList.ItemsSource = glDoc.Element("games").Elements();

                    //glDoc.InnerXml = tempListDocument.ToString();

                    //glDoc = xmlDoc;


                    //eglWindow.GamesList.ItemsSource = xmlDoc.GetElementsByTagName("game");

                    //eglWindow.GamesList.ItemsSource = eglWindow.elemList;

                    //XmlDataProvider dp = this.Resources["Data"] as XmlDataProvider;
                    //dp.Document = xmlDoc;
                    //dp.Refresh();

                    //XmlDataProvider parentWindowDataProvider = eglWindow.Resources["Data"] as XmlDataProvider;
                    //parentWindowDataProvider.Document = xmlDoc;
                    //parentWindowDataProvider.Refresh();

                    MessageBox.Show(count.ToString("N0") + " Games added! Make sure you add some snapshots for them and give them proper titles!", "Big Blue Configuration");

                    // reset this thing so people don't bungle it out?
                    //BulkAddToggle.IsChecked = false;
                    //EmulatorList.Visibility = System.Windows.Visibility.Hidden;
                    //EmulatorList.SelectedIndex = -1;

                    //MameQueryResults.SelectedItems.RemoveAt()
                    //MameQueryResults.SelectedIndex = -1;


                }
                else
                {
                    MessageBox.Show("You must select a M.A.M.E. template for the ROM(s) that you're going to add.", "Big Blue Configuration");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't add M.A.M.E. games: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void ListBox_Drop_1(object sender, DragEventArgs e)
        {
            AddGameFromMame();
        }

        private XElement bulkAddGame(string fileNameWithoutExtension, string gameTitle, string dir, string exe, string args, string thumb, string video, string marquee, string flyer, string instruct, string preDir, string preExe, string preArgs, string postDir, string postExe, string postArgs, bool kill, bool fullPath)
        {
            //var games = glDoc.Descendants("games").FirstOrDefault();

            XElement gameElement = new XElement("item");

            if (kill)
            {
                XAttribute killAttribute = new XAttribute("kill", "True");
                gameElement.Add(killAttribute);
            }

            string formattedGameTitle = gameTitle;

            if (CleanGameNameCheckbox.IsChecked == true)
            {
                formattedGameTitle = CleanGameName(gameTitle);
            }

            if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                XElement gameTitleElement = new XElement("name", formattedGameTitle);
                gameElement.Add(gameTitleElement);

                if (!string.IsNullOrWhiteSpace(thumb))
                {
                    XElement thumbnailElement = new XElement("snap", BigBlueConfig.Utilities.GenerateBulkMediaValue(thumb, fileNameWithoutExtension));
                    gameElement.Add(thumbnailElement);
                }

                if (!string.IsNullOrWhiteSpace(video))
                {
                    XElement videoElement = new XElement("video", BigBlueConfig.Utilities.GenerateBulkMediaValue(video, fileNameWithoutExtension));
                    gameElement.Add(videoElement);
                }

                if (!string.IsNullOrWhiteSpace(marquee))
                {
                    XElement marqueeElement = new XElement("marquee", BigBlueConfig.Utilities.GenerateBulkMediaValue(marquee, fileNameWithoutExtension));
                    gameElement.Add(marqueeElement);
                }

                if (!string.IsNullOrWhiteSpace(flyer))
                {
                    XElement flyerElement = new XElement("flyer", BigBlueConfig.Utilities.GenerateBulkMediaValue(flyer, fileNameWithoutExtension));
                    gameElement.Add(flyerElement);
                }

                if (!string.IsNullOrWhiteSpace(instruct))
                {
                    XElement instructElement = new XElement("instruct", BigBlueConfig.Utilities.GenerateBulkMediaValue(instruct, fileNameWithoutExtension));
                    gameElement.Add(instructElement);
                }

                if (!string.IsNullOrWhiteSpace(args))
                {
                    XElement argsElement = new XElement("args", BigBlueConfig.Utilities.GenerateBulkArgs(string.Empty, args, fileNameWithoutExtension, fullPath));
                    gameElement.Add(argsElement);
                }

                if (!string.IsNullOrWhiteSpace(preArgs))
                {
                    XElement preArgsElement = new XElement("preargs", BigBlueConfig.Utilities.GenerateBulkArgs(string.Empty, preArgs, fileNameWithoutExtension, fullPath));
                    gameElement.Add(preArgsElement);
                }

                if (!string.IsNullOrWhiteSpace(postArgs))
                {
                    XElement postArgsElement = new XElement("postargs", BigBlueConfig.Utilities.GenerateBulkArgs(string.Empty, postArgs, fileNameWithoutExtension, fullPath));
                    gameElement.Add(postArgsElement);
                }
            }

            if (!string.IsNullOrWhiteSpace(dir))
            {
                XElement gameDirElement = new XElement("dir", dir);
                gameElement.Add(gameDirElement);
            }

            if (!string.IsNullOrWhiteSpace(exe))
            {
                XElement gameExeElement = new XElement("exe", exe);
                gameElement.Add(gameExeElement);
            }

            if (!string.IsNullOrWhiteSpace(preDir))
            {
                XElement preDirElement = new XElement("predir", preDir);
                gameElement.Add(preDirElement);
            }

            if (!string.IsNullOrWhiteSpace(preExe))
            {
                XElement preExeElement = new XElement("preexe", preExe);
                gameElement.Add(preExeElement);
            }

            if (!string.IsNullOrWhiteSpace(postDir))
            {
                XElement postDirElement = new XElement("postdir", postDir);
                gameElement.Add(postDirElement);
            }

            if (!string.IsNullOrWhiteSpace(postExe))
            {
                XElement postExeElement = new XElement("postexe", postExe);
                gameElement.Add(postExeElement);
            }

            return gameElement;
        }



        private void MameQueryResults_DragLeave_1(object sender, DragEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            //dragSource = parent;
            //object data = GetDataFromListBox(dragSource, e.GetPosition(parent));

            if (MameQueryResults.SelectedIndex != -1)
            {

                //if (data != null)
                //{
                DragDrop.DoDragDrop(parent, MameQueryResults.SelectedItem, DragDropEffects.Move);
                //}
            }
        }

        private void MameQueryResults_PreviewDragLeave_1(object sender, DragEventArgs e)
        {
            MessageBox.Show("anything?");
        }

        private void SelectMameButton_Click(object sender, RoutedEventArgs e)
        {
            MameLocation.Text = BigBlueConfig.Utilities.GetFileNameFromWindowDialog(".exe", "EXE files (.exe)|*.exe");
        }

        private void UpdateMameList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MameLocation.Text))
            {
                return;
            }

            System.Windows.Input.Mouse.OverrideCursor = Cursors.Wait;

            disableControls();

            QueryMameXml(true, true, MameLocation.Text);
        }

        private void Manufacturers_DropDownClosed(object sender, EventArgs e)
        {
            updateDataProvider(false);
        }

        private void GameTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateDataProvider(false);
        }

        private void ScreenTypeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            updateDataProvider(false);
        }

        private void From_DropDownClosed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(To.Text) && !string.IsNullOrEmpty(From.Text))
            {
                int toDate = Convert.ToInt32(To.Text);
                int fromDate = Convert.ToInt32(From.Text);

                if (fromDate > toDate)
                {
                    MessageBox.Show("The From date can't be later than the To date!", "Big Blue Configuration");
                    To.SelectedIndex = From.SelectedIndex;
                }
            }

            updateDataProvider(false);
        }

        private void To_DropDownClosed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(To.Text) && !string.IsNullOrEmpty(From.Text))
            {
                int toDate = Convert.ToInt32(To.Text);
                int fromDate = Convert.ToInt32(From.Text);

                if (fromDate > toDate)
                {
                    MessageBox.Show("The To date can't be earlier than the From date!", "Big Blue Configuration");
                    From.SelectedIndex = To.SelectedIndex;
                }
            }

            updateDataProvider(false);
        }

        private void NumberOfPlayers_DropDownClosed(object sender, EventArgs e)
        {
            updateDataProvider(false);
        }

        private void CloneCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            updateDataProvider(false);
        }

        private void CloneCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            updateDataProvider(false);
        }

        private void ScreenOrientationComboBox_DropDownClosed(object sender, EventArgs e)
        {
            updateDataProvider(false);
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                AddGameFromMame();
            }
        }

        private void TargetGameList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                BigBlueConfig.Utilities.DeleteFromGameList(this, GamesList, glFileName);
            }
        }

        private void RefreshTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            updateDataProvider(false);

        }
    }
}
