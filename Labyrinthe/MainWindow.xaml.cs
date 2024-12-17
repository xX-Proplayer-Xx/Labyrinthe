
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;



namespace Labyrinthe
{
    //RESTE A FAIRE :
    /// Spawn ennemie (PARTIEL)
    /// PATHFINDER DES ENNEMIES 
    /// MODIFICATION DE LA VITTESSE DANS LES BUISSONS (FONCTIONNE!!)
    /// MESSAGES CADEAUX 
    /// SON 
    /// SAPIN
    /// BACKGROUND
    /// TIMER + IN CANVAS (fONCTIONNEL)
    ///GERER LES SORTIES 
    public partial class MainWindow : Window
    {
        //Lutin
        private int nbLutin = 0;

        //Musique
        public static MediaPlayer musique;
        //Position init papa noel
        private double positionXJoueur = 10;
        private double positionYJoueur = 842;

        //Annimation PAPA NOEL
        private int tempsPapaNoel = 48;
        private string repertoirePapaNoel = "AnnimationPapaNoel";
        private string nomImagePapaNoel = "PapaNoel_";

        //Constante deplacement image 
        static readonly int AGNLEHAUT = 0;
        static readonly int AGNLEDROITE = 90;
        static readonly int AGNLEGAUCHE = -90;
        static readonly int AGNLEBAS = 180;
        static readonly int AGNLEHAUTDROITE = 45;
        static readonly int AGNLEHAUTGAUCHE = -45;
        static readonly int AGNLEBASDROITE = 135;
        static readonly int AGNLEBASGAUCHE = -135;

        //Cadeaux
        private int nbMaxCadeaux = 10;
        private int nbCadeaux = 0;
        private int cadeauxRamene = 0;

        ///Deplacements 
        private bool goDroite, goGauche, goHaut, goBas;


        //Temps 
        static readonly int TEMPS = 180;

        //SpawnLuttins
        static readonly int LUTTINX = 800;
        static readonly int LUTTINY = 300;
        static readonly int CREATION = 200;
        private int tempsCreationLutin = CREATION;

        //Liste
        private List<Luttin> luttins = new List<Luttin>();
        private List<Rect> gifles = new List<Rect>();

        //VITESSE
        static readonly double VITESSE = 5;
        static readonly double VITESSERALENTI = 1;
        static readonly int VITESSELUTIN = 2;

        //Intervale coordonnés cadeaux 
        static readonly int POSCADEAUX1 = 50;
        static readonly int POSCADEAUX2 = 1550;
        static readonly int POSCADEAUY1 = 50;
        static readonly int POSCADEAUY2 = 850;
        //Random coordonnés cadeaux 
        private Random rndLeft = new Random();
        private Random rndTop = new Random();
        //Score

        //Rectangle 
        Rectangle gifle = new Rectangle();
        //Coups
        private bool gifleActif = false;
        private bool tempsEntreCoup = true;
        private int tempsCoup = 5;

        private int vitesseAnnimation = 1;
        private bool claque;
        private DispatcherTimer minuterie;
        private DispatcherTimer tempsRestant;
        private int secondesRestantes = TEMPS;
        private double vitesse = 10;

        public MainWindow()
        {

            Menu_Acceuil acceuil = new Menu_Acceuil();
            acceuil.ShowDialog();
            InitializeComponent();
            InitMusique();
            InitMinuterie();
            InitTempsRestant();
            InitMusique();
            

        }
        private void InitBitmap()
        {

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
        private void InitTempsRestant()
        {
            progressBar.Maximum = TEMPS;
            tempsRestant = new DispatcherTimer();
            tempsRestant.Interval = TimeSpan.FromSeconds(1);
            tempsRestant.Tick += TempsRestantTick;
            tempsRestant.Start();
        }
        private void TempsRestantTick(object sender, EventArgs e)
        {
            if (secondesRestantes > 0)
            {
                secondesRestantes--;
                progressBar.Value = TEMPS - secondesRestantes;
            }
            else
            {
                tempsRestant.Stop();
                minuterie.Stop();

                MessageBox.Show("Temps ecoule");
            }
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
            Rect joueurRect = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            Lutin();
            Deplacement();
            Collision(joueurRect);
            CollisionCadeaux();
            CoupAttaque();
            CollisionSapin();
            NbPoint.Content = nbCadeaux;

            tempsCreationLutin--;
            if (tempsCreationLutin <= 0)
            {
                ApparitionLuttins();
                tempsCreationLutin = CREATION;
                Console.WriteLine("Un lutin a été créé");

            }
        }
        private void Deplacement()
        {
            Rect rect1 = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            Rect rect2 = new Rect(Canvas.GetLeft(fondJeu), Canvas.GetTop(fondJeu), fondJeu.Width, fondJeu.Height);

            //
            if (goDroite == true && Canvas.GetLeft(Joueur) + (Joueur.Width * 2) < Application.Current.MainWindow.Width)
            {
                DeplacementImage(AGNLEDROITE);
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("Droite");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) + vitesse);
                positionXJoueur = positionXJoueur + vitesse;

            }
            if (goGauche == true && Canvas.GetLeft(Joueur) + (Joueur.Width * 2) > 0)
            {
                DeplacementImage(AGNLEGAUCHE);
                //Console.ForegroundColor = ConsoleColor.Blue;
                //Console.WriteLine("Gauche");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) - vitesse);
                positionXJoueur = positionXJoueur - vitesse;
            }
            if (goHaut == true && Canvas.GetTop(Joueur) + (Joueur.Height * 2) > 0)
            {
                DeplacementImage(AGNLEHAUT);
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine("Haut");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) - vitesse);
                positionYJoueur = positionYJoueur - vitesse;
            }
            if (goBas == true && Canvas.GetTop(Joueur) + (Joueur.Height * 2) < Application.Current.MainWindow.Height)
            {
                DeplacementImage(AGNLEBAS);
                //Console.ForegroundColor = ConsoleColor.Magenta;
                //Console.WriteLine("Bas");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) + vitesse);
                positionYJoueur = positionYJoueur + vitesse;
            }
            //DIAGONALES
            if (goHaut == true && goDroite == true)
            {

                DeplacementImage(AGNLEHAUTDROITE);
            }
            if (goHaut == true && goGauche == true)
            {

                DeplacementImage(AGNLEHAUTGAUCHE);
            }
            if (goBas == true && goDroite == true)
            {

                DeplacementImage(AGNLEBASDROITE);
            }
            if (goBas == true && goGauche == true)
            {

                DeplacementImage(AGNLEBASGAUCHE);
            }

        }

        private void DeplacementImage(int position)
        {
            RotateTransform rotateTransform = (RotateTransform)Joueur.RenderTransform;
            rotateTransform.Angle = position;
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
            tempsCoup = 3;
            tempsEntreCoup = false;
        }
        private void CoupAttaque()
        {
            fondJeu.Children.Remove(gifle);
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
                if (gifleActif == false)
                {
                    tempsEntreCoup = true;
                    fondJeu.Children.Remove(gifle);

                }

            }
        }

        private void Collision(Rect hitBox)
        {
            bool collision = false;
            foreach (var element in fondJeu.Children)
            {
                if (element is Rectangle rect && rect.Tag?.ToString() == "Mur")
                {

                    Rect mur = new Rect(Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.Width, rect.Height);
                    if (hitBox.IntersectsWith(mur))
                    {
                        collision = true;
                    }
                }
            }
            if (collision == true)
            {
                vitesse = VITESSERALENTI;
            }
            if (collision == false)
            {
                vitesse = VITESSE;
            }
        }
        private void CollisionCadeaux() 
        {
            Rect rect1 = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            Rect rect2 = new Rect(Canvas.GetLeft(Cadeaux1), Canvas.GetTop(Cadeaux1), Cadeaux1.Width, Cadeaux1.Height);
            if (rect1.IntersectsWith(rect2))
            {
                if (nbCadeaux == 3 && rect1.IntersectsWith(rect2))
                {
                    MessageBox.Show("Vous ne pouvez pas porter plus de 3 cadeaux à la fois");
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
                    cadeauxRamene++;
                    Console.WriteLine("Collision Cadeaux");

                }

            }

        }
        private void CollisionSapin()
        {
            Rect papanoel = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            Rect sapin = new Rect(Canvas.GetLeft(Sapin), Canvas.GetTop(Sapin), Sapin.Width, Sapin.Height);
            if (papanoel.IntersectsWith(sapin))
            {

                if (nbCadeaux > 0)
                {
                    ///Modifier l'image du sapin en fonction du nombre de cadeaux apportés
                    //ImageBrush sapinImage = new ImageBrush();
                    //sapinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Sapin/Sapin" + totalCadeaux+1 + ".png"));
                    //Sapin.Fill = sapinImage;
                    nbCadeaux--;
                }
                else { nbCadeaux = 0; }
            }
        }

        private void ApparitionLuttins()
        {
            int x = 0;
            int y = 0;
            Rectangle nouveauLutin = new Rectangle
            {

                Tag = "luttin",
                Height = 32,
                Width = 32,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
            };
            Canvas.SetTop(nouveauLutin, y);
            Canvas.SetLeft(nouveauLutin, LUTTINX);
            fondJeu.Children.Add(nouveauLutin);
            //ImageBrush lutinCostume = new ImageBrush();
            //lutinCostume.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Lutin/Lutin_Bas/LUTTIN-1.png")); // Remplacez "chemin_vers_image_fantome" par le chemin réel de votre image de fantôme
            //nouveauLutin.Fill = lutinCostume;
            luttins.Add(new Luttin(nouveauLutin, x, y, 32, 32, VITESSELUTIN));

        }
        private void Lutin()
        {
            Rect papanoel = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);

            for (int i = 0; i < luttins.Count(); i++)
            {

                Luttin lutin = luttins[i];
                Rectangle x = lutin.sprite;

                double posistionXLutin = Canvas.GetLeft(x);
                double posistionYLutin = Canvas.GetTop(x);
                double vitesseLutin = VITESSELUTIN;
                bool collision = false;
                Rect LutinHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                //Collision Lutin/Obstacle
                foreach (var element in fondJeu.Children)
                {
                    if (element is Rectangle rect && rect.Tag?.ToString() == "Mur")
                    {

                        Rect mur = new Rect(Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.Width, rect.Height);
                        if (LutinHitBox.IntersectsWith(mur))
                        {
                            collision = true;
                        }
                    }
                }
                if (collision == true)
                {
                    vitesseLutin = VITESSERALENTI;
                }
                if (collision == false)
                {
                    vitesseLutin = VITESSE;
                }
                //Console.WriteLine($"Lutin Position: X={posistionXLutin}, Y={posistionYLutin} | Joueur Position: X={positionXJoueur}, Y={positionYJoueur}");
                if (positionXJoueur > posistionXLutin && positionXJoueur != posistionXLutin)
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseLutin);
                    posistionXLutin = posistionXLutin + vitesseLutin;
                    //Console.WriteLine("Le lutin se deplace vers la gauche");
                }
                else
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseLutin);
                    posistionXLutin = posistionXLutin - vitesseLutin;
                    //Console.WriteLine("Le lutin se deplace vers la droite");
                }
                if (positionYJoueur > posistionYLutin && positionYJoueur != posistionYLutin)
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + vitesseLutin);
                    posistionYLutin = posistionYLutin + vitesseLutin;
                    //Console.WriteLine("Le lutin se deplace vers le bas");
                }
                else
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - vitesseLutin);
                    posistionYLutin = posistionYLutin - vitesseLutin;
                    //Console.WriteLine("Le lutin se deplace vers le haut");
                }
                //Vol de Cadeaux
                if (LutinHitBox.IntersectsWith(papanoel))
                {
                    //Console.WriteLine("Cadeux volé");
                    nbCadeaux--;
                    luttins.Remove(lutin);
                    fondJeu.Children.Remove(lutin.sprite);
                    if (nbCadeaux < 0)
                    {
                        nbCadeaux = 0;
                    }
                }
                //Collision Lutin/Gifle
                Rect gifle = new Rect(Canvas.GetLeft(this.gifle), Canvas.GetTop(this.gifle), this.gifle.Width, this.gifle.Height);
                if (gifle.IntersectsWith(LutinHitBox))
                {
                    //Console.WriteLine("Lutin giflé");
                    luttins.Remove(lutin);
                    fondJeu.Children.Remove(lutin.sprite);
                }
            }
        }

        private void Joueur_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Attaque();
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
            minuterie.Stop();
            tempsRestant.Stop();
            dialogue_Pause pause = new dialogue_Pause();
            bool? result = pause.ShowDialog();
            if (result == true)
            {
                minuterie.Start();
                tempsRestant.Start();
            }
            else if (result == false)
            {
                Application.Current.Shutdown();
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