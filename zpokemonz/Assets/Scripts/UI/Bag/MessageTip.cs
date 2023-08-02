using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 信息提示面板,只需确认退出
/// </summary>
public class MessageTip : BasePanel
{
    [SerializeField] Text mesText;
    public void Tip(string str)
    {
        mesText.text = str;

        OnOpen();
    }
}