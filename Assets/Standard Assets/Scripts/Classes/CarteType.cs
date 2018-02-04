using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

/// <summary>
/// Cette classe de type aura pour but une fois une carte instanciée de pouvoir désactiver le(s) script(s) inutile à la carte. 
/// Exemple, si la carte est un sort, on ne gardera actif que la classe sort.
/// </summary>
public class CarteType : NetworkBehaviourAntinomia {

    /// <summary>
    /// Type de la carte
    /// </summary>
    public enum Type {
        /// <summary>
        /// La carte est une entité.
        /// </summary>
        ENTITE,
        /// <summary>
        /// La carte est un sort.
        /// </summary>
        SORT,
        /// <summary>
        /// La carte est une assistance. 
        /// </summary>
        ASSISTANCE,
        /// <summary>
        /// La carte est une émanation. 
        /// </summary>
        EMANATION,
        /// <summary>
        /// Une entité temporaire, 
        /// instanciée lors d'une invocation par aka ou élémentaire. 
        /// </summary>
        ENTITE_TEMPORAIRE, 
        /// <summary>
        /// Equivalent d'une entité temporaire pour les assistances. 
        /// </summary>
        ASSISTANCE_TEMPORAIRE,
        /// <summary>
        /// La carte n'est d'aucun type. 
        /// </summary>
        AUCUN
    };


    public Type thisCarteType = Type.AUCUN;

    /// <summary>
    /// True si la carte a été instanciée
    /// </summary>
    public bool instanciee = false; 

    // Use this for initialization
    public override void Start () {
        StartCoroutine(SetUpCard()); 
	}

    /// <summary>
    /// Changer le type de la carte grâce au string donné au paramètre. 
    /// Agit directement sur le thisTypeCarte de la classe.
    /// </summary>
    /// <param name="_type"></param>
    public void setTypeFromString(string _type) {
        switch (_type) {
            case "entité":
                thisCarteType = Type.ENTITE;
                break;
            case "sort":
                thisCarteType = Type.SORT;
                break;
            case "assistance":
                thisCarteType = Type.ASSISTANCE;
                break;
            case "emanation":
                thisCarteType = Type.EMANATION;
                break; 
            default:
                throw new Exception("Le type est inconnu"); 
        }
    }

    /// <summary>
    /// Lors du spawn de la carte on lui envoie des attributs
    /// </summary>
    /// <returns>None</returns>
    IEnumerator SetUpCard() {
        yield return new WaitForSeconds(0.04f);

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        if (((Players[0].GetComponent<Player>().isLocalPlayer && Players[0].GetComponent<Player>().isServer) ||
            (Players[1].GetComponent<Player>().isLocalPlayer && Players[1].GetComponent<Player>().isServer)) && netId.Value != 0) {
            // Dans le cas d'une instantiation d'une carte sur le réseau.
            RpcSetType(thisCarteType); 
            // Inutile normalement.
            // RpcChangeParent (); 
        }

        yield return new WaitForSeconds(0.1f); 

        if (thisCarteType == Type.AUCUN) {
            yield break; 
        }

        instanciee = true; 

        switch (thisCarteType) {
            case Type.SORT:
                DetruireComposant(Type.ENTITE);
                DetruireComposant(Type.ASSISTANCE);
                DetruireComposant(Type.EMANATION); 
                break;
            case Type.ENTITE:
                DetruireComposant(Type.SORT);
                DetruireComposant(Type.ASSISTANCE);
                DetruireComposant(Type.EMANATION);
                break;
            case Type.ASSISTANCE:
                DetruireComposant(Type.SORT);
                DetruireComposant(Type.ENTITE);
                DetruireComposant(Type.EMANATION);
                break;
            case Type.EMANATION:
                DetruireComposant(Type.SORT);
                DetruireComposant(Type.ENTITE);
                DetruireComposant(Type.ASSISTANCE);
                break;
            case Type.ENTITE_TEMPORAIRE:
                break; 
            default:
                throw new Exception("Cette carte n'a pas de type");
        }
    }

    /// <summary>
    /// Changer le type sur tous les clients.
    /// </summary>
    /// <param name="_type"></param>
    [ClientRpc(channel=0)]
    void RpcSetType(Type _type) {
        /*
         * Ecrire le bon type sur tous les clients. 
         */ 
        thisCarteType = _type; 
    }

    /// <summary>
    /// Sert à détruire les composants inutiles d'une carte
    /// </summary>
    /// <param name="carteType"></param>
    private void DetruireComposant(Type carteType) {
        // Il faut vérifier à chaque fois que le composant n'a pas été détruit. 
        switch (carteType) {
            case Type.ENTITE:
                if (GetComponent<Entite>() != null) {
                    Destroy(GetComponent<Entite>()); 
                }
                break;
            case Type.EMANATION:
                if (GetComponent<Emanation>() != null) {
                    Destroy(GetComponent<Emanation>());
                }
                break;
            case Type.SORT:
                if (GetComponent<Sort>() != null) {
                    Destroy(GetComponent<Sort>());
                }
                break;
            case Type.ASSISTANCE:
                if (GetComponent<Assistance>() != null) {
                    Destroy(GetComponent<Assistance>());
                }
                break;
            default:
                Debug.Log("Ce composant n'existe pas");
                break; 
        }
    }

    /// <summary>
    /// Récupérer le type de la carte
    /// </summary>
    /// <returns>Type de la carte. </returns>
    public Type getType() {
        return thisCarteType;
    } 
}
