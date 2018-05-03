// ====================================================================================================
//
// Cloud Code for get_deck_pre_enregistre, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

require("utils");

var deckNumber = Spark.getData().deckNumber; 
var langue = Spark.getData().langue; 

if (deckNumber == -1){
    // Dans ce cas là on veut récupérer tous les decks
    var allDecks = Spark.metaCollection("decks_pre_enregistres").find().toArray();  
    var decksReturn  = []; 
    for (i = 0; i < allDecks.length; i++){
        var OneDeckCards = []; 
        for (j = 0; j < allDecks[i].cards.length; j++){
            OneDeckCards.splice(OneDeckCards.length, 0, getCard(allDecks[i].cards[j], langue)); 
        }
        var OneDeckReturn = {
            "deckNumber" : allDecks[i].deckNumber, 
            "cards" : OneDeckCards
        }
        decksReturn.splice(decksReturn.length, 0, OneDeckReturn); 
    }
    Spark.setScriptData("decks", decksReturn); 
} else {
    var deck = Spark.metaCollection("decks_pre_enregistres").findOne({"deckNumber" : deckNumber}); 
    if (deck === null){
        Spark.setScriptData("deck", null); 
    } else {
        var OneDeckCards = []; 
        for (j = 0; j < deck.cards.length; j++){
            OneDeckCards.splice(OneDeckCards.length, 0, getCard(deck.cards[j], langue)); 
        }
        var OneDeckReturn = {
            "deckNumber" : deck.deckNumber, 
            "cards" : OneDeckCards
        }
        Spark.setScriptData("deck", OneDeckReturn); 
    }
}