// ====================================================================================================
//
// Cloud Code for addCardToDeck, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

// On récupère l'ID de la carte, et l'ID du deck
var cardID = Spark.getData().cardID;
var deckID = Spark.getData().deckID;

// Carte récupérée 
var card; 

// On récupère la carte. 
card = Spark.runtimeCollection('player_allCards').findOne({'player_id':Spark.getPlayer().getPlayerId(), "card_ID" : cardID})._id.$oid; 
var deck = Spark.runtimeCollection('decks').findOne({'player_id':Spark.getPlayer().getPlayerId(), 'number' : deckID}); 

var deckArray = deck["cards"]; 

/*
On regarde si la carte n'est pas déjà présente dans le deck. 
*/
var countArray = deckArray.length;  
var existsIn = false; 

for (i = 0; i < countArray; i++){
    // Spark.setScriptData("AH", cardID); 
    // Spark.setScriptData("AHH", deckArray[i]["card_ID"]);
    if (deckArray[i]["card_ID"] == cardID){
        existsIn = true; 
        break; 
    }
}

if (existsIn === true){
    // La carte est déjà dans le deck. 
    Spark.setScriptData('La carte est déjà dans le deck', card); 
}
else {
    // La carte n'est pas dans le deck. 
    Spark.setScriptData('La carte n est pas dans le deck', card); 
    Spark.setScriptData('les cartes du deck 1 ', deck["cards"]);
    // On insere la carte dans le deck au rang "countArray" c'est-à-dire en dernier dans la liste. 
    deckArray.splice(countArray, 0, card); 
    // Spark.setScriptData('les cartes du deck', deckArray);
    // Spark.setScriptData('les cartes du deck 2 ', deck["cards"]);
    // On update la runtimeCollection. 
    var success = Spark.runtimeCollection('decks').update({'player_id' : Spark.getPlayer().getPlayerId(), 'number' : deckID},
        {'$set' : {"cards" : deckArray}});
    Spark.setScriptData("ca a marché", success); 
    deck = Spark.runtimeCollection('decks').findOne({'number' : deckID}); 
    Spark.setScriptData("nouveau deck", deck); 
    
}
        
    