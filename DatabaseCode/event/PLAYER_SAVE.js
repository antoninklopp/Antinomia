// ====================================================================================================
//
// Cloud Code for PLAYER_SAVE, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// Ici on sauve l'état du joueur. 
//
//
// ====================================================================================================
var playerDataList = Spark.runtimeCollection("player_data");
var playerID = Spark.getPlayer().getPlayerId(); 
var playerLevel = Spark.getData().LEVEL; 
var playerGames = Spark.getData().NUMBER_OF_GAMES; 

var currentPlayer = {
    "playerID":playerID,
    "playerLevel":playerLevel,
    "playerGames":playerGames
}

// On update le statut du joueur. 
playerDataList.update({
    "playerID":playerID
},
{
    $set:currentPlayer
}, 
true, 
true);