
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
        //Mode Illimité : pas d'objectif, on bat son record de cadeaux (record gardé le temps de la session)
        private bool modeIllimite = false;
        private int meilleurScore = 0;

        //Lutin
        private int nbLutin = 0;

        //Musique
        public static MediaPlayer musique;
        //Sons
        private static SoundPlayer sonDefaite, sonVictoire, sonColisionCadeau, sonFrappe, sonColisionSapin, sonColisionLutin;
        //Position init papa noel
        private double positionXJoueur = 10;
        private double positionYJoueur = 810;

        //Annimation PAPA NOEL
        private int tempsPapaNoel = 48;
        private string repertoirePapaNoel = "AnimationPapaNoel";
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

        //Porte (s'ouvre quand un lutin en sort puis se referme)
        static readonly int DUREEPORTEOUVERTE = 30;
        private int tempsPorteOuverte = 0;

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
        private double vitesseNormale = VITESSE; // vitesse de base du joueur selon la difficulté
        private double vitesseDesLutins = 2;

        //Intervale coordonnés cadeaux 
        static readonly int POSCADEAUX1 = 50;
        static readonly int POSCADEAUX2 = 1550;
        static readonly int POSCADEAUY1 = 18;
        static readonly int POSCADEAUY2 = 816;
        //Random coordonnés cadeaux 
        private Random rndLeft = new Random();
        private Random rndTop = new Random();
        //Score


        //Rectangle 
        Rectangle gifle = new Rectangle();
        //Coups
        private bool gifleActif = false;
        static readonly int RECHARGECOUP = 30; // recharge de la frappe (en frames, ~0.5s)
        private int tempsEntreCoup = 0;        // recharge restante (0 = frappe prête)
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
            // Afficher le panneau de fin de partie
            FinPartiePanel.Visibility = Visibility.Visible;

            tempsRestant.Stop();
            minuterie.Stop();

            if (modeIllimite)
            {
                // Mode Illimité : on compare le score (cadeaux déposés) au record de la session
                if (cadeauxRamene > meilleurScore)
                {
                    meilleurScore = cadeauxRamene;
                    TextBlockRes.Text = "Nouveau record !";
                    sonVictoire.Play();
                }
                else
                {
                    TextBlockRes.Text = "Temps écoulé !";
                    sonDefaite.Play();
                }
                TempsEcouleText.Text = $"Score : {cadeauxRamene}   -   Record : {meilleurScore}";
            }
            else
            {
                sonDefaite.Play();

                // Calculer et afficher le temps écoulé
                int tempsEcoule = TEMPS - secondesRestantes; // Temps total moins le temps restant
                int minutes = tempsEcoule / 60;
                int secondes = tempsEcoule % 60;

                TempsEcouleText.Text = $"Temps Écoulé : {minutes:00}:{secondes:00}";
                TextBlockRes.Text = "Vous avez perdu !";
            }
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
            Canvas.SetTop(Joueur, 810);
            positionXJoueur = 10;
            positionYJoueur = 810;
            // On relâche les touches (sinon le joueur peut repartir tout seul au redémarrage)
            goHaut = false;
            goBas = false;
            goGauche = false;
            goDroite = false;

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
            SetImage("pack://application:,,,/img/Sapin/sapin1.png");

            // Afficher les commandes initiales si nécessaire
            Console.WriteLine("Le jeu a été redémarré !");

        }

        // Revient à l'écran de choix de difficulté (au retour vers l'accueil)
        private void RetourSelectionDifficulte()
        {
            minuterie.Stop();
            tempsRestant.Stop();

            // Remise à zéro de la partie
            secondesRestantes = TEMPS;
            nbCadeaux = 0;
            cadeauxRamene = 0;
            objectifCadeaux = 0;
            NbPoint.Content = nbCadeaux;
            NbPointDepose.Content = cadeauxRamene;
            Canvas.SetLeft(Joueur, 10);
            Canvas.SetTop(Joueur, 810);
            positionXJoueur = 10;
            positionYJoueur = 810;
            // On relâche les touches (sinon le joueur peut repartir tout seul au redémarrage)
            goHaut = false;
            goBas = false;
            goGauche = false;
            goDroite = false;
            foreach (var lutin in luttins)
            {
                fondJeu.Children.Remove(lutin.sprite);
            }
            luttins.Clear();
            SetImage("pack://application:,,,/img/Sapin/sapin1.png");

            // On cache la partie et le panneau de fin
            FinPartiePanel.Visibility = Visibility.Hidden;
            Joueur.Visibility = Visibility.Hidden;
            Porte.Visibility = Visibility.Hidden;
            Cadeaux1.Visibility = Visibility.Hidden;
            NbPoint.Visibility = Visibility.Hidden;
            NbPointLab.Visibility = Visibility.Hidden;
            NbPointDepose.Visibility = Visibility.Hidden;
            NbPointDeposeLab.Visibility = Visibility.Hidden;
            labFrappe.Visibility = Visibility.Hidden;

            // On réaffiche l'écran de choix de difficulté
            difficulteComboBox.SelectedIndex = 0;
            BackAttente.Visibility = Visibility.Visible;
            LabAttente.Visibility = Visibility.Visible;
            ButJouerAttente.Visibility = Visibility.Visible;
            difficulteComboBox.Visibility = Visibility.Visible;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Menu_Acceuil acceuil = new Menu_Acceuil();
            acceuil.ShowDialog();
            RetourSelectionDifficulte();
        }

        private void Jeu(object? sender, EventArgs e)
        {
            Rect joueurRect = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            Lutin();
            Deplacement();
            AnimationJoueur();
            AnimationPorte();
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
            if (objectifCadeaux > 0 && cadeauxRamene >= objectifCadeaux)
            {
                ConditionsVictoire();
            }
        }

        private void Deplacement()
        {
            Rect rect1 = new Rect(Canvas.GetLeft(Joueur), Canvas.GetTop(Joueur), Joueur.Width, Joueur.Height);
            //HAUT,GAUCHE,BAS,DROITE
            if (goDroite == true && Canvas.GetLeft(Joueur) + Joueur.Width < fondJeu.Width)
            {
                DeplacementImageJoueur(AGNLEDROITE);
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("Droite");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) + vitesse);
                positionXJoueur = positionXJoueur + vitesse;

            }
            if (goGauche == true && Canvas.GetLeft(Joueur) > 0)
            {
                DeplacementImageJoueur(AGNLEGAUCHE);
                //Console.ForegroundColor = ConsoleColor.Blue;
                //Console.WriteLine("Gauche");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) - vitesse);
                positionXJoueur = positionXJoueur - vitesse;
            }
            if (goHaut == true && Canvas.GetTop(Joueur) > 16)
            {
                DeplacementImageJoueur(AGNLEHAUT);
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine("Haut");
                //Console.ForegroundColor = ConsoleColor.White;
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) - vitesse);
                positionYJoueur = positionYJoueur - vitesse;
            }
            if (goBas == true && Canvas.GetTop(Joueur) + Joueur.Height < fondJeu.Height)
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
            if (tempsEntreCoup > 0) // encore en recharge : on ne frappe pas
            {
                return;
            }
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
            sonFrappe.Play();              // le son ne joue qu'une fois, au moment de la frappe
            tempsEntreCoup = RECHARGECOUP; // lance la recharge
        }
        private void CoupAttaque()
        {
            // Recharge de la frappe
            if (tempsEntreCoup > 0)
            {
                tempsEntreCoup--;
            }
            // Durée de la gifle à l'écran (quelques frames), puis on l'enlève
            if (gifleActif)
            {
                tempsCoup--;
                if (tempsCoup <= 0)
                {
                    gifleActif = false;
                    fondJeu.Children.Remove(gifle);
                }
            }
            // Indicateur : la frappe est prête quand la recharge est finie
            labFrappe.Visibility = (tempsEntreCoup <= 0) ? Visibility.Visible : Visibility.Hidden;
        }

        private void InitMurs()
        {
            // L'image des buissons est appliquée une seule fois (avant c'était refait à chaque frame dans Collision)
            ImageBrush buisson = new ImageBrush();
            buisson.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/Buisson/buisson.png"));
            buisson.Stretch = Stretch.Fill;
            foreach (var element in fondJeu.Children)
            {
                if (element is Rectangle rect && rect.Tag?.ToString() == "Mur")
                {
                    rect.Fill = buisson;
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
                vitesse = vitesseNormale;
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
                    sonColisionSapin.Play();

                    // Le sapin se remplit proportionnellement à l'objectif (niveau 0 à 10)
                    int niveauSapin;
                    if (objectifCadeaux > 0)
                    {
                        niveauSapin = (cadeauxRamene * 10) / objectifCadeaux;
                    }
                    else
                    {
                        niveauSapin = cadeauxRamene; // mode Illimité : 1 image par dépôt
                    }
                    if (niveauSapin > 10)
                    {
                        niveauSapin = 10;
                    }
                    SetImage("pack://application:,,,/img/Sapin/sapin" + (niveauSapin + 1) + ".png");


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
            int y = 16;
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
            //lutinCostume.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/LutinDirections/Lutin_Bas/LUTTIN-1.png")); // Remplacez "chemin_vers_image_fantome" par le chemin réel de votre image de fantôme
            //nouveauLutin.Fill = lutinCostume;
            luttins.Add(new Luttin(nouveauLutin, x, y, 32, 32, vitesseDesLutins));

            // La porte s'ouvre quand un lutin en sort
            Porte.Source = new BitmapImage(new Uri("pack://application:,,,/img/Porte/porteOuverte.png"));
            tempsPorteOuverte = DUREEPORTEOUVERTE;
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
                //Reinitialisation des directions du lutin (sinon l'orientation reste bloquée)
                goDLutin = false;
                goGLutin = false;
                goBLutin = false;
                goHLutin = false;
                //Animation du lutin : une seule fois par frame (et pas dans la boucle des murs)
                LutinImage(x);
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
                    vitesseLutin = vitesseDesLutins;
                }
                //Console.WriteLine($"Lutin Position: X={posistionXLutin}, Y={posistionYLutin} | Joueur Position: X={positionXJoueur}, Y={positionYJoueur}");
                // Zone morte = la distance parcourue en un tick. Si le lutin est deja
                // aligne avec le joueur sur un axe (a moins d'un pas), on ne bouge pas et
                // on ne met pas de direction sur cet axe : ca evite qu'il oscille et que
                // son image tourne sans arret gauche/droite.
                if (positionXJoueur - posistionXLutin > vitesseLutin)
                {
                    goDLutin = true;
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseLutin);
                    posistionXLutin = posistionXLutin + vitesseLutin;
                }
                else if (positionXJoueur - posistionXLutin < -vitesseLutin)
                {
                    goGLutin = true;
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseLutin);
                    posistionXLutin = posistionXLutin - vitesseLutin;
                }
                if (positionYJoueur - posistionYLutin > vitesseLutin)
                {
                    goBLutin = true;
                    Canvas.SetTop(x, Canvas.GetTop(x) + vitesseLutin);
                    posistionYLutin = posistionYLutin + vitesseLutin;
                }
                else if (positionYJoueur - posistionYLutin < -vitesseLutin)
                {
                    goHLutin = true;
                    Canvas.SetTop(x, Canvas.GetTop(x) - vitesseLutin);
                    posistionYLutin = posistionYLutin - vitesseLutin;
                }

                // On garde le lutin dans la map
                if (Canvas.GetLeft(x) < 0) Canvas.SetLeft(x, 0);
                if (Canvas.GetLeft(x) > fondJeu.Width - x.Width) Canvas.SetLeft(x, fondJeu.Width - x.Width);
                if (Canvas.GetTop(x) < 0) Canvas.SetTop(x, 0);
                if (Canvas.GetTop(x) > fondJeu.Height - x.Height) Canvas.SetTop(x, fondJeu.Height - x.Height);

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
                if (gifleActif && gifle.IntersectsWith(LutinHitBox))
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
            modeIllimite = false;
            if (selectedDifficulty == "Choisis")
            {


            }
            if (selectedDifficulty == "Facile")
            {
                vitesseNormale = 10;
                vitesseDesLutins = VITESSELUTINFACILE;
                tempsCreationLutin = 300;
                objectifCadeaux = OBJCADEAUXFACILE;
                //    MessageBox.Show($"Difficulté sélectionnée : {selectedDifficulty}, Ne te fait pas mal surtout !");
            }
                if (selectedDifficulty == "Moyen")
                {
                    vitesseNormale = 8;
                    vitesseDesLutins = VITESSELUTINNORMALE;
                    tempsCreationLutin = 200;
                    objectifCadeaux = OBJCADEAUXNORMALE;
                Console.WriteLine($"Difficulty set to Medium. Objective: {objectifCadeaux}");
                //MessageBox.Show($"Difficulté sélectionnée : {selectedDifficulty}, Bonne chance !");


            }
                if (selectedDifficulty == "Difficile")
                {
                    vitesseNormale = 6;
                    vitesseDesLutins = VITESSELUTINDIFFICILE;
                    tempsCreationLutin = 100;
                    objectifCadeaux = OBJCADEAUXDIFFICILE;
                    //MessageBox.Show($"Difficulté sélectionnée : {selectedDifficulty}, Courage à vous !");


                }
                if (selectedDifficulty == "Illimité")
                {
                    vitesseNormale = 10;
                    vitesseDesLutins = VITESSELUTINNORMALE;
                    tempsCreationLutin = 300;
                    objectifCadeaux = 0;   // pas d'objectif : on joue pour le record
                    modeIllimite = true;
                    //MessageBox.Show($"Difficulté sélectionnée : {selectedDifficulty}, N'y passer pas trop de temps XD");


                }

            }
            private void Button_Click(object sender, RoutedEventArgs e)
            {
                // On exige une difficulté : sinon objectifCadeaux reste à 0 et la partie est gagnée d'entrée
                if (difficulteComboBox.SelectedItem == null ||
                    ((ComboBoxItem)difficulteComboBox.SelectedItem).Content.ToString() == "Choisis")
                {
                    MessageBox.Show("Veuillez choisir une difficulté avant de jouer !");
                    return;
                }
                InitMurs(); // applique l'image des buissons une fois, au lancement de la partie
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
                lutinCostume.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/Lutin/lutin" + tempsSkinLutin + ".png"));
                nomObjet.Fill = lutinCostume;

                if (tempsSkinLutin == 8)
                {
                    tempsSkinLutin = 0;
                }
            }
            private void AnimationJoueur()
            {
                // Le Père Noël s'anime seulement quand il se déplace
                if (goDroite || goGauche || goHaut || goBas)
                {
                    tempsSkinPapaNoel += 1;
                    Joueur.Source = new BitmapImage(new Uri("pack://application:,,,/img/AnimationPapaNoel/papaNoel" + tempsSkinPapaNoel + ".png"));
                    if (tempsSkinPapaNoel == 9)
                    {
                        tempsSkinPapaNoel = 0;
                    }
                }
            }

            private void AnimationPorte()
            {
                // Referme la porte peu de temps après qu'un lutin soit sorti
                if (tempsPorteOuverte > 0)
                {
                    tempsPorteOuverte--;
                    if (tempsPorteOuverte == 0)
                    {
                        Porte.Source = new BitmapImage(new Uri("pack://application:,,,/img/Porte/porteFermee.png"));
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
                pause.ShowDialog();
                if (pause.RetourAccueil)
                {
                    // Retour au menu d'accueil, puis écran de choix de difficulté
                    Menu_Acceuil acceuil = new Menu_Acceuil();
                    acceuil.ShowDialog();
                    RetourSelectionDifficulte();
                }
                else
                {
                    // Reprendre (ou fenêtre fermée) : on relance les compteurs
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