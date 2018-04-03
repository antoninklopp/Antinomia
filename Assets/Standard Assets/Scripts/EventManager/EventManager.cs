
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AntinomiaException;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events; 

/// <summary>
/// Gestion des evenements s'il y a plusieurs effets à faire en même temps. 
/// Script attaché à un objet qui représente cet event manager. 
/// </summary>
public class EventManager : MonoBehaviourAntinomia {

    private List<GameObject> listeButtons;

    public GameObject EventButtonPrefab; 

    List<EventEffet> listeEvents = new List<EventEffet>();

    /// <summary>
    /// Entier utilisé pour l'effet (choix de l'ordre). 
    /// </summary>
    private int EventCourant;

    /// <summary>
    /// Nombre total d'event. 
    /// </summary>
    public int EventTotal;

    private bool EffetFini = false;

    private GameObject ButtonFin;

    /// <summary>
    /// Passe à true quand TOUS les effets qui doivent être joués sont finis.
    /// </summary>
    private bool TousEffetsFini = true; 

    /// <summary>
    /// Bouton qui permet de remettre l'ordre des effets à 0
    /// Dans le cas où le joueur s'est trompé et veut changer l'ordre des effets. 
    /// </summary>
    private GameObject ResetOrdreButton; 

	// Use this for initialization
	public override void Start () {
        ButtonFin = GameObject.Find("FinEventManager");
        ButtonFin.SetActive(false);
        ResetOrdreButton = GameObject.Find("ResetOrdre");
        ResetOrdreButton.SetActive(false); 
	}

    public void AjouterEffet(EventEffet ef) {
        listeEvents.Add(ef);
    }

    public void EnleverEffet(EventEffet ef) {
        listeEvents.Remove(ef); 
    }

    /// <summary>
    /// Choix de l'ordre d'un effet. 
    /// Fonction delegate des boutons. 
    /// </summary>
    public void CliqueEffet(int nombre) {
        listeEvents[nombre].EventInt = EventCourant;
        EventCourant++;

        listeButtons[nombre].transform.Find("Number").gameObject.GetComponent<Text>().text = EventCourant.ToString(); 
    }

    /// <summary>
    /// Appui sur le bouton de fin de choix
    /// Il faut vérifier que tous les effets aient bien été choisis. 
    /// </summary>
    public void ChoixFini() {
        if(EventCourant != EventTotal) {
            GameObject.Find("GameManager").GetComponent<GameManager>().DisplayMessage("Vous n'avez pas tout choisi"); 
        }
        ButtonFin.SetActive(false);
        StartCoroutine(JouerEffets()); 
    }

    /// <summary>
    /// Créer un nouvel ensemble d'events
    /// </summary>
    public void CreerNouvellePileEvent() {
        // On reset les booleens à la création de la pile d'event. 
        EffetFini = false;
        TousEffetsFini = false; 

        // S'il n'y a qu'un seul évênement, on ne propose pas de choisir
        if (listeEvents.Count == 0) {
            Debug.Log("Il n'y a pas d'effets dans l'eventManager");
            // On indique que c'est à l'autre joueur de jouer. 
            StartCoroutine(AutoPropose());
            StartCoroutine(ChangerEffetFiniJoueur());
            TousEffetsFini = true;
            return; 
        }

        Debug.Log("<color=purple>Nombre d'effet avec interaction " + NombreEffetsDemandeInteraction() + "</color>"); 

        // Ici il n'y aucun effet à rajouter à la pile. 
        // On peut donc demander à ce joueur s'il veut défaire la pile ou ajouter un effet.
        // Si effet player != 0, alors le joueur est le deuxième à jouer.
        if (NombreEffetsDemandeInteraction() <= 0) {
            // Il faut d'abord jouer les effets sans interaction ici. 
            JouerEffetSansInteraction(); 

            // Puis on s'auto-propose de défaire la pile. 
            StartCoroutine(AutoPropose()); 
        }
        // Sinon c'est ce joueur ci qui demandera à defaire la pile. 
        else {
            // On joue d'abord les effets qui ne demandent pas d'interaction. 
            JouerEffetSansInteraction(); 
            if (NombreEffetsDemandeInteraction() <= 1) {
                // Il y a un effet à jouer. 
                EventTotal = 1; 
                Debug.Log("Il n'y a qu'un seul effet dans l'eventManager");

                // Si le joueur est le deuxieme joueur, il propose de défaire la pile
                FindLocalPlayer().GetComponent<Player>().CmdOnEffetPlayer(FindLocalPlayer().GetComponent<Player>().PlayerID);

                StartCoroutine(JouerEffets()); 
                return;
            }
            // S'il y a plus d'un effet qui demande une interaction. 
            else {
                // On indique à l'autre joueur qu'on est en train de choisir des effets. 
                Debug.Log("Je suis ici");
                FindLocalPlayer().GetComponent<Player>().CmdOnEffetPlayer(FindLocalPlayer().GetComponent<Player>().PlayerID);

                ButtonFin.SetActive(true);
                ResetOrdreButton.SetActive(true);
                EventTotal = listeEvents.Count;
                SetUpButtons();
            }
        }
    }

    /// <summary>
    /// On regarde si le joueur doit s'auto proposer les effets. 
    /// </summary>
    private IEnumerator AutoPropose() {
        Debug.Log(FindLocalPlayer().GetComponent<Player>().EffetPlayer);
        Debug.Log(GameObject.FindGameObjectWithTag("Pile")); 
        // S'il y a des effets à jouer. 
        if (FindLocalPlayer().GetComponent<Player>().EffetPlayer == FindLocalPlayer().GetComponent<Player>().PlayerID
                && GameObject.FindGameObjectWithTag("Pile").GetComponent<PileAppelEffet>().NombreEffetsPile() != 0) {
            // On propose de défaire la pile.
            Debug.Log("On s'auto propose"); 
            yield return GameObject.Find("GameManager").GetComponent<GameManager>().ProposeToPauseGame(0, message: "Voulez réagir aux effets?");
            // Puis on remet l'effet à 0
            FindLocalPlayer().GetComponent<Player>().CmdOnEffetPlayer(0);
        }
    }


    /// <summary>
    /// Créer tous les boutons de choix des events
    /// </summary>
    public void SetUpButtons() {
        Debug.Log("On crée les bouttons"); 
        listeButtons = new List<GameObject>(); 
        for (int i = 0; i < EventTotal; i++) {
            GameObject newButton = Instantiate(EventButtonPrefab);
            newButton.transform.SetParent(transform, false);
            int number = i;
            // Changement de deck. 
            newButton.GetComponent<Button>().onClick.AddListener(delegate { CliqueEffet(number); });
            newButton.AddComponent<EventButton>();
            newButton.GetComponent<EventButton>().IDCarte = listeEvents[i].CarteAssociee.GetComponent<Carte>().IDCardGame; 
            // On informe le joueur de l'effet qui peut être joué ainsi que de la carte qui peut le jouer. 
            newButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = listeEvents[i].effet.EffetString + "\n" + 
                "<color=purple>" + listeEvents[i].CarteAssociee.GetComponent<Carte>().Name + "</color>";
            listeButtons.Add(newButton);
        }
    }

    /// <summary>
    /// Reset tous les evenements. 
    /// </summary>
    public void Reset() {
        Debug.Log("Reset"); 
        listeEvents = new List<EventEffet>();
        EventCourant = 0;
        EventTotal = 0;
    }

    /// <summary>
    /// Jouer tous les effets dans l'eventManager. 
    /// A la fin on reset l'eventManager
    /// </summary>
    public IEnumerator JouerEffets() {
        // On détruit tous les boutons pour qu'ils ne soient plus visibles. 
        TousEffetsFini = false; 
        Debug.Log("On joue les effets");

        if (listeButtons != null) {
            foreach (GameObject o in listeButtons) {
                Destroy(o);
            }
            listeButtons = new List<GameObject>();
        }

        ButtonFin.SetActive(false); 
        ResetOrdreButton.SetActive(false);

        // On joue les effets dans l'ordre de tri
        Debug.Log("Il y a " + EventTotal + " à jouer"); 
        for (int i = 0; i < EventTotal; i++) {

            // Si l'effet n'est pas déclarable, il a déjà été joué avant. 
            if (!FindEffet(i).IsDeclarable()) {
                continue;
            }

            Debug.Log("On joue l'effet " + i);
            // On met TousEffetsFini à true avant de jouer le dernier effet pour que le joueur adverse puisse répondre.
            // On attend la fin de l'effet avant de passer au suivant.


            // Le seul moyen pour un joueur de proposer à son adversaire de jouer est que chacun ait fait ses effets. 
            // Il ne peut pas y avoir d'interaction entre les effets du joueur 1 et du joueur 2
            // car tout est dans le même flux! 
            // On checkera au début du joueur 2 si celui ci a un effet à proposer ou pas, il creera à ce moment lui 
            // même la possibilité de créer un effet
            // EventTotal - 1 == Dernier event. 
            if ((i == EventTotal - 1) 
                && (FindLocalPlayer().GetComponent<Player>().PlayerID != GameObject.Find("GameManager").GetComponent<GameManager>().Tour)) {
                Debug.Log("On propose de défaire la pile"); 
                // Si on est au dernier effet on proposera au joueur de defaire la pile. 
                JouerUnEffet(FindEffet(i), true); 
            } else {
                Debug.Log("On ne propose pas de defaire la pile"); 
                // Sinon on propose au joueur. 
                JouerUnEffet(FindEffet(i), false);
            }
            Debug.Log("On a fini l'effet"); 
            while (!EffetFini) {
                yield return new WaitForSeconds(0.2f); 
            }
            Debug.Log("<color=purple> Un effet est fini</color>"); 
            EffetFini = false;
            if (i == EventTotal - 1) {
                // On a besoin d'avoir changer l'entier d'effet fini pour une vérification dans la pile. 
                yield return ChangerEffetFiniJoueur();
                TousEffetsFini = true;
            }
        }

        Debug.Log("Tous les effets sont finis"); 
        Reset();
    }

    /// <summary>
    /// Une fois que tous les effets ont été vérifiés chez un joueur, on vérifie tous les effets chez l'autre
    /// </summary>
    /// <returns>None</returns>
    private IEnumerator ChangerEffetFiniJoueur() {
        // On indique à l'autre joueur que c'est à lui de joueur.
        int entierAttendu = 0;
        Debug.Log("EffetPlayer" + FindLocalPlayer().GetComponent<Player>().EffetPlayer); 

        // Et on passe l'entier au bon nombre (dans le Player). 
        // Si on est le premier joueur 
        if (FindLocalPlayer().GetComponent<Player>().EffetPlayer == 0 || (FindLocalPlayer().GetComponent<Player>().EffetPlayer == 
            FindLocalPlayer().GetComponent<Player>().PlayerID && FindLocalPlayer().GetComponent<Player>().PlayerID == 
            GameObject.Find("GameManager").GetComponent<GameManager>().Tour)) {  // On change seulement si c'est le joueur dont c'est le tour. 
            // Dans le cas où c'était à nous de jouer en premier.
            entierAttendu = FindNotLocalPlayer().GetComponent<Player>().PlayerID;
        }
        Debug.Log("On change l'effet player en  : " + entierAttendu); 
        FindLocalPlayer().GetComponent<Player>().CmdOnEffetPlayer(entierAttendu);

        // On vérifie que l'information est bien arrivée à l'autre joueur. 

        int decompte = 0; 
        while (FindNotLocalPlayer().GetComponent<Player>().EffetPlayer != entierAttendu && decompte < 10) {
            yield return new WaitForSeconds(0.1f);
            decompte++; 
        }

        // Dans ce cas l'information n'est pas arrivée. 
        if (decompte == 10) {
            // On rappelle la meme methode. 
            Debug.LogError("Probleme lors de l'envoi des informations");
            StartCoroutine(ChangerEffetFiniJoueur()); 
        }
    }

    /// <summary>
    /// Trouve l'effet dans la liste correspondant au nombre recherché. 
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private EventEffet FindEffet(int number) {
        foreach (EventEffet ef in listeEvents) {
            if (ef.EventInt == number) {
                return ef; 
            }
        }
        throw new UnusualBehaviourException("L'effet n'a pas été trouvé");
    }

    /// <summary>
    /// Passe EffetFini à true lorsqu'on trouve la fin d'un effet. 
    /// </summary>
    public void EffetFiniOK() {
        EffetFini = true; 
    }

    /// <summary>
    /// Remettre l'ordre des event à 0. 
    /// </summary>
    public void ResetOrdre() {
        EventCourant = 0;
        foreach (GameObject o in listeButtons) {
            o.transform.Find("Number").gameObject.GetComponent<Text>().text = ""; 
        }
    }

    /// <summary>
    /// Renvoie le nombre d'effets qui demandent un
    /// </summary>
    /// <returns></returns>
    private int NombreEffetsDemandeInteraction() {
        int somme = 0; 
        foreach (EventEffet ef in listeEvents) {
            if (ef.IsDeclarable()) {
                somme++; 
            } else {
            }
        }
        return somme; 
    }

    /// <summary>
    /// Renvoi true si tous les effets sont finis. 
    /// </summary>
    /// <returns></returns>
    public bool IsAllEffetsFini() {
        return TousEffetsFini; 
    }

    /// <summary>
    /// Jouer un effet. 
    /// </summary>
    /// <param name="ef"></param>
    /// <param name="ProposerDefairePile">True si c'est le dernier effet du dernier joueur et qu'il propose de défaire la pile. </param>
    private void JouerUnEffet(EventEffet ef, bool ProposerDefairePile) {
        // il faut recréer une liste d'effets avec effet dans la bonne position, pour qu'il n'y ait pas de problèems
        // lors de la transmission des infos..
        // On mettra donc des elements à null pour compléter
        if (!ef.CarteAssociee.GetComponent<Carte>().GererEffets(numeroListEffet:ef.effet.numeroListEffet, jouerDirect: true, 
                ProposerDefairePile:ProposerDefairePile)) {
            throw new UnusualBehaviourException("Cet effet aurait du etre joué"); 
        }
    }

    /// <summary>
    /// On joue tous les effets sans interaction, ceux qui ne rentrent pas dans le flux en premier, 
    /// pour s'en débarassern puis on jouera les suivants. 
    /// </summary>
    private void JouerEffetSansInteraction() {
        foreach (EventEffet ef in listeEvents) {
            if (!ef.IsDeclarable()) {
                JouerUnEffet(ef, false); 
            }
        }
    }

    /// <summary>
    /// Renvoie la liste d'event qui demandent une interaction. 
    /// </summary>
    /// <returns></returns>
    private List<EventEffet> EffetsDemandeInteraction() {
        List<EventEffet> interaction = new List<EventEffet>();
        foreach (EventEffet ef in listeEvents) {
            Debug.Log(ef); 
            if (ef.IsDeclarable()) {
                interaction.Add(ef); 
            }
        }
        return interaction; 
    } 
}
