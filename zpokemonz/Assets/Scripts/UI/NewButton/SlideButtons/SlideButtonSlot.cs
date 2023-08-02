using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace ZUI.SlideButton
{
    public class SlideButtonSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] CanvasGroup canvas;
        [SerializeField] UnityEvent<int> pressed;
        [SerializeField] int selection;
        private bool enter;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!enter)
            {
                enter = true;
                canvas.alpha = 0.5f;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(enter)
            {
                canvas.alpha = 1f;
                enter = false;
                pressed.Invoke(selection);
            }
        }
    }
}