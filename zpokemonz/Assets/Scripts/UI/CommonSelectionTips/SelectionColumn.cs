using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 公共选择列表
/// </summary>
public class SelectionColumn : BasePanel
{
    [SerializeField] CanvasGroup[] buttonsCG;
    [SerializeField] Text[] buttonsText;
    [SerializeField] Transform[] targetTrans;
    private Action[] actions;

    public void Set(Action[] _actions, string[] buttonTextMessages, SelectionColumnPosType posType = SelectionColumnPosType.Center)
    {
        OnOpen();

        actions = _actions;

        int count = actions.Length;
        for(int i = 0; i < count; ++i)
        {
            ShowOrHide(buttonsCG[i], true);
            buttonsText[i].text = buttonTextMessages[i];
        }

        //关掉多余的
        int buttonsCount = buttonsCG.Length;
        while(count < buttonsCount)
        {
            ShowOrHide(buttonsCG[count], false);
            count++;
        }

        Vector3 targetPos = targetTrans[(int) posType].position;
        if(transform.position != targetPos)
        {
            transform.position = targetPos;
        }
    }

    public void ButtonClicked(int index)
    {
        OnClose();
        actions[index]?.Invoke();
    }
}
public enum SelectionColumnPosType{ Center, TopRightCorner }