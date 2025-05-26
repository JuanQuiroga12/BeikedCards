using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SerializablePair
{
    public string key;
    public string value;
}

[System.Serializable]
public class UserData
{
    public string username;
    public string password;
    public List<string> usedQRCodes = new List<string>();
    public List<Card> collectedCards = new List<Card>();
    public List<string> completedStories = new List<string>();

    // Reemplaza el diccionario con una lista serializable
    public List<SerializablePair> cookieCodesList = new List<SerializablePair>();

    // Propiedad para mantener compatibilidad con el código existente
    [System.NonSerialized]
    public Dictionary<string, string> _cookieCodes;
    public Dictionary<string, string> cookieCodes
    {
        get
        {
            if (_cookieCodes == null)
            {
                _cookieCodes = new Dictionary<string, string>();
                foreach (var pair in cookieCodesList)
                {
                    _cookieCodes[pair.key] = pair.value;
                }
            }
            return _cookieCodes;
        }
    }

    // Método para actualizar la lista serializable desde el diccionario
    public void UpdateCookieCodesList()
    {
        if (_cookieCodes == null)
        {
            _cookieCodes = new Dictionary<string, string>();
            return; // No hay nada que actualizar
        }
        
        cookieCodesList.Clear();
        foreach (var pair in _cookieCodes)
        {
            cookieCodesList.Add(new SerializablePair { key = pair.Key, value = pair.Value });
        }
    }
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
    private static string currentUsername = "";

    // En la clase DataManager, añade un método para asegurar que el usuario no sea nulo
    public static void EnsureValidUsername()
    {
        // Si el nombre de usuario está vacío, intenta recuperarlo de PlayerPrefs
        if (string.IsNullOrEmpty(currentUsername) || currentUsername == "default")
        {
            currentUsername = PlayerPrefs.GetString("CurrentUser", "");

            // Si aún está vacío, no hay usuario activo
            if (string.IsNullOrEmpty(currentUsername))
            {
                Debug.LogWarning("No hay usuario activo en el sistema");
                return;
            }

            // Actualizar ruta de datos para este usuario
            dataPath = GetUserSpecificPath(currentUsername);

            // Cargar datos del usuario si no están cargados
            if (userData == null || userData.username != currentUsername)
            {
                LoadData();
            }

            Debug.Log($"Usuario recuperado: {currentUsername}");
        }
    }

    private static string GetUserSpecificPath(string username)
    {
        return Path.Combine(Application.persistentDataPath, username + "_userData.json");
    }

    public static void SwitchUser(string username)
    {
        if (currentUsername == username && isInitialized)
        {
            Debug.Log($"Usuario {username} ya está activo");
            return;
        }

        // Guardar datos del usuario actual antes de cambiar
        if (isInitialized && userData != null && !string.IsNullOrEmpty(currentUsername))
        {
            SaveData();
        }

        // Cambiar al nuevo usuario
        currentUsername = username;
        PlayerPrefs.SetString("CurrentUser", username);
        PlayerPrefs.Save();

        isInitialized = false;  // Forzar reinicialización
        dataPath = GetUserSpecificPath(username);
        LoadData();
        Debug.Log($"Cambiado a usuario: {username}, cartas cargadas: {userData.collectedCards.Count}");
    }

    public static void Initialize()
    {
        if (isInitialized) return;

        Debug.Log("Inicializando DataManager...");

        // Obtener el usuario actual de PlayerPrefs
        currentUsername = PlayerPrefs.GetString("CurrentUser", "default");

        // Usar ruta específica del usuario
        dataPath = GetUserSpecificPath(currentUsername);
        LoadData();

        // Verificación adicional
        if (userData == null)
        {
            Debug.LogError("LoadData no inicializó userData correctamente. Creando nuevo...");
            userData = new UserData();
            userData.username = currentUsername; // Asignar nombre de usuario
        }

        Debug.Log($"DataManager inicializado para usuario {currentUsername}: collectedCards tiene {userData.collectedCards.Count} cartas");
        isInitialized = true;
    }

    public static void SaveData()
    {
        // Asegurar que el usuario sea válido antes de guardar
        EnsureValidUsername();
        if (userData == null)
        {
            Debug.LogError("⚠️ ERROR: Intentando guardar datos nulos");
            return;
        }

        try
        {
            // FORZAR SIEMPRE la asignación del username correcto
            userData.username = currentUsername;

            // Verificar que _cookieCodes no sea null antes de actualizar
            if (userData._cookieCodes == null)
            {
                userData._cookieCodes = new Dictionary<string, string>();
            }

            // Actualizar lista serializable
            userData.UpdateCookieCodesList();

            // Actualizar lista serializable
            userData.UpdateCookieCodesList();

            // Generar JSON
            string json = JsonUtility.ToJson(userData);
            Debug.Log($"💾 GUARDANDO: Datos para usuario {currentUsername}. JSON: {json.Substring(0, Mathf.Min(100, json.Length))}...");

            // Guardar en PlayerPrefs como respaldo
            PlayerPrefs.SetString(currentUsername + "_Backup", json);
            PlayerPrefs.Save();

            // Método simplificado para guardar archivo
            File.WriteAllText(dataPath, json);
            Debug.Log($"💾 GUARDADO EXITOSO: {userData.collectedCards.Count} cartas en {dataPath}");

            // Código para Android (mantener sincronización)
#if UNITY_ANDROID && !UNITY_EDITOR
        try {
            // FORZAR SIEMPRE la asignación del username correcto
            userData.username = currentUsername;

            // Verificar que _cookieCodes no sea null antes de actualizar
            if (userData._cookieCodes == null)
            {
                userData._cookieCodes = new Dictionary<string, string>();
            }

            // Actualizar lista serializable
            userData.UpdateCookieCodesList();

            // Actualizar lista serializable
            userData.UpdateCookieCodesList();

            // Generar JSON
            string json = JsonUtility.ToJson(userData);
            Debug.Log($"💾 GUARDANDO: Datos para usuario {currentUsername}. JSON: {json.Substring(0, Mathf.Min(100, json.Length))}...");

            // Guardar en PlayerPrefs como respaldo
            PlayerPrefs.SetString(currentUsername + "_Backup", json);
            PlayerPrefs.Save();

            // Método simplificado para guardar archivo
            File.WriteAllText(dataPath, json);
            Debug.Log($"💾 GUARDADO EXITOSO: {userData.collectedCards.Count} cartas en {dataPath}");
        }
        catch (System.Exception e) {
            Debug.LogError($"⚠️ ERROR ANDROID: {e.Message}");
        }
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"⚠️ ERROR GUARDADO: {e.Message}\n{e.StackTrace}");
        }
    }

    public static void LoadData()
    {
        try
        {
            Debug.Log($"📂 CARGANDO: Datos de usuario {currentUsername} desde {dataPath}");
            string json = "";

            // Intentar cargar desde archivo
            if (File.Exists(dataPath))
            {
                json = File.ReadAllText(dataPath);

                if (!string.IsNullOrEmpty(json))
                {
                    userData = JsonUtility.FromJson<UserData>(json);
                    Debug.Log($"📂 DATOS CARGADOS: {userData.collectedCards.Count} cartas desde archivo");
                }
            }

            // Si no hay datos desde archivo, intentar desde PlayerPrefs
            if (userData == null || userData.collectedCards == null)
            {
                string backupJson = PlayerPrefs.GetString(currentUsername + "_Backup", "");

                if (!string.IsNullOrEmpty(backupJson))
                {
                    userData = JsonUtility.FromJson<UserData>(backupJson);
                    Debug.Log($"📂 DATOS RECUPERADOS: {userData.collectedCards.Count} cartas desde PlayerPrefs");

                    // Sincronizar al archivo
                    File.WriteAllText(dataPath, backupJson);
                }
            }

            // Si todavía no hay datos, crear nuevos
            if (userData == null)
            {
                userData = new UserData();
                userData.username = currentUsername;
                Debug.Log($"📂 DATOS NUEVOS: Creando para usuario {currentUsername}");
            }

            // Asegurar que las listas no sean nulas
            if (userData.collectedCards == null)
                userData.collectedCards = new List<Card>();
            if (userData.usedQRCodes == null)
                userData.usedQRCodes = new List<string>();
            if (userData.completedStories == null)
                userData.completedStories = new List<string>();
            if (userData.cookieCodesList == null)
                userData.cookieCodesList = new List<SerializablePair>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"⚠️ ERROR CARGA: {e.Message}\n{e.StackTrace}");

            // Crear datos nuevos en caso de error
            userData = new UserData();
            userData.username = currentUsername;
            userData.collectedCards = new List<Card>();
            userData.usedQRCodes = new List<string>();
            userData.completedStories = new List<string>();
            userData.cookieCodesList = new List<SerializablePair>();
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
                userData.username = currentUsername;
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
        if (userData == null || userData.collectedCards == null)
        {
            Debug.LogError("⚠️ GetCardsByStory: userData o collectedCards es nulo");
            return new List<Card>();
        }

        List<Card> storyCards = userData.collectedCards.FindAll(card => card.storyId == storyId);

        Debug.Log($"⭐ GetCardsByStory: Buscando cartas para {storyId}, encontradas: {storyCards.Count}");
        foreach (Card card in storyCards)
        {
            Debug.Log($"⭐ - Carta: ID={card.id}, Tipo={card.type}, Parte={card.storyPart}");
        }

        return storyCards;
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
        if (userData._cookieCodes == null)
        {
            userData._cookieCodes = new Dictionary<string, string>();
        }
        userData._cookieCodes[storyId] = code;
        userData.UpdateCookieCodesList(); // Actualiza la lista serializable
        SaveData();
    }

    public static string GetCookieCode(string storyId)
    {
        // Asegurarse de que _cookieCodes esté inicializado
        if (userData._cookieCodes == null)
        {
            userData._cookieCodes = new Dictionary<string, string>();

            // Cargar los datos desde la lista serializable
            foreach (var pair in userData.cookieCodesList)
            {
                userData._cookieCodes[pair.key] = pair.value;
            }
        }

        // Verificar si existe el código
        if (userData._cookieCodes.ContainsKey(storyId))
        {
            return userData._cookieCodes[storyId];
        }
        return null;
    }

    // Método para obtener el nombre de usuario actual
    public static string GetCurrentUsername()
    {
        return currentUsername;
    }
}