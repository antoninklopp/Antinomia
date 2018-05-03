// ====================================================================================================
//
// Cloud Code for addACommentBeta, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

/*
Permettre aux utilisateurs de la phase de beta de faire des retours sur le jeu, 
et ce depuis n'importe quelle plateforme.
*/

var nameTester  = Spark.getData().name; 
var comment = Spark.getData().comment;

var commentAndName = {
    "testeur" : nameTester, 
    "Commentaire" : comment
}

Spark.runtimeCollection("commentaires_Beta").insert(commentAndName); 