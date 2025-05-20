using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardTransferSystem
{
    private static List<Card> lastObtainedCards = new List<Card>();

    public static void StoreCards(List<Card> cards)
    {
        lastObtainedCards = new List<Card>(cards);
        Debug.Log($"CardTransferSystem: {cards.Count} cartas almacenadas en memoria");
    }

    public static List<Card> RetrieveCards()
    {
        Debug.Log($"CardTransferSystem: Recuperando {lastObtainedCards.Count} cartas de memoria");
        return new List<Card>(lastObtainedCards);
    }

    public static void ClearCards()
    {
        lastObtainedCards.Clear();
    }
}
