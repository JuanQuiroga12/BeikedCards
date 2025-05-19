using UnityEngine;

public class CardCameraController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (targetTransform == null) return;

        // Calcular posición ideal
        Vector3 targetPosition = targetTransform.position + new Vector3(0, heightOffset, -distance);

        // Mover la cámara suavemente hacia esa posición
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Mantener la cámara mirando hacia el objetivo
        transform.LookAt(targetTransform);
    }

    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
    }
}