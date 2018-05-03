// ====================================================================================================
//
// Cloud Code for createMetaDataLanguage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

require("utils"); 

var langue = Spark.getData().langue; 

var allCartes =  Spark.metaCollection('cards').find().toArray();

for (i = 0; i < allCartes.length; i++){
    var CarteLangue = Spark.metaCollection(langue).findOne({"shortCode" : allCartes[i].shortCode}); 
    // Si la carte n'existe pas dans la base de données
    if (CarteLangue == null){
        CarteLangue = getCardLanguageInfo(allCartes[i]._id.$oid);
        if (allCartes[i].type == "entité" || allCartes[i].type == "entite" || allCartes[i].type == "emanation"){
        Spark.metaCollection(langue).update(
            {"shortCode" : CarteLangue.shortCode }, 
            {
                $set : { // on remplit tous les champs importants
                   
                    "name" : CarteLangue.name, 
                    "description" : CarteLangue.description,
                    "EffetString" : CarteLangue.EffetString, 
                    "AstralString" : CarteLangue.AstralString, 
                    "MalefiqueString" : CarteLangue.MalefiqueString, 
                }
            },
            true, 
            false); 
        } else {
            Spark.metaCollection(langue).update(
            {"shortCode" : CarteLangue.shortCode }, 
            {
                $set : { // on remplit tous les champs importants
                   
                    "name" : CarteLangue.name, 
                    "description" : CarteLangue.description,
                    "EffetString" : CarteLangue.EffetString, 
                }
            },
            true, 
            false); 
            
        }
    }
}