using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CardDetailView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cardModelParent;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardTypeText;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;
    [SerializeField] private TextMeshProUGUI storyTitleText;
    [SerializeField] private GameObject rotationInstructions;

    [Header("Card Models")]
    [SerializeField] private GameObject commonCardPrefab;
    [SerializeField] private GameObject strangeCardPrefab;
    [SerializeField] private GameObject deluxeCardPrefab;

    [Header("Effects")]
    [SerializeField] private ParticleSystem cardParticles;
    [SerializeField] private AudioSource cardSound;

    private GameObject currentCardModel;
    private Card displayedCard;
    private Coroutine instructionsCoroutine;
    private string currentPrefabName;

    private void Start()
    {
        // Configure back button
        backButton.onClick.AddListener(() => SceneManager.LoadScene("CollectionScene"));

        // Load selected card data
        LoadSelectedCard();

        // Configure the RotationLimiter
        RotationLimiter rotationLimiter = GetComponentInChildren<RotationLimiter>();
        if (rotationLimiter != null && cardModelParent != null)
        {
            rotationLimiter.SetTargetTransform(cardModelParent);
        }

        // Setup UI elements for better presentation
        SetupCardInfoPanel();

        // Show instructions temporarily
        if (rotationInstructions != null)
        {
            StartCoroutine(ShowInstructionsTemporarily());
        }
    }

    private void SetupCardInfoPanel()
    {
        // Make sure the description text has word wrapping
        if (cardDescriptionText != null)
        {
            cardDescriptionText.enableWordWrapping = true;
            cardDescriptionText.overflowMode = TextOverflowModes.Ellipsis;
            cardDescriptionText.margin = new Vector4(10, 10, 10, 10); // Add some padding
        }

        // Ensure card name is prominent
        if (cardNameText != null)
        {
            cardNameText.fontStyle = FontStyles.Bold;
        }

        // Make card type stand out
        if (cardTypeText != null)
        {
            cardTypeText.fontStyle = FontStyles.Italic;
        }
    }

    private IEnumerator ShowInstructionsTemporarily()
    {
        rotationInstructions.SetActive(true);

        // Esperar 5 segundos
        yield return new WaitForSeconds(5f);

        // Desvanecer instrucciones
        CanvasGroup canvasGroup = rotationInstructions.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float duration = 1.0f;
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        rotationInstructions.SetActive(false);
    }

    private void OnDestroy()
    {
        // Guardar datos antes de destruir el objeto (cambio de escena)
        DataManager.SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // Guardar datos cuando la app se pausa
            DataManager.SaveData();
        }
        else
        {
            // Recargar datos cuando la app se reanuda
            DataManager.LoadData();
        }
    }

    private void LoadSelectedCard()
    {
        // Obtener ID y nombre del prefab de la carta seleccionada
        string cardId = PlayerPrefs.GetString("SelectedCardId", "");
        string prefabName = PlayerPrefs.GetString("SelectedCardPrefab", "");
        currentPrefabName = prefabName;

        if (string.IsNullOrEmpty(cardId) || string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("No hay información completa de la carta seleccionada");
            return;
        }

        // Obtener datos de la carta
        List<Card> allCards = DataManager.GetAllCards();
        displayedCard = allCards.Find(c => c.id == cardId);

        if (displayedCard == null)
        {
            Debug.LogError($"No se encontró la carta con ID: {cardId}");
            return;
        }

        // Get the real card name and description from the database
        string cardRealName = CardDatabase.GetCardName(prefabName);
        string cardRealDescription = CardDatabase.GetCardDescription(prefabName);
        string storyName = CardDatabase.GetStoryName(displayedCard.storyId);

        // Mostrar información de la carta
        cardNameText.text = cardRealName;

        // Set story title if available
        if (storyTitleText != null)
        {
            storyTitleText.text = storyName;
        }

        // Configurar texto del tipo de carta
        switch (displayedCard.type)
        {
            case CardType.CommonBeiked:
                cardTypeText.text = "Común";
                cardTypeText.color = new Color(0.2f, 0.8f, 0.2f);
                break;
            case CardType.StrangeBeiked:
                cardTypeText.text = "Extraña";
                cardTypeText.color = new Color(0.2f, 0.2f, 0.8f);
                break;
            case CardType.DeluxeBeiked:
                cardTypeText.text = "Deluxe";
                cardTypeText.color = new Color(0.8f, 0.8f, 0.2f);
                break;
        }

        // Use the real description instead of the generic one
        cardDescriptionText.text = cardRealDescription;

        // Cargar el prefab específico
        InstantiateCardPrefab(prefabName);
    }

    private void InstantiateCardPrefab(string prefabName)
    {
        // Limpiar modelo anterior si existe
        if (currentCardModel != null)
        {
            Destroy(currentCardModel);
        }

        // Construimos las dos posibles rutas para encontrar el prefab
        string cardType = "";
        switch (displayedCard.type)
        {
            case CardType.CommonBeiked:
                cardType = "Common";
                break;
            case CardType.StrangeBeiked:
                cardType = "Strange";
                break;
            case CardType.DeluxeBeiked:
                cardType = "Deluxe";
                break;
        }

        // Intentamos primero cargar desde Resources
        GameObject prefab = Resources.Load<GameObject>($"Cards/{prefabName}");

        // Si no está en Resources, intentamos cargarlo desde la ruta de prefabs
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>($"Prefabs/Cards/CardPrefabs/{cardType}/{prefabName}");
        }

        // Si todavía no se encuentra, intenta una ruta más genérica
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>(prefabName);
        }

        if (prefab != null)
        {
            // Instanciamos el prefab
            currentCardModel = Instantiate(prefab, cardModelParent);

            // Configurar transformación del modelo
            currentCardModel.transform.localPosition = Vector3.zero;
            currentCardModel.transform.localRotation = Quaternion.Euler(90, 180, 0);
            currentCardModel.transform.localScale = Vector3.one * 40.0f;

            // Añadir efectos según el tipo de carta
            ApplyCardEffects();
        }
        else
        {
            Debug.LogError($"No se pudo cargar el prefab: {prefabName}");
            CreatePlaceholderCard();
        }
    }

    private void InstantiateCardModel(string prefabName)
    {
        // Limpiar modelo anterior si existe
        if (currentCardModel != null)
        {
            Destroy(currentCardModel);
        }

        // Intentar cargar el prefab desde Resources
        GameObject cardPrefab = Resources.Load<GameObject>("Cards/" + prefabName);

        if (cardPrefab != null)
        {
            // Instanciar modelo desde resources
            currentCardModel = Instantiate(cardPrefab, cardModelParent);

            // Configurar modelo
            ConfigureCardModel(currentCardModel);
        }
        else
        {
            // Si no se encuentra el prefab específico, usar el prefab genérico según tipo
            GameObject fallbackPrefab = null;

            switch (displayedCard.type)
            {
                case CardType.CommonBeiked:
                    fallbackPrefab = commonCardPrefab;
                    break;
                case CardType.StrangeBeiked:
                    fallbackPrefab = strangeCardPrefab;
                    break;
                case CardType.DeluxeBeiked:
                    fallbackPrefab = deluxeCardPrefab;
                    break;
            }

            if (fallbackPrefab != null)
            {
                currentCardModel = Instantiate(fallbackPrefab, cardModelParent);
                ConfigureCardModel(currentCardModel);
            }
            else
            {
                Debug.LogError($"No hay prefab disponible para la carta: {prefabName} de tipo {displayedCard.type}");
                // Crear un placeholder simple si no hay prefabs
                CreatePlaceholderCard();
            }
        }

        // Reproducir efectos
        if (cardParticles != null)
        {
            cardParticles.Play();
        }

        if (cardSound != null)
        {
            cardSound.Play();
        }
    }

    private void ConfigureCardModel(GameObject cardModel)
    {
        // Configurar posición, rotación y escala iniciales
        cardModel.transform.localPosition = Vector3.zero;
        cardModel.transform.localRotation = Quaternion.Euler(90, 180, 0);
        cardModel.transform.localScale = Vector3.one * 120.0f;

        // Buscar MeshRenderer para asignar texturas si hay
        MeshRenderer renderer = cardModel.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            // Intentar cargar texturas específicas para esta carta
            Texture2D frontTexture = Resources.Load<Texture2D>(displayedCard.imagePath + "_Front");
            Texture2D backTexture = Resources.Load<Texture2D>(displayedCard.imagePath + "_Back");

            if (frontTexture != null && renderer.materials.Length > 0)
            {
                renderer.materials[0].mainTexture = frontTexture;
            }

            if (backTexture != null && renderer.materials.Length > 1)
            {
                renderer.materials[1].mainTexture = backTexture;
            }
        }

        // Añadir efectos de iluminación para destacar los bordes según tipo
        switch (displayedCard.type)
        {
            case CardType.CommonBeiked:
                // Sin efectos especiales para cartas comunes
                break;

            case CardType.StrangeBeiked:
                // Efecto metálico para Strange
                AddMetallicEffect(cardModel);
                break;

            case CardType.DeluxeBeiked:
                // Efecto brillante para Deluxe
                AddGlowEffect(cardModel);
                break;
        }
    }

    private void ApplyCardEffects()
    {
        if (currentCardModel == null) return;

        // Aplicar efectos visuales según el tipo de carta
        switch (displayedCard.type)
        {
            case CardType.CommonBeiked:
                // No efectos especiales para cartas comunes
                break;

            case CardType.StrangeBeiked:
                // Efecto metálico para Strange
                AddMetallicEffect(currentCardModel);
                break;

            case CardType.DeluxeBeiked:
                // Efecto brillante para Deluxe
                AddGlowEffect(currentCardModel);
                break;
        }
    }

    private void AddMetallicEffect(GameObject cardModel)
    {
        Renderer[] renderers = cardModel.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                // Configurar efecto metálico
                if (m.HasProperty("_Metallic"))
                    m.SetFloat("_Metallic", 0.8f);

                if (m.HasProperty("_Glossiness"))
                    m.SetFloat("_Glossiness", 0.8f);
            }
        }
    }

    private void AddGlowEffect(GameObject cardModel)
    {
        Renderer[] renderers = cardModel.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                // Configurar efecto metálico
                if (m.HasProperty("_Metallic"))
                    m.SetFloat("_Metallic", 0.8f);

                if (m.HasProperty("_Glossiness"))
                    m.SetFloat("_Glossiness", 0.8f);
            }
        }
    }

    private void CreatePlaceholderCard()
    {
        // Crear un placeholder simple para mostrar algo si no hay prefab
        GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        placeholder.transform.SetParent(cardModelParent);
        placeholder.transform.localPosition = Vector3.zero;
        placeholder.transform.localScale = new Vector3(2f, 3f, 0.1f);
        placeholder.transform.localRotation = Quaternion.identity;

        // Asignar material según tipo
        Renderer renderer = placeholder.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));

            switch (displayedCard.type)
            {
                case CardType.CommonBeiked:
                    mat.color = new Color(0.2f, 0.8f, 0.2f);
                    break;
                case CardType.StrangeBeiked:
                    mat.color = new Color(0.2f, 0.2f, 0.8f);
                    break;
                case CardType.DeluxeBeiked:
                    mat.color = new Color(0.8f, 0.8f, 0.2f);
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.2f) * 0.5f);
                    break;
            }

            renderer.material = mat;
        }

        // Add the real card name to the placeholder
        GameObject textObj = new GameObject("CardText");
        textObj.transform.SetParent(placeholder.transform);
        textObj.transform.localPosition = new Vector3(0, 0.1f, 0);
        textObj.transform.localRotation = Quaternion.Euler(90, 0, 0);
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = CardDatabase.GetCardName(currentPrefabName);
        textMesh.characterSize = 0.1f;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.black;

        // Guardar referencia
        currentCardModel = placeholder;
    }
}