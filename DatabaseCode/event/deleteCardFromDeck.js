// ====================================================================================================
//
// Cloud Code for deleteCardFromDeck, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
// TODO: A modifier car ne fonctionne pas. 
// La carte est bien trouvée mais elle n'est pas enlevée du deck. 
//
// ====================================================================================================

var cardID = Spark.getData().cardID; 
var deckID = Spark.getData().deckID; 

// On récupère la carte à enlever du deck grâce à son ID et à l'ID du deck. 
var deckWithCard = Spark.runtimeCollection('decks').findOne({
            'player_id' : Spark.getPlayer().getPlayerId(), 'number' : deckID}); 
var cardToRemoveoID = Spark.runtimeCollection("player_allCards").findOne({'player_id' : Spark.getPlayer().getPlayerId(), 
                        "card_ID" : cardID})._id.$oid; 
var cardToRemove = null; 

Spark.setScriptData("cardsAll",  Spark.runtimeCollection('decks').findOne({
            'player_id' : Spark.getPlayer().getPlayerId(), 'number' : deckID}));
Spark.setScriptData("card", Spark.runtimeCollection("player_allCards").findOne({'player_id' : Spark.getPlayer().getPlayerId(),
                        "card_ID" : cardID})); 

for (i = 0; i < deckWithCard["cards"].length; ++i){
    if (deckWithCard["cards"][i] == cardToRemoveoID){
        cardToRemove = deckWithCard["cards"][i];
        break; 
    }
}

// On enlève la carte. 
var deckWithCard2 = deckWithCard["cards"]; 
deckWithCard2.splice(i, 1); 

if (cardToRemove != null){
    // Si on a trouvé la carte à enlever. 
    // Pour l'instant return true mais ne fonctionne pas.
    var success = Spark.runtimeCollection('decks').update({
            'player_id' : Spark.getPlayer().getPlayerId(), 'number' : deckID}, {'$set' : {"cards" : deckWithCard2}});
    Spark.setScriptData('nombre de cartes enlevées', cardToRemove);
    Spark.setScriptData('le deck', Spark.runtimeCollection('decks').findOne({'number' : deckID})); 
    Spark.setScriptData('reussi', success); 
} else {
    // Si on a pas trouvé la carte à enlever.
    Spark.getLog().debug("Remove Item | Player Has No Items With ShortCode - " + cardID); 
    Spark.setScriptData("Carte non trouvée.", cardID); 
}