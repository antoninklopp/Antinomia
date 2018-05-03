// ====================================================================================================
//
// Cloud Code for addMonnaie, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// Créditer le compte d'un joueur. 
//
// ====================================================================================================

//Spark.getPlayer().credit1(Spark.getData().amount);
Spark.getPlayer().credit("Monnaie", Spark.getData().value, "No reason");