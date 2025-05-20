using System.Collections.Generic;
using UnityEngine;

public static class AndroidDataPersistence
{
    // Claves para PlayerPrefs
    private const string CARDS_KEY = "SavedCards";
    private const string QR_CODES_KEY = "SavedQRCodes";
    private const string COMPLETE_STORIES_KEY = "CompletedStories";
    private const string COOKIE_CODES_KEY = "CookieCodes";

    // Guardar toda la colección de cartas
    public static void SaveCards(List<Card> cards)
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.Log("No hay cartas para guardar");
            return;
        }

        try
        {
            // Convertir lista de cartas a JSON
            CardListWrapper wrapper = new CardListWrapper { cards = cards };
            string json = JsonUtility.ToJson(wrapper);

            // Guardar en PlayerPrefs
            PlayerPrefs.SetString(CARDS_KEY, json);
            PlayerPrefs.Save();

            Debug.Log($"AndroidDataPersistence: {cards.Count} cartas guardadas correctamente en PlayerPrefs");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar cartas en PlayerPrefs: {e.Message}");
        }
    }

    // Obtener todas las cartas guardadas
    public static List<Card> GetCards()
    {
        try
        {
            string json = PlayerPrefs.GetString(CARDS_KEY, "");

            if (string.IsNullOrEmpty(json))
            {
                Debug.Log("No hay cartas guardadas en PlayerPrefs");
                return new List<Card>();
            }

            CardListWrapper wrapper = JsonUtility.FromJson<CardListWrapper>(json);
            if (wrapper == null || wrapper.cards == null)
            {
                Debug.LogWarning("Error al deserializar cartas desde PlayerPrefs");
                return new List<Card>();
            }

            Debug.Log($"AndroidDataPersistence: {wrapper.cards.Count} cartas cargadas desde PlayerPrefs");
            return wrapper.cards;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar cartas desde PlayerPrefs: {e.Message}");
            return new List<Card>();
        }
    }

    // Guardar códigos QR usados
    public static void SaveUsedQRCodes(List<string> qrCodes)
    {
        if (qrCodes == null)
            return;

        StringListWrapper wrapper = new StringListWrapper { strings = qrCodes };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(QR_CODES_KEY, json);
        PlayerPrefs.Save();
    }

    // Obtener códigos QR usados
    public static List<string> GetUsedQRCodes()
    {
        string json = PlayerPrefs.GetString(QR_CODES_KEY, "");
        if (string.IsNullOrEmpty(json))
            return new List<string>();

        StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(json);
        return wrapper?.strings ?? new List<string>();
    }

    // Guardar historias completadas
    public static void SaveCompletedStories(List<string> stories)
    {
        if (stories == null)
            return;

        StringListWrapper wrapper = new StringListWrapper { strings = stories };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(COMPLETE_STORIES_KEY, json);
        PlayerPrefs.Save();
    }

    // Obtener historias completadas
    public static List<string> GetCompletedStories()
    {
        string json = PlayerPrefs.GetString(COMPLETE_STORIES_KEY, "");
        if (string.IsNullOrEmpty(json))
            return new List<string>();

        StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(json);
        return wrapper?.strings ?? new List<string>();
    }

    // Guardar códigos de galletas
    public static void SaveCookieCodes(Dictionary<string, string> cookieCodes)
    {
        if (cookieCodes == null)
            return;

        List<StringKeyValuePair> pairs = new List<StringKeyValuePair>();
        foreach (var pair in cookieCodes)
        {
            pairs.Add(new StringKeyValuePair(pair.Key, pair.Value));
        }

        StringDictionaryWrapper wrapper = new StringDictionaryWrapper { pairs = pairs };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(COOKIE_CODES_KEY, json);
        PlayerPrefs.Save();
    }

    // Obtener códigos de galletas
    public static Dictionary<string, string> GetCookieCodes()
    {
        string json = PlayerPrefs.GetString(COOKIE_CODES_KEY, "");
        if (string.IsNullOrEmpty(json))
            return new Dictionary<string, string>();

        StringDictionaryWrapper wrapper = JsonUtility.FromJson<StringDictionaryWrapper>(json);
        if (wrapper == null || wrapper.pairs == null)
            return new Dictionary<string, string>();

        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach (var pair in wrapper.pairs)
        {
            result[pair.key] = pair.value;
        }

        return result;
    }

    // Guardar todos los datos del usuario (método consolidado)
    public static void SaveAllUserData(UserData userData)
    {
        if (userData == null)
            return;

        SaveCards(userData.collectedCards);
        SaveUsedQRCodes(userData.usedQRCodes);
        SaveCompletedStories(userData.completedStories);

        // Convertir StringDictionary a Dictionary normal
        Dictionary<string, string> cookieCodes = new Dictionary<string, string>();
        if (userData.cookieCodes != null && userData.cookieCodes.pairs != null)
        {
            foreach (var pair in userData.cookieCodes.pairs)
            {
                cookieCodes[pair.key] = pair.value;
            }
        }
        SaveCookieCodes(cookieCodes);

        // Guardar nombres de usuario y contraseña
        if (!string.IsNullOrEmpty(userData.username))
        {
            PlayerPrefs.SetString("CurrentUser", userData.username);
            if (!string.IsNullOrEmpty(userData.password))
            {
                PlayerPrefs.SetString(userData.username, userData.password);
            }
        }

        PlayerPrefs.Save();
        Debug.Log("AndroidDataPersistence: Todos los datos guardados correctamente");
    }

    // Obtener todos los datos del usuario
    public static UserData GetAllUserData()
    {
        UserData userData = new UserData();

        // Cargar cartas
        userData.collectedCards = GetCards();

        // Cargar códigos QR usados
        userData.usedQRCodes = GetUsedQRCodes();

        // Cargar historias completadas
        userData.completedStories = GetCompletedStories();

        // Cargar códigos de galletas
        Dictionary<string, string> cookieCodes = GetCookieCodes();
        userData.cookieCodes = new StringDictionary();
        foreach (var pair in cookieCodes)
        {
            userData.cookieCodes.SetValue(pair.Key, pair.Value);
        }

        // Cargar nombre de usuario
        userData.username = PlayerPrefs.GetString("CurrentUser", "");
        if (!string.IsNullOrEmpty(userData.username))
        {
            userData.password = PlayerPrefs.GetString(userData.username, "");
        }

        Debug.Log($"AndroidDataPersistence: Datos cargados - {userData.collectedCards.Count} cartas, {userData.usedQRCodes.Count} QR usados");
        return userData;
    }

    // Eliminar todos los datos (para pruebas)
    public static void ClearAllData()
    {
        PlayerPrefs.DeleteKey(CARDS_KEY);
        PlayerPrefs.DeleteKey(QR_CODES_KEY);
        PlayerPrefs.DeleteKey(COMPLETE_STORIES_KEY);
        PlayerPrefs.DeleteKey(COOKIE_CODES_KEY);
        PlayerPrefs.Save();
        Debug.Log("AndroidDataPersistence: Todos los datos eliminados");
    }

    // Clases internas para serialización 
    [System.Serializable]
    private class CardListWrapper
    {
        public List<Card> cards;
    }

    [System.Serializable]
    private class StringListWrapper
    {
        public List<string> strings;
    }

    [System.Serializable]
    private class StringDictionaryWrapper
    {
        public List<StringKeyValuePair> pairs;
    }
}