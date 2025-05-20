using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CardSlot : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Image slotBackground;
    [SerializeField] private Button cardButton;
    [SerializeField] private GameObject emptyStateOverlay;
    [SerializeField] private Transform cardModelContainer; // Contenedor para el modelo 3D

    private Card card = null;
    private string prefabName = "";
    private GameObject currentModel = null;

    public void SetCard(Card card, string prefabName)
    {
        this.card = card;
        this.prefabName = prefabName;

        // Get the real card name from the database
        string realCardName = CardDatabase.GetCardName(prefabName);


        // Configurar apariencia según tipo
        switch (card.type)
        {
            case CardType.CommonBeiked:
                slotBackground.color = new Color(0.7f, 0.9f, 0.7f);
                break;
            case CardType.StrangeBeiked:
                slotBackground.color = new Color(0.7f, 0.7f, 0.9f);
                break;
            case CardType.DeluxeBeiked:
                slotBackground.color = new Color(0.9f, 0.9f, 0.5f);
                break;
        }

        // Ocultar overlay de slot vacío
        if (emptyStateOverlay != null)
            emptyStateOverlay.SetActive(false);

        // Limpiar modelos anteriores
        if (cardModelContainer != null)
        {
            foreach (Transform child in cardModelContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // MODIFICACIÓN: Primero verificamos si existe un prefab con este nombre en Resources
        GameObject cardPrefab = Resources.Load<GameObject>("Cards/" + prefabName);

        if (cardPrefab != null)
        {
            // Instanciar el prefab de la carta
            GameObject cardInstance = Instantiate(cardPrefab, cardModelContainer);

            // MODIFICACIÓN: Configuración mejorada para la carta
            cardInstance.transform.localPosition = Vector3.zero;
            cardInstance.transform.localRotation = Quaternion.Euler(90, 180, 0); // Ajuste de rotación para que mire al frente
            cardInstance.transform.localScale = Vector3.one * 120f; // Ajuste de escala más pequeña para que quepa en el slot

            // MODIFICACIÓN: Asegurarse de que los materiales son visibles
            Renderer[] renderers = cardInstance.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                // Asegurarse de que esté en una capa visible para la cámara principal
                renderer.gameObject.layer = LayerMask.NameToLayer("Default");

                // Verificar si hay materiales asignados
                if (renderer.sharedMaterial == null)
                {
                    // Crear un material por defecto si no hay uno
                    Material defaultMat = new Material(Shader.Find("Standard"));
                    defaultMat.color = Color.white;
                    renderer.material = defaultMat;
                }
            }

            // Asegurarse de que todos los componentes del prefab estén activados
            foreach (Transform child in cardInstance.transform)
            {
                child.gameObject.SetActive(true);
            }

            // Guardar referencia al modelo actual
            currentModel = cardInstance;

            Debug.Log($"Carta cargada: {prefabName} en {card.name}. Posición: {cardInstance.transform.localPosition}, Escala: {cardInstance.transform.localScale}");
        }
        else
        {
            Debug.LogWarning($"No se pudo cargar el prefab: Cards/{prefabName}. Creando placeholder...");
            CreatePlaceholderCard(card);
        }

        // Configurar botón para ver detalle
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(ViewCardDetail);
    }

    private void SetupFor3DCard()
    {
        // Verificar si tenemos un contenedor para el modelo 3D
        if (cardModelContainer == null)
        {
            // Crear un contenedor si no existe
            GameObject containerObj = new GameObject("CardModelContainer");
            containerObj.transform.SetParent(transform);
            cardModelContainer = containerObj.transform;

            // Configurar posición y escala
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 size = rectTransform != null ? rectTransform.sizeDelta : new Vector2(100, 150);

            // Configurar el contenedor para que esté centrado en el CardSlot
            cardModelContainer.localPosition = Vector3.zero;
            cardModelContainer.localRotation = Quaternion.identity;

            // Añadir un componente para controlar la rotación de la carta (opcional)
            CardRotator rotator = containerObj.AddComponent<CardRotator>();
            rotator.rotationSpeed = 20f;
        }
    }

    private void CreatePlaceholderCard(Card card)
    {
        // Crear un cubo como placeholder visual
        GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        placeholder.transform.SetParent(cardModelContainer);
        placeholder.transform.localPosition = Vector3.zero;
        placeholder.transform.localScale = new Vector3(0.8f, 0.1f, 1.2f);
        placeholder.transform.localRotation = Quaternion.Euler(0, 0, 0);

        // Asignar material según el tipo de carta
        Renderer renderer = placeholder.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));

            switch (card.type)
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
                    mat.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.2f, 0.5f) * 0.5f);
                    break;
            }

            renderer.material = mat;
        }

        // Agregar texto con el nombre de la carta
        GameObject textObj = new GameObject("CardText");
        textObj.transform.SetParent(placeholder.transform);
        textObj.transform.localPosition = new Vector3(0, 0.1f, 0);
        textObj.transform.localRotation = Quaternion.Euler(90, 0, 0);
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = card.name.Split('-').Length > 2 ? card.name.Split('-')[2] : "Carta";
        textMesh.characterSize = 0.1f;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.black;

        currentModel = placeholder;
    }

    public void SetEmpty()
    {
        this.card = null;
        this.prefabName = "";

        // Ocultar imagen de carta
        if (cardImage != null)
            cardImage.gameObject.SetActive(false);

        // Limpiar modelos 3D
        if (cardModelContainer != null)
        {
            foreach (Transform child in cardModelContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Configurar apariencia de slot vacío
        slotBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        // Mostrar overlay de slot vacío
        if (emptyStateOverlay != null)
            emptyStateOverlay.SetActive(true);

        // Desactivar botón
        cardButton.onClick.RemoveAllListeners();
    }

    private void ViewCardDetail()
    {
        if (card != null)
        {
            // Guardar referencia a la carta seleccionada
            PlayerPrefs.SetString("SelectedCardId", card.id);
            PlayerPrefs.SetString("SelectedCardPrefab", prefabName);

            // Cargar escena de detalle
            SceneManager.LoadScene("CardDetailScene");
        }
    }
}