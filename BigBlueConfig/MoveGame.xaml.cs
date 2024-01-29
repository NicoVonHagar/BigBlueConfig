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
    /// Interaction logic for MoveGame.xaml
    /// </summary>
    public partial class MoveGame : Window
    {
        //XmlDocument listDocument;
        List<XmlNode> originalGameNodes;
        bool moveGame = false;

        public MoveGame(string fileName, List<XmlNode> gamesToMove, bool move)
        {
            InitializeComponent();

            this.PreviewKeyUp += HandlePress;
            Closing += MoveGame_Closing;

            // listDocument = sourceGameList;

            moveGame = move;

            if (moveGame == false)
            {
                this.Title = "Copy Game";
                MoveGameButton.Content = "Copy item to selected list";
                MoveRootButton.Content = "Copy item to root list";
            }

            originalGameNodes = gamesToMove;

            XmlDataProvider xdp = (XmlDataProvider)Resources["Data"];
            xdp.Document = ((App)Application.Current).listDocument;

            xdp.XPath = "//list/item";
        }

        private void HandlePress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        void MoveGame_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }


        private void MoveGameButton_Click_1(object sender, RoutedEventArgs e)
        {
            // we need to check to make sure that the target isn't the same as the current node or a child of it

            if (GameList.SelectedItem != null)
            {
                XmlNode selectedElement = (XmlNode)GameList.SelectedItem;

                if (selectedElement != null)
                {
                    List<XmlNode> nodesToMove = null;

                    if (moveGame)
                    {
                        nodesToMove = originalGameNodes;
                    }
                    else
                    {
                        nodesToMove = new List<XmlNode>();

                        foreach (XmlNode n in originalGameNodes)
                        {
                            nodesToMove.Add(n.CloneNode(true));
                        }
                    }

                    XmlNode children = selectedElement.SelectSingleNode("list");

                    try
                    {
                        if (children != null)
                        {
                            foreach (XmlNode n in nodesToMove)
                            {
                                children.AppendChild(n);
                            }
                        }
                        else
                        {
                            foreach (XmlNode n in nodesToMove)
                            {
                                selectedElement.ParentNode.AppendChild(n);
                            }
                        }

                        ((App)Application.Current).listDocument.Save("lists.xml");

                        //listDocument.Save("lists.xml");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Big Blue Configuration");
                    }
                }

                //string xPath = BigBlueConfig.Utilities.FindXPath(children);

                //selectedElement.AppendChild(originalGameNode);

                // get the owning edit game list window
                System.Windows.WindowCollection windowsToClose = this.Owner.Owner.OwnedWindows;

                foreach (System.Windows.Window w in windowsToClose)
                {
                    if (w.Title != "Edit items in Root list")
                    {
                        w.Close();
                    }

                }
            }
            else
            {
                MessageBox.Show("You must select a list to move the game to.", "Big Blue Configuration");
            }
        }

        private void GamesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tviSender = sender as TreeViewItem;

            if (tviSender.IsSelected)
            {
                int glIndex = tviSender.TabIndex;

                XmlNode listItemElement = (XmlNode)GameList.SelectedItem;

                XmlNode children = listItemElement.SelectSingleNode("list");

                if (children != null)
                {
                    string xPath = BigBlueConfig.Utilities.FindXPath(children);

                    XmlAttribute oldDefaultAttribute = children.Attributes["default"];

                    bool removeCurrent = false;

                    if (oldDefaultAttribute != null)
                    {
                        children.Attributes.Remove(oldDefaultAttribute);
                        removeCurrent = true;
                    }

                    if (!removeCurrent)
                    {
                        foreach (XmlNode el in ((App)Application.Current).listDocument.SelectNodes(".//*"))
                        {

                            XmlAttribute a = el.Attributes["default"];

                            if (a != null)
                            {
                                el.Attributes.Remove(a);
                            }
                        }

                        /*

                        XDocument xdoc = XDocument.Load("lists.xml");

                        foreach (var node in xdoc.Descendants().Where(f => f.Attribute("default") != null))
                        {
                            node.Attribute("default").Remove();
                        }

                        string result = xdoc.ToString();

                        XmlDocument newXmlDocument = new XmlDocument();
                        newXmlDocument.LoadXml(xdoc.ToString());
                        */

                        XmlAttribute defaultAttribute = ((App)Application.Current).listDocument.CreateAttribute("default");
                        defaultAttribute.Value = "true";

                        children.Attributes.Append(defaultAttribute);
                        //newXmlDocument.SelectSingleNode(xPath).Attributes.Append(defaultAttribute);

                        ((App)Application.Current).listDocument.Save("lists.xml");

                        //((App)Application.Current).listDocument = newXmlDocument;

                        //xdp.Document = ((App)Application.Current).listDocument;
                    }

                    ((App)Application.Current).listDocument.Save("lists.xml");


                    //xdp.Refresh();

                }



                e.Handled = true;
            }
        }

        private void MoveRootButton_Click(object sender, RoutedEventArgs e)
        {
            // we need to check to make sure that the target isn't the same as the current node or a child of it
            List<XmlNode> nodesToMove = null;

            if (moveGame)
            {
                nodesToMove = originalGameNodes;
            }
            else
            {
                foreach (XmlNode n in originalGameNodes)
                {
                    nodesToMove.Add(n.CloneNode(true));
                }
            }

            foreach (XmlNode n in nodesToMove)
            {
                XmlNode firstItem = (XmlNode)GameList.Items[0];

                if (firstItem != null)
                {
                    firstItem.ParentNode.AppendChild(n);
                }
            }

            ((App)Application.Current).listDocument.Save("lists.xml");


            //listDocument.Save("lists.xml");

            if (this.Owner.Title != "Edit items in Root list")
            {
                this.Owner.Close();
            }
        }
    }
}
