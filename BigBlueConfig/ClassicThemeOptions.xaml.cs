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
    /// Interaction logic for ClassicThemeOptions.xaml
    /// </summary>
    public partial class ClassicThemeOptions : Window
    {
        XmlDocument configXmlDocument;

        public ClassicThemeOptions(XmlDocument configDocument)
        {
            InitializeComponent();

            this.Closing += ClassicThemeOptions_Closing;

            configXmlDocument = configDocument;
        }

        private void ClassicThemeOptions_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }

        private void GameListOptionsButton_Click(object sender, RoutedEventArgs e)
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

                    if (win.Title == "Edit Program Templates")
                    {
                        MessageBox.Show("You can't open the options menu when the program templates menu is already open!", "Big Blue Configuration");
                        return;
                    }
                }

                GameListOptions newOptionWindow = new GameListOptions(configXmlDocument);
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

        private void DisplayOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            ClassicDisplayOptions cdoWindow = new ClassicDisplayOptions(configXmlDocument);
            cdoWindow.Owner = this;
            cdoWindow.Show();
        }
    }
}
