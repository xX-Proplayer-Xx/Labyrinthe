using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Labyrinthe
{
    internal class Luttin
    {
        static readonly int VITESSELUTIN = 3;
        public double x, y, longueur, largeur, vitesse;
        public Rectangle sprite = new Rectangle();
        public int vitesseLutin = VITESSELUTIN; 
        public Luttin(Rectangle sprite, int x, int y, int longueur, int largeur, int vitesse)
        {
                this.sprite = sprite;
                this.x = x;
                this.y = y;
                this.longueur = longueur;
                this.largeur = largeur;
                this.vitesse = vitesseLutin;
        }
        
    }
}
