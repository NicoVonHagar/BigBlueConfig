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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlDocument configXmlFile;
        //XmlDocument listsXmlFile;
        XmlDocument emulatorXmlFile;

        string currentDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // verify XML configuration files
                // should make this a preprocessor directive
                //verifyConfigFile();
                verifyEmulatorFile();
                verifyListFile();

                // need some code that checks for the required XML files and creates them if they're not there
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't initialize the program: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void verifyEmulatorFile()
        {

            if (System.IO.File.Exists(currentDirectoryPath + "programs.xml"))
            {
                try
                {
                    XmlDocument emulators = new XmlDocument();
                    emulators.Load("programs.xml");

                    XmlNodeList emus = emulators.SelectNodes("/program");

                    foreach (XmlNode emu in emus)
                    {
                        string emulatorTitle = BigBlueConfig.Utilities.GetNodeValue(emu, "name");
                        string emulatorDir = BigBlueConfig.Utilities.GetNodeValue(emu, "dir");
                        string emulatorExe = BigBlueConfig.Utilities.GetNodeValue(emu, "exe");
                        string emulatorArgs = BigBlueConfig.Utilities.GetNodeValue(emu, "args");
                        string preEmulatorDir = BigBlueConfig.Utilities.GetNodeValue(emu, "predir");
                        string preEmulatorExe = BigBlueConfig.Utilities.GetNodeValue(emu, "preexe");
                        string preEmulatorArgs = BigBlueConfig.Utilities.GetNodeValue(emu, "preargs");
                        string postEmulatorDir = BigBlueConfig.Utilities.GetNodeValue(emu, "postdir");
                        string postEmulatorExe = BigBlueConfig.Utilities.GetNodeValue(emu, "postexe");
                        string postEmulatorArgs = BigBlueConfig.Utilities.GetNodeValue(emu, "postargs");

                        bool emulatorFullRomPath;

                        if (emu["fullpath"] != null)
                        {
                            bool.TryParse(emu["fullpath"].InnerText, out emulatorFullRomPath);
                        }
                    }

                    emulatorXmlFile = emulators;
                }
                catch (Exception)
                {
                    createEmulatorFile();
                }
            }
            else
            {
                createEmulatorFile();
            }
        }

        private void createEmulatorFile()
        {
            new XDocument(
                new XElement("programs")
            ).Save("programs.xml");

            emulatorXmlFile = new XmlDocument();
            emulatorXmlFile.Load("programs.xml");

            MigrateOldEmulatorTemplates();

            MessageBox.Show("Tried to migrate existing program templates!", "Big Blue Configuration");
        }

        private void MigrateOldEmulatorTemplates()
        {
            if (System.IO.File.Exists(currentDirectoryPath + "emulators.xml"))
            {
                XmlDocument emulators = new XmlDocument();
                emulators.Load("emulators.xml");

                XmlNodeList emus = emulators.SelectNodes("/emulators/emulator");

                XmlNode programsRootNode = emulatorXmlFile.SelectSingleNode("programs");

                foreach (XmlNode emu in emus)
                {
                    string emulatorTitle = BigBlueConfig.Utilities.GetNodeValue(emu, "title");
                    string emulatorDir = BigBlueConfig.Utilities.GetNodeValue(emu, "dir");
                    string emulatorExe = BigBlueConfig.Utilities.GetNodeValue(emu, "exe");
                    string emulatorArgs = BigBlueConfig.Utilities.GetNodeValue(emu, "args");
                    string preEmulatorDir = BigBlueConfig.Utilities.GetNodeValue(emu, "predir");
                    string preEmulatorExe = BigBlueConfig.Utilities.GetNodeValue(emu, "preexe");
                    string preEmulatorArgs = BigBlueConfig.Utilities.GetNodeValue(emu, "preargs");
                    string postEmulatorDir = BigBlueConfig.Utilities.GetNodeValue(emu, "postdir");
                    string postEmulatorExe = BigBlueConfig.Utilities.GetNodeValue(emu, "postexe");
                    string postEmulatorArgs = BigBlueConfig.Utilities.GetNodeValue(emu, "postargs");
                    string emulatorThumbnail = BigBlueConfig.Utilities.GetNodeValue(emu, "thumbnail");
                    string emulatorVideo = BigBlueConfig.Utilities.GetNodeValue(emu, "video");
                    string emulatorMarquee = BigBlueConfig.Utilities.GetNodeValue(emu, "marquee");
                    string emulatorFullRomPath = BigBlueConfig.Utilities.GetNodeValue(emu, "fullrompath");

                    XmlNode newProgram = emulatorXmlFile.CreateNode(XmlNodeType.Element, "program", string.Empty);

                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "name", emulatorTitle);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "dir", emulatorDir);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "exe", emulatorExe);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "args", emulatorArgs);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "predir", preEmulatorDir);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "preexe", preEmulatorExe);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "preargs", preEmulatorArgs);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "postdir", postEmulatorDir);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "postexe", postEmulatorExe);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "postargs", postEmulatorArgs);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "snap", emulatorThumbnail);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "video", emulatorVideo);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "marquee", emulatorMarquee);
                    BigBlueConfig.Utilities.AddValueToNode(emulatorXmlFile, newProgram, "fullpath", emulatorFullRomPath);

                    programsRootNode.AppendChild(newProgram);
                }

                emulatorXmlFile.Save("programs.xml");
            }
        }

        private void verifyListFile()
        {
            if (System.IO.File.Exists(currentDirectoryPath + "lists.xml"))
            {
                try
                {
                    // verify
                    XmlDocument lists = new XmlDocument();
                    lists.Load("lists.xml");

                    ((App)Application.Current).listDocument = lists;
                    //listsXmlFile = lists;
                }
                catch (Exception)
                {
                    createListFile();
                }
            }
            else
            {
                createListFile();
            }
        }

        private void createListFile()
        {
            // recreate empty categories.xml file
            new XDocument(
                new XElement("list")
            ).Save("lists.xml");

            //listsXmlFile = new XmlDocument();
            //listsXmlFile.Load("lists.xml");

            ((App)Application.Current).listDocument = new XmlDocument();
            ((App)Application.Current).listDocument.Load("lists.xml");

            MigrateOldData();
        }

        private void MigrateOldData()
        {
            if (System.IO.File.Exists(currentDirectoryPath + "categories.xml"))
            {
                try
                {
                    // get the categories xml file
                    XmlDocument categories = new XmlDocument();
                    categories.Load("categories.xml");

                    XmlNodeList cats = categories.SelectNodes("/categories/category");

                    foreach (XmlNode cat in cats)
                    {
                        string categoryTitle = string.Empty;

                        if (cat["title"] != null)
                        {
                            categoryTitle = cat["title"].InnerText;
                        }
                        else
                        {
                            categoryTitle = "N/A";
                        }

                        string categoryThumbnail = string.Empty;

                        if (cat["thumbnail"] != null)
                        {
                            categoryThumbnail = cat["thumbnail"].InnerText;
                        }

                        string categoryVideo = string.Empty;

                        if (cat["video"] != null)
                        {
                            categoryVideo = cat["video"].InnerText;
                        }

                        string categoryMarquee = string.Empty;

                        if (cat["marquee"] != null)
                        {
                            categoryMarquee = cat["marquee"].InnerText;
                        }

                        string categoryFlyer = string.Empty;

                        if (cat["flyer"] != null)
                        {
                            categoryFlyer = cat["flyer"].InnerText;
                        }

                        string categoryInstruction = string.Empty;

                        if (cat["instruction"] != null)
                        {
                            categoryInstruction = cat["instruction"].InnerText;
                        }

                        XmlNode migratedListItem = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "item", "");

                        BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedListItem, "name", categoryTitle);
                        BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedListItem, "snap", categoryThumbnail);
                        BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedListItem, "video", categoryVideo);
                        BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedListItem, "marquee", categoryMarquee);
                        BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedListItem, "flyer", categoryFlyer);
                        BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedListItem, "instruct", categoryInstruction);

                        XmlNode migratedGameList = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "list", "");

                        string categoryFile = cat["list"].InnerText;

                        XmlDocument oldGameList = new XmlDocument();

                        oldGameList.Load(categoryFile);

                        XmlNodeList games = oldGameList.SelectNodes("/games/game");

                        foreach (XmlNode g in games)
                        {
                            string gameTitle = string.Empty;

                            if (g["title"] != null)
                            {
                                gameTitle = g["title"].InnerText;
                            }
                            else
                            {
                                gameTitle = "N/A";
                            }

                            string gameDir = BigBlueConfig.Utilities.GetNodeValue(g, "dir");
                            string gameExe = BigBlueConfig.Utilities.GetNodeValue(g, "exe");
                            string gameThumbnail = BigBlueConfig.Utilities.GetNodeValue(g, "thumbnail");
                            string gameArgs = BigBlueConfig.Utilities.GetNodeValue(g, "args");
                            string gamePreExe = BigBlueConfig.Utilities.GetNodeValue(g, "preexe");
                            string gamePreDir = BigBlueConfig.Utilities.GetNodeValue(g, "predir");
                            string gamePreArgs = BigBlueConfig.Utilities.GetNodeValue(g, "preargs");
                            string gamePostExe = BigBlueConfig.Utilities.GetNodeValue(g, "postexe");
                            string gamePostDir = BigBlueConfig.Utilities.GetNodeValue(g, "postdir");
                            string gamePostArgs = BigBlueConfig.Utilities.GetNodeValue(g, "postargs");
                            string gameVideo = BigBlueConfig.Utilities.GetNodeValue(g, "video");
                            string gameMarquee = BigBlueConfig.Utilities.GetNodeValue(g, "marquee");
                            string gameFlyer = BigBlueConfig.Utilities.GetNodeValue(g, "flyer");
                            string gameInstruction = BigBlueConfig.Utilities.GetNodeValue(g, "instruction");

                            bool gameKill = false;

                            if (g.Attributes["kill"] != null)
                            {
                                gameKill = true;
                            }

                            bool autoStart = false;

                            if (g.Attributes["autorun"] != null)
                            {
                                autoStart = true;
                            }

                            XmlNode migratedGame = ((App)Application.Current).listDocument.CreateNode(XmlNodeType.Element, "item", "");

                            if (gameKill)
                            {
                                XmlAttribute killAttribute = ((App)Application.Current).listDocument.CreateAttribute("kill");
                                killAttribute.InnerText = "true";
                                migratedGame.Attributes.Append(killAttribute);
                            }

                            if (autoStart)
                            {
                                XmlAttribute autoStartAttribute = ((App)Application.Current).listDocument.CreateAttribute("autorun");
                                autoStartAttribute.InnerText = "true";
                                migratedGame.Attributes.Append(autoStartAttribute);
                            }

                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "name", gameTitle);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "dir", gameDir);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "exe", gameExe);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "snap", gameThumbnail);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "args", gameArgs);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "preexe", gamePreExe);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "predir", gamePreDir);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "preargs", gamePreArgs);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "postexe", gamePostExe);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "postdir", gamePostDir);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "postargs", gamePostArgs);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "video", gameVideo);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "marquee", gameMarquee);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "flyer", gameFlyer);
                            BigBlueConfig.Utilities.AddValueToNode(((App)System.Windows.Application.Current).listDocument, migratedGame, "instruct", gameInstruction);

                            // add this game to the list
                            migratedGameList.AppendChild(migratedGame);
                        }

                        migratedListItem.AppendChild(migratedGameList);

                        ((App)Application.Current).listDocument.SelectSingleNode("list").AppendChild(migratedListItem);
                    }

                    ((App)Application.Current).listDocument.Save("lists.xml");
                }
                catch (Exception)
                {
                }
            }
            else
            {
            }
        }

        private void createConfigFile()
        {
            // recreate the config.xml file with default settings
            new XDocument(
                new XElement("config",
                    new XElement("width", "0"),
                    new XElement("height", "0"),
                    new XElement("windowborder", "False"),
                    new XElement("rotate", "0"),
                    new XElement("music", "False"),
                    new XElement("letterbox", "False"),
                    new XElement("stretch", "False"),
                    new XElement("textscale", "False"),
                    new XElement("screensavertime", "5"),
                    new XElement("snapshotflicker", "False"),
                    new XElement("sf2art", "hf"),
                    new XElement("inputdelayonlaunch", "0"),
                    new XElement("hidemousecursor", "False"),
                    new XElement("marqueedisplay", string.Empty),
                    new XElement("flyerdisplay", ""),
                    new XElement("instructiondisplay", ""),
                    new XElement("exitoption", "True"),
                    new XElement("globalinputs", "False"),
                    new XElement("minigamevolume", "1"),
                    new XElement("videovolume", "1"),
                    new XElement("trapcursor", "false"),
                    new XElement("font", "Arial"),
                    new XElement("listalignment", "Center"),
                    new XElement("freeforall", "True"),
                    new XElement("aspectratio", "-1"),
                    new XElement("gamelistmarginx", "20"),
                    new XElement("gamelistmarginy", "20"),
                    new XElement("listitempaddingx", "20"),
                    new XElement("listitempaddingy", "20"),
                    new XElement("gamelistoverscanx", "0"),
                    new XElement("gamelistoverscany", "0"),
                    new XElement("listitemhorizontalpadding", "20"),
                    new XElement("selecteditemverticalpadding", "20"),
                    new XElement("unselecteditemverticalpadding", "40"),
                    new XElement("selectedtextsize", "100"),
                    new XElement("unselectedtextsize", "40"),
                    new XElement("surroundmonitor", "None"),
                    new XElement("surroundposition", "None"),
                    new XElement("timeofday", "-1"),
                    new XElement("cleanstretch", "False"),
                    new XElement("mainmenutextsize", "48"),
                    new XElement("mainmenupaddingx", "16"),
                    new XElement("mainmenupaddingy", "16"),
                    new XElement("returnlabel", "Return"),
                    new XElement("exitlabel", "Exit"),
                    new XElement("shutdownlabel", "Shutdown"),
                    new XElement("restartlabel", "Restart"),
                    new XElement("disablemenu", "False"),
                    new XElement("clock", "False"),
                    new XElement("stretchsnapshot", "False"),
                    new XElement("antialiastext", "True"),
                    new XElement("loopvideo", "True"),
                    new XElement("interface", "Classic"),
                    new XElement("theme", string.Empty),
                    new XElement("controls",
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "Escape"),
                            new XAttribute("action", "BIG_BLUE_EXIT")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "OemPlus"),
                            new XAttribute("action", "BIG_BLUE_VOLUME_UP")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "OemMinus"),
                            new XAttribute("action", "BIG_BLUE_VOLUME_DOWN")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "D0"),
                            new XAttribute("action", "BIG_BLUE_MUTE")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "Up"),
                            new XAttribute("action", "RAMPAGE_PREVIOUS_ITEM")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "Left"),
                            new XAttribute("action", "RAMPAGE_PREVIOUS_PAGE")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "Right"),
                            new XAttribute("action", "RAMPAGE_NEXT_PAGE")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "Down"),
                            new XAttribute("action", "RAMPAGE_NEXT_ITEM")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "LeftCtrl"),
                            new XAttribute("action", "RAMPAGE_PUNCH_LEFT")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "LeftAlt"),
                            new XAttribute("action", "RAMPAGE_PUNCH_RIGHT")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "Space"),
                            new XAttribute("action", "RAMPAGE_SPECTATE")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "Enter"),
                            new XAttribute("action", "RAMPAGE_START")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "LeftShift"),
                            new XAttribute("action", "RAMPAGE_BACK")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "R"),
                            new XAttribute("action", "RTYPE_PREVIOUS_ITEM")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "F"),
                            new XAttribute("action", "RTYPE_NEXT_ITEM")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "D"),
                            new XAttribute("action", "RTYPE_PREVIOUS_PAGE")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "G"),
                            new XAttribute("action", "RTYPE_NEXT_PAGE")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "D2"),
                            new XAttribute("action", "RTYPE_START")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "L"),
                            new XAttribute("action", "RTYPE_BACK")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "A"),
                            new XAttribute("action", "RTYPE_SHOOT")),
                        new XElement("control",
                            new XAttribute("device", "ANY_KEYBOARD"),
                            new XAttribute("input", "S"),
                            new XAttribute("action", "RTYPE_WARP")))
                )
            ).Save("config.xml");

            configXmlFile = new XmlDocument();
            configXmlFile.Load("config.xml");
        }

        bool configUpdated = false;

        private void CreateOptionNodeIfMissing(XmlNode configNode, string nodeName, string defaultValue)
        {
            if (configNode[nodeName] == null)
            {
                XmlNode gameNode = configXmlFile.CreateNode(XmlNodeType.Element, nodeName, "");
                gameNode.InnerText = defaultValue;
                configNode.AppendChild(gameNode);

                configUpdated = true;

                //configXmlFile = new XmlDocument();
            }
        }

        private void verifyConfigFile()
        {
            if (System.IO.File.Exists(currentDirectoryPath + "config.xml"))
            {
                try
                {
                    // verify
                    configXmlFile = new XmlDocument();
                    configXmlFile.Load("config.xml");

                    XmlNode configNode = configXmlFile.GetElementsByTagName("config")[0];

                    CreateOptionNodeIfMissing(configNode, "prevgame", "Up");
                    CreateOptionNodeIfMissing(configNode, "nextgame", "Down");
                    CreateOptionNodeIfMissing(configNode, "prevpage", "Left");
                    CreateOptionNodeIfMissing(configNode, "nextpage", "Right");
                    CreateOptionNodeIfMissing(configNode, "selectgame", "Return");
                    CreateOptionNodeIfMissing(configNode, "prevgame2", "R");
                    CreateOptionNodeIfMissing(configNode, "nextgame2", "F");
                    CreateOptionNodeIfMissing(configNode, "prevpage2", "D");
                    CreateOptionNodeIfMissing(configNode, "nextpage2", "G");
                    CreateOptionNodeIfMissing(configNode, "selectgame2", "W");
                    CreateOptionNodeIfMissing(configNode, "punch", "Space");
                    CreateOptionNodeIfMissing(configNode, "exit", "Escape");
                    CreateOptionNodeIfMissing(configNode, "george", "LeftCtrl");
                    CreateOptionNodeIfMissing(configNode, "george2", "LeftAlt");
                    CreateOptionNodeIfMissing(configNode, "rtype", "A");
                    CreateOptionNodeIfMissing(configNode, "rtypewarp", "S");
                    CreateOptionNodeIfMissing(configNode, "backkey", "LeftShift");
                    CreateOptionNodeIfMissing(configNode, "backkey2", "L");
                    CreateOptionNodeIfMissing(configNode, "width", "0");
                    CreateOptionNodeIfMissing(configNode, "height", "0");
                    CreateOptionNodeIfMissing(configNode, "windowborder", "False");
                    CreateOptionNodeIfMissing(configNode, "rotate", "0");
                    CreateOptionNodeIfMissing(configNode, "music", "False");
                    CreateOptionNodeIfMissing(configNode, "stretch", "False");
                    CreateOptionNodeIfMissing(configNode, "screensavertime", "0");
                    CreateOptionNodeIfMissing(configNode, "snapshotflicker", "False");
                    CreateOptionNodeIfMissing(configNode, "keepaspect", "False");
                    CreateOptionNodeIfMissing(configNode, "sf2art", "hf");
                    CreateOptionNodeIfMissing(configNode, "inputdelayonlaunch", "False");
                    CreateOptionNodeIfMissing(configNode, "hidemousecursor", "False");
                    CreateOptionNodeIfMissing(configNode, "marqueedisplay", string.Empty);
                    CreateOptionNodeIfMissing(configNode, "flyerdisplay", string.Empty);
                    CreateOptionNodeIfMissing(configNode, "instructiondisplay", string.Empty);
                    CreateOptionNodeIfMissing(configNode, "exitoption", "True");
                    CreateOptionNodeIfMissing(configNode, "minigamevolume", "1");
                    CreateOptionNodeIfMissing(configNode, "videovolume", "1");
                    CreateOptionNodeIfMissing(configNode, "trapcursor", "False");
                    CreateOptionNodeIfMissing(configNode, "freeforall", "False");
                    CreateOptionNodeIfMissing(configNode, "listalignment", "Center");
                    CreateOptionNodeIfMissing(configNode, "font", "Arial");
                    CreateOptionNodeIfMissing(configNode, "aspectratio", "-1");
                    CreateOptionNodeIfMissing(configNode, "gamelistmarginx", "20");
                    CreateOptionNodeIfMissing(configNode, "gamelistmarginy", "20");
                    CreateOptionNodeIfMissing(configNode, "listitempaddingx", "20");
                    CreateOptionNodeIfMissing(configNode, "listitempaddingy", "20");
                    CreateOptionNodeIfMissing(configNode, "gamelistoverscanx", "0");
                    CreateOptionNodeIfMissing(configNode, "gamelistoverscany", "0");
                    CreateOptionNodeIfMissing(configNode, "listitemhorizontalpadding", "20");
                    CreateOptionNodeIfMissing(configNode, "selecteditemverticalpadding", "20");
                    CreateOptionNodeIfMissing(configNode, "unselecteditemverticalpadding", "40");
                    CreateOptionNodeIfMissing(configNode, "selectedtextsize", "100");
                    CreateOptionNodeIfMissing(configNode, "unselectedtextsize", "40");
                    CreateOptionNodeIfMissing(configNode, "surroundmonitor", "None");
                    CreateOptionNodeIfMissing(configNode, "surroundposition", "None");
                    CreateOptionNodeIfMissing(configNode, "timeofday", "-1");
                    CreateOptionNodeIfMissing(configNode, "cleanstretch", "False");
                    CreateOptionNodeIfMissing(configNode, "mainmenutextsize", "48");
                    CreateOptionNodeIfMissing(configNode, "mainmenupaddingx", "16");
                    CreateOptionNodeIfMissing(configNode, "mainmenupaddingy", "16");
                    CreateOptionNodeIfMissing(configNode, "returnlabel", "Return");
                    CreateOptionNodeIfMissing(configNode, "exitlabel", "Exit");
                    CreateOptionNodeIfMissing(configNode, "shutdownlabel", "Shutdown");
                    CreateOptionNodeIfMissing(configNode, "restartlabel", "Restart");
                    CreateOptionNodeIfMissing(configNode, "disablemenu", "False");
                    CreateOptionNodeIfMissing(configNode, "clock", "False");
                    CreateOptionNodeIfMissing(configNode, "stretchsnapshot", "False");
                    CreateOptionNodeIfMissing(configNode, "antialiastext", "True");
                    CreateOptionNodeIfMissing(configNode, "loopvideo", "True");
                    CreateOptionNodeIfMissing(configNode, "interface", "Classic");
                    CreateOptionNodeIfMissing(configNode, "theme", string.Empty);
                    CreateOptionNodeIfMissing(configNode, "title", "Big Blue Frontend");
                    CreateOptionNodeIfMissing(configNode, "desktopwidth", "800");
                    CreateOptionNodeIfMissing(configNode, "desktopheight", "600");
                    CreateOptionNodeIfMissing(configNode, "desktopbg", string.Empty);
                    CreateOptionNodeIfMissing(configNode, "desktopicon", string.Empty);
                    CreateOptionNodeIfMissing(configNode, "desktopitemfont", "Arial");
                    CreateOptionNodeIfMissing(configNode, "desktoplistfont", "Arial");
                    CreateOptionNodeIfMissing(configNode, "desktopsearchfont", "Arial");
                    CreateOptionNodeIfMissing(configNode, "desktopitemfontsize", "12");
                    CreateOptionNodeIfMissing(configNode, "desktoplistfontsize", "12");
                    CreateOptionNodeIfMissing(configNode, "desktopsearchfontsize", "12");

                    if (configUpdated == true)
                    {
                        configXmlFile.Save("config.xml");
                        configXmlFile.Load("config.xml");
                    }
                }
                catch (Exception)
                {
                    createConfigFile();
                }
            }
            else
            {
                createConfigFile();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowCollection children = this.OwnedWindows;

                foreach (Window win in children)
                {
                    if (win.Title == "Edit Controls")
                    {
                        MessageBox.Show("The controls menu is already open!", "Big Blue Configuration");
                        return;
                    }
                }

                EditMouseBindings mouseBindingsWindow = new EditMouseBindings();
                mouseBindingsWindow.ProvisionKeyboardMap();
                mouseBindingsWindow.Owner = this;
                mouseBindingsWindow.Show();
                
                // set the hooks
                mouseBindingsWindow.ProvisionWindowsHooks();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't open the controls menu: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowCollection children = this.OwnedWindows;

                foreach (Window win in children)
                {
                    if (win.Title == "Lists")
                    {
                        MessageBox.Show("The list menu is already open!", "Big Blue Configuration");
                        return;
                    }

                    if (win.Title == "Options")
                    {
                        MessageBox.Show("You can't open the game list menu when the options menu is already open!", "Big Blue Configuration");
                        return;
                    }
                }

                //List<int> listLevels = new List<int>();

                //ListTree newGameListWindow = new ListTree(listsXmlFile, emulatorXmlFile);

                //newGameListWindow.Owner = this;
                //newGameListWindow.Show();                

                //((App)Application.Current).listDocument, 
                EditGameList newEditGameListsWindow = new EditGameList(string.Empty, emulatorXmlFile, string.Empty);
                newEditGameListsWindow.Owner = this;
                newEditGameListsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't open the game list menu: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowCollection children = this.OwnedWindows;

                foreach (Window win in children)
                {
                    if (win.Title == "Options")
                    {
                        MessageBox.Show("The options menu is already open!", "Big Blue Configuration");
                        return;
                    }

                    if (win.Title == "Lists")
                    {
                        MessageBox.Show("You can't open the options menu when the list menu is already open!", "Big Blue Configuration");
                        return;
                    }
                }

                Options newOptionWindow = new Options(configXmlFile);
                newOptionWindow.Owner = this;
                //newOptionWindow.Left = location.X;
                //newOptionWindow.Top = location.Y;
                newOptionWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't open the options menu: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void EmulatorsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowCollection children = this.OwnedWindows;

                foreach (Window win in children)
                {
                    if (win.Title == "Edit Program Templates")
                    {
                        MessageBox.Show("The program templates menu is already open!", "Big Blue Configuration");
                        return;
                    }
                }

                EditEmulators editEmulatorsWindow = new EditEmulators(emulatorXmlFile);
                editEmulatorsWindow.Owner = this;
                editEmulatorsWindow.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't open the emulator menu: " + ex.Message, "Big Blue Configuration");
            }
        }

        private void MigrateButton_Click(object sender, RoutedEventArgs e)
        {
            WindowCollection children = this.OwnedWindows;

            foreach (Window win in children)
            {
                if (win.Title == "Migrate")
                {
                    MessageBox.Show("The migrate window is already open!", "Big Blue Configuration");
                    return;
                }
            }

            Migrate newMigrationWindow = new Migrate();
            newMigrationWindow.Owner = this;
            newMigrationWindow.Show();
        }

        private void LaunchBBButton_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = currentDirectory;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "BigBlue.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.Arguments = args;

            Process process = new Process();
            process.StartInfo = startInfo;

            process.Start();
            process.Dispose();
        }
    }
}
