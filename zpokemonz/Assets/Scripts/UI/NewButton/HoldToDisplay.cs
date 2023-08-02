using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class HoldToDisplay : BasePanel, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float duration = 0.5f;
    private bool wasPress = false;
    private bool wasDisplay = false;
    private float pressTime;
    private event Action<bool> holdAction;

    //void Start()
    //{
    //    SetAction((bool value) => {print(value? "Hold" : "Exit");});
    //}

    public void SetAction(Action<bool> action)
    {
        holdAction = action;
    }

    void Update()
    {
        if(!wasDisplay && wasPress)
        {
            if(Time.time - pressTime > duration)
            {
                holdAction?.Invoke(wasDisplay = true);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        wasPress = true;
        pressTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        wasPress = false;
        holdAction?.Invoke(wasDisplay = false);
    }

    public override void OnOpen()
    {
        gameObject.SetActive(true);
    }
    public override void OnClose()
    {
        gameObject.SetActive(false);
    }
}