using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UserInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userInfoText;
    [SerializeField] private bool forceDataManagerInit = true;
    [SerializeField] private bool redirectToLoginIfNoUser = false;

    private void Start()
    {
        // Asegurar que DataManager esté inicializado
        if (forceDataManagerInit)
        {
            DataManager.Initialize();
        }

        UpdateUserInfo();
    }

    public void UpdateUserInfo()
    {
        // Obtener el nombre de usuario desde PlayerPrefs como fuente primaria de verdad
        string userFromPrefs = PlayerPrefs.GetString("CurrentUser", "");

        // Obtener el nombre de usuario desde DataManager
        string userFromDataManager = DataManager.GetCurrentUsername();

        string currentUser = "";

        // Verificar inconsistencias entre PlayerPrefs y DataManager
        if (!string.IsNullOrEmpty(userFromPrefs) && userFromPrefs != "default")
        {
            // Si hay un usuario válido en PlayerPrefs, usarlo y asegurarnos que DataManager lo use
            currentUser = userFromPrefs;

            // Si DataManager tiene un usuario diferente o default, actualizarlo
            if (userFromDataManager != currentUser || userFromDataManager == "default")
            {
                Debug.Log($"UserInfoDisplay: Corrigiendo usuario en DataManager - de '{userFromDataManager}' a '{currentUser}'");
                DataManager.SwitchUser(currentUser);

                // Forzar un guardado para asegurar que userData.username se actualice
                DataManager.SaveData();
            }
        }
        else if (!string.IsNullOrEmpty(userFromDataManager) && userFromDataManager != "default")
        {
            // Si no hay usuario en PlayerPrefs pero sí en DataManager, usarlo y actualizarlo en PlayerPrefs
            currentUser = userFromDataManager;
            PlayerPrefs.SetString("CurrentUser", currentUser);
            PlayerPrefs.Save();
            Debug.Log($"UserInfoDisplay: Actualizando PlayerPrefs con usuario '{currentUser}' desde DataManager");
        }

        // Verificar si hay un usuario válido
        if (!string.IsNullOrEmpty(currentUser) && currentUser != "default")
        {
            userInfoText.text = "¡Hola, " + currentUser + "!";
            Debug.Log($"UserInfoDisplay: Mostrando bienvenida para usuario '{currentUser}'");
        }
        else
        {
            // Si no hay usuario válido, mostrar mensaje genérico
            userInfoText.text = "¡Bienvenido!";
            Debug.LogWarning("UserInfoDisplay: No se encontró usuario activo");

            // Redirigir al login si está configurado
            if (redirectToLoginIfNoUser)
            {
                Debug.Log("UserInfoDisplay: Redirigiendo a la pantalla de login por falta de usuario");
                SceneManager.LoadScene("LoginScene");
            }
        }
    }
}