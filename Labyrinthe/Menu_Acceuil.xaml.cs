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
    
    public partial class Menu_Acceuil : Window
    {
        //private static MediaPlayer musique;

        public Menu_Acceuil()
        {
            InitializeComponent();
            //musique = new MediaPlayer();
            //musique.Open(new Uri(""));
        }
            
        private void JouerButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Bouton_Click_Quitter(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Options(object sender, RoutedEventArgs e)
        {
            var menuOptions = new Menu_Options();
            menuOptions.ShowDialog();
        }
    }
}
