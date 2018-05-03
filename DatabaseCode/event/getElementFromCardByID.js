// ====================================================================================================
//
// Cloud Code for getElementFromCardByID, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

var ID = Spark.getData().ID;

Spark.setScriptData('Element', Spark.metaCollection('cards').findOne({"_id" : {"$oid" : ID}})["Element"]);