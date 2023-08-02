using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace ZUI.SlideButton
{
    //public enum SlideButtonSelection{ Mega, Dynamax }
    public class SlideButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] CanvasGroup _self;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] List<UnityEvent> evolutionEvent;

        public void OnPointerDown(PointerEventData eventData)
        {
            _self.alpha = 0.5f;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _self.alpha = 1f;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        public void EndOfSelection(int selection)
        {
            //evolutionEvent[(int) selection].Invoke();
            evolutionEvent[selection].Invoke();
        }
    }
}