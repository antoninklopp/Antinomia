// ====================================================================================================
//
// Cloud Code for getSTATFromCardByID, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

/*
Récupérer la stat d'une carte grâce à son oID
*/
var ID = Spark.getData().ID;

Spark.setScriptData('STAT', Spark.metaCollection('cards').findOne({"_id" : {"$oid" : ID}})["STAT"]);