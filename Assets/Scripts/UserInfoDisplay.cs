using UnityEngine;
using TMPro;

public class UserInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userInfoText;

    private void Start()
    {
        UpdateUserInfo();
    }

    public void UpdateUserInfo()
    {
        // Obtener el nombre de usuario desde PlayerPrefs
        string currentUser = PlayerPrefs.GetString("CurrentUser", "");

        // Verificar si hay un usuario
        if (!string.IsNullOrEmpty(currentUser))
        {
            userInfoText.text = "¡Hola, " + currentUser + "!";
        }
        else
        {
            // Si por alguna razón no hay usuario, mostrar mensaje genérico
            userInfoText.text = "¡Bienvenido!";

            // Opcionalmente, redirigir al login
            // UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
        }
    }
}