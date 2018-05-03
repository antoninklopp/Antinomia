// ====================================================================================================
//
// Cloud Code for getPlayerDeck, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

//On récupère toutes les cartes du joueur!
// TODO : Ajouter la fonction getCard. 

require("utils"); 

var type = Spark.getData().type; 
var langue = Spark.getData().langue; 
var allCardsoID; 

if (type != "all" && type != ""){
    allCardsoID = Spark.runtimeCollection('player_allCards').find({
        'player_id' : Spark.getPlayer().getPlayerId(), 
        'card.type' : type }).toArray();
} else {
    allCardsoID = Spark.runtimeCollection('player_allCards').find({
        'player_id' : Spark.getPlayer().getPlayerId()}).toArray(); 
}


    
var allCardsNormal = []; 

for (i = 0; i < allCardsoID.length; i++){
    allCardsNormal.push(getCard(allCardsoID[i].card, langue));
    allCardsNormal[i].oID = allCardsNormal[i]._id.$oid; 
    allCardsNormal[i]._id.$oid = allCardsoID[i]._id.$oid;  
    allCardsNormal[i].card_ID = allCardsoID[i].card_ID; 
}
    
Spark.setScriptData('cards', allCardsNormal); 