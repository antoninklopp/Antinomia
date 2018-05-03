// ====================================================================================================
//
// Cloud Code for ResetDeck, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

var number = Spark.getData().number; 

// On enlève toutes les cartes d'un deck. 
Spark.runtimeCollection('decks').update({'player_id' : Spark.getPlayer().getPlayerId(), 'number' : number},
        {'$set' : 
            {"cards" : []}
        }); 
        
Spark.setScriptData("reset", number); 