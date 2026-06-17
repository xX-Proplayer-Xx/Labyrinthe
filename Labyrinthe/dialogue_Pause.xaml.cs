using System.Windows;

namespace Labyrinthe
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class dialogue_Pause : Window
    {
        // Mis à true si le joueur a cliqué sur "Accueil" : MainWindow le lit pour
        // rouvrir le menu d'accueil au lieu de reprendre la partie.
        public bool RetourAccueil { get; private set; }

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
            menuOptions.ShowDialog();
        }

        private void buttAccueil_Click(object sender, RoutedEventArgs e)
        {
            RetourAccueil = true;
            this.DialogResult = true;
            this.Close();
        }
    }
}
