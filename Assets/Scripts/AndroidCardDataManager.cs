using System.Collections.Generic;
using UnityEngine;

public static class AndroidCardDataManager
{
    private const string CARDS_KEY = "AndroidCards";
    private const string SERIALIZED_CARDS_COUNT = "AndroidCardsCount";
    private const string SERIALIZED_CARD_PREFIX = "AndroidCard_";
    private const string LAST_SAVE_TIME = "AndroidLastSaveTime";

    // Guardar las cartas de manera individual para evitar límites de tamaño en PlayerPrefs
    public static void SaveCards(List<Card> cards)
    {
        if (cards == null || cards.Count == 0)
        {
            PlayerPrefs.SetInt(SERIALIZED_CARDS_COUNT, 0);
            PlayerPrefs.Save();
            Debug.Log("No hay cartas para guardar en AndroidCardDataManager");
            return;
        }

        // Guardar el conteo total
        PlayerPrefs.SetInt(SERIALIZED_CARDS_COUNT, cards.Count);

        // Guardar cada carta individualmente
        for (int i = 0; i < cards.Count; i++)
        {
            SaveSingleCard(cards[i], i);
        }

        // Registrar tiempo de guardado
        PlayerPrefs.SetString(LAST_SAVE_TIME, System.DateTime.Now.ToString());
        PlayerPrefs.Save();

        Debug.Log($"AndroidCardDataManager: {cards.Count} cartas guardadas en {System.DateTime.Now}");
    }

    private static void SaveSingleCard(Card card, int index)
    {
        string cardKey = SERIALIZED_CARD_PREFIX + index;

        // Guardar campos individuales para evitar problemas con JsonUtility
        PlayerPrefs.SetString(cardKey + "_id", card.id);
        PlayerPrefs.SetString(cardKey + "_name", card.name);
        PlayerPrefs.SetInt(cardKey + "_type", (int)card.type);
        PlayerPrefs.SetString(cardKey + "_storyId", card.storyId);
        PlayerPrefs.SetInt(cardKey + "_storyPart", card.storyPart);
        PlayerPrefs.SetString(cardKey + "_description", card.description);
        PlayerPrefs.SetString(cardKey + "_imagePath", card.imagePath);
    }

    // Cargar todas las cartas
    public static List<Card> LoadCards()
    {
        List<Card> result = new List<Card>();

        int count = PlayerPrefs.GetInt(SERIALIZED_CARDS_COUNT, 0);
        if (count == 0)
        {
            Debug.Log("No hay cartas guardadas en AndroidCardDataManager");
            return result;
        }

        for (int i = 0; i < count; i++)
        {
            Card card = LoadSingleCard(i);
            if (card != null)
            {
                result.Add(card);
            }
        }

        Debug.Log($"AndroidCardDataManager: {result.Count} cartas cargadas de {count} almacenadas");
        return result;
    }

    private static Card LoadSingleCard(int index)
    {
        string cardKey = SERIALIZED_CARD_PREFIX + index;

        // Verificar si esta carta existe
        if (!PlayerPrefs.HasKey(cardKey + "_id"))
        {
            return null;
        }

        Card card = new Card();
        card.id = PlayerPrefs.GetString(cardKey + "_id");
        card.name = PlayerPrefs.GetString(cardKey + "_name");
        card.type = (CardType)PlayerPrefs.GetInt(cardKey + "_type");
        card.storyId = PlayerPrefs.GetString(cardKey + "_storyId");
        card.storyPart = PlayerPrefs.GetInt(cardKey + "_storyPart");
        card.description = PlayerPrefs.GetString(cardKey + "_description");
        card.imagePath = PlayerPrefs.GetString(cardKey + "_imagePath");

        return card;
    }

    // Método para forzar una carga inmediata
    public static void ForceLoadToDataManager()
    {
        List<Card> cards = LoadCards();
        Debug.Log($"Forzando carga de {cards.Count} cartas en DataManager");

        // Obtener cartas actuales
        List<Card> currentCards = DataManager.GetAllCards();

        // Si no hay cartas en DataManager, reemplazar completamente
        if (currentCards == null || currentCards.Count == 0)
        {
            DataManager.RefreshCardCollection(cards);
            return;
        }

        // Combinar cartas (evitando duplicados)
        foreach (Card card in cards)
        {
            if (!currentCards.Exists(c => c.id == card.id))
            {
                currentCards.Add(card);
            }
        }

        // Actualizar DataManager
        DataManager.RefreshCardCollection(currentCards);
    }

    // Limpiar todos los datos (para pruebas)
    public static void ClearAllData()
    {
        int count = PlayerPrefs.GetInt(SERIALIZED_CARDS_COUNT, 0);

        for (int i = 0; i < count; i++)
        {
            string cardKey = SERIALIZED_CARD_PREFIX + i;
            PlayerPrefs.DeleteKey(cardKey + "_id");
            PlayerPrefs.DeleteKey(cardKey + "_name");
            PlayerPrefs.DeleteKey(cardKey + "_type");
            PlayerPrefs.DeleteKey(cardKey + "_storyId");
            PlayerPrefs.DeleteKey(cardKey + "_storyPart");
            PlayerPrefs.DeleteKey(cardKey + "_description");
            PlayerPrefs.DeleteKey(cardKey + "_imagePath");
        }

        PlayerPrefs.DeleteKey(SERIALIZED_CARDS_COUNT);
        PlayerPrefs.DeleteKey(LAST_SAVE_TIME);
        PlayerPrefs.Save();

        Debug.Log("AndroidCardDataManager: Todos los datos limpiados");
    }
}