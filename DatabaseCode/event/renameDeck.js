// ====================================================================================================
//
// Cloud Code for renameDeck, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

// Changer le nom d'un deck
var nameDeck = Spark.getData().name; 
var deckID = Spark.getData().deckID; 

// On update dans la collection
Spark.runtimeCollection('decks').update({'player_id' : Spark.getPlayer().getPlayerId(), 'number' : deckID},
        {'$set' : 
            {"name" : nameDeck}
        }, false, true); 
        
Spark.setScriptData("name", nameDeck)