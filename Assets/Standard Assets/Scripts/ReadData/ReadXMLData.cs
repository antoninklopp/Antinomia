using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class ReadXMLData {
	/* 
	 * TODO: A changer une fois que les classes créatures et sorts seront connues. 
	 */

	public string Name; 
	public int ID; 
	public int _PV; 
	public int _CoutAKA; 
	public Entite.Element carteElement; 
	//public Carte.Type carteType;
	public Entite.Ascendance carteAscendance; 
	public int _coutElementaire; 
	public Capacite carteCapacite; 
	// public Entite.EffetMalefique carteEffetMalefique; 
	// public Entite.EffetAstral carteEffetAstral; 
	public int puissance;

	string filepath; 

	public void LoadFromXml(string name)
	{
		filepath = "cartes"; 
		XmlDocument xmlDoc = new XmlDocument(); 

		TextAsset textAsset = (TextAsset)Resources.Load(filepath, typeof(TextAsset));
		xmlDoc.LoadXml(textAsset.text); 
		XmlNodeList transformList = xmlDoc.GetElementsByTagName(name);
		foreach (XmlNode transformInfo in transformList)
		{
			XmlNodeList transformcontent = transformInfo.ChildNodes;
			//CheckValueAndLoadStrings(xmlDoc, name);
			foreach (XmlNode transformItens in transformcontent) 
			{
				//CheckValueAndLoadInt(transformItens, xmlDoc);
			}
		}

	}

//	public void CheckValueAndLoadStrings(XmlDocument xmlDoc, string name)
//	{
//		//Très important de mettre transforms.
//		XmlNode node = xmlDoc.SelectSingleNode ("transforms/" + name + "/Attaque1"); 
//		attaque1 = node.InnerText;
//		XmlNode node2 = xmlDoc.SelectSingleNode ("transforms/" + name + "/Attaque2"); 
//		attaque2 = node2.InnerText;
//		XmlNode node4 = xmlDoc.SelectSingleNode ("transforms/" + name + "/AttaqueInfo1"); 
//		attaqueInfo1 = node4.InnerText;
//		XmlNode node5 = xmlDoc.SelectSingleNode ("transforms/" + name + "/AttaqueInfo2"); 
//		attaqueInfo2 = node5.InnerText;
//		XmlNode node6 = xmlDoc.SelectSingleNode ("transforms/" + name + "/photo"); 
//		photo = node6.InnerText;
//		XmlNode node7 = xmlDoc.SelectSingleNode ("transforms/" + name + "/sac"); 
//		if (node7.InnerText.Equals ("oui")) {
//			sac = true; 
//		} else {
//			sac = false; 
//		}
//		XmlNode node8 = xmlDoc.SelectSingleNode ("transforms/" + name + "/evilornot"); 
//		if (node8.InnerText.Equals ("méchant")) {
//			gentil = false; 
//		} else {
//			gentil = true;
//		}
//		XmlNode node9 = xmlDoc.SelectSingleNode ("transforms/" + name + "/Description"); 
//		Description = node9.InnerText;
//		XmlNode node10 = xmlDoc.SelectSingleNode ("transforms/" + name + "/celldex"); 
//		if (node10.InnerText.Equals ("oui")) {
//			celldex = true; 
//		} else {
//			celldex = false; 
//		}
//		XmlNode node11 = xmlDoc.SelectSingleNode ("transforms/" + name + "/fightagainst"); 
//		fightAgainst = node11.InnerText;
//	}
//
//	private void CheckValueAndLoadInt(XmlNode transformItens, XmlDocument xmlDoc)
//	{
//		if (transformItens.Name == "pointsAttaque1")
//		{
//			pointsAttaque1 = int.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "pointsAttaque2")
//		{
//			pointsAttaque2 = int.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "PV")
//		{
//			PV = int.Parse(transformItens.InnerText);
//		} 
//		if (transformItens.Name == "abondance_tissu")
//		{
//			abondancetissu = float.Parse(transformItens.InnerText);
//		} 
//		if (transformItens.Name == "abondance_sang")
//		{
//			abondancesang = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_lymphe")
//		{
//			abondanceLymphe = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_jambe")
//		{
//			abondanceJambe = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_moelle")
//		{
//			abondanceMoelle = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_pulpe_rouge")
//		{
//			abondancePulpeRouge = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_pulpe_blanche")
//		{
//			abondancePulpeBlanche = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_cortex")
//		{
//			abondanceCortex = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_medulla")
//		{
//			abondanceMedulla = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_cortex_ganglion")
//		{
//			abondanceCortexGanglion = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_medulla_ganglion")
//		{
//			abondanceMedullaGanglion = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_paracortex")
//		{
//			abondanceParacortex = float.Parse(transformItens.InnerText);
//		}
//		if (transformItens.Name == "abondance_follicule")
//		{
//			abondanceFollicule = float.Parse(transformItens.InnerText);
//		}
//
//	}

	// Use this for initialization
	void Start () {
		filepath = Application.dataPath + @"/Data/gamexmldata.xml";
	}

	// Update is called once per frame
	void Update () {

	}

//	public Carte retourneCarte(string namme){
//		/*
//		 * Retour de toutes les informations de le cellule. 
//		 * En donnant le nom de l'name. 
//		 * 
//		 */ 
//		LoadFromXml (name); 
//		Carte _carte = new Carte (); 
//
//		return _carte; 
//	}

}
