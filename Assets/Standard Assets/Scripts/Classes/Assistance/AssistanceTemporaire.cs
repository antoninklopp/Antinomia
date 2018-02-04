using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistanceTemporaire : Carte {

    public Assistance.State carteState;

    // Use this for initialization
    public override void Start() {

    }

    public override void OnMouseDown() { }
    public override void OnMouseDrag() { }

    /// <summary>
    /// Définition d'une entité temporaire à partir d'une autre entité. 
    /// </summary>
    /// <param name="_assistance"></param>
    public AssistanceTemporaire(Assistance _assistance) {
        setInfoEntiteTemporaire(_assistance);
    }

    public void setInfoEntiteTemporaire(Assistance _assistance) {
        Name = _assistance.Name;
        IDCardGame = _assistance.IDCardGame;
        shortCode = _assistance.shortCode;

        // Comme l'entité est temporaire, on l'indique aussi avec un alpha de 1/2
        Color ancienneCouleur = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(ancienneCouleur.r, ancienneCouleur.g, ancienneCouleur.b, 0.5f);

        gameObject.tag = "CarteTemporaire";

        GetComponent<ImageCardBattle>().setImage(shortCode);
        GetComponent<CarteType>().thisCarteType = CarteType.Type.ASSISTANCE_TEMPORAIRE;
        GetComponent<CarteType>().instanciee = false;
    }

    /// <summary>
    /// Detruire l'entité temporaire.
    /// </summary>
    public void DetruireTemporaire() {
        Destroy(gameObject);
    }

    public GameObject getVraieAssistance() {
        return NetworkBehaviourAntinomia.FindCardWithID(IDCardGame);
    }

}
