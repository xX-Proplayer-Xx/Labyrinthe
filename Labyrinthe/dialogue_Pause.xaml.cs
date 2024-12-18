using System.Windows;

namespace Labyrinthe
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class dialogue_Pause : Window
    {
        public dialogue_Pause()
        {
            InitializeComponent();
        }

        private void butQuitt_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Application.Current.Shutdown();
        }

        private void buttRepre_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void buttOption_pause_Click(object sender, RoutedEventArgs e)
        {
            Menu_Options menuOptions = new Menu_Options();
        }
    }
}
