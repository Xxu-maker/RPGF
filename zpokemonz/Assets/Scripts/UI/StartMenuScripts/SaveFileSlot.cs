using UnityEngine;
using UnityEngine.UI;
public class SaveFileSlot : BasePanel
{
    [SerializeField] CanvasGroup messageCG;//存档信息
    [SerializeField] CanvasGroup createCG;//创建图标
    [SerializeField] Text nameText;//存档名
    [SerializeField] Text timeText;//存档时间
    [SerializeField] Toggle toggle;
    public bool IsOn => toggle.isOn;
    private bool exist;
    public bool Exist => exist;
    public void SetData(GameMessage gameMessage, bool exist)
    {
        this.exist = exist;
        OnOpen();
        if(exist)
        {
            messageCG.alpha = 1;
            createCG.alpha = 0;
            nameText.text = gameMessage.name;
            timeText.text = gameMessage.time;
        }
        else
        {
            messageCG.alpha = 0;
            createCG.alpha = 1;
        }
    }
    public string GetSaveFileName => nameText.text;
}