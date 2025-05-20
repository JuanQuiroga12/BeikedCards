using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class UserData
{
    public string username;
    public string password;
    public List<string> usedQRCodes = new List<string>();
    public List<Card> collectedCards = new List<Card>();
    public List<string> completedStories = new List<string>();
    public Dictionary<string, string> cookieCodes = new Dictionary<string, string>();


}

[System.Serializable]
public class Card
{
    public string id;
    public string name;
    public CardType type;
    public string storyId;
    public int storyPart;
    public string description;
    public string imagePath;
}

public enum CardType
{
    CommonBeiked,
    StrangeBeiked,
    DeluxeBeiked
}

public static class DataManager
{
    private static UserData userData;
    private static string dataPath;

    public static void Initialize()
    {
        Debug.Log("Inicializando DataManager...");
        dataPath = Path.Combine(Application.persistentDataPath, "userData.json");
        LoadData();

        // Verificación adicional
        if (userData == null)
        {
            Debug.LogError("LoadData no inicializó userData correctamente. Creando nuevo...");
            userData = new UserData();
        }

        Debug.Log($"DataManager inicializado: collectedCards tiene {userData.collectedCards.Count} cartas");
    }

    public static void LoadData()
    {
        try
        {
            if (File.Exists(dataPath))
            {
                string json = File.ReadAllText(dataPath);
                userData = JsonUtility.FromJson<UserData>(json);

                // Verificar que se hayan deserializado correctamente las colecciones
                if (userData.collectedCards == null)
                    userData.collectedCards = new List<Card>();
                if (userData.usedQRCodes == null)
                    userData.usedQRCodes = new List<string>();
                if (userData.completedStories == null)
                    userData.completedStories = new List<string>();
                if (userData.cookieCodes == null)
                    userData.cookieCodes = new Dictionary<string, string>();

                Debug.Log($"Datos cargados exitosamente: {userData.collectedCards.Count} cartas");
            }
            else
            {
                Debug.Log("No se encontró archivo de datos. Creando nuevo usuario...");
                userData = new UserData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar datos: {e.Message}");
            userData = new UserData();
        }
    }

    public static void SaveData()
    {
        string json = JsonUtility.ToJson(userData);
        File.WriteAllText(dataPath, json);
    }

    // Añadir este método a la clase DataManager
    public static void RefreshCardCollection(List<Card> newCollection)
    {
        if (userData == null)
        {
            Initialize();
        }

        if (newCollection != null && newCollection.Count > 0)
        {
            userData.collectedCards = new List<Card>(newCollection);
            Debug.Log($"DataManager: Colección actualizada con {newCollection.Count} cartas");

            // Guardar inmediatamente para persistir los cambios
            SaveData();
        }
        else
        {
            Debug.LogWarning("DataManager: Intento de actualizar colección con lista vacía o nula");
        }
    }

    // Métodos para acceder y modificar los datos
    public static bool IsQRCodeUsed(string qrCode)
    {
        return userData.usedQRCodes.Contains(qrCode);
    }

    public static void AddUsedQRCode(string qrCode)
    {
        if (!userData.usedQRCodes.Contains(qrCode))
        {
            userData.usedQRCodes.Add(qrCode);
            SaveData();
        }
    }

    // Método para añadir cartas obtenidas en el pack opening
    public static void AddCardsFromPack(List<Card> cards)
    {
        // Verificar que userData no sea nulo
        if (userData == null)
        {
            Debug.LogError("userData es nulo. Inicializando DataManager...");
            Initialize();

            // Si sigue siendo nulo después de inicializar, crear un nuevo objeto
            if (userData == null)
            {
                Debug.LogError("No se pudo inicializar userData. Creando nuevo objeto...");
                userData = new UserData();
            }
        }

        // Verificar que collectedCards no sea nulo
        if (userData.collectedCards == null)
        {
            Debug.LogError("collectedCards es nulo. Creando nueva lista...");
            userData.collectedCards = new List<Card>();
        }

        if (cards == null)
        {
            Debug.LogError("La lista de cartas a añadir es nula");
            return;
        }

        foreach (Card card in cards)
        {
            if (card == null)
            {
                Debug.LogWarning("Se intentó añadir una carta nula. Omitiendo...");
                continue;
            }

            // Verificar si ya tenemos esta carta
            if (!userData.collectedCards.Exists(c => c.id == card.id))
            {
                userData.collectedCards.Add(card);
            }
        }
        SaveData();
    }
    public static List<Card> GetAllCards()
    {
        return userData.collectedCards;
    }

    public static List<Card> GetCardsByStory(string storyId)
    {
        return userData.collectedCards.FindAll(card => card.storyId == storyId);
    }

    public static bool IsStoryComplete(string storyId)
    {
        return userData.completedStories.Contains(storyId);
    }

    public static void AddCompletedStory(string storyId)
    {
        if (!userData.completedStories.Contains(storyId))
        {
            userData.completedStories.Add(storyId);
            SaveData();
        }
    }

    public static void AddCookieCode(string storyId, string code)
    {
        userData.cookieCodes[storyId] = code;
        SaveData();
    }


    public static string GetCookieCode(string storyId)
    {
        if (userData.cookieCodes.ContainsKey(storyId))
        {
            return userData.cookieCodes[storyId];
        }
        return null;
    }
}