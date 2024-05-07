using UnityEngine.EventSystems;

public interface IDragAndDrop
{
    void OnDrag(PointerEventData eventData);
    void OnPointerUp(PointerEventData eventData);
}