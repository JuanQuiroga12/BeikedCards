using UnityEngine;

public class DataSaveManager : MonoBehaviour
{
    private static DataSaveManager instance;
    private float autoSaveInterval = 30f; // Guardar automáticamente cada 30 segundos
    private string logPrefix = "[DataSaveManager] ";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log(logPrefix + "Instancia duplicada, destruyendo");
            Destroy(gameObject);
            return;
        }

        // Mensaje explícito para verificar que se está ejecutando
        Debug.Log(logPrefix + "¡DATASAVEMANAGER INICIADO! Plataforma: " + Application.platform);

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Asegurar que DataManager esté inicializado
        if (!IsDataManagerInitialized())
        {
            Debug.Log(logPrefix + "Inicializando DataManager desde DataSaveManager");
            DataManager.Initialize();
        }

        // Iniciar la rutina de guardado automático
        InvokeRepeating("PerformAutoSave", 5f, autoSaveInterval);

        // Asegurar que DataManager esté inicializado
        DataManager.Initialize();

    }


    private bool IsDataManagerInitialized()
    {
        string username = DataManager.GetCurrentUsername();
        Debug.Log(logPrefix + "Verificando inicialización: usuario actual = " + username);
        return !string.IsNullOrEmpty(username);
    }

    private void PerformAutoSave()
    {
        // Verificar si hay un usuario activo antes de guardar
        string username = DataManager.GetCurrentUsername();
        if (string.IsNullOrEmpty(username) || username == "default")
        {
            Debug.Log(logPrefix + "No hay usuario activo, omitiendo guardado automático");
            return;
        }

        Debug.Log(logPrefix + "INICIANDO GUARDADO AUTOMÁTICO");
        DataManager.SaveData();
        Debug.Log(logPrefix + "GUARDADO AUTOMÁTICO COMPLETADO");
    }

    // También modificar otros métodos que llaman a SaveData()
    // También modificar otros métodos que llaman a SaveData()
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Verificar si hay un usuario activo
            string username = DataManager.GetCurrentUsername();
            if (string.IsNullOrEmpty(username) || username == "default")
            {
                Debug.Log(logPrefix + "No hay usuario activo, omitiendo guardado");
                return;
            }

            Debug.Log(logPrefix + "APLICACIÓN PAUSADA - GUARDANDO DATOS");
            DataManager.SaveData();
        }
        else
        {
            Debug.Log(logPrefix + "APLICACIÓN REANUDADA - VERIFICANDO DATOS");
            DataManager.LoadData();
        }
    }

    // Este método es clave para Android
    private void OnApplicationFocus(bool hasFocus)
    {
        Debug.Log(logPrefix + "OnApplicationFocus: " + hasFocus);
        if (!hasFocus)
        {
            // Verificar si hay un usuario activo
            string username = DataManager.GetCurrentUsername();
            if (string.IsNullOrEmpty(username) || username == "default")
            {
                Debug.Log(logPrefix + "No hay usuario activo, omitiendo guardado");
                return;
            }

            Debug.Log(logPrefix + "APLICACIÓN PERDIÓ FOCO - GUARDANDO DATOS");
            DataManager.SaveData();
        }
    }

    private void OnApplicationQuit()
    {
        // Verificar si hay un usuario activo
        string username = DataManager.GetCurrentUsername();
        if (string.IsNullOrEmpty(username) || username == "default")
        {
            Debug.Log(logPrefix + "No hay usuario activo, omitiendo guardado final");
            return;
        }

        Debug.Log(logPrefix + "APLICACIÓN CERRÁNDOSE - GUARDADO FINAL");
        DataManager.SaveData();
    }

    // Método público para forzar un guardado desde cualquier parte de la aplicación
    public static void ForceSave()
    {
        if (instance != null)
        {
            // Verificar si hay un usuario activo
            string username = DataManager.GetCurrentUsername();
            if (string.IsNullOrEmpty(username) || username == "default")
            {
                Debug.Log("[DataSaveManager] No hay usuario activo, omitiendo guardado forzado");
                return;
            }

            Debug.Log("[DataSaveManager] GUARDADO FORZADO INICIADO");
            DataManager.SaveData();
            Debug.Log("[DataSaveManager] GUARDADO FORZADO COMPLETADO");
        }
    }
}