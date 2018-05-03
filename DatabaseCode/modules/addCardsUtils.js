// ====================================================================================================
//
// Cloud Code for addCardsUtils, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

function addCardsBeginning(number){
    
    var allCards = Spark.metaCollection('cards').find(); 
    var allCardsCount = allCards.count(); 
    var IDCard = 1; 
    var card; 
    var shortCode; 

    for (j = 0; j < allCardsCount; ++j){
        for (i = 0; i < number; ++i){
            shortCode = allCards.toArray()[j].shortCode; 
            
            card = Spark.metaCollection('cards').findOne({"shortCode" : shortCode})._id.$oid; 
        
            var newDeckCard = { // New Card in the Deck
                "player_id" : Spark.getPlayer().getPlayerId(), // we need the player id to be able 
                // to get the player's full inventory later on
                // Il faut rajouter une ID de carte pour pouvoir réussir à l'insérer dans des decks. 
                "card_ID" : IDCard,
                "card" : card
            }
            
            IDCard++; 
            // Le deck du joueur sera appelé player_deck
            Spark.runtimeCollection('player_allCards').insert(newDeckCard); 
        
        }
    }
    
    // On crée aussi 3 dekcs
    for (k = 0; k < 3; ++k){
        
        var newDeck = {
        "player_id" : Spark.getPlayer().getPlayerId(), // On ajoute l'ID du player
        "number" : k + 1,
        // Toutes les cartes du deck. 
        "cards" : []
        }
    
        Spark.runtimeCollection('decks').insert(newDeck);   
    }
}