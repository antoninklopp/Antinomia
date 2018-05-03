// ====================================================================================================
//
// Cloud Code for utils, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

// Récupérer une carte à partir de son oID et de sa langue. 
// Il faut pouvoir exporter cette fonction
/*
Récupérer une carte dans une certaine langue
*/
function getCard(oID, langue){
    // On récupère la carte
    var Card = Spark.metaCollection('cards').findOne({"_id" : {"$oid" : oID}}); 
    var CarteLangue = Spark.metaCollection(langue).findOne({"shortCode" : Card.shortCode});
    
    if (CarteLangue != null){
        // Si la carte est a null c'est qu'elle n'a pas encore été implémentée 
        // dans la base de données des langues
        Card.name = CarteLangue.name; 
        Card.description = CarteLangue.description; 
        Card.EffetString = CarteLangue.EffetString; 
        
        if (Card.type == "entité" || Card.type == "entite"){
            Card.MalefiqueString = CarteLangue.MalefiqueString; 
            Card.AstralString = CarteLangue.AstralString; 
        }
    }
    
    return Card; 
}

/*
Récupérer uniquement les infos de langue d'une carte
*/
function getCardLanguageInfo(oID){
    var Card = Spark.metaCollection('cards').findOne({"_id" : {"$oid" : oID}}); 
    
    var CarteLangue = []; 
    
    if (Card != null){
        CarteLangue.shortCode = Card.shortCode; 
        CarteLangue.description = Card.description; 
        CarteLangue.name = Card.name; 
        CarteLangue.EffetString = Card.EffetString; 
        
        if (Card.type == "entité" || Card.type == "entite"){
            CarteLangue.MalefiqueString = Card.MalefiqueString; 
            CarteLangue.AstralString = Card.AstralString; 
        }
        
    }
    
    return CarteLangue;
}