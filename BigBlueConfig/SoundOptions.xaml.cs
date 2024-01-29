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
    /// Interaction logic for SoundOptions.xaml
    /// </summary>
    public partial class SoundOptions : Window
    {
        XmlDocument configXmlDocument;

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

                configNode["music"].InnerText = MusicCheckBox.IsChecked.ToString();


                configNode["minigamevolume"].InnerText = GameVolumeTextBox.Text;
                configNode["videovolume"].InnerText = VideoVolumeTextBox.Text;

                configXmlDocument.Save("config.xml");

                this.Close();
                this.Owner.Activate();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't save config.xml: " + ex.Message, "Big Blue Configuration");
            }
        }

        public SoundOptions(XmlDocument configDocument)
        {
            InitializeComponent();

            configXmlDocument = configDocument;

            XmlNode configNode = configXmlDocument.SelectSingleNode("/config");

            string gameVolume = configNode["minigamevolume"].InnerText;
            GameVolumeTextBox.Text = gameVolume;

            string videoVolume = configNode["videovolume"].InnerText;
            VideoVolumeTextBox.Text = videoVolume;


            bool music = Convert.ToBoolean(configNode["music"].InnerText);
            MusicCheckBox.IsChecked = music;

            Closing += Options_Closing;

        }

        void Options_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Activate();
        }
    }
}
