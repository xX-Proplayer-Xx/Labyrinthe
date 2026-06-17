# 🎄 Labyrinthe – Le Père Noël et les Lutins

Petit jeu de Noël en 2D fait avec **C# / .NET 8 (WPF)**, dans le cadre d'une SAÉ de BUT Informatique.

Tu incarnes le **Père Noël** : ramasse les cadeaux disséminés dans le labyrinthe et ramène-les au **sapin** avant la fin du temps, tout en évitant les **lutins** qui essaient de te voler tes cadeaux !

<!-- Astuce : ajoute une capture d'écran du jeu ici, par ex. :
![Aperçu du jeu](Labyrinthe/img/apercu.png) -->

## 🎮 But du jeu

- Ramasse les cadeaux (tu peux en porter **3 à la fois** maximum).
- Dépose-les au **sapin** pour les compter.
- Atteins l'**objectif de cadeaux déposés** avant la fin du chrono pour **gagner**.
- Les **lutins** apparaissent par la porte et te poursuivent pour voler tes cadeaux.
- Les **buissons** te ralentissent (mais tu peux passer à travers).

## ⌨️ Commandes

| Touche | Action |
|--------|--------|
| **Z / Q / S / D** | Se déplacer |
| **Espace** | Frapper (renvoie un lutin) |
| **Échap** | Pause |

## 🎚️ Difficultés

| Mode | Objectif | Particularité |
|------|----------|---------------|
| Facile | 10 cadeaux | Lutins lents |
| Moyen | 15 cadeaux | — |
| Difficile | 20 cadeaux | Lutins rapides et nombreux |
| Illimité | — | Pas d'objectif : bats ton **record** de cadeaux déposés |

## ▶️ Jouer (Windows uniquement)

1. Va dans la section **Releases** du dépôt.
2. Télécharge le `.zip`, **extrais tout le dossier**.
3. Lance **`Labyrinthe.exe`** depuis le dossier extrait.

> Si Windows affiche un avertissement bleu (« Windows a protégé votre PC »), clique sur **Informations complémentaires → Exécuter quand même**. C'est normal pour un exécutable non signé.

## 🛠️ Lancer depuis le code (pour développer)

Nécessite le **SDK .NET 8**.

```bash
git clone <url-du-depot>
cd Labyrinthe
dotnet run
```

Ou ouvre `Labyrinthe/Labyrinthe.csproj` dans **Visual Studio** et appuie sur **F5**.

## 🧰 Technologies

- C# / .NET 8
- WPF (interface et rendu)
- Images et sons embarqués comme ressources du projet

---

*Projet réalisé dans le cadre d'un BUT Informatique.*
