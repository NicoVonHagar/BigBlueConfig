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
    /// Interaction logic for EditEmulators.xaml
    /// </summary>
    public partial class EditEmulators : Window
    {
        XmlDocument emuDocument;

        public EditEmulators(XmlDocument emulatorsXmlDocument)
        {
            InitializeComponent();

            this.PreviewKeyUp += HandlePress;
            Closing += EditEmulators_Closing;
            XmlDataProvider p = (XmlDataProvider)Resources["Data"];
            p.Document = emulatorsXmlDocument;

            emuDocument = emulatorsXmlDocument;
        }

        private void HandlePress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        void EditEmulators_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void EmulatorsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = EmulatorsList.SelectedIndex;

            if (index > -1)
            {
                EditEmulator newEditEmulatorWindow = new EditEmulator(emuDocument, index);
                newEditEmulatorWindow.Owner = this;
                newEditEmulatorWindow.Show();
            }
        }

        private void AddEmulatorButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmulator addEmulatorWindow = new AddEmulator(EmulatorsList.SelectedIndex, emuDocument);
            addEmulatorWindow.Owner = this;
            addEmulatorWindow.Show();
        }

        private void ReorderUp_Click(object sender, RoutedEventArgs e)
        {
            int index = EmulatorsList.SelectedIndex;

            if (index > -1)
            {
                XmlNodeList elemList = emuDocument.GetElementsByTagName("program");
                XmlNode el = elemList[index];

                XmlNode elClone = el.Clone();

                if (el.PreviousSibling != null)
                {
                    XmlNode movedItem = el.ParentNode.InsertBefore(elClone, el.PreviousSibling);

                    el.ParentNode.RemoveChild(el);

                    emuDocument.Save("programs.xml");

                    EmulatorsList.SelectedItem = movedItem;
                }
            }
        }

        private void ReorderDown_Click(object sender, RoutedEventArgs e)
        {
            int index = EmulatorsList.SelectedIndex;

            if (index > -1)
            {
                XmlNodeList elemList = emuDocument.GetElementsByTagName("program");
                XmlNode el = elemList[index];

                XmlNode elClone = el.Clone();

                if (el.NextSibling != null)
                {
                    XmlNode movedItem = el.ParentNode.InsertAfter(elClone, el.NextSibling);

                    el.ParentNode.RemoveChild(el);

                    emuDocument.Save("programs.xml");

                    EmulatorsList.SelectedItem = movedItem;
                }
            }
        }
    }
}
