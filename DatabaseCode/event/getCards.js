// ====================================================================================================
//
// Cloud Code for getCards, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

// Récupérer les cartes de la base de données globale

require("utils"); 

// On crée la variable de retour. 
var AllCardsNormal = []; 

var type = Spark.getData().type; 
// On récupère la langue que l'on veut. 
var langue = Spark.getData().langue; 

if (type != "" && type != "all"){
    //Spark.setScriptData('cards', Spark.metaCollection('cards').find({"type" : type}));
} else {
    var allCards = Spark.metaCollection('cards').find().toArray(); 
    for (i = 0; i < allCards.length; i++){
        AllCardsNormal.push(getCard(allCards[i]._id.$oid, langue)); 
    }
    Spark.setScriptData('cards', AllCardsNormal); 
}