using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class CookieRedemptionManager : MonoBehaviour
{
    [SerializeField] private GameObject giftBoxModel;
    [SerializeField] private GameObject cookieModel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button redeemButton;
    [SerializeField] private GameObject codePanel;
    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private Button closeCodeButton;

    private string storyId;
    private float rotationSpeed = 30f;
    private bool isBoxOpened = false;
    private bool isDragging = false;
    private Vector3 previousMousePosition;

    private void Start()
    {
        backButton.onClick.AddListener(() => SceneManager.LoadScene("CollectionScene"));
        redeemButton.onClick.AddListener(ShowRedemptionCode);
        closeCodeButton.onClick.AddListener(() => codePanel.SetActive(false));

        // Ocultar panel de código inicialmente
        codePanel.SetActive(false);

        // Ocultar modelo de galleta inicialmente
        cookieModel.SetActive(false);

        // Obtener ID de historia para redimir
        storyId = PlayerPrefs.GetString("RedeemStoryId", "");

        if (string.IsNullOrEmpty(storyId))
        {
            // Si no hay historia, volver a colección
            SceneManager.LoadScene("CollectionScene");
            return;
        }

        // Iniciar animación de caja de regalo
        StartCoroutine(PlayGiftBoxAnimation());
    }

    private IEnumerator PlayGiftBoxAnimation()
    {
        // Escalar la caja
        giftBoxModel.transform.localScale = Vector3.zero;
        giftBoxModel.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(1.5f);

        // Animación de apertura
        giftBoxModel.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.5f);

        // Mostrar galleta
        isBoxOpened = true;
        cookieModel.SetActive(true);
        cookieModel.transform.localScale = Vector3.zero;
        cookieModel.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack);

        // Ocultar caja
        giftBoxModel.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack);

        yield return new WaitForSeconds(0.7f);

        // Habilitar interacción con galleta
        redeemButton.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isBoxOpened && cookieModel.activeSelf)
        {
            // Rotar lentamente la galleta
            cookieModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Manejar rotación por arrastre
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                previousMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - previousMousePosition;

                // Cambiar rotación basado en movimiento del mouse
                cookieModel.transform.Rotate(Vector3.up, -delta.x * 0.5f);

                previousMousePosition = Input.mousePosition;
            }
        }
    }

    private void ShowRedemptionCode()
    {
        // Generar código de 9 dígitos
        string code = GenerateRedemptionCode();

        // Guardar código
        DataManager.AddCookieCode(storyId, code);

        // Mostrar código
        codeText.text = code;
        codePanel.SetActive(true);
    }

    private string GenerateRedemptionCode()
    {
        // Generar código aleatorio de 9 dígitos
        string code = "";
        for (int i = 0; i < 9; i++)
        {
            code += Random.Range(0, 10).ToString();
        }

        return code;
    }
}