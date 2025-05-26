using UnityEngine;

public class DataSaveManager : MonoBehaviour
{
    private static DataSaveManager instance;
    private float autoSaveInterval = 30f; // Guardar autom�ticamente cada 30 segundos
    private string logPrefix = "[DataSaveManager] ";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log(logPrefix + "Instancia duplicada, destruyendo");
            Destroy(gameObject);
            return;
        }

        // Mensaje expl�cito para verificar que se est� ejecutando
        Debug.Log(logPrefix + "�DATASAVEMANAGER INICIADO! Plataforma: " + Application.platform);

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Asegurar que DataManager est� inicializado
        if (!IsDataManagerInitialized())
        {
            Debug.Log(logPrefix + "Inicializando DataManager desde DataSaveManager");
            DataManager.Initialize();
        }

        // Iniciar la rutina de guardado autom�tico
        InvokeRepeating("PerformAutoSave", 5f, autoSaveInterval);

        // Asegurar que DataManager est� inicializado
        DataManager.Initialize();

    }


    private bool IsDataManagerInitialized()
    {
        string username = DataManager.GetCurrentUsername();
        Debug.Log(logPrefix + "Verificando inicializaci�n: usuario actual = " + username);
        return !string.IsNullOrEmpty(username);
    }

    private void PerformAutoSave()
    {
        // Verificar si hay un usuario activo antes de guardar
        string username = DataManager.GetCurrentUsername();
        if (string.IsNullOrEmpty(username) || username == "default")
        {
            Debug.Log(logPrefix + "No hay usuario activo, omitiendo guardado autom�tico");
            return;
        }

        Debug.Log(logPrefix + "INICIANDO GUARDADO AUTOM�TICO");
        DataManager.SaveData();
        Debug.Log(logPrefix + "GUARDADO AUTOM�TICO COMPLETADO");
    }

    // Tambi�n modificar otros m�todos que llaman a SaveData()
    // Tambi�n modificar otros m�todos que llaman a SaveData()
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

            Debug.Log(logPrefix + "APLICACI�N PAUSADA - GUARDANDO DATOS");
            DataManager.SaveData();
        }
        else
        {
            Debug.Log(logPrefix + "APLICACI�N REANUDADA - VERIFICANDO DATOS");
            DataManager.LoadData();
        }
    }

    // Este m�todo es clave para Android
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

            Debug.Log(logPrefix + "APLICACI�N PERDI� FOCO - GUARDANDO DATOS");
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

        Debug.Log(logPrefix + "APLICACI�N CERR�NDOSE - GUARDADO FINAL");
        DataManager.SaveData();
    }

    // M�todo p�blico para forzar un guardado desde cualquier parte de la aplicaci�n
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