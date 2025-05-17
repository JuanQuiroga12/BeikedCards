using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using ZXing;
using ZXing.QrCode;
using TMPro;

public class QRScanner : MonoBehaviour
{
    [SerializeField] private RawImage cameraDisplay;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject scanningUI;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button openPackButton;
    [SerializeField] private TextMeshProUGUI resultText;

    private bool isCameraActive = false;
    private WebCamTexture cameraTexture;
    private string lastResult = string.Empty;
    private float qrCheckCooldown = 0.5f;
    private float lastCheckTime = 0;

    private void Start()
    {
        // Inicializar sistema de QR
        QRCodeManager.Initialize();

        backButton.onClick.AddListener(StopCameraAndGoBack);
        openPackButton.onClick.AddListener(StopCameraAndOpenPack);

        resultPanel.SetActive(false);
        scanningUI.SetActive(true);

        // Iniciar cámara
        StartCoroutine(StartCameraWithDelay(0.5f));
    }

    private IEnumerator StartCameraWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCamera();
    }

    private void StartCamera()
    {
        // Si hay recursos activos, liberarlos primero
        if (isCameraActive || cameraTexture != null)
        {
            ForceReleaseCamera();
        }

        Debug.Log("Iniciando cámara...");
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No se encontró ninguna cámara");
            return;
        }

        // Buscar cámara trasera
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"Cámara encontrada: {devices[i].name}, es frontal: {devices[i].isFrontFacing}");
            if (!devices[i].isFrontFacing)
            {
                cameraTexture = new WebCamTexture(devices[i].name, 1280, 720, 30);
                break;
            }
        }

        // Si no hay cámara trasera, usar la primera
        if (cameraTexture == null)
        {
            cameraTexture = new WebCamTexture(devices[0].name, 1280, 720, 30);
        }

        // Asignar textura y comenzar
        cameraDisplay.texture = cameraTexture;
        cameraTexture.Play();
        isCameraActive = true;
        Debug.Log("Cámara iniciada correctamente");
    }

    private void Update()
    {
        if (!isCameraActive || cameraTexture == null || !cameraTexture.isPlaying)
            return;

        // Asegurarse de que la cámara está activa y funcionando
        if (cameraTexture.width <= 16 || cameraTexture.height <= 16)
            return;

        // Ajustar ratio de aspecto
        aspectRatioFitter.aspectRatio = (float)cameraTexture.width / cameraTexture.height;

        // Rotar la imagen si es necesario
        Vector3 rotation = cameraDisplay.rectTransform.localEulerAngles;
        rotation.z = -cameraTexture.videoRotationAngle;
        cameraDisplay.rectTransform.localEulerAngles = rotation;

        // Escanear QR cada cierto tiempo para no sobrecargar la CPU
        if (Time.time > lastCheckTime + qrCheckCooldown)
        {
            lastCheckTime = Time.time;
            ScanQRCode();
        }
    }

    private void ScanQRCode()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();

            // Obtener la imagen de la cámara
            Color32[] pixelData = cameraTexture.GetPixels32();
            int width = cameraTexture.width;
            int height = cameraTexture.height;

            // Decodificar el QR
            Result result = barcodeReader.Decode(pixelData, width, height);
            if (result != null && !string.IsNullOrEmpty(result.Text) && result.Text != lastResult)
            {
                lastResult = result.Text;
                ProcessQRCode(result.Text);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error al escanear QR: " + ex.Message);
        }
    }

    private void ProcessQRCode(string qrCode)
    {
        // Verificar si el QR ya fue usado
        if (QRCodeManager.IsQRCodeUsed(qrCode))
        {
            resultText.text = "Este código QR ya ha sido utilizado.";
        }
        else
        {
            resultText.text = "¡Has obtenido un paquete de cartas!";
            // Guardar el QR como usado
            QRCodeManager.AddUsedQRCode(qrCode);
            // Guardar referencia al paquete obtenido
            PlayerPrefs.SetString("LastScannedQR", qrCode);
        }

        // Pausar cámara y mostrar panel de resultado
        isCameraActive = false;
        scanningUI.SetActive(false);
        resultPanel.SetActive(true);
    }

    private void StopCameraAndGoBack()
    {
        ForceReleaseCamera();
        SceneManager.LoadScene("Scenes/MainScene");
    }

    private void StopCameraAndOpenPack()
    {
        ForceReleaseCamera();
        SceneManager.LoadScene("Scenes/PackOpeningScene");
    }

    private void ForceReleaseCamera()
    {
        if (cameraTexture != null)
        {
            if (cameraTexture.isPlaying)
                cameraTexture.Stop();

            cameraTexture = null;
        }

        // Limpiar texture de RawImage
        if (cameraDisplay != null)
            cameraDisplay.texture = null;

        isCameraActive = false;

        // Forzar recolección de basura
        System.GC.Collect();
    }

    private void OnDestroy()
    {
        ForceReleaseCamera();
    }

    private void OnDisable()
    {
        ForceReleaseCamera();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            ForceReleaseCamera();
        }
        else if (gameObject.activeInHierarchy)
        {
            StartCoroutine(StartCameraWithDelay(0.5f));
        }
    }
}