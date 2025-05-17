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

        // Configurar apariencia según tipo antes de intentar cargar modelo
        switch (card.type)
        {
            case CardType.CommonBeiked:
                slotBackground.color = new Color(0.7f, 0.9f, 0.7f); // Verde claro
                break;
            case CardType.StrangeBeiked:
                slotBackground.color = new Color(0.7f, 0.7f, 0.9f); // Azul claro
                break;
            case CardType.DeluxeBeiked:
                slotBackground.color = new Color(0.9f, 0.9f, 0.5f); // Amarillo claro
                break;
        }

        // Ocultar imagen 2D si existe
        if (cardImage != null)
            cardImage.gameObject.SetActive(false);

        // Verificar si tenemos contenedor para modelos 3D
        if (cardModelContainer == null)
        {
            Debug.LogWarning("No hay contenedor para modelos 3D en el CardSlot");
            if (cardImage != null)
            {
                // Si no hay contenedor 3D, mostrar una imagen básica
                cardImage.gameObject.SetActive(true);
                cardImage.color = slotBackground.color;
            }
        }
        else
        {
            // Limpiar modelos anteriores
            foreach (Transform child in cardModelContainer)
            {
                Destroy(child.gameObject);
            }

            // Crear un cubo simple como representación visual
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
                currentModel = placeholder;
            }

            // Mostrar información de la carta
            GameObject textObj = new GameObject("CardText");
            textObj.transform.SetParent(cardModelContainer);
            textObj.transform.localPosition = new Vector3(0, 0.1f, 0);
            textObj.transform.localRotation = Quaternion.Euler(90, 0, 0);
            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = card.name.Split('-').Length > 2 ? card.name.Split('-')[2] : "Carta";
            textMesh.characterSize = 0.1f;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;
        }

        // Ocultar overlay de slot vacío
        if (emptyStateOverlay != null)
            emptyStateOverlay.SetActive(false);

        // Configurar botón para ver detalle
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(ViewCardDetail);
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