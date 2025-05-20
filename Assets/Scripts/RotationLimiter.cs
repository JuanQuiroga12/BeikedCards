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
    [SerializeField] private float rotationSensitivity = 1.0f;
    [SerializeField] private float returnSpeed = 3.0f;
    [SerializeField] private float quickFlipThreshold = 800f; // Velocity threshold for quick flips

    [Header("Default Settings")]
    [SerializeField] private Vector3 defaultRotation = new Vector3(0, 180, 0);
    [SerializeField] private bool returnToDefaultOnRelease = true;

    // Internal tracking variables
    private bool isDragging = false;
    private Vector2 previousMousePosition;
    private float dragTime = 0f;
    private Vector2 dragVelocity;
    private Vector3 targetRotation;
    private bool isFlipped = false;

    private void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = transform.parent;
        }

        // Initialize rotation to default
        if (targetTransform != null)
        {
            targetRotation = defaultRotation;
        }
    }

    public void SetTargetTransform(Transform newTarget)
    {
        targetTransform = newTarget;

        // Reset rotation when changing target
        if (targetTransform != null)
        {
            targetTransform.localRotation = Quaternion.Euler(defaultRotation);
            targetRotation = defaultRotation;
        }
    }

    private void Update()
    {
        if (targetTransform == null) return;

        // Handle input
        HandleInput();

        // Apply rotation with smoothing
        ApplyRotation();
    }

    private void HandleInput()
    {
        // Get current mouse state
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // Get current touch state (for mobile)
        Touchscreen touchscreen = Touchscreen.current;
        bool isTouching = touchscreen != null && touchscreen.primaryTouch.press.isPressed;

        // Current pointer position (mouse or touch)
        Vector2 pointerPosition = mouse.position.ReadValue();
        if (isTouching && touchscreen != null)
        {
            pointerPosition = touchscreen.primaryTouch.position.ReadValue();
        }

        // Start dragging
        if (mouse.leftButton.wasPressedThisFrame || (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame))
        {
            isDragging = true;
            previousMousePosition = pointerPosition;
            dragTime = Time.time;
            dragVelocity = Vector2.zero;
        }
        // End dragging
        else if (mouse.leftButton.wasReleasedThisFrame || (isTouching && touchscreen != null && touchscreen.primaryTouch.press.wasReleasedThisFrame))
        {
            // Calculate final velocity
            Vector2 currentVelocity = CalculateDragVelocity(pointerPosition);

            // Check for quick flip
            if (Mathf.Abs(currentVelocity.x) > quickFlipThreshold)
            {
                PerformQuickFlip(currentVelocity.x > 0);
            }

            isDragging = false;
        }

        // Process dragging
        if (isDragging)
        {
            // Calculate relative position to screen center
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 mouseOffset = new Vector2(
                (screenCenter.x - pointerPosition.x) / screenCenter.x,
                (screenCenter.y - pointerPosition.y) / screenCenter.y
            );

            // Calculate base rotation
            float baseYRotation = isFlipped ? 0 : 180;

            // Create look-at-mouse effect with limits
            targetRotation = new Vector3(
                Mathf.Clamp(-mouseOffset.y * 30f, minXRotation, maxXRotation),
                baseYRotation + mouseOffset.x * 40f,
                0
            );

            // Update drag velocity
            dragVelocity = CalculateDragVelocity(pointerPosition);
            previousMousePosition = pointerPosition;
            dragTime = Time.time;
        }
        else if (returnToDefaultOnRelease)
        {
            // Calculate the default rotation based on flip state
            Vector3 returnTarget = isFlipped ?
                new Vector3(0, 0, 0) :
                defaultRotation;

            // Smoothly return to default
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
        // Calculate time delta (avoid division by zero)
        float deltaTime = Mathf.Max(Time.time - dragTime, 0.001f);

        // Calculate velocity
        return new Vector2(
            (currentPosition.x - previousMousePosition.x) / deltaTime,
            (currentPosition.y - previousMousePosition.y) / deltaTime
        );
    }

    private void PerformQuickFlip(bool flipRight)
    {
        // Toggle flip state
        isFlipped = !isFlipped;

        // Set target rotation based on flip direction
        float targetY = isFlipped ? 0 : 180;
        targetRotation = new Vector3(0, targetY, 0);
    }
}