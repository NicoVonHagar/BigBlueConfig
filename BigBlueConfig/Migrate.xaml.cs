using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace BigBlueConfig
{
    /// <summary>
    /// Interaction logic for Migrate.xaml
    /// </summary>
    public partial class Migrate : Window
    {
        public Migrate()
        {
            InitializeComponent();

            this.PreviewKeyUp += HandlePress;
            Closing += Migrate_Closing;
        }

        private void HandlePress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        void Migrate_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void SourceDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog(this.GetIWin32Window());

            SourcePath.Text = dlg.SelectedPath + @"\";
        }

        private void TargetDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog(this.GetIWin32Window());

            TargetPath.Text = dlg.SelectedPath + @"\";
        }

        private void replaceValueOfElement(XmlDocument document, string xpath)
        {
            XmlNodeList nodes = document.SelectNodes(xpath);

            foreach (XmlNode node in nodes)
            {
                if (!string.IsNullOrEmpty(node.InnerText))
                {
                    node.InnerText = node.InnerText.ToLower().Replace(SourcePath.Text.ToLower(), TargetPath.Text.ToLower());
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (XmlNodeToMigrate.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("You must select a value to migrate!");
                return;
            }

            if (string.IsNullOrEmpty(SourcePath.Text) == true)
            {
                System.Windows.MessageBox.Show("You must select an original directory!");
                return;
            }

            string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            string valueToMigrate = XmlNodeToMigrate.Text;

            foreach (String file in Directory.GetFiles(currentDirectory))
            {
                if (System.IO.Path.GetExtension(file) == ".xml")
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(file);

                    if (valueToMigrate == "Migrate all values")
                    {
                        replaceValueOfElement(document, "//dir");

                        replaceValueOfElement(document, "//exe");

                        replaceValueOfElement(document, "//thumbnail");

                        replaceValueOfElement(document, "//args");

                        replaceValueOfElement(document, "//preexe");

                        replaceValueOfElement(document, "//postexe");

                        replaceValueOfElement(document, "//predir");

                        replaceValueOfElement(document, "//postdir");

                        replaceValueOfElement(document, "//preargs");

                        replaceValueOfElement(document, "//postargs");

                        replaceValueOfElement(document, "//video");

                        replaceValueOfElement(document, "//marquuee");
                    }
                    else
                    {
                        replaceValueOfElement(document, "//" + valueToMigrate);
                    }

                    document.Save(file);
                }
            }

            System.Windows.MessageBox.Show("Migrated " + valueToMigrate + " from " + SourcePath.Text + " to " + TargetPath.Text + "!", "Big Blue Configuration");
        }
    }
}
