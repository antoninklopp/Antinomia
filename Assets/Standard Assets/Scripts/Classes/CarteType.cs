using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

/// <summary>
/// Cette classe de type aura pour but une fois une carte instanciée de pouvoir désactiver le(s) script(s) inutile à la carte. 
/// Exemple, si la carte est un sort, on ne gardera actif que la classe sort.
/// </summary>
public class CarteType : NetworkBehaviour {

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
    void Start () {
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
            default:
                throw new Exception("Le type est inconnu"); 
        }
    }

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
                Destroy(GetComponent<Entite>());
                Destroy(GetComponent<Assistance>()); 
                break;
            case Type.ENTITE:
                Destroy(GetComponent<Sort>());
                Destroy(GetComponent<Assistance>());
                break;
            case Type.ASSISTANCE:
                Destroy(GetComponent<Sort>());
                Destroy(GetComponent<Entite>());
                break; 
            default:
                throw new Exception("Cette carte n'a pas de type");
        }
    }


    [ClientRpc]
    void RpcSetType(Type _type) {
        /*
         * Ecrire le bon type sur tous les clients. 
         */ 
        thisCarteType = _type; 
    }
}
