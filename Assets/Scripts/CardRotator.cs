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

        // Iniciar animaci�n sutil
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
        // Usar secuencia de animaci�n sutil con LeanTween si est� disponible
        if (typeof(LeanTween) != null)
        {
            LeanTween.moveLocalY(gameObject, initialPosition.y + hoverAmount, 1.0f)
                .setLoopPingPong()
                .setEaseInOutSine();
        }
    }

    // M�todo que puede ser llamado cuando el usuario hace hover sobre la carta
    public void OnHoverEnter()
    {
        rotationSpeed = 40f; // Aumentar velocidad de rotaci�n
    }

    // M�todo que puede ser llamado cuando el usuario sale del hover
    public void OnHoverExit()
    {
        rotationSpeed = 20f; // Velocidad normal
    }
}