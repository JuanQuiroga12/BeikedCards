using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Image cardBorder;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;
    [SerializeField] private GameObject commonElements;
    [SerializeField] private GameObject strangeElements;
    [SerializeField] private GameObject deluxeElements;

    public void SetupCard(Card card)
    {
        // Configurar nombre y descripción
        cardNameText.text = card.name;
        cardDescriptionText.text = card.description;

        // Cargar imagen de la carta
        Sprite cardSprite = Resources.Load<Sprite>(card.imagePath);
        if (cardSprite != null)
        {
            cardImage.sprite = cardSprite;
        }
        else
        {
            // Usar una imagen predeterminada si no se encuentra la específica
            cardImage.sprite = Resources.Load<Sprite>("Cards/DefaultCard");
        }

        // Configurar apariencia según tipo
        switch (card.type)
        {
            case CardType.CommonBeiked:
                commonElements.SetActive(true);
                strangeElements.SetActive(false);
                deluxeElements.SetActive(false);
                cardBorder.color = Color.white;
                break;

            case CardType.StrangeBeiked:
                commonElements.SetActive(false);
                strangeElements.SetActive(true);
                deluxeElements.SetActive(false);
                cardBorder.color = new Color(0.8f, 0.8f, 1f); // Tono plateado/cromado
                break;

            case CardType.DeluxeBeiked:
                commonElements.SetActive(false);
                strangeElements.SetActive(false);
                deluxeElements.SetActive(true);
                cardBorder.color = new Color(1f, 0.8f, 0.2f); // Tono dorado
                break;
        }
    }
}