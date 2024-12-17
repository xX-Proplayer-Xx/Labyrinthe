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
            //MainWindow.InitMusique();
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
