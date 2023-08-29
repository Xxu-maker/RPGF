using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonType
{
    CHOICE,
    FUNC,
    PETITION,
    ROUNDEND
}
public static class UIButtonTextChange
{
    static Color32 initColor = new Color32(106, 52, 30, 255);
    static Color32 newColor = new Color32(226, 169, 115, 255);
    static Color32 ChoiceInitColor = new Color32(106, 52, 30, 255);
    static Color32 ChoicenewColor = new Color32(255, 247, 227, 255);
    public static Color32 initPetitionColor = new Color32(146, 45, 4, 255);
    public static Color32 newPetitionColor = new Color32(226, 169, 115, 255);
    static Color32 roundPressColor = new Color32(181, 137, 93, 255);
    static Color32 roundHoverColor = new Color32(218, 166, 113, 255);
    static Color32 roundCommandColor = new Color32(106, 52, 30, 255);



    static Button tempBtn;
    static Toggle tempToggle;
    static bool isClick;


    public static void ColorChange(GameObject btn, ButtonType type)
    {
        Text text = btn.GetComponent<Text>();
        if (type == ButtonType.CHOICE)
        {
            UIEventManager.Get(btn.gameObject).OnPointerEnterCallBack += ChoicePointerOnEnter;
            UIEventManager.Get(btn.gameObject).OnPointerExitCallBack += ChoicePointerOnExit;
            UIEventManager.Get(btn.gameObject).OnPointerDownCallBack += ChoicePointerDown;
            UIEventManager.Get(btn.gameObject).OnPointerUpCallBack += ChoicePointerUp;
        }
        else
        {
            //后续拓展
        }
    }
    static void RoundPointerOnEnter(PointerEventData data)
    {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();

        text.color = roundHoverColor;
    }
    static void RoundPointerOnExit(PointerEventData data)
    {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();
        //Image text = data.pointerEnter.transform.Find("img_text").GetComponent<Image>();
        if (tempBtn == null)
        {
            text.color = roundCommandColor;
        }
        else
        {
            if (data.pointerEnter != tempBtn.gameObject)
            {
                text.color = roundCommandColor;
            }
        }
    }
    static void RoundPointerDown(PointerEventData data)
    {

        if (data.pointerEnter != null)
        {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();
            tempBtn = data.pointerEnter.GetComponent<Button>();
            text.color = roundPressColor;
        }
    }
    static void RoundPointerUp(PointerEventData data)
    {
        if (tempBtn != null)
        {
            Text text = tempBtn.transform.Find("Text").GetComponent<Text>();
            //Image text = data.pointerEnter.transform.Find("img_text").GetComponent<Image>();
            text.color = roundCommandColor;
            tempBtn.OnDeselect(null);
            tempBtn = null;
        }
    }

    static void PointerOnEnter(PointerEventData data)
    {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();

        text.color = newColor;
    }
    static void PointerOnExit(PointerEventData data)
    {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();
        //Image text = data.pointerEnter.transform.Find("img_text").GetComponent<Image>();
        if (tempBtn == null)
        {
            text.color = initColor;
        }
        else
        {
            if (data.pointerEnter != tempBtn.gameObject)
            {
                text.color = initColor;
            }
        }
    }
    static void PointerDown(PointerEventData data)
    {
        if (data.pointerEnter != null)
        {
            tempBtn = data.pointerEnter.GetComponent<Button>();
        }
    }
    static void PointerUp(PointerEventData data)
    {
        if (tempBtn != null)
        {
            Text text = tempBtn.transform.Find("Text").GetComponent<Text>();
            //Image text = data.pointerEnter.transform.Find("img_text").GetComponent<Image>();
            text.color = initColor;
            tempBtn.OnDeselect(null);
            tempBtn = null;
        }
    }
    static void ChoicePointerOnEnter(PointerEventData data)
    {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();
        text.color = ChoicenewColor;
    }
    static void ChoicePointerOnExit(PointerEventData data)
    {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();
        if (tempBtn == null)
        {
            text.color = data.pointerEnter.transform.GetComponent<Button>().IsInteractable() ? ChoiceInitColor : ChoicenewColor;
        }
        else
        {
            if (data.pointerEnter != tempBtn.gameObject)
            {
                text.color = ChoiceInitColor;
            }
        }
    }
    static void ChoicePointerDown(PointerEventData data)
    {
        if (data.pointerEnter != null)
        {
            tempBtn = data.pointerEnter.GetComponent<Button>();
        }
    }
    static void ChoicePointerUp(PointerEventData data)
    {
        if (tempBtn != null)
        {
            Text text = tempBtn.transform.Find("Text").GetComponent<Text>();
            text.color = tempBtn.IsInteractable() ? ChoiceInitColor : ChoicenewColor;
            tempBtn.OnDeselect(null);
            tempBtn = null;
        }
    }

    static void PetitionBtnOnEnter(PointerEventData data)
    {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();
        text.color = newPetitionColor;
    }
    static void PetitionBtnOnExit(PointerEventData data)
    {
        Text text = data.pointerEnter.transform.Find("Text").GetComponent<Text>();
        if (tempToggle == null)
        {
            text.color = initPetitionColor;
        }
        else
        {
            if (data.pointerEnter != tempToggle.gameObject)
            {
                text.color = initPetitionColor;
            }
        }
    }
    static void PetitionBtnDown(PointerEventData data)
    {
        if (data.pointerEnter != null)
        {
            tempToggle = data.pointerEnter.GetComponent<Toggle>();
        }
    }
    static void PetitionBtnUp(PointerEventData data)
    {

        if (tempToggle != null)
        {
            Text text = tempToggle.transform.Find("Text").GetComponent<Text>();
            text.color = initPetitionColor;
            tempToggle.OnDeselect(null);
            tempToggle = null;
        }
    }
}
