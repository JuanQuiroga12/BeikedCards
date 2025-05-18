using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryCollection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI storyTitleText;
    [SerializeField] private Transform commonCardsContainer;
    [SerializeField] private Transform strangeCardsContainer;
    [SerializeField] private Transform deluxeCardContainer;
    [SerializeField] private GameObject cardSlotPrefab;
    [SerializeField] private Button redeemButton;
    [SerializeField] private Button viewCodeButton;

    private string storyId;
    private bool isComplete = false;
    private Dictionary<string, CardSlot> cardSlots = new Dictionary<string, CardSlot>();
    private bool hasAnyCards = false;

    private void Start()
    {
        // Configurar contenedores de cartas con tama�os apropiados
        if (commonCardsContainer != null)
        {
            RectTransform rectTransform = commonCardsContainer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(800, 200); // Ancho, Alto
            }

            // Agregar horizontal layout group si no existe
            HorizontalLayoutGroup layout = commonCardsContainer.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = commonCardsContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 10;
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
            }
        }

        // Similar para strangeCardsContainer y deluxeCardContainer
        // ...

        // Configurar contenedores de cartas con tama�os apropiados
        if (commonCardsContainer != null)
        {
            RectTransform rectTransform = strangeCardsContainer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(800, 200); // Ancho, Alto
            }

            // Agregar horizontal layout group si no existe
            HorizontalLayoutGroup layout = strangeCardsContainer.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = commonCardsContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 10;
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
            }
        }

        if (commonCardsContainer != null)
        {
            RectTransform rectTransform = deluxeCardContainer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(800, 200); // Ancho, Alto
            }
        }
    }

    public void Initialize(string id, string title, Dictionary<string, string> cardToPrefabMap)
    {
        this.storyId = id;
        storyTitleText.text = title;

        ConfigureCardContainers();

        // Cargar todas las cartas de esta colecci�n
        LoadStoryCards(cardToPrefabMap);

        // Verificar completitud
        CheckCompletion();

        // Configurar botones
        redeemButton.onClick.AddListener(RedeemCookie);
        viewCodeButton.onClick.AddListener(ViewCookieCode);

        // Inicialmente ocultar botones
        redeemButton.gameObject.SetActive(isComplete && !HasCookieCode());
        viewCodeButton.gameObject.SetActive(isComplete && HasCookieCode());

        DebugCardData();
    }

    private void LoadStoryCards(Dictionary<string, string> cardToPrefabMap)
    {
        try
        {
            // Obtener todas las cartas de esta historia
            List<Card> storyCards = DataManager.GetCardsByStory(storyId);
            hasAnyCards = storyCards != null && storyCards.Count > 0;

            // Para limpiar slots anteriores
            if (commonCardsContainer != null)
                foreach (Transform child in commonCardsContainer) Destroy(child.gameObject);
            if (strangeCardsContainer != null)
                foreach (Transform child in strangeCardsContainer) Destroy(child.gameObject);
            if (deluxeCardContainer != null)
                foreach (Transform child in deluxeCardContainer) Destroy(child.gameObject);

            // Crear slots para Common (3 slots)
            for (int i = 1; i <= 3; i++)
            {
                try
                {
                    if (cardSlotPrefab == null || commonCardsContainer == null)
                    {
                        Debug.LogError("CardSlotPrefab o commonCardsContainer son nulos");
                        continue;
                    }

                    GameObject slotObj = Instantiate(cardSlotPrefab, commonCardsContainer);
                    CardSlot slot = slotObj.GetComponent<CardSlot>();

                    if (slot == null)
                    {
                        Debug.LogError("El prefab CardSlot no tiene componente CardSlot");
                        continue;
                    }

                    // Guardar referencia al slot
                    string cardKey = $"{storyId.Replace("story_", "")}-{i}-Common";
                    cardSlots[cardKey] = slot;

                    // Buscar si tenemos esta carta
                    Card card = null;
                    if (storyCards != null)
                    {
                        card = storyCards.Find(c => c.type == CardType.CommonBeiked && c.storyPart == i);
                    }

                    if (card != null)
                    {
                        // Verificar si la clave existe antes de usarla
                        if (cardToPrefabMap.TryGetValue(cardKey, out string prefabName))
                        {
                            slot.SetCard(card, prefabName);
                        }
                        else
                        {
                            Debug.LogWarning($"Clave no encontrada en el mapeo: {cardKey}");
                            slot.SetEmpty();
                        }
                    }
                    else
                    {
                        slot.SetEmpty();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error al procesar carta Common {i}: {e.Message}");
                }
            }

            // Crear slots para Strange (2 slots)
            for (int i = 1; i <= 2; i++)
            {
                try
                {
                    if (cardSlotPrefab == null || strangeCardsContainer == null)
                    {
                        Debug.LogError("CardSlotPrefab o strangeCardsContainer son nulos");
                        continue;
                    }

                    GameObject slotObj = Instantiate(cardSlotPrefab, strangeCardsContainer);
                    CardSlot slot = slotObj.GetComponent<CardSlot>();

                    // Guardar referencia al slot
                    string cardKey = $"{storyId.Replace("story_", "")}-{i}-Strange";
                    cardSlots[cardKey] = slot;

                    // Buscar si tenemos esta carta
                    Card card = storyCards?.Find(c => c.type == CardType.StrangeBeiked && c.storyPart == i);

                    if (card != null)
                    {
                        // Verificar si la clave existe antes de usarla
                        if (cardToPrefabMap.TryGetValue(cardKey, out string prefabName))
                        {
                            slot.SetCard(card, prefabName);
                        }
                        else
                        {
                            Debug.LogWarning($"Clave no encontrada en el mapeo: {cardKey}");
                            slot.SetEmpty();
                        }
                    }
                    else
                    {
                        slot.SetEmpty();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error al procesar carta Strange {i}: {e.Message}");
                }
            }

            // Crear slot para Deluxe (1 slot)
            try
            {
                if (cardSlotPrefab == null || deluxeCardContainer == null)
                {
                    Debug.LogError("CardSlotPrefab o deluxeCardContainer son nulos");
                    return;
                }

                GameObject deluxeSlotObj = Instantiate(cardSlotPrefab, deluxeCardContainer);
                CardSlot deluxeSlot = deluxeSlotObj.GetComponent<CardSlot>();

                // Guardar referencia al slot
                string deluxeKey = $"{storyId.Replace("story_", "")}-1-Deluxe";
                cardSlots[deluxeKey] = deluxeSlot;

                // Buscar si tenemos la carta Deluxe
                Card deluxeCard = storyCards?.Find(c => c.type == CardType.DeluxeBeiked);

                if (deluxeCard != null)
                {
                    // Verificar si la clave existe antes de usarla
                    if (cardToPrefabMap.TryGetValue(deluxeKey, out string prefabName))
                    {
                        deluxeSlot.SetCard(deluxeCard, prefabName);
                    }
                    else
                    {
                        Debug.LogWarning($"Clave no encontrada en el mapeo: {deluxeKey}");
                        deluxeSlot.SetEmpty();
                    }
                }
                else
                {
                    deluxeSlot.SetEmpty();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al procesar carta Deluxe: {e.Message}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar cartas para la historia {storyId}: {e.Message}");
        }
    }

    private void CheckCompletion()
    {
        // Verificar si ya est� marcada como completa
        if (DataManager.IsStoryComplete(storyId))
        {
            isComplete = true;
            return;
        }

        // Obtener todas las cartas de esta historia
        List<Card> storyCards = DataManager.GetCardsByStory(storyId);

        // Verificar Common (necesitamos 3)
        int commonCount = storyCards.FindAll(c => c.type == CardType.CommonBeiked).Count;

        // Verificar Strange (necesitamos 2)
        int strangeCount = storyCards.FindAll(c => c.type == CardType.StrangeBeiked).Count;

        // Verificar Deluxe (necesitamos 1)
        int deluxeCount = storyCards.FindAll(c => c.type == CardType.DeluxeBeiked).Count;

        // Est� completa si tenemos todas las cartas necesarias
        isComplete = (commonCount >= 3 && strangeCount >= 2 && deluxeCount >= 1);

        // Si est� completa, marcarla como tal
        if (isComplete)
        {
            DataManager.AddCompletedStory(storyId);
        }
    }

    private void RedeemCookie()
    {
        // Guardar referencia a la historia para redimir
        PlayerPrefs.SetString("RedeemStoryId", storyId);

        // Cargar escena de redenci�n
        SceneManager.LoadScene("CookieRedemptionScene");
    }

    private bool HasCookieCode()
    {
        return DataManager.GetCookieCode(storyId) != null;
    }

    private void ViewCookieCode()
    {
        string code = DataManager.GetCookieCode(storyId);

        if (!string.IsNullOrEmpty(code))
        {
            // Crear un panel para mostrar el c�digo
            GameObject codePanel = new GameObject("CodePanel");
            codePanel.transform.SetParent(transform);

            // A�adir componentes UI
            RectTransform rt = codePanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(1080.0f, 1920.0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(400, 300);

            // Fondo del panel
            Image bg = codePanel.AddComponent<Image>();
            bg.color = new Color(0.9f, 0.9f, 0.9f, 0.95f);

            // Texto del c�digo
            GameObject textObj = new GameObject("CodeText");
            textObj.transform.SetParent(codePanel.transform);
            TextMeshProUGUI codeText = textObj.AddComponent<TextMeshProUGUI>();
            codeText.text = "Tu c�digo de galleta:\n\n" + code;
            codeText.fontSize = 24;
            codeText.alignment = TextAlignmentOptions.Center;

            RectTransform textRT = codeText.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = new Vector2(20, 60);
            textRT.offsetMax = new Vector2(-20, -20);

            // Bot�n para cerrar
            GameObject closeButtonObj = new GameObject("CloseButton");
            closeButtonObj.transform.SetParent(codePanel.transform);
            Button closeButton = closeButtonObj.AddComponent<Button>();
            Image closeButtonImage = closeButtonObj.AddComponent<Image>();
            closeButtonImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);

            RectTransform closeButtonRT = closeButton.GetComponent<RectTransform>();
            closeButtonRT.anchorMin = new Vector2(0.5f, 0);
            closeButtonRT.anchorMax = new Vector2(0.5f, 0);
            closeButtonRT.pivot = new Vector2(0.5f, 0);
            closeButtonRT.sizeDelta = new Vector2(120, 40);
            closeButtonRT.anchoredPosition = new Vector2(0, 20);

            // Texto del bot�n
            GameObject closeTextObj = new GameObject("CloseText");
            closeTextObj.transform.SetParent(closeButtonObj.transform);
            TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
            closeText.text = "Cerrar";
            closeText.fontSize = 20;
            closeText.alignment = TextAlignmentOptions.Center;

            RectTransform closeTextRT = closeText.GetComponent<RectTransform>();
            closeTextRT.anchorMin = Vector2.zero;
            closeTextRT.anchorMax = Vector2.one;
            closeTextRT.offsetMin = Vector2.zero;
            closeTextRT.offsetMax = Vector2.zero;

            // Evento de cierre
            closeButton.onClick.AddListener(() => Destroy(codePanel));
        }
    }

    private void DebugCardData()
    {
        // Obtener las cartas de esta historia
        List<Card> storyCards = DataManager.GetCardsByStory(storyId);

        // Imprimir informaci�n
        Debug.Log($"Historia: {storyId}, T�tulo: {storyTitleText.text}");
        Debug.Log($"Total de cartas para esta historia: {(storyCards != null ? storyCards.Count : 0)}");

        if (storyCards != null)
        {
            foreach (Card card in storyCards)
            {
                Debug.Log($"Carta: {card.id}, Tipo: {card.type}, Parte: {card.storyPart}, Nombre: {card.name}");
            }
        }
    }

    private void ConfigureCardContainers()
    {
        // Configurar contenedor para Common Cards
        if (commonCardsContainer != null)
        {
            // Asegurarse de que tiene un RectTransform adecuado
            RectTransform rectTransform = commonCardsContainer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(800, 150);
            }

            // Configurar HorizontalLayoutGroup
            HorizontalLayoutGroup layout = commonCardsContainer.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = commonCardsContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        // Configuraci�n similar para Strange y Deluxe
        if (strangeCardsContainer != null)
        {
            RectTransform rectTransform = strangeCardsContainer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(800, 150);
            }

            HorizontalLayoutGroup layout = strangeCardsContainer.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = strangeCardsContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        if (deluxeCardContainer != null)
        {
            RectTransform rectTransform = deluxeCardContainer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(800, 150);
            }

            // Para el contenedor de Deluxe, podr�amos usar HorizontalLayoutGroup tambi�n
            // para mantener consistencia, aunque solo tenga una carta
            HorizontalLayoutGroup layout = deluxeCardContainer.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = deluxeCardContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }
    }

    public bool HasAnyCards()
    {
        return hasAnyCards;
    }
}