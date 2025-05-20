using UnityEngine;

public class DataManagerInitializer : MonoBehaviour
{
    private void Awake()
    {
        // Inicializa DataManager antes que cualquier otro script
        DataManager.Initialize();
        Debug.Log("DataManager inicializado en Awake de DataManagerInitializer");
        DontDestroyOnLoad(gameObject);
    }
}