// ====================================================================================================
//
// Cloud Code for createDeck, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

// on regarde le nombre de decks déjà présents. 

var numberOfDeck = Spark.getData().number; 

var countList = Spark.runtimeCollection('decks').find({
        'player_id' : Spark.getPlayer().getPlayerId() }, { "_id" : 0}).count(); 

if (numberOfDeck == 0){
    numberOfDeck = countList + 1; 
}

var newDeck = {
    "player_id" : Spark.getPlayer().getPlayerId(), // On ajoute l'ID du player
    "number" : numberOfDeck,
    // Toutes les cartes du deck. 
    "cards" : []
}

Spark.runtimeCollection('decks').insert(newDeck);
Spark.setScriptData('deck created', countList + 1); 