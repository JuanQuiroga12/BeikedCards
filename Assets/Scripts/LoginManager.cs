using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        DataManager.Initialize();

        loginButton.onClick.AddListener(Login);
        registerButton.onClick.AddListener(Register);

        // Verificar si ya existe un DataSaveManager
        if (FindObjectOfType<DataSaveManager>() == null)
        {
            GameObject saveManagerObj = new GameObject("DataSaveManager");
            saveManagerObj.AddComponent<DataSaveManager>();
            Debug.Log("DataSaveManager creado");
        }

    }

    private void Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Por favor, ingresa usuario y contraseña";
            return;
        }

        string savedPassword = PlayerPrefs.GetString(username, "");

        if (string.IsNullOrEmpty(savedPassword))
        {
            statusText.text = "Usuario no encontrado";
            return;
        }

        if (savedPassword != password)
        {
            statusText.text = "Contraseña incorrecta";
            return;
        }
        // Al final del método Login() y Register()
        PlayerPrefs.SetString("CurrentUser", username);
        PlayerPrefs.Save();  // Forzar guardado inmediato en Android

        // ¡NUEVO! - Asegurarse de cargar los datos del usuario correcto
        DataManager.SwitchUser(username);

        // Guarda el usuario activo
        PlayerPrefs.SetString("CurrentUser", username);

        // Avanza a la escena principal
        SceneManager.LoadScene("MainScene");
    }

    private void Register()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Por favor, ingresa usuario y contraseña";
            return;
        }

        string savedPassword = PlayerPrefs.GetString(username, "");

        if (!string.IsNullOrEmpty(savedPassword))
        {
            statusText.text = "El usuario ya existe";
            return;
        }

        // Guarda la contraseña para este usuario
        PlayerPrefs.SetString(username, password);

        // Al final del método Login() y Register()
        PlayerPrefs.SetString("CurrentUser", username);
        PlayerPrefs.Save();  // Forzar guardado inmediato en Android

        statusText.text = "¡Registro exitoso!";

        // Espera un momento y avanza a la escena principal
        Invoke("GoToMainScene", 1.5f);
    }

    private void GoToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}