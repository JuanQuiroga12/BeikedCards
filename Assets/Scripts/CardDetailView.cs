using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CardDetailView : MonoBehaviour
{
    [SerializeField] private Transform cardModelParent;
    [SerializeField] private GameObject commonCardModel;
    [SerializeField] private GameObject strangeCardModel;
    [SerializeField] private GameObject deluxeCardModel;
    [SerializeField] private Button backButton;

    private GameObject currentCardModel;
    private Card displayedCard;
    private float rotationSpeed = 50f;
    private bool isDragging = false;
    private Vector3 previousMousePosition;

    private void Start()
    {
        backButton.onClick.AddListener(() => SceneManager.LoadScene("CollectionScene"));

        // Cargar carta seleccionada
        string cardId = PlayerPrefs.GetString("SelectedCardId", "");

        if (!string.IsNullOrEmpty(cardId))
        {
            // Encontrar la carta en la colección
            List<Card> allCards = DataManager.GetAllCards();
            displayedCard = allCards.Find(c => c.id == cardId);

            if (displayedCard != null)
            {
                // Instanciar modelo 3D según tipo
                InstantiateCardModel();

                // Configurar modelo (texturas, etc.)
                ConfigureCardModel();
            }
        }
    }

    private void InstantiateCardModel()
    {
        // Limpiar modelo anterior si existe
        if (currentCardModel != null)
        {
            Destroy(currentCardModel);
        }

        // Instanciar según tipo
        switch (displayedCard.type)
        {
            case CardType.CommonBeiked:
                currentCardModel = Instantiate(commonCardModel, cardModelParent);
                break;

            case CardType.StrangeBeiked:
                currentCardModel = Instantiate(strangeCardModel, cardModelParent);
                break;

            case CardType.DeluxeBeiked:
                currentCardModel = Instantiate(deluxeCardModel, cardModelParent);
                break;
        }
    }

    private void ConfigureCardModel()
    {
        // Configurar el modelo con texturas, materiales, etc.
        MeshRenderer renderer = currentCardModel.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            // Cargar textura desde recursos
            Texture2D frontTexture = Resources.Load<Texture2D>(displayedCard.imagePath + "_3D_Front");
            Texture2D backTexture = Resources.Load<Texture2D>(displayedCard.imagePath + "_3D_Back");

            if (frontTexture != null && backTexture != null)
            {
                Material[] materials = renderer.materials;

                // Asignar texturas (asumiendo que el primer material es el frente y el segundo es el reverso)
                materials[0].mainTexture = frontTexture;
                materials[1].mainTexture = backTexture;

                renderer.materials = materials;
            }
        }
    }

    private void Update()
    {
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

        if (isDragging && currentCardModel != null)
        {
            Vector3 delta = Input.mousePosition - previousMousePosition;

            // Rotar el modelo basado en movimiento horizontal del mouse
            currentCardModel.transform.Rotate(Vector3.up, -delta.x * rotationSpeed * Time.deltaTime);

            previousMousePosition = Input.mousePosition;
        }
    }
}