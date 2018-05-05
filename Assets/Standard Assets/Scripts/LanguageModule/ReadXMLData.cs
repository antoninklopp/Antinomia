
// Copyright (c) 2017-2018 Antonin KLOPP-TOSSER
 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Antinomia.Battle;

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

	// Use this for initialization
	void Start () {
		filepath = Application.dataPath + @"/Data/gamexmldata.xml";
	}

}
