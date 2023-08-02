using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 通用 "是/否" 选择面板
/// </summary>
public class BoolSelectionTip : BasePanel
{
    [SerializeField] Text questionText;
    private event Action<bool> OnSelectionFinish;

    public void SetData(string question, Action<bool> _action)
    {
        OnSelectionFinish = _action;
        if(questionText != null)
        {
            questionText.text = question;
        }
        OnOpen();
    }

    public void Selection(bool value)
    {
        OnClose();
        OnSelectionFinish.Invoke(value);
    }
}