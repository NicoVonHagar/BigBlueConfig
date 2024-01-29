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
    /// Interaction logic for ListTree.xaml
    /// </summary>
    public partial class ListTree : Window
    {
        XmlDataProvider xdp;

        public ListTree()
        {
            InitializeComponent();

            this.Title = "Select Default List";

            xdp = (XmlDataProvider)Resources["Data"];
            xdp.Document = ((App)Application.Current).listDocument;

            xdp.XPath = "//list/item";
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

                        //((App)Application.Current).listDocument.Save("lists.xml");

                        //((App)Application.Current).listDocument = newXmlDocument;

                        //xdp.Document = ((App)Application.Current).listDocument;
                    }

                    ((App)Application.Current).listDocument.Save("lists.xml");


                    //xdp.Refresh();

                }



                e.Handled = true;
            }
        }

        private void RemoveDefault_Click(object sender, RoutedEventArgs e)
        {
            foreach (XmlNode el in ((App)Application.Current).listDocument.SelectNodes(".//*"))
            {
                XmlAttribute a = el.Attributes["default"];

                if (a != null)
                {
                    el.Attributes.Remove(a);
                }
            }
        }
    }
}
