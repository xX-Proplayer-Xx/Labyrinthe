
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Labyrinthe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private int vitesseAnnimation = 1;
        private bool goDroite, goGauche, goHaut, goBas,claque;
        private DispatcherTimer minuterie;
        private int vitesse = 10;
        public MainWindow()
        {
            
            InitializeComponent();
            InitMinuterie();
            //AnnimationPerso();
        }
        private void InitMinuterie()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval += TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;
            minuterie.Start();
        }

        private void Jeu(object? sender, EventArgs e)
        {
            Deplacement();
        }
        private void Deplacement()
        {
            if (goDroite == true /*&&*/)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Droite");
                Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) + vitesse);
            }
            if (goGauche == true)
            {

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Gauche");
                Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) - vitesse);
            }
            if (goHaut == true)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Haut");
                Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) - vitesse);
            }
            if (goBas == true)
            {

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Bas");
                Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) + vitesse);
            }
        }
        private void Attaque()
        {
            if (claque == true)
            {
                 //Faire en sorte que la claque soit dans la direction que le deplacement 
            }
        }
        private void Collision()
        {

        } 
        //private void AnnimationPerso()
        //{
        //    vitesseAnnimation += 1;
        //    ImageBrush joueur = new ImageBrush();
        //    joueur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/LUTTIN/LUTTIN-" + vitesseAnnimation + ".png"));
        //    Joueur.Fill = joueur;
        //    if (vitesseAnnimation == 8)
        //    {
        //        vitesseAnnimation = 1;
        //    }
        //}





        private void Joueur_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                claque = true;
            }
            if (e.Key == Key.Z)
            {
                goHaut = true;
            }
            if (e.Key == Key.S)
            {
                goBas = true;
            }
            if (e.Key == Key.Q)
            {
                goGauche = true;
            }
            if (e.Key == Key.D)
            {
                goDroite = true;
            }
        }

        private void Joueur_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                claque = false;
            }
            if (e.Key == Key.Z)
            {
                goHaut = false;
            }
            if (e.Key == Key.S)
            {
                goBas = false;
            }
            if (e.Key == Key.Q)
            {
                goGauche = false;
            }
            if (e.Key == Key.D)
            {
                goDroite = false;
            }
        }
    }
}