using UnityEngine;

public class CardRotator : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public float hoverAmount = 0.1f;

    private Vector3 initialPosition;
    private bool isAnimating = true;

    private void Start()
    {
        initialPosition = transform.localPosition;

        // Iniciar animación sutil
        StartHoverAnimation();
    }

    private void Update()
    {
        if (isAnimating)
        {
            // Rotar suavemente
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    private void StartHoverAnimation()
    {
        // Usar secuencia de animación sutil con LeanTween si está disponible
        if (typeof(LeanTween) != null)
        {
            LeanTween.moveLocalY(gameObject, initialPosition.y + hoverAmount, 1.0f)
                .setLoopPingPong()
                .setEaseInOutSine();
        }
    }

    // Método que puede ser llamado cuando el usuario hace hover sobre la carta
    public void OnHoverEnter()
    {
        rotationSpeed = 40f; // Aumentar velocidad de rotación
    }

    // Método que puede ser llamado cuando el usuario sale del hover
    public void OnHoverExit()
    {
        rotationSpeed = 20f; // Velocidad normal
    }
}