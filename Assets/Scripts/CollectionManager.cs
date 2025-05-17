using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CollectionManager : MonoBehaviour
{
    [SerializeField] private RectTransform collectionContent;
    [SerializeField] private GameObject storyCollectionPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject emptyCollectionMessage;

    // Definiciуn de historias disponibles
    private Dictionary<string, string> storyNames = new Dictionary<string, string>
    {
        { "story_1", "Crónicas de una familia dulce" },
        { "story_2", "El círculo de las galletas encantadas" },
        { "story_3", "El Despertar de Campfire" }
    };

    // Definición del mapeo de cartas por ID a prefabs
    private Dictionary<string, string> cardToPrefabMap = new Dictionary<string, string>
    {
        // Historia 1
        { "1-1-Common", "1-Common-Cinnamon" },
        { "1-2-Common", "1-Common-Klim" },
        { "1-3-Common", "1-Common-Kinder" },
        { "1-1-Strange", "1-Strange-Birthday" },
        { "1-2-Strange", "1-Strange-Milo" },
        { "1-1-Deluxe", "1-Deluxe-BrowniePistacho" },
    
        // Historia 2
        { "2-1-Common", "2-Common-ChocolateChips" },
        { "2-2-Common", "2-Common-KeyLimePie" },
        { "2-3-Common", "2-Common-CaramelPecans" },
        { "2-1-Strange", "2-Strange-Birthday" },
        { "2-2-Strange", "2-Strange-Klim" },
        { "2-1-Deluxe", "2-Deluxe-Nutella-Deluxe" },
    
        // Historia 3
        { "3-1-Common", "3-Common-ChocolateChips" },
        { "3-2-Common", "3-Common-CinnamonRoll" },
        { "3-3-Common", "3-Common-KeyLimePie" },
        { "3-1-Strange", "3-Strange-LaDeMilo" },
        { "3-2-Strange", "3-Strange-MoltenLava" },
        { "3-1-Deluxe", "3-Deluxe-HCampfire" }
    };

    private void Awake()
    {
        // Verificar referencias críticas
        if (collectionContent == null)
            Debug.LogError("Error: CollectionContent no está asignado en el Inspector");

        if (storyCollectionPrefab == null)
            Debug.LogError("Error: StoryCollectionPrefab no está asignado en el Inspector");

        if (backButton == null)
            Debug.LogError("Error: BackButton no está asignado en el Inspector");

        if (emptyCollectionMessage == null)
            Debug.LogError("Error: EmptyCollectionMessage no está asignado en el Inspector");
    }

    private void Start()
    {
        // Inicializar DataManager
        DataManager.Initialize();

        // Configurar botón de retroceso (con verificación de nulo)
        if (backButton != null)
            backButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));
        else
            Debug.LogWarning("BackButton no asignado, no se puede configurar la navegación de retorno");

        // Cargar todas las colecciones
        LoadCollections();
    }

    private void LoadCollections()
    {
        // Verificar componentes críticos
        if (collectionContent == null || storyCollectionPrefab == null)
        {
            Debug.LogError("No se pueden cargar colecciones: collectionContent o storyCollectionPrefab son nulos");
            return;
        }
        Debug.Log($"Usando prefab: {storyCollectionPrefab.name} de tipo: {storyCollectionPrefab.GetType()}");

        // Limpiar contenido actual
        foreach (Transform child in collectionContent)
        {
            Destroy(child.gameObject);
        }

        bool anyCardsFound = false;

        // Cargar cada historia/colecciуn
        foreach (var story in storyNames)
        {
            GameObject storyObj = Instantiate(storyCollectionPrefab, collectionContent);
            StoryCollection storyCollection = storyObj.GetComponent<StoryCollection>();

            if (storyCollection != null)
            {
                // Configurar la colecciуn
                storyCollection.Initialize(story.Key, story.Value, cardToPrefabMap);

                // Verificar si hay cartas en esta colecciуn
                if (storyCollection.HasAnyCards())
                {
                    anyCardsFound = true;
                }
            }
            else
            {
                Debug.LogError($"El prefab {storyCollectionPrefab.name} no tiene componente StoryCollection");
            }
        }

        // Mostrar u ocultar mensaje de colecciуn vacнa
        if (emptyCollectionMessage != null)
            emptyCollectionMessage.SetActive(!anyCardsFound);
    }

    // Mйtodo para actualizar todas las colecciones (llamado desde otras escenas)
    public void RefreshCollections()
    {
        LoadCollections();
    }
}