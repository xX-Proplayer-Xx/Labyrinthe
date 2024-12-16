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

namespace Labyrinthe
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Menu_Options : Window
    {
        //public Menu_Options()
        //{
        //    InitializeComponent();
        //    VolumeSlider.Value = MainWindow.musique.Volume;
        //}
        //private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if (MainWindow.musique != null)
        //    {
        //        MainWindow.musique.Volume = VolumeSlider.Value; // Ajuster le volume en temps réel
        //    }


        //}
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
