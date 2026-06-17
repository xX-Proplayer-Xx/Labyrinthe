using System.Windows;


namespace Labyrinthe
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>

    public partial class Menu_Acceuil : Window
    {
        public Menu_Acceuil()
        {
            InitializeComponent();
            MainWindow.InitMusique();
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
            Menu_Options menuOptions = new Menu_Options();
            menuOptions.ShowDialog();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Si on ferme l'accueil avec la croix (pas via "Jouer"), on quitte l'application
            if (this.DialogResult != true)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
