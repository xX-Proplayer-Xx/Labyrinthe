
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Labyrinthe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        //Musique
        public static MediaPlayer musique;

        //VITESSE
        static readonly double VITESSE = 10;
        static readonly double VITESSERALENTI = 1;

        //Intervale coordonnés cadeaux 
        static readonly int POSCADEAUX1 = 50;
        static readonly int POSCADEAUX2 = 1550;
        static readonly int POSCADEAUY1 = 50;
        static readonly int POSCADEAUY2 = 850;
        //Random coordonnés cadeaux 
        private Random rndLeft = new Random();
        private Random rndTop = new Random();
        //Score
        private int nbCadeaux = 0;
        //Rectangle 
        Rectangle gifle = new Rectangle();
        //Coups
        private bool gifleActif = false;
        private bool tempsEntreCoup = true;
        private int tempsCoup = 3;

        private int vitesseAnnimation = 1;
        private bool goDroite, goGauche, goHaut, goBas,claque;
        private DispatcherTimer minuterie;
        private double vitesse = 10;

        public MainWindow()
        {

            Menu_Acceuil acceuil = new Menu_Acceuil();
            acceuil.ShowDialog();
            InitializeComponent();
            InitMusique();
            InitMinuterie();
            
            //AnnimationPerso();
        }
        public static void InitMusique()
        {
            if (musique == null) // Vérifier que la musique n'a pas déjà été initialisée
            {
                musique = new MediaPlayer();
                musique.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Son/musique.mp3"));
                musique.MediaEnded += RelanceMusique;
                musique.Volume = 0.5;
                musique.Play();
            }
        }
        public static void RelanceMusique(object? sender, EventArgs e)
        {
            musique.Position = TimeSpan.Zero;
            musique.Play();
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
            Collision();
            CollisionCadeaux();
            CoupAttaque();
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
            gifle = new Rectangle
            {
                Tag = "calque",
                Height = 64,
                Width = 64,


            };
            //ImageBrush attaque = new ImageBrush();
            //attaque.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Image/Attaque.png"));
            gifle.Fill = Brushes.Red;
            Canvas.SetTop(gifle, Canvas.GetTop(Joueur) - 16);
            Canvas.SetLeft(gifle, Canvas.GetLeft(Joueur) - 16);
            fondJeu.Children.Add(gifle);
            gifleActif = true;
            tempsCoup = 4;
            tempsEntreCoup = false;
        }
        private void CoupAttaque()
        {
            if (gifleActif)
            {
                Rect maxiGifle = new Rect(Canvas.GetLeft(this.gifle), Canvas.GetTop(this.gifle), this.gifle.Width, this.gifle.Height);
                if (tempsCoup > 0)
                {
                    tempsCoup--;
                    if (tempsCoup <= 0)
                    {
                        gifleActif = false;
                    }
                }
                /// si le rectangle est un ennemi
                /// création d’un rectangle correspondant à l’ennemi
                /// on vérifie la collision
                /// appel à la méthode IntersectsWith pour détecter la collision                 
                //if (coupEpee.IntersectsWith(MonstreHItBox))
                //{
                //    coupEpeeActif = false;
                //    monstres.Remove(monstre); // enlève la présence du monstre
                //    MyCanvas.Children.Remove(monstre.sprite); // enlève graphiquement
                //    monstreTue++;
                //}
                if (gifleActif == false)
                {
                    tempsEntreCoup = true;
                    fondJeu.Children.Remove(this.gifle);
                }
            }
        }
        private void Collision() // Pas mal en vrai mais bug un peu 
        {
            Rect playerRect = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            foreach (var element in fondJeu.Children)
            {
                if (element is Rectangle rect && rect.Tag?.ToString() == "Mur")
                {

                    Rect mur = new Rect(Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.Width, rect.Height);
                    if (playerRect.IntersectsWith(mur))
                    {
                        vitesse = VITESSERALENTI;
                        Console.WriteLine("Collision !!!");

                    }
                    vitesse = VITESSE;
                }
            }

        }
        private void CollisionCadeaux() // Utiliser le tag pour les 3 cadeaux 
        {
            Rect rect1 = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            Rect rect2 = new Rect(Canvas.GetLeft(Cadeaux1), Canvas.GetTop(Cadeaux1), Cadeaux1.Width, Cadeaux1.Height);
            if (rect1.IntersectsWith(rect2))
            {
                if (nbCadeaux == 3 && rect1.IntersectsWith(rect2))
                {

                    //msmCadeaux.Visibility = Visibility.Visible;
                }
                else if (nbCadeaux == 3 && !(rect1.IntersectsWith(rect2)))
                {
                    //msmCadeaux.Visibility = Visibility.Hidden;
                }


                else
                {
                    int posGauche = rndLeft.Next(POSCADEAUX1, POSCADEAUX2);
                    int posHaut = rndTop.Next(POSCADEAUY1, POSCADEAUY2);
                    Canvas.SetTop(Cadeaux1, posHaut);
                    Canvas.SetLeft(Cadeaux1, posGauche);
                    nbCadeaux++;
                    Console.WriteLine("Collision Cadeaux");
                    NbPoint.Content = nbCadeaux;
                }

            }

        }




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
            if (e.Key == Key.Escape)
            {
                Pause();
            }
        }
        private void Pause()
        {
            // Arrêter la minuterie avant d'ouvrir la fenêtre de pause
            minuterie.Stop();

            // Ouvrir la fenêtre de pause
            dialogue_Pause pause = new dialogue_Pause();
            bool? result = pause.ShowDialog();

            // Reprendre ou quitter selon l'action de l'utilisateur
            if (result == true) // Reprendre
            {
                minuterie.Start(); // Redémarrer la minuterie
            }
            else if (result == false) // Quitter
            {
                Application.Current.Shutdown(); // Fermer l'application
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