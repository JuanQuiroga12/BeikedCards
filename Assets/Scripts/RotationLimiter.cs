using UnityEngine;

public class RotationLimiter : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float minXRotation = -20f;
    [SerializeField] private float maxXRotation = 20f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float returnToDefaultSpeed = 2f;

    private bool isDragging = false;
    private Vector3 previousMousePosition;
    private float targetXRotation = 0f;
    private float currentXRotation = 0f;

    private void Start()
    {
        if (targetTransform == null)
        {
            // If no transform is assigned, use the parent of the component
            targetTransform = transform.parent;
        }
    }

    // Add this public method to set the target transform
    public void SetTargetTransform(Transform newTarget)
    {
        targetTransform = newTarget;
    }

    private void Update()
    {
        // Iniciar/detener arrastre
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            previousMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Manejar rotación por arrastre
        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - previousMousePosition;

            // Rotación horizontal (Y) - sin restricciones
            targetTransform.Rotate(Vector3.up, -delta.x * rotationSpeed * Time.deltaTime);

            // Rotación vertical (X) - con restricciones
            targetXRotation += delta.y * rotationSpeed * Time.deltaTime;
            targetXRotation = Mathf.Clamp(targetXRotation, minXRotation, maxXRotation);

            previousMousePosition = Input.mousePosition;
        }
        else
        {
            // Suavemente volver a la rotación X por defecto (0) cuando no se arrastra
            targetXRotation = Mathf.Lerp(targetXRotation, 0, returnToDefaultSpeed * Time.deltaTime);
        }

        // Aplicar rotación X con suavizado
        currentXRotation = Mathf.Lerp(currentXRotation, targetXRotation, 10 * Time.deltaTime);

        // Aplicar rotación X directamente
        Vector3 currentRotation = targetTransform.localEulerAngles;
        if (currentXRotation > 180f) currentXRotation -= 360f;
        targetTransform.localEulerAngles = new Vector3(currentXRotation, currentRotation.y, currentRotation.z);
    }
}