// ====================================================================================================
//
// Cloud Code for removeDeck, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

var deckID = Spark.getData().deckID; 
var decksToRemove; 

decksToRemove = Spark.runtimeCollection('decks').find({"player_id" : Spark.getPlayer().getPlayerId(), "number" : deckID}, {"_id" : 1}).limit(1);

var deckCount = decksToRemove.count(); 

Spark.setScriptData("numberOf Decsk", deckID);

if (deckCount > 0){
    while(decksToRemove.hasNext()){
        Spark.runtimeCollection('decks').remove({'_id' : { "$oid" : 
        decksToRemove.next()._id.$oid }}); 
    }
    Spark.setScriptData('decks-removed', deckCount);
    Spark.setScriptData('decks-shortCode', deckID); 
    
    // Il faut remettre les decks dans l'ordre. 
    var decksToRename = Spark.runtimeCollection('decks').find({"player_id" : Spark.getPlayer().getPlayerId()}); 
    for (i=1; i < decksToRename.count(); i++){
        if (decksToRename.number > deckID){
            // On décale tous les decks qui on un numéor supérieur vers la gauche.
            // TODO : A tester
            Spark.runtimeCollection('decks').update({"player_id" : Spark.getPlayer().getPlayerId(), "number" : decksToRename.number}, 
                {"player_id" : Spark.getPlayer().getPlayerId(), "number" : decksToRename.number - 1}); 
        }
    }
} else {
    Spark.setScriptData('decksCount', deckCount);
    Spark.getLog().debug("Remove Item | Player Has No Items With ShortCode - " + deckID); 
    Spark.setScriptError('no-item-to-remove', deckID); 
}