using UnityEngine;
using UnityEngine.UI;
public class SetPanel : BasePanel
{
    [SerializeField] Dropdown dropDown;
    private int oldValue;
    private bool changed;
    public override void OnOpen()
    {
        base.OnOpen();
        oldValue = dropDown.value;
    }
    public void SaveSelect()
    {
        changed = true;
        switch(dropDown.value)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = 120; break;
        }
        ExitPanel();
    }
    public void ExitPanel()
    {
        OnClose();
        if(!changed)
        {
            dropDown.value = oldValue;
        }
        changed = false;
        UIManager.Instance.BackCtrlPanel();
    }
}
public class PlayerSetSaveData//开始界面读取，直接把开始界面置顶，在下层设置
{
    public int fps;
    public Vector3 originCtrlUIPos = new Vector3(380f, 350f, 0);
    public Vector3 originAButtonPos = new Vector3(1995f, -345f, 0);
    public bool change = false;
}