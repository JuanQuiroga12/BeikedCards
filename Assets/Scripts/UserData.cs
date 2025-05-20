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
    public StringDictionary cookieCodes = new StringDictionary();
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
    private static bool isInitialized = false;
    private static bool isUsingPlayerPrefs = false;

    public static void Initialize()
    {
        if (isInitialized) return;

        Debug.Log("Inicializando DataManager...");

        // Decidir si usar PlayerPrefs (Android) o archivo JSON (Editor)
        isUsingPlayerPrefs = Application.platform == RuntimePlatform.Android;

        if (isUsingPlayerPrefs)
        {
            Debug.Log("DataManager usando PlayerPrefs para persistencia (Android)");
            LoadDataFromPlayerPrefs();
        }
        else
        {
            Debug.Log("DataManager usando archivo JSON para persistencia (Editor)");
            dataPath = Path.Combine(Application.persistentDataPath, "userData.json");
            LoadData();
        }

        // Verificación adicional
        if (userData == null)
        {
            Debug.LogError("No se pudo inicializar userData. Creando nuevo...");
            userData = new UserData();
        }

        Debug.Log($"DataManager inicializado: collectedCards tiene {userData.collectedCards.Count} cartas");
        isInitialized = true;
    }

    private static void LoadDataFromPlayerPrefs()
    {
        try
        {
            // Usar nuestro nuevo sistema de persistencia para Android
            userData = AndroidDataPersistence.GetAllUserData();

            // Verificación extra para garantizar que las estructuras de datos están inicializadas
            if (userData.collectedCards == null)
                userData.collectedCards = new List<Card>();
            if (userData.usedQRCodes == null)
                userData.usedQRCodes = new List<string>();
            if (userData.completedStories == null)
                userData.completedStories = new List<string>();
            if (userData.cookieCodes == null)
                userData.cookieCodes = new StringDictionary();
            if (userData.cookieCodes.pairs == null)
                userData.cookieCodes.pairs = new List<StringKeyValuePair>();

            Debug.Log($"Datos cargados desde PlayerPrefs: {userData.collectedCards.Count} cartas, {userData.usedQRCodes.Count} QR usados");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar datos desde PlayerPrefs: {e.Message}\n{e.StackTrace}");
            userData = new UserData();
        }
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
                    userData.cookieCodes = new StringDictionary();
                if (userData.cookieCodes.pairs == null)
                    userData.cookieCodes.pairs = new List<StringKeyValuePair>();

                Debug.Log($"Datos cargados exitosamente desde archivo: {userData.collectedCards.Count} cartas");

                // Loguear el contenido del JSON para debugging
                Debug.Log($"JSON cargado: {json.Substring(0, Mathf.Min(json.Length, 200))}...");
            }
            else
            {
                Debug.Log("No se encontró archivo de datos. Creando nuevo usuario...");
                userData = new UserData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar datos desde archivo: {e.Message}\n{e.StackTrace}");
            userData = new UserData();
        }
    }

    public static void SaveData()
    {
        try
        {
            // Verificar que userData esté inicializado
            if (userData == null)
            {
                Debug.LogError("userData es nulo al intentar guardar. Inicializando...");
                Initialize();
            }

            if (isUsingPlayerPrefs)
            {
                // Usar sistema de persistencia para Android
                AndroidDataPersistence.SaveAllUserData(userData);
                Debug.Log("Datos guardados en PlayerPrefs exitosamente");
            }
            else
            {
                // Guardar en archivo JSON (Editor)
                string json = JsonUtility.ToJson(userData);
                Debug.Log($"Guardando datos en archivo. JSON: {json.Substring(0, Mathf.Min(json.Length, 200))}...");

                File.WriteAllText(dataPath, json);
                Debug.Log($"Datos guardados exitosamente en archivo: {dataPath}");
            }

            // Guardar una versión de respaldo en PlayerPrefs siempre
            string backupJson = JsonUtility.ToJson(userData);
            PlayerPrefs.SetString("UserDataBackup", backupJson);
            PlayerPrefs.Save();
            Debug.Log("Respaldo guardado en PlayerPrefs");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar datos: {e.Message}\n{e.StackTrace}");

            // Intentar guardar en PlayerPrefs como fallback
            try
            {
                string json = JsonUtility.ToJson(userData);
                PlayerPrefs.SetString("UserDataBackup", json);
                PlayerPrefs.Save();
                Debug.Log("Datos guardados como fallback en PlayerPrefs");
            }
            catch (System.Exception e2)
            {
                Debug.LogError($"Error en fallback: {e2.Message}");
            }
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

        Debug.Log($"Añadiendo {cards.Count} cartas a la colección...");

        foreach (Card card in cards)
        {
            if (card == null)
            {
                Debug.LogWarning("Se intentó añadir una carta nula. Omitiendo...");
                continue;
            }

            Debug.Log($"Añadiendo carta: {card.name}, ID: {card.id}, Tipo: {card.type}");

            // Verificar si ya tenemos esta carta
            if (!userData.collectedCards.Exists(c => c.id == card.id))
            {
                userData.collectedCards.Add(card);
                Debug.Log($"Carta añadida con éxito. Total de cartas: {userData.collectedCards.Count}");
            }
            else
            {
                Debug.Log($"La carta {card.id} ya está en la colección. No se añadió duplicado.");
            }
        }

        // Guardar forzosamente después de añadir cartas
        SaveData();

        // AÑADIDO: Guardar respaldo adicional de cartas recientes
        try
        {
            string cardsJson = JsonUtility.ToJson(new { cards });
            PlayerPrefs.SetString("LastObtainedCards", cardsJson);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"No se pudo guardar respaldo adicional: {e.Message}");
        }

        // Verificar después de guardar
        Debug.Log($"Verificación post-guardado: {userData.collectedCards.Count} cartas en total");
    }

    public static List<Card> GetAllCards()
    {
        if (userData == null || userData.collectedCards == null)
        {
            Debug.LogWarning("GetAllCards: userData o collectedCards es nulo. Inicializando...");
            Initialize();

            if (userData == null || userData.collectedCards == null)
            {
                Debug.LogError("No se pudo inicializar collectedCards");
                return new List<Card>();
            }
        }

        Debug.Log($"GetAllCards: Devolviendo {userData.collectedCards.Count} cartas");
        return userData.collectedCards;
    }

    public static List<Card> GetCardsByStory(string storyId)
    {
        if (userData == null || userData.collectedCards == null)
        {
            Debug.LogWarning("GetCardsByStory: userData es nulo. Inicializando...");
            Initialize();
            return new List<Card>();
        }

        var result = userData.collectedCards.FindAll(card => card.storyId == storyId);
        Debug.Log($"GetCardsByStory: Encontradas {result.Count} cartas para historia {storyId}");
        return result;
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
        userData.cookieCodes.SetValue(storyId, code);
        SaveData();
    }

    public static string GetCookieCode(string storyId)
    {
        if (userData.cookieCodes.ContainsKey(storyId))
        {
            return userData.cookieCodes.GetValue(storyId);
        }
        return null;
    }

    // Método para cargar el respaldo desde PlayerPrefs si falla la carga normal
    public static bool LoadBackupIfNeeded()
    {
        if (userData == null || userData.collectedCards == null || userData.collectedCards.Count == 0)
        {
            string backupJson = PlayerPrefs.GetString("UserDataBackup", "");
            if (!string.IsNullOrEmpty(backupJson))
            {
                try
                {
                    userData = JsonUtility.FromJson<UserData>(backupJson);
                    Debug.Log("Datos restaurados desde el respaldo en PlayerPrefs");

                    // Verificar estructuras deserializadas
                    if (userData.collectedCards == null)
                        userData.collectedCards = new List<Card>();
                    if (userData.usedQRCodes == null)
                        userData.usedQRCodes = new List<string>();
                    if (userData.completedStories == null)
                        userData.completedStories = new List<string>();
                    if (userData.cookieCodes == null)
                        userData.cookieCodes = new StringDictionary();
                    if (userData.cookieCodes.pairs == null)
                        userData.cookieCodes.pairs = new List<StringKeyValuePair>();

                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error al cargar respaldo: {e.Message}");
                }
            }
        }
        return false;
    }

    // AÑADIDO: Diagnóstico y recuperación específica para Android
    public static bool DiagnoseAndRecover()
    {
        Debug.Log("Ejecutando diagnóstico y recuperación de datos...");

        // Verificar PlayerPrefs directamente
        string backupJson = PlayerPrefs.GetString("UserDataBackup", "");
        string lastCardsJson = PlayerPrefs.GetString("LastObtainedCards", "");

        Debug.Log($"PlayerPrefs encontrados - Backup: {!string.IsNullOrEmpty(backupJson)}, LastCards: {!string.IsNullOrEmpty(lastCardsJson)}");

        bool recovered = false;

        // Si hay un respaldo principal, intentar usarlo
        if (!string.IsNullOrEmpty(backupJson))
        {
            try
            {
                UserData backupData = JsonUtility.FromJson<UserData>(backupJson);
                if (backupData != null && backupData.collectedCards != null && backupData.collectedCards.Count > 0)
                {
                    userData = backupData;
                    Debug.Log($"Datos recuperados de backup: {userData.collectedCards.Count} cartas");
                    recovered = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al recuperar desde backup: {e.Message}");
            }
        }

        // Reiniciar datos si no hay nada
        if (userData == null)
        {
            userData = new UserData();
            userData.collectedCards = new List<Card>();
            userData.usedQRCodes = new List<string>();
            userData.completedStories = new List<string>();
            userData.cookieCodes = new StringDictionary();
            userData.cookieCodes.pairs = new List<StringKeyValuePair>();
        }

        // Guardar inmediatamente para asegurar que hay datos disponibles
        SaveData();

        return recovered;
    }
}