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
        { "2-1-Deluxe", "2-Deluxe-Nutella" },
    
        // Historia 3
        { "3-1-Common", "3-Common-ChocolateChips" },
        { "3-2-Common", "3-Common-Cinnamon" },
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
        Debug.Log("CollectionManager: Iniciando carga de datos");

        // Inicializar DataManager
        DataManager.Initialize();

        // NUEVO: Intento de diagnóstico y recuperación específicamente para Android
        if (Application.platform == RuntimePlatform.Android)
        {
            if (DataManager.GetAllCards().Count == 0)
            {
                Debug.Log("CollectionManager: No se encontraron cartas, intentando recuperación...");

                // Intento 1: Cargar desde el respaldo estándar
                bool backupLoaded = DataManager.LoadBackupIfNeeded();

                // Intento 2: Diagnóstico y recuperación personalizada
                if (!backupLoaded || DataManager.GetAllCards().Count == 0)
                {
                    Debug.Log("CollectionManager: Intento 1 falló, ejecutando diagnóstico avanzado...");
                    DataManager.DiagnoseAndRecover();
                }

                // Intento 3: Usar AndroidDataPersistence directamente
                if (DataManager.GetAllCards().Count == 0)
                {
                    Debug.Log("CollectionManager: Intento 2 falló, intentando carga directa desde AndroidDataPersistence...");
                    List<Card> cards = AndroidDataPersistence.GetCards();

                    if (cards != null && cards.Count > 0)
                    {
                        Debug.Log($"CollectionManager: Recuperadas {cards.Count} cartas de AndroidDataPersistence");

                        // Forzar la reintegración de estos datos
                        DataManager.AddCardsFromPack(cards);
                    }
                }
            }
        }
        else
        {
            // Comportamiento estándar para Editor
            if (DataManager.GetAllCards().Count == 0)
            {
                Debug.Log("CollectionManager: No se encontraron cartas, intentando cargar respaldo...");
                bool backupLoaded = DataManager.LoadBackupIfNeeded();
                if (backupLoaded)
                {
                    Debug.Log("CollectionManager: Datos cargados desde respaldo en PlayerPrefs");
                }
            }
        }

        Debug.Log($"CollectionManager: Carga completada. Total de cartas: {DataManager.GetAllCards().Count}");

        // Configurar botón de retroceso
        backButton.onClick.AddListener(() => SceneManager.LoadScene("Scenes/MainScene"));

        // Cargar colecciones
        LoadCollections();
    }


    // Agregar este método a CollectionManager.cs
    private void CreateTestCards()
    {
        // Verificar si ya existe la carpeta Resources/Cards
        if (!System.IO.Directory.Exists(Application.dataPath + "/Resources"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
        }

        if (!System.IO.Directory.Exists(Application.dataPath + "/Resources/Cards"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/Cards");
        }

        // Crear un prefab de prueba básico para cada tipo de carta mencionado en el mapeo
        foreach (var cardMapping in cardToPrefabMap)
        {
            string prefabName = cardMapping.Value;

            // Verificar si ya existe este prefab
            GameObject existingPrefab = Resources.Load<GameObject>("Cards/" + prefabName);
            if (existingPrefab != null)
            {
                continue; // Ya existe, pasamos al siguiente
            }

            // Crear un GameObject básico para representar la carta
            GameObject cardObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cardObj.name = prefabName;

            // Ajustar tamaño para que parezca una carta
            cardObj.transform.localScale = new Vector3(0.7f, 0.1f, 1f);

            // Asignar un material con color según el tipo de carta
            Renderer renderer = cardObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = new Material(Shader.Find("Standard"));

                // Determinar color basado en el nombre del prefab
                if (prefabName.Contains("Common"))
                {
                    material.color = new Color(0.2f, 0.8f, 0.2f); // Verde para Common
                }
                else if (prefabName.Contains("Strange"))
                {
                    material.color = new Color(0.2f, 0.2f, 0.8f); // Azul para Strange
                }
                else if (prefabName.Contains("Deluxe"))
                {
                    material.color = new Color(0.8f, 0.8f, 0.2f); // Amarillo para Deluxe
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", new Color(0.8f, 0.8f, 0.2f, 0.5f) * 0.5f);
                }

                renderer.material = material;
            }

            // Agregar texto con el nombre de la carta
            GameObject textObj = new GameObject("CardName");
            textObj.transform.SetParent(cardObj.transform);
            textObj.transform.localPosition = new Vector3(0, 0.05f, 0);
            textObj.transform.localRotation = Quaternion.Euler(90, 0, 0);

            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = prefabName;
            textMesh.fontSize = 10;
            textMesh.characterSize = 0.03f;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;

            // Guardar este GameObject como prefab en Resources/Cards/
#if UNITY_EDITOR
            if (!System.IO.Directory.Exists("Assets/Resources/Cards"))
            {
                System.IO.Directory.CreateDirectory("Assets/Resources/Cards");
            }

            // Crear el prefab
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(cardObj, "Assets/Resources/Cards/" + prefabName + ".prefab");

            // Destruir el GameObject temporal
            DestroyImmediate(cardObj);
#endif
        }
    }

    private void ConfigureCollectionLayout()
    {
        if (collectionContent == null) return;

        // Asegurar que tiene VerticalLayoutGroup
        VerticalLayoutGroup vlg = collectionContent.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
        {
            vlg = collectionContent.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        // Configurar el layout
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;

        // Agregar Content Size Fitter si no existe
        ContentSizeFitter fitter = collectionContent.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = collectionContent.gameObject.AddComponent<ContentSizeFitter>();
        }
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        Debug.Log("Layout del contenedor de colecciones configurado");
    }

    private void VerifyCardResources()
    {
        // Verificar que los prefabs de cartas existen en Resources
        foreach (var cardMapping in cardToPrefabMap)
        {
            GameObject cardPrefab = Resources.Load<GameObject>("Cards/" + cardMapping.Value);
            if (cardPrefab == null)
            {
                Debug.LogWarning($"Prefab no encontrado: Cards/{cardMapping.Value}");
            }
        }
    }


    // Método LoadCollections mejorado para mostrar mejor información de depuración
    private void LoadCollections()
    {
        // Verificar componentes críticos
        if (collectionContent == null || storyCollectionPrefab == null)
        {
            Debug.LogError("No se pueden cargar colecciones: collectionContent o storyCollectionPrefab son nulos");
            return;
        }

        Debug.Log($"CollectionManager.LoadCollections: Usando prefab: {storyCollectionPrefab.name}");

        // Limpiar contenido actual
        foreach (Transform child in collectionContent)
        {
            Destroy(child.gameObject);
        }

        // Verificar los datos de forma explícita
        List<Card> allCards = DataManager.GetAllCards();
        Debug.Log($"LoadCollections: Total de cartas en colección: {allCards.Count}");

        if (allCards.Count > 0)
        {
            // Listar algunas cartas para depuración
            for (int i = 0; i < Mathf.Min(allCards.Count, 5); i++)
            {
                Card card = allCards[i];
                Debug.Log($"Carta {i + 1}: ID={card.id}, Nombre={card.name}, StoryID={card.storyId}, Tipo={card.type}");
            }
        }

        bool anyCardsFound = false;

        // Cargar cada historia/colección
        foreach (var story in storyNames)
        {
            Debug.Log($"LoadCollections: Procesando historia {story.Key} - {story.Value}");

            GameObject storyObj = Instantiate(storyCollectionPrefab, collectionContent);
            StoryCollection storyCollection = storyObj.GetComponent<StoryCollection>();

            if (storyCollection != null)
            {
                // Configurar la colección
                storyCollection.Initialize(story.Key, story.Value, cardToPrefabMap);

                // Verificar si hay cartas en esta colección
                if (storyCollection.HasAnyCards())
                {
                    anyCardsFound = true;
                    Debug.Log($"Colección {story.Key} tiene cartas");
                }
                else
                {
                    Debug.Log($"Colección {story.Key} está vacía");
                }
            }
            else
            {
                Debug.LogError($"El prefab {storyCollectionPrefab.name} no tiene componente StoryCollection");
            }
        }

        // Mostrar u ocultar mensaje de colección vacía
        if (emptyCollectionMessage != null)
        {
            emptyCollectionMessage.SetActive(!anyCardsFound);
            Debug.Log($"Mensaje de colección vacía: {!anyCardsFound}");
        }
    }


    // Mйtodo para actualizar todas las colecciones (llamado desde otras escenas)
    public void RefreshCollections()
    {
        LoadCollections();
    }

    // AÑADIR ESTE MÉTODO AL FINAL DEL COLLECTIONMANAGER
    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            // Refrescar al volver
            Debug.Log("CollectionManager: Aplicación reanudada, refrescando colecciones");
            LoadCollections();
        }
    }
}