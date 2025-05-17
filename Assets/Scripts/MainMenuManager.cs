using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button scanQRButton;
    [SerializeField] private Button collectionButton;
    [SerializeField] private Button settingsButton;

    private void Start()
    {

        // Verificar que los botones tienen referencias asignadas
        if (scanQRButton == null || collectionButton == null || settingsButton == null)
        {
            Debug.LogError("�MainMenuManager: Faltan referencias de botones! Asigna los botones en el Inspector.");
            return;
        }

        // Asignar los listener para la navegaci�n
        scanQRButton.onClick.AddListener(() => SceneManager.LoadScene("QRScanScene"));
        collectionButton.onClick.AddListener(() => SceneManager.LoadScene("CollectionScene"));
        settingsButton.onClick.AddListener(OpenSettings);

        Debug.Log("MainMenuManager inicializado correctamente");
    }

    private void OpenSettings()
    {
        // Aqu� puedes implementar un panel de configuraci�n o cargar una escena de configuraci�n
        Debug.Log("Abriendo configuraci�n");
    }
}