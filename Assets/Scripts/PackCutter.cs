using UnityEngine;
using UnityEngine.EventSystems;

public class PackCutter : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private PackOpeningManager packManager;
    private Vector2 dragStartPos;
    private bool isDragging = false;
    private float minDragDistance = 100f; // Distancia mínima para considerar un corte válido

    public void Initialize(PackOpeningManager manager)
    {
        packManager = manager;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.position;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Opcional: Dibujar línea de corte mientras se arrastra
        if (!isDragging) return;

        // Aquí podrías implementar un efecto visual de línea de corte
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Calcular distancia arrastrada
        float dragDistance = Vector2.Distance(dragStartPos, eventData.position);

        // Verificar si el arrastre fue suficiente para considerar un corte
        if (dragDistance >= minDragDistance)
        {
            // Notificar al PackOpeningManager
            packManager.PackCut();
        }

        isDragging = false;
    }
}