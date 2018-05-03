// ====================================================================================================
//
// Cloud Code for getDecks, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

// Récupérer un ou tous les decks du joueur. 

require("utils");

var number = Spark.getData().number; 
var langue = Spark.getData().langue; 

var decks; 

var allCardsoID; 
var decksWithAllCards = []; 

// On définit le nombre 0 , comme la manière de récupérer tous les decks. 
if (number != 0 ){
    decks = Spark.runtimeCollection('decks').find({
        'player_id' : Spark.getPlayer().getPlayerId(), 
        'number' : number }).sort({"number" : 1}).toArray();
} else {
    decks = Spark.runtimeCollection('decks').find({
        'player_id' : Spark.getPlayer().getPlayerId()}).sort({"number" : 1}).toArray(); 
}
 
//Spark.setScriptData("decks", decks); 
for (i = 0; i < decks.length; ++i){
    allCardsoID = []; 
    for (j = 0; j < decks[i]["cards"].length; ++j){ 
        // oID de la carte du joueur
        var cardoID = Spark.runtimeCollection('player_allCards').findOne({"_id" : { "$oid" : decks[i]["cards"][j]}}); 
        // oID de la carte en général
        allCardsoID.push(getCard(cardoID.card, langue)); 
        allCardsoID[j].oID = allCardsoID[j]._id.$oid; 
        allCardsoID[j]._id.$oid = decks[i]["cards"][j]; 
        allCardsoID[j].card_ID = cardoID.card_ID; 
    }
    var newDeck = {
        "number" : i + 1,
        "name" : decks[i].name, 
        "cards" : allCardsoID
    }
    decksWithAllCards.push(newDeck); 
}


Spark.setScriptData("decks", decksWithAllCards);
