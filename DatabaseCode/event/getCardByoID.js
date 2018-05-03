// ====================================================================================================
//
// Cloud Code for getCardByoID, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

require("utils"); 

var ID = Spark.getData().ID;
var langue = Spark.getData().langue; 

// Spark.setScriptData('cards', Spark.metaCollection('cards').findOne({"_id" : {"$oid" : ID}}));
Spark.setScriptData('cards', getCard(ID, langue)); 