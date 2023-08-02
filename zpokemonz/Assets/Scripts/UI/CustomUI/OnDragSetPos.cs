using UnityEngine;
using UnityEngine.EventSystems;
public class OnDragSetPos : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] CanvasGroup canvas;
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvas.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvas.blocksRaycasts = true;
    }
}