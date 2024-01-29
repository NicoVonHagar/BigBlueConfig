using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using static BigBlueConfig.Models;

namespace BigBlueConfig
{
    public static class Utilities
    {
        
        public static string GetVendorId(string rawName)
        {
            string vId = string.Empty;

            try
            {
                string[] delimiter1 = new string[] { "#VID_" };
                string[] delimiter3 = new string[] { "&" };
                string split1 = rawName.Split(delimiter1, StringSplitOptions.None)[1];
                vId = split1.Split(delimiter3, StringSplitOptions.None)[0].ToLower();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't read Vendor ID: " + ex.Message);
            }

            return vId;
        }

        public static string GetProductId(string rawName)
        {
            string pId = "0";

            try
            {
                string[] delimiter2 = new string[] { "&PID_" };
                string[] delimiter3 = new string[] { "&" };
                string split3 = rawName.Split(delimiter2, StringSplitOptions.None)[1];
                pId = split3.Split(delimiter3, StringSplitOptions.None)[0].ToLower();

                if (pId.Contains('#'))
                {
                    string[] delimiter4 = new string[] { "#" };
                    pId = pId.Split(delimiter4, StringSplitOptions.None)[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't read Product ID: " + ex.Message);
            }

            return pId;
        }

        public static bool IsListItemAFolder(XmlAttribute folderAttribute)
        {
            if (folderAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(folderAttribute.InnerText))
                {
                    return true;
                }
            }

            return false;
        }

        public static void PopulateFontComboBoxItems(System.Windows.Controls.ComboBox cb)
        {
            ICollection<System.Windows.Media.FontFamily> fonts = System.Windows.Media.Fonts.SystemFontFamilies;

            foreach (System.Windows.Media.FontFamily ff in fonts)
            {
                ComboBoxItem fcb = new()
                {
                    Content = ff.ToString()
                };

                cb.Items.Add(fcb);
            }
        }

        public static void SelectFontComboBoxChoice(System.Windows.Controls.ComboBox cb, XmlNode optionNode)
        {
            if (optionNode != null)
            {
                if (!string.IsNullOrWhiteSpace(optionNode.InnerText))
                {
                    foreach (ComboBoxItem cbi in cb.Items)
                    {
                        if (optionNode.InnerText == (string)cbi.Content)
                        {
                            cb.SelectedItem = cbi;
                            return;
                        }
                    }
                }
            }
        }

        public static string FindXPath(XmlNode node)
        {
            StringBuilder builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        int index = FindElementIndex((XmlElement)node);
                        builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new ArgumentException("Node was not in a document");
        }

        public static int FindElementIndex(XmlElement element)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            throw new ArgumentException("Couldn't find element within parent");
        }

        public static System.Windows.Forms.IWin32Window GetIWin32Window(this System.Windows.Media.Visual visual)
        {
            var source = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }

        public static string GenerateBulkMediaValue(string format, string value)
        {
            if (string.IsNullOrWhiteSpace(value) == true)
            {
                return string.Empty;
            }

            return string.Format(format, value);
        }

        public static string GenerateBulkArgs(string path, string args, string fileNameWithoutExtension, bool fullPath)
        {
            if (string.IsNullOrEmpty(args) == true)
            {
                return string.Empty;
            }

            string finalArg = string.Empty;
            //string spacer = string.Empty;

            if (fullPath == true)
            {
                finalArg = string.Format(args, path);
            }
            else
            {
                finalArg = string.Format(args, fileNameWithoutExtension);
            }

            return finalArg;
        }

        public static string ValidatePath(string input)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(input, "");
        }

        public static string GetDirectoryOfProgram(string input)
        {
            if (string.IsNullOrWhiteSpace(input) == true)
            {
                return string.Empty;
            }
            else
            {
                if (input.Contains('\\') == true)
                {
                    int lastSlashIndex = input.LastIndexOf(@"\");
                    return input.Substring(0, lastSlashIndex);
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        public static string GetExeOfProgram(string input)
        {
            if (string.IsNullOrWhiteSpace(input) == true)
            {
                return string.Empty;
            }
            else
            {
                if (input.Contains('\\') == true)
                {
                    int lastSlashIndex = input.LastIndexOf(@"\");
                    string preDir = input.Substring(0, lastSlashIndex);

                    return input.Substring(lastSlashIndex + 1);
                }
                else
                {
                    return string.Empty;
                }
            }
        }


        public static void DeleteFromGameList(System.Windows.Window currentWindow, System.Windows.Controls.ListBox listBox, string xPath)
        {
            CloseChildWindows(currentWindow);

            System.Collections.IList selectedListBoxItems = listBox.SelectedItems;
            ItemCollection listBoxItems = listBox.Items;

            // get the total number of selected items in the list
            int totalSelectedItems = selectedListBoxItems.Count;

            if (totalSelectedItems == 0)
            {
                return;
            }

            // get the total number of items in the list
            int totalItems = listBoxItems.Count;

            // what this should do is sort the games that are selected and then put the focus back on the index before the first one that was selected
            //IOrderedEnumerable<XmlElement> sortedSelectedMameItems = (from XmlElement item in selectedListBoxItems orderby listBoxItems.IndexOf(item) select item);

            //var items = selectedListBoxItems.Cast<object>().Select((item, index) => new { item, index }).Select(ix => ix.index);

            //xPath = xPath + "[" + (listBox.SelectedIndex + 1).ToString() + "]";



            XDocument tempXml = XDocument.Load("lists.xml");

            if (totalItems == totalSelectedItems)
            {
                tempXml.XPathSelectElements(xPath).Remove();
            }
            else
            {
                IEnumerable<XElement> games = tempXml.XPathSelectElements(xPath);

                IList<XElement> toRemove = new List<XElement>(totalSelectedItems);

                foreach (object game in selectedListBoxItems)
                {
                    int indexInList = listBoxItems.IndexOf(game);
                    toRemove.Add(games.ElementAt(indexInList));
                }

                foreach (XElement game in toRemove)
                {
                    game.Remove();
                }
            }




            /*
            
            foreach (object game in sortedSelectedMameItems)
            {
                int indexInList = listBoxItems.IndexOf(game);
                toRemove.Add(games.ElementAt(indexInList));
            }
            */

            /*
        foreach (int i in items)
        {
            MessageBox.Show(i.ToString());
            toRemove.Add(games.ElementAt(i));
        }
        */



            tempXml.Save("lists.xml");


            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(tempXml.ToString());

            ((App)System.Windows.Application.Current).listDocument = xmlDoc;



            RefreshListDataForAllWindows();

            listBox.SelectedItem = -1;

            // the new index will always be the selectedgame's index - 1;
            // if it's less than 0, just set it to 0

            /*
            int newItemToSelect = toRemove[0] - 1;

            if (newItemToSelect >= 0)
            {
                ListBoxItem item = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(newItemToSelect);
                item.Focus();
            }
            

            // if we didn't delete every item in the list, we're going to focus and select another one and scroll to it in the list
            if (totalItems > totalSelectedItems)
            {
                listBox.SelectedItem = newNodeToSelect;
                listBox.ScrollIntoView(newNodeToSelect);  
            }
            */
        }

        /*
        public static void CenterWindow(System.Windows.Window win)
        {
            // get the screen resolution
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;

            // get a third way of the way from the left and top
            double horizontalCenter = width / 2;
            double verticalCenter = height / 2;

            // get the dimensions of the current window
            double menuWidth = win.Width;
            double menuHeight = win.Height;

            // get the center offset of the current window
            double windowHorizontalCenter = menuWidth / 2;
            double windowVerticalCenter = menuHeight / 2;

            // define the coordinates of the window
            double leftPosition = horizontalCenter - windowHorizontalCenter;
            double topPosition = verticalCenter - windowVerticalCenter;

            //MessageBox.Show("resolution is: " + width.ToString() + "x" + height.ToString() + "; left: " + leftPosition.ToString() + " top: " + topPosition.ToString());

            // set the window position
            win.Left = leftPosition;
            win.Top = topPosition;
        }
        */

        public static string GetFileNameFromWindowDialog(string defaultExtension, string filter)
        {
            string file = string.Empty;

            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                // Set filter for file extension and default file extension
                DefaultExt = defaultExtension,
                Filter = filter
            };

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                file = dlg.FileName;
            }

            return file;
        }

        public static void oldAndBusted()
        {
            //int index = listBox.SelectedIndex;
            //int newIndex = index + 1;

            //if (index > -1)
            //{
            /*
            XmlNode el = elemList[index];

            XmlNode elClone = el.Clone();

            if (el.NextSibling != null)
            {
                XmlNode movedItem = el.ParentNode.InsertAfter(elClone, el.NextSibling);

                el.ParentNode.RemoveChild(el);

                document.Save(fileName);

                listBox.SelectedItem = movedItem;
                listBox.ScrollIntoView(movedItem);
            }
             */

            /*
            int newIndex = 0;

            int indexOfItemToSortDownm = listBox.Items.IndexOf(listBox.SelectedItems[0]);

                

            XmlElement lastSelectedItem = (XmlElement)listBox.SelectedItems[totalSelectedItems];

            int indexOfLastSelectedItem = listBox.Items.IndexOf(lastSelectedItem);

            XmlElement itemToInsertAfter = (XmlElement)listBox.Items[indexOfLastSelectedItem + 1];
             */

            //MessageBox.Show("Total Selected Items: " + totalSelectedItems.ToString());

            // determine which item you're going to insert the selected items before
            //XmlElement targetIndexItem = (XmlElement)listBox.Items[totalSelectedItems + 1];

            //int totalSelectedItems = listBox.SelectedItems.Count - 1;

            //int weow = 0;

            /*
            while (listBox.SelectedItems.Count > 0)
            {
                weow = weow + 1;
                XmlElement xmlToMoveDown = (XmlElement)listBox.SelectedItems[0];
                XmlNode clonedItem = xmlToMoveDown.Clone();

                XmlNode movedItem = xmlToMoveDown.ParentNode.InsertAfter(clonedItem, xmlToMoveDown.NextSibling);

                int indexOfMovedItem = listBox.Items.IndexOf(movedItem);

                xmlToMoveDown.ParentNode.RemoveChild(xmlToMoveDown);

                MessageBox.Show(indexOfMovedItem.ToString());

                listBox.SelectedItems.Add(listBox.Items[indexOfMovedItem]);

                    
                for (int i = weow; i <= totalSelectedItems; i++)
                {
                    listBox.SelectedItems.Add(listBox.Items[i]);
                }
                     

                //listBox.SelectedItem = movedItem;
                //listBox.ScrollIntoView(movedItem);
             */
        }

        public static void RefreshListDataForAllWindows()
        {
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window.Resources["Data"] != null)
                {
                    XmlDataProvider resource = (XmlDataProvider)window.Resources["Data"];

                    resource.Document = ((App)System.Windows.Application.Current).listDocument;
                    resource.Refresh();
                }
            }
        }

        //XmlDataProvider xdp,
        public static void SortUp(System.Windows.Window currentWindow, XmlNode currentNode, System.Windows.Controls.ListBox listBox, string xPath, string fileName)
        {
            // make a copy of the selecteditems
            IList<XmlElement> selectedItems = new List<XmlElement>();

            // all the items
            ItemCollection listBoxItemCollection = listBox.Items;

            var sortedSelectedItems = (from XmlElement item in listBox.SelectedItems
                                       orderby listBox.Items.IndexOf(item)
                                       select item
                                );

            int indexOfFirstSelectedItem = 0;
            bool intAssigned = false;

            foreach (XmlElement item in sortedSelectedItems)
            {
                if (intAssigned == false)
                {
                    indexOfFirstSelectedItem = listBoxItemCollection.IndexOf(item);
                    intAssigned = true;
                }
                selectedItems.Add(item);
            }

            // get the total selected items
            int totalSelectedItems = selectedItems.Count - 1;
            int totalItems = listBoxItemCollection.Count - 1;
            int insertionIndex = indexOfFirstSelectedItem - 1;

            // determine whether we're going to add the items to the end of the list or not
            if (insertionIndex <= 0)
            {
                insertionIndex = 0;
            }

            // loop through the selected items and delete them
            for (int i = totalSelectedItems; i >= 0; i--)
            {
                XmlElement el = (XmlElement)listBox.SelectedItems[i];

                currentNode.ParentNode.RemoveChild(el);
                //document.DocumentElement.RemoveChild(el);
            }

            // loop through the selected items and delete them
            int offset = 0;
            foreach (XmlElement item in selectedItems)
            {
                XmlElement targetIndexItem = (XmlElement)listBox.Items[insertionIndex + offset];

                //currentNode.ParentNode.InsertBefore(item, targetIndexItem);

                ((App)System.Windows.Application.Current).listDocument.SelectSingleNode(xPath).InsertBefore(item, targetIndexItem);

                //document.DocumentElement.InsertBefore(item, targetIndexItem);

                listBox.SelectedItems.Add(item);
                offset++;
            }

            ((App)System.Windows.Application.Current).listDocument.Save(fileName);

            RefreshListDataForAllWindows();

            listBox.ScrollIntoView(listBoxItemCollection[insertionIndex]);

            CloseChildWindows(currentWindow);
        }

        public static void CloseChildWindows(System.Windows.Window window)
        {
            if (window != null)
            {
                System.Windows.WindowCollection childWindows = window.OwnedWindows;

                foreach (System.Windows.Window w in childWindows)
                {
                    w.Close();
                }
            }
        }

        public static void SortDown(System.Windows.Window currentWindow, XmlDataProvider xdp, XmlNode currentNode, System.Windows.Controls.ListBox listBox, string xPath, string fileName)
        {
            // make a copy of the selecteditems
            IList<XmlElement> selectedItems = new List<XmlElement>();

            // all the items
            ItemCollection listBoxItemCollection = listBox.Items;

            //IList<XmlElement> sortedSelectedItems = from selectedItem as XmlElement in listBox.SelectedItems selec

            var sortedSelectedItems = (from XmlElement item in listBox.SelectedItems
                                       orderby listBox.Items.IndexOf(item)
                                       select item
                                );

            int indexOfLastSelectedItem = 0;

            foreach (XmlElement item in sortedSelectedItems)
            {
                indexOfLastSelectedItem = listBoxItemCollection.IndexOf(item);
                selectedItems.Add(item);
            }

            // get the total selected items
            int totalSelectedItems = selectedItems.Count - 1;

            int totalItems = listBoxItemCollection.Count - 1;
            int indexOfNextItem = indexOfLastSelectedItem + 1;
            int insertionIndex = indexOfNextItem - totalSelectedItems;

            bool addToEnd = false;

            // determine whether we're going to add the items to the end of the list or not
            if (insertionIndex >= (totalItems - totalSelectedItems))
            {
                addToEnd = true;
            }

            // loop through the selected items and delete them
            for (int i = totalSelectedItems; i >= 0; i--)
            {
                XmlElement el = (XmlElement)listBox.SelectedItems[i];
                currentNode.ParentNode.RemoveChild(el);
            }

            XmlElement targetIndexItem = null;

            if (addToEnd == false)
            {
                targetIndexItem = (XmlElement)listBox.Items[insertionIndex];
            }

            int postIndexOfLastSelectedItem = 0;

            // start adding the items before that item
            foreach (XmlElement item in selectedItems)
            {
                if (addToEnd == true)
                {
                    ((App)System.Windows.Application.Current).listDocument.SelectSingleNode(xPath).AppendChild(item);
                    //document.DocumentElement.AppendChild(item);
                }
                else
                {
                    targetIndexItem.ParentNode.InsertBefore(item, targetIndexItem);
                }

                postIndexOfLastSelectedItem = listBoxItemCollection.IndexOf(item);

                listBox.SelectedItems.Add(item);
            }

            ((App)System.Windows.Application.Current).listDocument.Save(fileName);

            RefreshListDataForAllWindows();

            listBox.ScrollIntoView(listBoxItemCollection[postIndexOfLastSelectedItem]);

            CloseChildWindows(currentWindow);
        }

        public static void AddValueToNode(XmlDocument document, XmlNode node, string nodeName, string valueToAdd)
        {
            if (!string.IsNullOrEmpty(valueToAdd))
            {
                XmlNode migratedNode = document.CreateNode(XmlNodeType.Element, nodeName, "");
                migratedNode.InnerText = valueToAdd;
                node.AppendChild(migratedNode);
            }
        }

        public static string GetAttributeValue(XmlNode node, string attributeName)
        {
            if (node.Attributes[attributeName] != null)
            {
                return node.Attributes[attributeName].Value;
            }

            return string.Empty;
        }

        public static string GetNodeValue(XmlNode node, string nodeName)
        {
            if (node[nodeName] != null)
            {
                return node[nodeName].InnerText.Trim().TrimEnd('\r', '\n');
            }

            return string.Empty;
        }

        public static void EditNodeValue(XmlDocument document, XmlNode parentNode, string nodeName, string newValue)
        {
            if (parentNode != null && !string.IsNullOrWhiteSpace(nodeName))
            {
                XmlNode nodeToEdit = parentNode.SelectSingleNode(nodeName);

                if (nodeToEdit != null)
                {
                    if (string.IsNullOrWhiteSpace(newValue))
                    {
                        // remove the node entirely if you're blanking it out
                        parentNode.RemoveChild(nodeToEdit);
                    }
                    else
                    {
                        nodeToEdit.InnerText = newValue;
                    }
                }
                else
                {
                    // don't add it if it's empty
                    if (!string.IsNullOrWhiteSpace(newValue))
                    {
                        XmlNode newNode = document.CreateNode(XmlNodeType.Element, nodeName, "");
                        newNode.InnerText = newValue;
                        parentNode.AppendChild(newNode);
                    }
                }
            }
        }
    }
}
