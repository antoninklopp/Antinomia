using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking; 

public class CarteType : NetworkBehaviour {
    /*
     * Cette classe de type aura pour but une fois une carte instanciée de pouvoir désactiver le(s) script(s) inutile à la carte. 
     * Exemple, si la carte est un sort, on ne gardera actif que la classe sort. 
     */ 

    public enum Type { ENTITE, // La carte est une entité
        SORT, // La carte est un sort
        ASSISTANCE, // La carte est une assistance. 
        AUCUN
    };

    public Type thisCarteType = Type.AUCUN; 

    // Use this for initialization
    void Start () {
        StartCoroutine(SetUpCard()); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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

        Debug.Log(thisCarteType); 
    }

    [ClientRpc]
    void RpcSetType(Type _type) {
        /*
         * Ecrire le bon type sur tous les clients. 
         */ 
        thisCarteType = _type; 
    }
}
