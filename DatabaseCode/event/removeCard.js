// ====================================================================================================
//
// Cloud Code for removeCard, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

var shortCode = Spark.getData().shortCode; 
var cardID = Spark.getData().cardID; 
var cardsToRemove; 
// On enlève une carte de toutes les cartes du joueur, pas d'un deck!

// on cherche la carte à enlever du deck
// Pour l'instant on enlève les cartes une par une
if (shortCode == "None" && cardID == 0){
    Spark.setScriptError("Rentrer une id ou un nom de carte", shortCode); 
} else {
    if (cardID != 0){
        cardsToRemove = Spark.runtimeCollection('player_allCards').find({'player_id' : Spark.getPlayer().getPlayerId(), 'card_ID' : cardID}, {"_id" : 1}).limit(1);
    } else {
        cardsToRemove = Spark.runtimeCollection('player_allCards').find({'player_id' : Spark.getPlayer().getPlayerId(), 'card.shortCode' : shortCode}, {"_id" : 1}).limit(1);
    }
}
var cardsCount = cardsToRemove.count(); 

if (cardsCount > 0){
    while(cardsToRemove.hasNext()){
        Spark.runtimeCollection('player_allCards').remove({'_id' : { "$oid" : 
        cardsToRemove.next()._id.$oid }}); 
    }
    Spark.setScriptData('items-removed', cardsCount);
    Spark.setScriptData('items-shortCode', shortCode); 
} else {
    Spark.setScriptData('cardsCount', cardsCount);
    Spark.getLog().debug("Remove Item | Player Has No Items With ShortCode - " + shortCode); 
    Spark.setScriptError('no-item-to-remove', shortCode); 
}