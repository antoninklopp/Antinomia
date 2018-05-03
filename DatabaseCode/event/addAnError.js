// ====================================================================================================
//
// Cloud Code for addAnError, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

var _message = Spark.getData().message; 
var _Date = Spark.getData().Date; 
var _name = Spark.getPlayer().getUserName(); 

var comment = {
    "name " : _name, 
    "Date " : _Date,
    "message " : _message
}

Spark.runtimeCollection("errors").insert(commentAndName); 