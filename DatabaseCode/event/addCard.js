// ====================================================================================================
//
// Cloud Code for addCard, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
var shortCode = Spark.getData().shortCode; 
var card; 
var cardName;

if (shortCode == "random"){
    // Possibilité de choisir une carte aléatoire
    // TODO: Il faudra sûremnt implémenter un champ de probabilité pour l'apparition de chaque carte 
    // var cardList = Spark.setScriptData('cards', Spark.metaCollection('cards').find());
    
    var countList = Spark.metaCollection('cards').find().count(); 
    var rand = Math.floor(Math.random()*countList);
    var result = Spark.metaCollection('cards').find().skip(rand).limit(1).toArray();
    
    if(result){
       card = result[0]._id.$oid;
       Spark.setScriptData("RESULT : " , card); 
    }
    
} else {
    card = Spark.metaCollection('cards').findOne({"shortCode" : shortCode})._id.$oid; 
}

if (card){
    // on récupère le nombre de cartes du joueur. 
    var countCardsPlayer = Spark.runtimeCollection('player_allCards').find({
        'player_id' : Spark.getPlayer().getPlayerId()}, { "_id" : 0}).count(); 
    
    var IDOfLastCard; 
    if (countCardsPlayer == 0){
        IDOfLastCard = 0; 
    } else {
        // On recherche le numéro d'ID le plus grand
        var biggestID = 0; 
        var AllCards = Spark.runtimeCollection('player_allCards').find({
            'player_id' : Spark.getPlayer().getPlayerId()}, { "_id" : 0}).toArray(); 
        for (i = 0; i < AllCards.length; i++){
            if (AllCards[i].card_ID > biggestID){
                biggestID = AllCards[i].card_ID; 
            } 
        }
        //IDOfLastCard = Spark.runtimeCollection('player_allCards').find({
        //    'player_id' : Spark.getPlayer().getPlayerId()}, { "_id" : 0}).toArray()[countCardsPlayer - 1].card_ID; 
    }
    
    //var newID = Spark.runtimeCollection('player_allCards').find({
    //    'player_id' : Spark.getPlayer().getPlayerId()}, { "_id" : 0})[countCardsPlayer - 1].cardID; 
    var newDeckCard = { // New Card in the Deck
        "player_id" : Spark.getPlayer().getPlayerId(), // we need the player id to be able 
        // to get the player's full inventory later on
        // Il faut rajouter une ID de carte pour pouvoir réussir à l'insérer dans des decks. 
        "card_ID" : biggestID + 1,
        "card" : card
    }
    // Le deck du joueur sera appelé player_deck
    Spark.runtimeCollection('player_allCards').insert(newDeckCard); 
    Spark.setScriptData('card-added', shortCode); 
} else {
    Spark.getLog().debug("Add Card | Invalud Item ShortCode - " + shortCode); 
    Spark.setScriptError('card', 'invalid-card-id');
    
}