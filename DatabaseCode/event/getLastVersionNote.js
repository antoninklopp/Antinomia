// ====================================================================================================
//
// Cloud Code for getLastVersionNote, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

// Langue dans laquelle on demande la version. 
var langue = Spark.getData().langue; 

var lastVersion = Spark.metaCollection("version").find().toArray()[Spark.metaCollection("version").find().toArray().length - 1]; 

Spark.setScriptData("VersionNumber", lastVersion.versionNumber);
Spark.setScriptData("VersionNote", lastVersion.versionNote); 
Spark.setScriptData("UpdateNecessary", lastVersion.necessaryUpdate); 