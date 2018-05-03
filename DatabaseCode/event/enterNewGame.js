// ====================================================================================================
//
// Cloud Code for enterNewGame, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// Met à jour le nombre de game faites par un joueur
//
// ====================================================================================================

var currentNumberGame = Spark.getPlayer().getScriptData("gameNumber");
if (currentNumberGame == null){
    // Si c'est la première game du joueur
    Spark.getPlayer().setScriptData("gameNumber", 1); 
} else {
    Spark.getPlayer().setScriptData('gameNumber', currentNumberGame+1); 
}
