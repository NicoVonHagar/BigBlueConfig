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
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        XmlDocument configXmlDocument;

        private void HandlePress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        public Options(XmlDocument configDocument)
        {
            InitializeComponent();

            try
            {
                //XmlNode configNode = configDocument.SelectSingleNode("/config");

                configXmlDocument = configDocument;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't load config.xml data: " + ex.Message, "Big Blue Configuration");
            }

            Closing += Options_Closing;
        }

        void Options_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }





        private void SoundOptionsButton_Click(object sender, RoutedEventArgs e)
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

                SoundOptions newOptionWindow = new SoundOptions(configXmlDocument);
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

        private void ControlOptionsButton_Click(object sender, RoutedEventArgs e)
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

                ControlOptions newOptionWindow = new ControlOptions(configXmlDocument);
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

        private void MultiMonitorOptionsButton_Click(object sender, RoutedEventArgs e)
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

                MultiMonitorOptions newOptionWindow = new MultiMonitorOptions(configXmlDocument);
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

        private void GeneralOptionsButton_Click(object sender, RoutedEventArgs e)
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

                DisplayOptions newOptionWindow = new DisplayOptions(configXmlDocument);
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

        private void ClassicOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            ClassicThemeOptions classicOptionsWindow = new ClassicThemeOptions(configXmlDocument);
            classicOptionsWindow.Owner = this;
            classicOptionsWindow.Show();
        }

        private void DesktopOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            DesktopDisplayOptions desktopOptionsWindow = new DesktopDisplayOptions(configXmlDocument);
            desktopOptionsWindow.Owner = this;
            desktopOptionsWindow.Show();
        }
    }
}
