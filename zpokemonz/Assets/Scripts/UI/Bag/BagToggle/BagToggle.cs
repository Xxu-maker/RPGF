using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
namespace ZUI.BagToggle
{
    /// <summary>
    /// 背包切换动画
    /// </summary>
    public class BagToggle : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] CanvasGroup bottom;
        [SerializeField] CanvasGroup top;
        [SerializeField] float shakeStrength = 10f;
        [SerializeField] UnityEvent group;
        [SerializeField] UnityEvent<InventoryType> changeBag;
        [SerializeField] InventoryType bagType;
        [SerializeField] bool isOn;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnOpen();
        }

        public void OnOpen()
        {
            if(!isOn)
            {
                isOn = true;

                gameObject.transform.DOShakePosition(0.2f, shakeStrength);

                bottom.DOFade(0f, 0.1f);
                bottom.blocksRaycasts = false;

                top.DOFade(1f, 0.1f);
                top.blocksRaycasts = true;

                group.Invoke();

                changeBag.Invoke(bagType);

                bottom.alpha = 0f;
                bottom.blocksRaycasts = false;
                top.alpha = 1f;
                top.blocksRaycasts = true;
            }
        }

        public void OnClose()
        {
            if(isOn)
            {
                isOn = false;

                gameObject.transform.DOShakePosition(0.2f, shakeStrength);

                bottom.DOFade(1f, 0.1f);
                bottom.blocksRaycasts = true;

                top.DOFade(0f, 0.1f);
                top.blocksRaycasts = false;
            }
        }
    }
}