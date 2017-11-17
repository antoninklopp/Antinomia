using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe de l'intelligence artificielle du jeu. 
/// </summary>
public class IA : Player {
    /*
     * L'intelligence artificielle sera très basique:
     * Ses actions : 
     * 
     * Choix des cartes au début : Verifier qu'il y a au mois une élémentaire dans les 6 premières cartes. 
     * Il faudra pré-build des cartes pour l'intelligence artificielle. 
     * (A stocker sur la machine pour pouvoir jouer hors connexion). 
     * 
     * Lors des phases principales : Lorsqu'il n'y a pas de cartes : essayer d'en poser une avec son aka
     * Si il n'y a pas d'aka, essayer d'en poser une avec un coût élémentaire. 
     * 
     * Lors de la phase principale : Si toutes les cartes sur le champ de bataille sont plus forte, en rapatrier une au sanctuaire
     * et inversement si les cartes adverses sont moins fortes
     * 
     * Lors de la phase de combat : Attaquer la carte qui fait que celle de l'IA ne se détruit pas. 
     * 
     * 
     */ 


}
