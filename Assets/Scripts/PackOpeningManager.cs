using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PackOpeningManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject packStage;
    [SerializeField] private GameObject cardsRevealPanel;
    [SerializeField] private GameObject resultScreen;

    [Header("Pack Opening")]
    [SerializeField] private RawImage packObject;
    [SerializeField] private BoxCollider2D cutLineArea;
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Card Reveal")]
    [SerializeField] private Transform cardDisplayPosition;
    [SerializeField] private Button tapToContinueButton;
    [SerializeField] private TextMeshProUGUI cardCounter;

    [Header("Results")]
    [SerializeField] private TextMeshProUGUI resultTitle;
    [SerializeField] private Transform resultCardsContainer;
    [SerializeField] private Button continueButton;

    [Header("Card Prefabs")]
    [SerializeField] private List<GameObject> commonCardPrefabs;
    [SerializeField] private List<GameObject> strangeCardPrefabs;
    [SerializeField] private List<GameObject> deluxeCardPrefabs;
    [SerializeField] private List<GameObject> commonResultCardPrefabs;
    [SerializeField] private List<GameObject> strangeResultCardPrefabs;
    [SerializeField] private List<GameObject> deluxeResultCardPrefabs;

    [Header("Effects")]
    [SerializeField] private ParticleSystem cutPackEffect;
    [SerializeField] private ParticleSystem cardRevealEffect;
    [SerializeField] private AudioSource packOpenSound;
    [SerializeField] private AudioSource cardRevealSound;

    private List<CardInfo> cardsToReveal = new List<CardInfo>();
    private int currentCardIndex = 0;
    private bool isPackCut = false;
    private bool isRevealing = false;
    private string qrCode;

    [System.Serializable]
    private class CardInfo
    {
        public GameObject cardPrefab;
        public GameObject resultCardPrefab;
        public CardRarity rarity;
    }

    public enum CardRarity
    {
        Common,
        Strange,
        Deluxe
    }

    private void Start()
    {

        DataManager.Initialize();

        // Configurar botones
        tapToContinueButton.onClick.AddListener(RevealNextCard);
        continueButton.onClick.AddListener(() => SceneManager.LoadScene("Scenes/MainScene"));

        // Inicializar UI
        packStage.SetActive(true);
        cardsRevealPanel.SetActive(false);
        resultScreen.SetActive(false);

        // Obtener código QR escaneado
        qrCode = PlayerPrefs.GetString("LastScannedQR", "");
        if (string.IsNullOrEmpty(qrCode))
        {
            Debug.LogWarning("No se encontró código QR. Usando pack predeterminado.");
            qrCode = System.DateTime.Now.Ticks.ToString(); // Semilla aleatoria
        }

        // Determinar cartas basadas en el QR
        GenerateCardsFromQR();

        // Inicializar instrucciones
        instructionText.text = "Desliza para abrir el paquete";

        // Configurar detección de corte
        cutLineArea.gameObject.AddComponent<PackCutter>().Initialize(this);
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

    private void OnEnable()
    {
        // Al activarse la escena, verificar que los datos estén actualizados
        DataManager.LoadData();
    }

    private void OnDisable()
    {
        // Al desactivarse la escena, guardar datos
        DataManager.SaveData();
    }

    private void GenerateCardsFromQR()
    {
        // Usar QR como semilla para determinar cartas
        int seed = qrCode.GetHashCode();
        Random.InitState(seed);

        // Generar distribución: 3 Common, 2 Strange, 1 Deluxe
        // Common cards
        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, commonCardPrefabs.Count);
            CardInfo card = new CardInfo
            {
                cardPrefab = commonCardPrefabs[index],
                resultCardPrefab = commonResultCardPrefabs[index],
                rarity = CardRarity.Common
            };
            cardsToReveal.Add(card);
        }

        // Strange cards
        for (int i = 0; i < 2; i++)
        {
            int index = Random.Range(0, strangeCardPrefabs.Count);
            CardInfo card = new CardInfo
            {
                cardPrefab = strangeCardPrefabs[index],
                resultCardPrefab = strangeResultCardPrefabs[index],
                rarity = CardRarity.Strange
            };
            cardsToReveal.Add(card);
        }

        // Deluxe card (el último mostrado)
        int deluxeIndex = Random.Range(0, deluxeCardPrefabs.Count);
        CardInfo deluxeCard = new CardInfo
        {
            cardPrefab = deluxeCardPrefabs[deluxeIndex],
            resultCardPrefab = deluxeResultCardPrefabs[deluxeIndex],
            rarity = CardRarity.Deluxe
        };
        cardsToReveal.Add(deluxeCard);

        // Mezclar las cartas (excepto la Deluxe que siempre va al final)
        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, 5);
            CardInfo temp = cardsToReveal[i];
            cardsToReveal[i] = cardsToReveal[randomIndex];
            cardsToReveal[randomIndex] = temp;
        }
    }

    public void PackCut()
    {
        if (isPackCut) return;

        isPackCut = true;

        // Reproducir efectos de apertura
        if (cutPackEffect != null)
            cutPackEffect.Play();

        if (packOpenSound != null)
            packOpenSound.Play();

        // Animación de apertura del paquete
        StartCoroutine(OpenPackAnimation());
    }

    private void ConfigureResultCardLayout()
    {

        // Verificar que resultCardsContainer no sea nulo
        if (resultCardsContainer == null)
        {
            Debug.LogError("ResultCardsContainer no está asignado en el Inspector. Por favor, asigne este campo.");
            return;
        }

        // Obtener o añadir Grid Layout Group
        GridLayoutGroup gridLayout = resultCardsContainer.GetComponent<GridLayoutGroup>();


        // Configurar el RectTransform primero
        RectTransform rt = resultCardsContainer.GetComponent<RectTransform>();


        // Configurar Grid Layout para cartas pequeñas en 3 columnas
        gridLayout.cellSize = new Vector2(300, 400); // Ajustar según necesidad
        gridLayout.spacing = new Vector2(20, 20);

        // Asegurarse de que el Content Size Fitter funcione correctamente
        ContentSizeFitter fitter = resultCardsContainer.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = resultCardsContainer.gameObject.AddComponent<ContentSizeFitter>();

        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private IEnumerator OpenPackAnimation()
    {
        // Animar apertura del paquete
        float duration = 1.5f;
        float time = 0;

        while (time < duration)
        {
            // Animar paquete abriéndose (escala, rotación, etc.)
            float t = time / duration;
            packObject.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.2f, 0.8f, 1), t);
            packObject.color = new Color(1, 1, 1, 1 - t);

            time += Time.deltaTime;
            yield return null;
        }

        // Cambiar a panel de revelación de cartas
        packStage.SetActive(false);
        cardsRevealPanel.SetActive(true);

        // Iniciar revelación de la primera carta
        currentCardIndex = 0;
        RevealCurrentCard();
    }

    private void RevealCurrentCard()
    {
        if (currentCardIndex >= cardsToReveal.Count)
        {
            ShowResultScreen();
            return;
        }

        // Actualizar contador
        cardCounter.text = $"Carta {currentCardIndex + 1} de {cardsToReveal.Count}";

        // Limpiar posición de display
        foreach (Transform child in cardDisplayPosition)
        {
            Destroy(child.gameObject);
        }

        // Instanciar carta actual
        CardInfo currentCard = cardsToReveal[currentCardIndex];

        // Usar WorldSpace en lugar de coordenadas de pantalla
        GameObject cardObj = Instantiate(currentCard.cardPrefab, cardDisplayPosition);
        cardObj.transform.localPosition = Vector3.zero;

        // Forzar posición en coordenadas de mundo
        cardObj.transform.position = cardDisplayPosition.position;
        cardObj.transform.localRotation = Quaternion.Euler(-90, 180, 0); // Rota 90 grados en X para enderezar la carta

        // Mantén la escala original del prefab o ajústala si es necesario
        // cardObj.transform.localScale = Vector3.one;  // Descomentar si necesitas resetear la escala

        Debug.Log($"Carta instanciada: {currentCard.cardPrefab.name} en posición MUNDO: {cardObj.transform.position}");

        // Verificar si el renderer es visible
        Renderer renderer = cardObj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            // Forzar el renderer a ser visible
            renderer.enabled = true;
            Debug.Log($"Renderer habilitado forzosamente: {renderer.enabled}");
        }
        else
        {
            Debug.LogWarning($"No se encontró Renderer en {currentCard.cardPrefab.name}");
        }

        // Animar revelación
        StartCoroutine(AnimateCardReveal(cardObj, currentCard.rarity));
    }

    private IEnumerator AnimateCardReveal(GameObject card, CardRarity rarity)
    {
        isRevealing = true;
        tapToContinueButton.interactable = false;

        // Esperar un momento
        yield return new WaitForSeconds(0.5f);

        // Reproducir efectos según rareza
        // (código existente para efectos)

        // CAMBIO 1: Asegúrate de que la carta esté siempre delante del Canvas
        // Obtener el Canvas y poner la carta delante de él
        Canvas mainCanvas = cardsRevealPanel.GetComponent<Canvas>();
        if (mainCanvas != null)
        {
            // Obtener todos los renderers de la carta
            Renderer[] renderers = card.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                // Asignar un sortingOrder mayor que el Canvas
                renderer.sortingOrder = mainCanvas.sortingOrder + 1;

                // Opcional: asignar un sortingLayerName específico
                renderer.sortingLayerName = "ForegroundCards";
            }
        }

        // CAMBIO 2: Modificar la animación para controlar la posición Z
        float duration = 1.0f;
        float time = 0;

        Quaternion startRot = card.transform.localRotation;
        Quaternion targetRot = Quaternion.Euler(90, 180, 0);

        // Posición inicial
        Vector3 startPos = card.transform.position;
        // Guardar la posición actual, pero ajustar Z para estar siempre delante
        Vector3 targetPos = startPos;
        targetPos.z = startPos.z - 0.5f; // Acercar más a la cámara

        while (time < duration)
        {
            float t = time / duration;

            // Rotación gradual
            card.transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);

            // Interpolación de posición para ajustar Z durante la rotación
            // Esto evita que atraviese el fondo durante la rotación
            card.transform.position = Vector3.Lerp(startPos, targetPos, t);

            time += Time.deltaTime;
            yield return null;
        }

        // Asegurar posición y rotación finales
        card.transform.localRotation = targetRot;
        card.transform.position = targetPos;

        // Activar botón para continuar
        tapToContinueButton.interactable = true;
        isRevealing = false;
    }

    public void RevealNextCard()
    {
        if (isRevealing) return;

        currentCardIndex++;
        RevealCurrentCard();
    
    
    }
    private void CreateGlowEffect(GameObject cardObject)
    {
        // Verificar si es un objeto UI o 3D
        RectTransform cardRT = cardObject.GetComponent<RectTransform>();

        if (cardRT != null)
        {
            // Para objetos UI (con RectTransform)
            GameObject glowObj = new GameObject("GlowEffect");
            glowObj.transform.SetParent(cardObject.transform);
            glowObj.transform.SetAsFirstSibling(); // Poner detrás de la carta

            // Configurar RectTransform
            RectTransform glowRT = glowObj.AddComponent<RectTransform>();
            glowRT.anchorMin = Vector2.zero;
            glowRT.anchorMax = Vector2.one;
            glowRT.offsetMin = new Vector2(-10f, -10f); // Extender 10px en cada dirección
            glowRT.offsetMax = new Vector2(10f, 10f);

            // Añadir componente Image
            Image glowImage = glowObj.AddComponent<Image>();
            glowImage.color = new Color(1f, 0.8f, 0.2f, 0.5f); // Color dorado semi-transparente

            // Intentar cargar sprite para el glow
            Sprite glowSprite = Resources.Load<Sprite>("Effects/GlowSprite");
            if (glowSprite != null)
            {
                glowImage.sprite = glowSprite;
            }

            // Añadir animación de pulsación (opcional)
            StartCoroutine(PulseGlowEffect(glowImage));
        }
        else
        {
            // Para objetos 3D (sin RectTransform)
            // Crear un objeto con material brillante alrededor del objeto 3D
            GameObject glowObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            glowObj.transform.SetParent(cardObject.transform);
            glowObj.transform.localPosition = Vector3.zero;
            glowObj.transform.localScale = cardObject.transform.localScale * 1.1f; // Ligeramente más grande

            // Configurar material
            Renderer renderer = glowObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material glowMaterial = new Material(Shader.Find("Standard"));
                glowMaterial.SetColor("_EmissionColor", new Color(1f, 0.8f, 0.2f, 0.5f));
                glowMaterial.EnableKeyword("_EMISSION");
                renderer.material = glowMaterial;
            }

            // Desactivar el collider del glow
            Collider collider = glowObj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }

    private IEnumerator PulseGlowEffect(Image glowImage)
    {
        float duration = 1.5f;
        float minAlpha = 0.4f;
        float maxAlpha = 0.8f;

        while (true)
        {
            // Pulsar de menor a mayor intensidad
            float time = 0;
            while (time < duration)
            {
                float t = time / duration;
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

                Color c = glowImage.color;
                c.a = alpha;
                glowImage.color = c;

                time += Time.deltaTime;
                yield return null;
            }

            // Pulsar de mayor a menor intensidad
            time = 0;
            while (time < duration)
            {
                float t = time / duration;
                float alpha = Mathf.Lerp(maxAlpha, minAlpha, t);

                Color c = glowImage.color;
                c.a = alpha;
                glowImage.color = c;

                time += Time.deltaTime;
                yield return null;
            }
        }
    }
    private void ShowResultScreen()
    {

        // Verificar componentes críticos
        if (resultScreen == null || resultCardsContainer == null)
        {
            Debug.LogError("Referencias faltantes para mostrar la pantalla de resultados. Verifica en el Inspector.");
            // Volver a la escena principal como fallback
            SceneManager.LoadScene("Scenes/MainScene");
            return;
        }

        // Cambiar a pantalla de resultados
        cardsRevealPanel.SetActive(false);
        resultScreen.SetActive(true);

        // Cambiar color de fondo
        Image backgroundImage = resultScreen.GetComponent<Image>();
        if (backgroundImage != null)
            backgroundImage.color = new Color(1f, 0.8f, 0.8f, 1f); // Rosa claro

        // Configurar título
        resultTitle.text = "¡Cartas Obtenidas!";

        // Configurar layout ANTES de instanciar las cartas
        ConfigureResultCardLayout();

        // Limpiar cartas existentes
        foreach (Transform child in resultCardsContainer)
        {
            Destroy(child.gameObject);
        }

        // Instanciar las cartas con ajustes adicionales
        foreach (CardInfo card in cardsToReveal)
        {
            // Crear carta resultado
            GameObject resultCard = Instantiate(card.resultCardPrefab, resultCardsContainer);

            // Verificar si es un objeto UI o un objeto 3D
            RectTransform cardRT = resultCard.GetComponent<RectTransform>();
            if (cardRT != null)
            {
                // Si es un objeto UI (tiene RectTransform)
            }
            else
            {
                // Si es un objeto 3D
                resultCard.transform.localRotation = Quaternion.Euler(90, 180, 0); // Rotación para que mire al frente

                // Opcionalmente, crear un contenedor UI para los objetos 3D
                GameObject uiContainer = new GameObject(resultCard.name + "_Container");
                uiContainer.transform.SetParent(resultCardsContainer);
                RectTransform containerRT = uiContainer.AddComponent<RectTransform>();
                containerRT.position = resultCard.transform.position; // Mantener la posición del objeto 3D
                containerRT.sizeDelta = new Vector2(300, 400); // Tamaño igual al de la celda

                // Mover el objeto 3D al contenedor UI
                resultCard.transform.SetParent(uiContainer.transform);
            }

            // Verificar y ajustar colliders
            BoxCollider2D boxCollider2D = resultCard.GetComponent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                boxCollider2D.enabled = false;
            }

            BoxCollider boxCollider3D = resultCard.GetComponent<BoxCollider>();
            if (boxCollider3D != null)
            {
                boxCollider3D.enabled = false;
            }

            // Crear efecto glow para cartas Deluxe
            if (card.rarity == CardRarity.Deluxe)
            {
                CreateGlowEffect(resultCard);
            }
        }

        // Esperar un frame para que el layout se actualice
        StartCoroutine(FixLayoutAfterDelay());

        // Guardar información de cartas obtenidas
        SaveObtainedCards();
    }

    private IEnumerator FixLayoutAfterDelay()
    {
        // Esperar un frame para que todos los componentes se inicialicen
        yield return null;

        // Forzar actualización de layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(resultCardsContainer.GetComponent<RectTransform>());

        // Esperar otro frame
        yield return null;

        // Verificar posición de cada carta para debugging
        int index = 0;
        foreach (Transform child in resultCardsContainer)
        {
            // Verificar si tiene RectTransform antes de acceder
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Debug.Log($"Carta {index}: Posición = {child.localPosition}, RectPosition = {rectTransform.anchoredPosition}");
            }
            else
            {
                Debug.Log($"Carta {index}: Posición = {child.localPosition}, No tiene RectTransform");
            }
            index++;
        }
    }

    private void SaveObtainedCards()
    {
        Debug.Log("🃏 GUARDANDO CARTAS: Inicio del proceso");

        try
        {
            if (DataManager.GetCurrentUsername() == null)
            {
                Debug.LogWarning("DataManager no inicializado. Inicializando...");
                DataManager.Initialize();
            }

            List<Card> newCards = new List<Card>();

            // Convertir CardInfo a Card para guardar
            foreach (CardInfo cardInfo in cardsToReveal)
            {
                if (cardInfo == null || cardInfo.cardPrefab == null)
                {
                    Debug.LogWarning("CardInfo nula o sin prefab. Omitiendo...");
                    continue;
                }

                Card card = new Card();
                card.id = System.Guid.NewGuid().ToString(); // ID único

                // Determinar nombre y tipo basado en prefab
                string prefabName = cardInfo.cardPrefab.name;
                card.name = prefabName;

                // Determinar imagePath
                card.imagePath = "Cards/" + prefabName;

                // Determinar tipo
                switch (cardInfo.rarity)
                {
                    case CardRarity.Common:
                        card.type = CardType.CommonBeiked;
                        break;
                    case CardRarity.Strange:
                        card.type = CardType.StrangeBeiked;
                        break;
                    case CardRarity.Deluxe:
                        card.type = CardType.DeluxeBeiked;
                        break;
                }

                // Determinar storyId y storyPart basados en el nombre
                string[] parts = prefabName.Split('-');
                if (parts.Length >= 2)
                {
                    // El formato esperado es "1-Common-Cinnamon" o similar

                    // Extraer storyId de la primera parte (el número)
                    if (int.TryParse(parts[0], out int storyNumber))
                    {
                        card.storyId = "story_" + storyNumber;
                    }
                    else
                    {
                        card.storyId = "story_1"; // Valor por defecto
                        Debug.LogWarning($"🃏 ADVERTENCIA: No se pudo determinar storyId para {prefabName}");
                    }

                    // Asignar storyPart basado en la parte específica de la carta
                    // CORRECIÓN: Usar un sistema consistente y claro
                    if (parts.Length >= 3)
                    {
                        // Extraer storyPart del nombre específico
                        // NUEVO: Mapeo directo y explícito para cada tipo de carta
                        string cardSpecificName = parts[2];

                        // Story 1 mappings
                        if (parts[0] == "1")
                        {
                            if (parts[1].Equals("Common", System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (cardSpecificName.Contains("Cinnamon")) card.storyPart = 1;
                                else if (cardSpecificName.Contains("Klim")) card.storyPart = 2;
                                else if (cardSpecificName.Contains("Kinder")) card.storyPart = 3;
                                else card.storyPart = 1;
                            }
                            else if (parts[1].Equals("Strange", System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (cardSpecificName.Contains("Birthday")) card.storyPart = 1;
                                else if (cardSpecificName.Contains("Milo")) card.storyPart = 2;
                                else card.storyPart = 1;
                            }
                            else card.storyPart = 1; // Deluxe siempre es 1
                        }
                        // Story 2 mappings
                        else if (parts[0] == "2")
                        {
                            if (parts[1].Equals("Common", System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (cardSpecificName.Contains("ChocolateChips")) card.storyPart = 1;
                                else if (cardSpecificName.Contains("KeyLimePie")) card.storyPart = 2;
                                else if (cardSpecificName.Contains("CaramelPecans")) card.storyPart = 3;
                                else card.storyPart = 1;
                            }
                            else if (parts[1].Equals("Strange", System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (cardSpecificName.Contains("Birthday")) card.storyPart = 1;
                                else if (cardSpecificName.Contains("Klim")) card.storyPart = 2;
                                else card.storyPart = 1;
                            }
                            else card.storyPart = 1; // Deluxe siempre es 1
                        }
                        // Story 3 mappings
                        else if (parts[0] == "3")
                        {
                            if (parts[1].Equals("Common", System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (cardSpecificName.Contains("ChocolateChips")) card.storyPart = 1;
                                else if (cardSpecificName.Contains("Cinnamon")) card.storyPart = 2;
                                else if (cardSpecificName.Contains("KeyLimePie")) card.storyPart = 3;
                                else card.storyPart = 1;
                            }
                            else if (parts[1].Equals("Strange", System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (cardSpecificName.Contains("LaDeMilo")) card.storyPart = 1;
                                else if (cardSpecificName.Contains("MoltenLava")) card.storyPart = 2;
                                else card.storyPart = 1;
                            }
                            else card.storyPart = 1; // Deluxe siempre es 1
                        }
                    }
                    else
                    {
                        card.storyPart = 1; // Valor por defecto
                    }
                }
                else
                {
                    // Formato de nombre no reconocido
                    card.storyId = "story_1";
                    card.storyPart = 1;
                    Debug.LogWarning($"🃏 ADVERTENCIA: Formato de nombre no reconocido: {prefabName}");
                }

                // Logging detallado para diagnóstico
                Debug.Log($"🃏 CARTA CREADA: ID={card.id}, Nombre={prefabName}, " +
                          $"Historia={card.storyId}, Parte={card.storyPart}, Tipo={card.type}");

                newCards.Add(card);
            }

            // Guardar todas las cartas
            if (newCards.Count > 0)
            {
                Debug.Log($"🃏 GUARDANDO: {newCards.Count} cartas nuevas");
                DataManager.AddCardsFromPack(newCards);
                Debug.Log("🃏 GUARDADO COMPLETADO");
            }
            else
            {
                Debug.LogWarning("🃏 ADVERTENCIA: No hay cartas para guardar");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"🃏 ERROR GUARDANDO CARTAS: {e.Message}\n{e.StackTrace}");
        }
    }
}