using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventManager : EventTrigger
{
    public static UIEventManager Get(GameObject obj)
    {
        UIEventManager instance = obj.GetComponent<UIEventManager>();
        if (instance == null)
        {
            instance = obj.AddComponent<UIEventManager>();
        }
        return instance;
    }

    public UnityAction<PointerEventData> OnClickCallBack;
    public UnityAction<PointerEventData> OnPointerEnterCallBack;
    public UnityAction<PointerEventData> OnPointerExitCallBack;
    public UnityAction<PointerEventData> OnPointerDownCallBack;
    public UnityAction<PointerEventData> OnPointerUpCallBack;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (OnClickCallBack != null)
        {
            OnClickCallBack(eventData);
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (OnPointerEnterCallBack != null)
        {
            OnPointerEnterCallBack(eventData);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if(OnPointerExitCallBack != null)
        {
            OnPointerExitCallBack(eventData);
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (OnPointerDownCallBack != null)
        {
            OnPointerDownCallBack(eventData);
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (OnPointerUpCallBack != null)
        {
            OnPointerUpCallBack(eventData);
        }
    }
}
