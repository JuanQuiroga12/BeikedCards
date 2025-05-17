using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private float hoverAmount = 10f;
    [SerializeField] private float rotateSpeed = 20f;
    [SerializeField] private ParticleSystem hoverEffect;
    [SerializeField] private AudioSource hoverSound;

    private Vector3 originalPosition;
    private bool isHovering = false;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (isHovering)
        {
            // Animación suave mientras se hace hover
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
    }

    public void OnPointerEnter()
    {
        isHovering = true;

        // Efecto de elevación
        LeanTween.moveLocalY(gameObject, originalPosition.y + hoverAmount, 0.2f);
        LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.2f);

        // Efectos
        if (hoverEffect != null)
            hoverEffect.Play();

        if (hoverSound != null)
            hoverSound.Play();
    }

    public void OnPointerExit()
    {
        isHovering = false;

        // Volver a posición original
        LeanTween.moveLocalY(gameObject, originalPosition.y, 0.2f);
        LeanTween.scale(gameObject, Vector3.one, 0.2f);

        if (hoverEffect != null)
            hoverEffect.Stop();
    }

    public void OnPointerClick()
    {
        // Mostrar detalles completos de la carta
        Debug.Log("Mostrar detalles de la carta");
        // Implementar lógica para mostrar panel de detalles
    }
}