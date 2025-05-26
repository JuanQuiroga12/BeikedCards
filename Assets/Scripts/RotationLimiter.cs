using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RotationLimiter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetTransform;

    [Header("Rotation Limits")]
    [SerializeField] private float minXRotation = -30f;
    [SerializeField] private float maxXRotation = 30f;

    [Header("Speed Settings")]
    [SerializeField] private float rotationSensitivity = 100.0f;
    [SerializeField] private float returnSpeed = 3.0f;
    [SerializeField] private float quickFlipThreshold = 800f;

    [Header("Default Settings")]
    [SerializeField] private Vector3 defaultRotation = new Vector3(90, 180, 0);
    [SerializeField] private bool returnToDefaultOnRelease = true;

    // Internal tracking variables
    private bool isDragging = false;
    private Vector2 previousPosition;
    private float dragTime = 0f;
    private Vector2 dragVelocity;
    private Vector3 targetRotation;
    private bool isFlipped = false;
    private bool isMobileDevice;

    private void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = transform.parent;
        }

        // Detectar si estamos en dispositivo móvil
        isMobileDevice = Touchscreen.current != null;
        Debug.Log($"Dispositivo móvil detectado: {isMobileDevice}");

        // Initialize rotation to default
        if (targetTransform != null)
        {
            // Si estamos en CardDetailView, la tarjeta ya tendrá la rotación correcta
            if (targetTransform.localRotation != Quaternion.identity)
            {
                defaultRotation = targetTransform.localRotation.eulerAngles;
            }

            targetRotation = defaultRotation;
            Debug.Log($"Rotación inicial: {defaultRotation}");
        }
    }

    public void SetTargetTransform(Transform newTarget)
    {
        targetTransform = newTarget;

        // Reset rotation when changing target
        if (targetTransform != null)
        {
            // Preservar la rotación actual como predeterminada
            defaultRotation = targetTransform.localRotation.eulerAngles;
            targetRotation = defaultRotation;
        }
    }

    private void Update()
    {
        if (targetTransform == null) return;

        // Usar el handler adecuado según el dispositivo
        if (isMobileDevice)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }

        // Apply rotation with smoothing
        ApplyRotation();
    }

    private void HandleTouchInput()
    {
        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen == null || !touchscreen.primaryTouch.isInProgress) return;

        Vector2 touchPosition = touchscreen.primaryTouch.position.ReadValue();
        bool touchPressed = touchscreen.primaryTouch.press.isPressed;
        bool touchBegan = touchscreen.primaryTouch.press.wasPressedThisFrame;
        bool touchEnded = touchscreen.primaryTouch.press.wasReleasedThisFrame;

        // Comenzar arrastre
        if (touchBegan)
        {
            isDragging = true;
            previousPosition = touchPosition;
            dragTime = Time.time;
            dragVelocity = Vector2.zero;
            Debug.Log("Toque iniciado");
        }
        // Finalizar arrastre
        else if (touchEnded)
        {
            // Calcular velocidad para detectar gesto rápido
            Vector2 currentVelocity = CalculateDragVelocity(touchPosition);

            // Detectar gesto de volteo
            if (Mathf.Abs(currentVelocity.x) > quickFlipThreshold)
            {
                PerformQuickFlip(currentVelocity.x > 0);
            }

            isDragging = false;
            Debug.Log("Toque finalizado");
        }

        // Procesar arrastre
        if (isDragging && touchPressed)
        {
            // SOLO aplicar rotación horizontal
            float deltaX = touchPosition.x - previousPosition.x;

            // Mantener rotación vertical fija
            float baseYRotation = isFlipped ? 0 : 180;

            // SOLO modificar Y, manteniendo X y Z fijos en sus valores originales
            targetRotation = new Vector3(
                defaultRotation.x,
                targetRotation.y - (deltaX * rotationSensitivity * 0.1f),
                defaultRotation.z
            );

            // Actualizar para próximo frame
            dragVelocity = CalculateDragVelocity(touchPosition);
            previousPosition = touchPosition;
            dragTime = Time.time;
        }
        else if (!isDragging && returnToDefaultOnRelease)
        {
            // Volver a posición según estado de flip
            Vector3 returnTarget = isFlipped ?
                new Vector3(defaultRotation.x, 0, defaultRotation.z) :
                defaultRotation;

            targetRotation = Vector3.Lerp(targetRotation, returnTarget, returnSpeed * Time.deltaTime);
        }
    }

    private void HandleMouseInput()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mousePosition = mouse.position.ReadValue();

        // Start dragging
        if (mouse.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            previousPosition = mousePosition;
            dragTime = Time.time;
            dragVelocity = Vector2.zero;
        }
        // End dragging
        else if (mouse.leftButton.wasReleasedThisFrame)
        {
            Vector2 currentVelocity = CalculateDragVelocity(mousePosition);

            if (Mathf.Abs(currentVelocity.x) > quickFlipThreshold)
            {
                PerformQuickFlip(currentVelocity.x > 0);
            }

            isDragging = false;
        }

        // Process dragging
        if (isDragging && mouse.leftButton.isPressed)
        {
            // Calcular posición relativa (para PC funciona bien así)
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 mouseOffset = new Vector2(
                (screenCenter.x - mousePosition.x) / screenCenter.x,
                (screenCenter.y - mousePosition.y) / screenCenter.y
            );

            // Calcular rotación base
            float baseYRotation = isFlipped ? 0 : 180;

            // Efecto look-at-mouse
            targetRotation = new Vector3(
                defaultRotation.x + Mathf.Clamp(-mouseOffset.y * 30f, minXRotation, maxXRotation),
                baseYRotation + mouseOffset.x * 40f,
                defaultRotation.z
            );

            // Actualizar tracking
            dragVelocity = CalculateDragVelocity(mousePosition);
            previousPosition = mousePosition;
            dragTime = Time.time;
        }
        else if (!isDragging && returnToDefaultOnRelease)
        {
            Vector3 returnTarget = isFlipped ?
                new Vector3(defaultRotation.x, 0, defaultRotation.z) :
                defaultRotation;

            targetRotation = Vector3.Lerp(targetRotation, returnTarget, returnSpeed * Time.deltaTime);
        }
    }

    private void ApplyRotation()
    {
        // Smoothly apply target rotation
        targetTransform.localRotation = Quaternion.Slerp(
            targetTransform.localRotation,
            Quaternion.Euler(targetRotation),
            10f * Time.deltaTime
        );
    }

    private Vector2 CalculateDragVelocity(Vector2 currentPosition)
    {
        float deltaTime = Mathf.Max(Time.time - dragTime, 0.001f);
        return new Vector2(
            (currentPosition.x - previousPosition.x) / deltaTime,
            (currentPosition.y - previousPosition.y) / deltaTime
        );
    }

    private void PerformQuickFlip(bool flipRight)
    {
        isFlipped = !isFlipped;

        // Mantener componentes X y Z al voltear
        float targetY = isFlipped ? 0 : 180;
        targetRotation = new Vector3(defaultRotation.x, targetY, defaultRotation.z);

        Debug.Log($"Carta volteada: {(isFlipped ? "Reverso" : "Frente")}");
    }
}