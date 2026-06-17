
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        ///DIFFICULTE
        //FACILE
        static readonly int OBJCADEAUXFACILE = 10;
        //NORMALE
        static readonly int OBJCADEAUXNORMALE = 15;
        //DIFFICILE
        static readonly int OBJCADEAUXDIFFICILE = 20;

        //Condition WIN
        private int objectifCadeaux = 0;

        //Lutin
        private int nbLutin = 0;

        //Musique
        public static MediaPlayer musique;
        //Sons
        private static SoundPlayer sonDefaite, sonVictoire, sonColisionCadeau, sonFrappe, sonColisionSapin, sonColisionLutin;
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
        private bool goDLutin, goGLutin, goBLutin, goHLutin;

        //Temps 
        static readonly int TEMPS = 500;
        private int tempsSkinLutin = 0;
        private int tempsSkinPapaNoel = 0;

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
        static readonly int VITESSELUTINFACILE = 2;
        static readonly int VITESSELUTINNORMALE = 4;
        static readonly int VITESSELUTINDIFFICILE = 8;
        private double vitesse = 10;
        private double vitesseDesLutins = 2;

        //Intervale coordonnés cadeaux 
        static readonly int POSCADEAUX1 = 50;
        static readonly int POSCADEAUX2 = 1550;
        static readonly int POSCADEAUY1 = 50;
        static readonly int POSCADEAUY2 = 850;
        //Random coordonnés cadeaux 
        private Random rndLeft = new Random();
        private Random rndTop = new Random();
        //Score

        //Image
        private int imageActuelle = 0;

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




        public MainWindow()
        {

            Menu_Acceuil acceuil = new Menu_Acceuil();
            acceuil.ShowDialog();
            InitializeComponent();
            InitMusique();
            InitMinuterie();
            InitTempsRestant();
            InitSon();
            
        }
        public static void InitMusique()
        {
            if (musique == null) // Vérifier que la musique n'a pas déjà été initialisée
            {
                musique = new MediaPlayer();
                // Chemin relatif : la musique est copiée à côté de l'exe (Sons\musique.mp3)
                string cheminMusique = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sons", "musique.mp3");
                musique.Open(new Uri(cheminMusique));
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
        private void InitSon()
        {
            sonDefaite = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Sons/defaite.wav")).Stream);
            sonVictoire = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Sons/victoire.wav")).Stream);
            sonColisionCadeau = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Sons/colisioncadeau.wav")).Stream);
            sonFrappe = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Sons/frappe.wav")).Stream);
            sonColisionSapin = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Sons/colisionsapin.wav")).Stream);
            sonColisionLutin = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Sons/colisionlutin.wav")).Stream);
        }
        private void InitTempsRestant()
        {
            progressBar.Maximum = TEMPS;
            tempsRestant = new DispatcherTimer();
            tempsRestant.Interval = TimeSpan.FromSeconds(1);
            tempsRestant.Tick += TempsRestantTick;

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
                FinDePartie();
            }
        }
        private void InitMinuterie()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval += TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;

        }

        private void RejouerButton_Click(object sender, RoutedEventArgs e)
        {
            // Réinitialiser et redémarrer le jeu
            RelanceJeu();

            // Masquer le panneau de fin de partie
            FinPartiePanel.Visibility = Visibility.Hidden;
        }
        private void FinDePartie()
        {
            sonDefaite.Play();
            // Afficher le panneau de fin de partie
            FinPartiePanel.Visibility = Visibility.Visible;

            tempsRestant.Stop();
            minuterie.Stop();

            // Calculer et afficher le temps écoulé
            int tempsEcoule = TEMPS - secondesRestantes; // Temps total moins le temps restant
            int minutes = tempsEcoule / 60;
            int secondes = tempsEcoule % 60;

            TempsEcouleText.Text = $"Temps Écoulé : {minutes:00}:{secondes:00}";

            TextBlockRes.Text = "Vous avez perdu !";


        }

        public void RelanceJeu()
        {
            // Réinitialisation de l'état global
            secondesRestantes = TEMPS; // Remettre le temps initial
            nbCadeaux = 0;
            cadeauxRamene = 0;
            NbPoint.Content = nbCadeaux;
            NbPointDepose.Content = cadeauxRamene;

            // Réinitialiser la position du joueur
            Canvas.SetLeft(Joueur, 10);
            Canvas.SetTop(Joueur, 842);
            positionXJoueur = 10;
            positionYJoueur = 842;

            // Réinitialiser les lutins
            foreach (var lutin in luttins)
            {
                fondJeu.Children.Remove(lutin.sprite);
            }
            luttins.Clear();

            // Réinitialiser les positions des cadeaux
            int posGauche = rndLeft.Next(POSCADEAUX1, POSCADEAUX2);
            int posHaut = rndTop.Next(POSCADEAUY1, POSCADEAUY2);
            Canvas.SetLeft(Cadeaux1, posGauche);
            Canvas.SetTop(Cadeaux1, posHaut);

            // Réinitialiser timers
            if (minuterie == null || tempsRestant == null)
            {
                InitMinuterie();
                InitTempsRestant();
            }
            else
            {
                minuterie.Stop();
                tempsRestant.Stop();
                InitMinuterie();
                InitTempsRestant();
            }

            // Redémarrer les timers
            tempsRestant.Start();
            minuterie.Start();

            // Cacher le panneau de fin
            FinPartiePanel.Visibility = Visibility.Hidden;

            //remet l'image du sapin a celle de base
            SetImage("pack://application:,,,/img/Sapin/Sapin1.png");

            // Afficher les commandes initiales si nécessaire
            Console.WriteLine("Le jeu a été redémarré !");

        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Menu_Acceuil acceuil = new Menu_Acceuil();
            acceuil.ShowDialog();
            RelanceJeu();
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
            NbPointDepose.Content = cadeauxRamene;

            tempsCreationLutin--;
            if (tempsCreationLutin <= 0)
            {
                ApparitionLuttins();
                tempsCreationLutin = CREATION;
                Console.WriteLine("Un lutin a été créé");

            }
            if (cadeauxRamene >= objectifCadeaux )
            {
                ConditionsVictoire();
            }
        }

        private void Deplacement()
        {
            Rect rect1 = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            //HAUT,GAUCHE,BAS,DROITE
            if (goDroite == true && Canvas.GetLeft(Joueur) + (Joueur.Width * 2) < Application.Current.MainWindow.Width)
            {
                DeplacementImageJoueur(AGNLEDROITE);
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("Droite");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) + vitesse);
                positionXJoueur = positionXJoueur + vitesse;

            }
            if (goGauche == true && Canvas.GetLeft(Joueur) + (Joueur.Width * 2) > 0)
            {
                DeplacementImageJoueur(AGNLEGAUCHE);
                //Console.ForegroundColor = ConsoleColor.Blue;
                //Console.WriteLine("Gauche");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) - vitesse);
                positionXJoueur = positionXJoueur - vitesse;
            }
            if (goHaut == true && Canvas.GetTop(Joueur) + (Joueur.Height * 2) > 0)
            {
                DeplacementImageJoueur(AGNLEHAUT);
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine("Haut");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) - vitesse);
                positionYJoueur = positionYJoueur - vitesse;
            }
            if (goBas == true && Canvas.GetTop(Joueur) + (Joueur.Height * 2) < Application.Current.MainWindow.Height)
            {
                DeplacementImageJoueur(AGNLEBAS);
                //Console.ForegroundColor = ConsoleColor.Magenta;
                //Console.WriteLine("Bas");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) + vitesse);
                positionYJoueur = positionYJoueur + vitesse;
            }
            //DIAGONALES
            Diagonale(goGauche, goDroite, goBas, goHaut);
        }
        private void Diagonale(bool gauche, bool droite, bool bas, bool haut)
        {
            if (haut == true && droite == true)
            {

                DeplacementImageJoueur(AGNLEHAUTDROITE);
            }
            if (haut == true && gauche == true)
            {

                DeplacementImageJoueur(AGNLEHAUTGAUCHE);
            }
            if (bas == true && droite == true)
            {

                DeplacementImageJoueur(AGNLEBASDROITE);
            }
            if (bas == true && gauche == true)
            {

                DeplacementImageJoueur(AGNLEBASGAUCHE);
            }
        }






        private void DeplacementLutin()
        {

        }




        private void DeplacementImageJoueur(int position)
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
                sonFrappe.Play();
                Rect maxiGifle = new Rect(Canvas.GetLeft(this.gifle), Canvas.GetTop(this.gifle), this.gifle.Width, this.gifle.Height);
                if (tempsCoup > 0)
                {
                    sonFrappe.Play();
                    tempsCoup--;
                    if (tempsCoup <= 0)
                    {
                        gifleActif = false;
                        sonFrappe.Play();
                    }
                }
                if (gifleActif == false)
                {
                    tempsEntreCoup = true;
                    fondJeu.Children.Remove(gifle);
                    sonFrappe.Play();
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

                    ImageBrush murImage = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/Buisson/BushUpdate2.png"))
                    };
                    murImage.Stretch = Stretch.Fill;
                    rect.Fill = murImage;

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
                    msmCadeaux.Visibility = Visibility.Visible;
                }
                else
                {
                    msmCadeaux.Visibility = Visibility.Hidden;
                    int posGauche = rndLeft.Next(POSCADEAUX1, POSCADEAUX2);
                    int posHaut = rndTop.Next(POSCADEAUY1, POSCADEAUY2);
                    Canvas.SetTop(Cadeaux1, posHaut);
                    Canvas.SetLeft(Cadeaux1, posGauche);
                    nbCadeaux++;
                    sonColisionCadeau.Play();
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
                    
                    nbCadeaux--;
                    cadeauxRamene++;
                    imageActuelle++;
                    switch (imageActuelle)
                    {
                        case 1:
                            SetImage("pack://application:,,,/img/Sapin/Sapin2.png");
                            imageActuelle = 1; // Passe à l'image suivante
                            sonColisionSapin.Play();
                            Console.WriteLine("L'image est bien changé");
                            break;
                        case 2:
                            SetImage("pack://application:,,,/img/Sapin/Sapin3.png");
                            imageActuelle = 2;
                            sonColisionSapin.Play();
                            break;
                        case 3:
                            SetImage("pack://application:,,,/img/Sapin/Sapin4.png");
                            imageActuelle = 3;
                            sonColisionSapin.Play();
                            break;
                        case 4:
                            SetImage("pack://application:,,,/img/Sapin/Sapin5.png");
                            imageActuelle = 4;
                            sonColisionSapin.Play();
                            break;
                        case 5:
                            SetImage("pack://application:,,,/img/Sapin/Sapin6.png");
                            imageActuelle = 5;
                            sonColisionSapin.Play();
                            break;
                        case 6:
                            SetImage("pack://application:,,,/img/Sapin/Sapin7.png");
                            imageActuelle = 6;
                            sonColisionSapin.Play();
                            break;
                        case 7:
                            SetImage("pack://application:,,,/img/Sapin/Sapin8.png");
                            imageActuelle = 7;
                            sonColisionSapin.Play();
                            break;
                        case 8:
                            SetImage("pack://application:,,,/img/Sapin/Sapin9.png");
                            imageActuelle = 8;
                            sonColisionSapin.Play();
                            break;
                        case 9:
                            SetImage("pack://application:,,,/img/Sapin/Sapin10.png");
                            imageActuelle = 9;
                            sonColisionSapin.Play();
                            break;
                        case 10:
                            SetImage("pack://application:,,,/img/Sapin/Sapin11.png");
                            imageActuelle = 10;
                            sonColisionSapin.Play();
                            break;

                    }


                }
                else { nbCadeaux = 0; }
            }
        }

        private void ConditionsVictoire()
        {
            sonVictoire.Play();
            int tempsEcoule = TEMPS - secondesRestantes; // Temps total moins le temps restant
            int minutes = tempsEcoule / 60;
            int secondes = tempsEcoule % 60;

            TempsEcouleText.Text = $"Temps Écoulé : {minutes:00}:{secondes:00}";

            if (cadeauxRamene >= objectifCadeaux)
            {
                tempsRestant.Stop();
                minuterie.Stop();
                TextBlockRes.Text = "Vous avez gagné !";
                FinPartiePanel.Visibility = Visibility.Visible;
                Console.WriteLine("Vous avez gagné");
            }
        }

        private void SetImage(string nomImage)
        {

            Sapin.Fill = new ImageBrush(new BitmapImage(new Uri(nomImage)));

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
            };
            Canvas.SetTop(nouveauLutin, y);
            Canvas.SetLeft(nouveauLutin, LUTTINX);
            fondJeu.Children.Add(nouveauLutin);
            //ImageBrush lutinCostume = new ImageBrush();
            //lutinCostume.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Lutin/Lutin_Bas/LUTTIN-1.png")); // Remplacez "chemin_vers_image_fantome" par le chemin réel de votre image de fantôme
            //nouveauLutin.Fill = lutinCostume;
            luttins.Add(new Luttin(nouveauLutin, x, y, 32, 32, vitesseDesLutins));

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
                double vitesseLutin = vitesseDesLutins;
                bool collision = false;
                Rect LutinHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                //Collision Lutin/Obstacle
                foreach (var element in fondJeu.Children)
                {
                    LutinImage(x);

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
                    vitesseLutin = vitesseDesLutins;
                }
                //Console.WriteLine($"Lutin Position: X={posistionXLutin}, Y={posistionYLutin} | Joueur Position: X={positionXJoueur}, Y={positionYJoueur}");
                if (positionXJoueur > posistionXLutin && positionXJoueur != posistionXLutin)
                {
                    //DeplacementImageLutin(AGNLEDROITE, x);
                    goDLutin = true;
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseLutin);
                    posistionXLutin = posistionXLutin + vitesseLutin;
                    //Console.WriteLine("Le lutin se deplace vers la droite");
                }
                else
                {
                    //DeplacementImageLutin(AGNLEGAUCHE, x);
                    goGLutin = true;
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseLutin);
                    posistionXLutin = posistionXLutin - vitesseLutin;
                    //Console.WriteLine("Le lutin se deplace vers la gauche");
                }
                if (positionYJoueur > posistionYLutin && positionYJoueur != posistionYLutin)
                {
                    //DeplacementImageLutin(AGNLEBAS, x);
                    goBLutin = true;
                    Canvas.SetTop(x, Canvas.GetTop(x) + vitesseLutin);
                    posistionYLutin = posistionYLutin + vitesseLutin;
                    //Console.WriteLine("Le lutin se deplace vers le bas");
                }
                else
                {
                    //DeplacementImageLutin(AGNLEHAUT, x);
                    goHLutin = true;
                    Canvas.SetTop(x, Canvas.GetTop(x) - vitesseLutin);
                    posistionYLutin = posistionYLutin - vitesseLutin;
                    //Console.WriteLine("Le lutin se deplace vers le haut");
                }

                RotationImageLutin(x, goGLutin, goDLutin, goBLutin, goHLutin);
                //Vol de Cadeaux
                if (LutinHitBox.IntersectsWith(papanoel))
                {
                    //Console.WriteLine("Cadeux volé");
                    sonColisionLutin.Play();
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
        private void RotationImageLutin(Rectangle lutin, bool goGLutin, bool goDLutin, bool goBLutin, bool goHLutin)
        {
            double angle = 0;

            if (goGLutin && !goBLutin && !goHLutin) // Gauche
            {
                angle = AGNLEGAUCHE;
            }
            else if (goDLutin && !goBLutin && !goHLutin) // Droite
            {
                angle = AGNLEDROITE;
            }
            else if (goBLutin && !goGLutin && !goDLutin) // Bas
            {
                angle = AGNLEBAS;
            }
            else if (goHLutin && !goGLutin && !goDLutin) // Haut
            {
                angle = AGNLEHAUT;
            }
            else if (goGLutin && goHLutin) // Diagonale haut-gauche
            {
                angle = AGNLEHAUTGAUCHE;
            }
            else if (goDLutin && goHLutin) // Diagonale haut-droite
            {
                angle = AGNLEHAUTDROITE;
            }
            else if (goGLutin && goBLutin) // Diagonale bas-gauche
            {
                angle = AGNLEBASGAUCHE;
            }
            else if (goDLutin && goBLutin) // Diagonale bas-droite
            {
                angle = AGNLEBASDROITE;
            }

            // Appliquer la rotation au lutin
            RotateTransform rotation = new RotateTransform(angle);
            lutin.RenderTransform = rotation;

            // Définir le centre de rotation au milieu de l'image
            lutin.RenderTransformOrigin = new Point(0.5, 0.5);
        }
        private void SelectionDifficulte(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedDifficulty = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();
            if (selectedDifficulty == "Choisis")
            {


            }
            if (selectedDifficulty == "Facile")
            {
                vitesse = 10;
                vitesseDesLutins = VITESSELUTINFACILE;
                tempsCreationLutin = 300;
                objectifCadeaux = OBJCADEAUXFACILE;
                //    MessageBox.Show($"Difficulté sélectionnée : {selectedDifficulty}, Ne te fait pas mal surtout !");
            }
                if (selectedDifficulty == "Moyen")
                {
                    vitesse = 8;
                    vitesseDesLutins = VITESSELUTINNORMALE;
                    tempsCreationLutin = 200;
                    objectifCadeaux = OBJCADEAUXNORMALE;
                Console.WriteLine($"Difficulty set to Medium. Objective: {objectifCadeaux}");
                //MessageBox.Show($"Difficulté sélectionnée : {selectedDifficulty}, Bonne chance !");


            }
                if (selectedDifficulty == "Difficile")
                {
                    vitesse = 6;
                    vitesseDesLutins = VITESSELUTINDIFFICILE;
                    tempsCreationLutin = 100;
                    objectifCadeaux = OBJCADEAUXDIFFICILE;
                    //MessageBox.Show($"Difficulté sélectionnée : {selectedDifficulty}, Courage à vous !");


                }
                if (selectedDifficulty == "Illimité")
                {
                    vitesse = 10;
                    vitesseDesLutins = VITESSELUTINNORMALE;
                    tempsCreationLutin = 300;
                    //MessageBox.Show($"Difficulté sélectionnée : {selectedDifficulty}, N'y passer pas trop de temps XD");


                }

            }
            private void Button_Click(object sender, RoutedEventArgs e)
            {
                NbPointDepose.Visibility = Visibility.Visible;
                NbPointDeposeLab.Visibility = Visibility.Visible;
                NbPoint.Visibility = Visibility.Visible;
                NbPointLab.Visibility = Visibility.Visible;
                Joueur.Visibility = Visibility.Visible;
                Porte.Visibility = Visibility.Visible;
                Cadeaux1.Visibility = Visibility.Visible;
                tempsRestant.Start();
                minuterie.Start();
                BackAttente.Visibility = Visibility.Hidden;
                ButJouerAttente.Visibility = Visibility.Hidden;
                difficulteComboBox.Visibility = Visibility.Hidden;
                LabAttente.Visibility = Visibility.Hidden;

            }

            private void LutinImage(Rectangle nomObjet)
            {
                tempsSkinLutin += 1;
                ImageBrush lutinCostume = new ImageBrush();
                lutinCostume.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/LUTTIN/LUTTIN-" + tempsSkinLutin + ".png"));
                nomObjet.Fill = lutinCostume;

                if (tempsSkinLutin == 8)
                {
                    tempsSkinLutin = 1;
                }
            }
            //private void PapaNoelSkin()
            //{
            //    tempsSkinLutin += 1;
            //    ImageBrush assassinCostume = new ImageBrush();
            //    assassinCostume.ImageSource = new BitmapImage(new Uri("C:\\IUT\\SAE1.01 2024-2025\\Labyrinthe1.02\\Labyrinthe\\img\\LUTTIN\\LUTTIN-" + tempsSkinLutin + ").png"));
            //    Vitrine.Fill = assassinCostume;
            //    if (tempsSkinLutin == 8)
            //    {
            //        tempsSkinLutin = 1;
            //    }
            //}

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
            if (e.Key == Key.J)
            {

                cadeauxRamene = nbMaxCadeaux;
            }
            }
            private void Pause()
            {

               sonVictoire.Stop();
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
                    pause.Close();
                    minuterie.Start();
                    tempsRestant.Start();
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