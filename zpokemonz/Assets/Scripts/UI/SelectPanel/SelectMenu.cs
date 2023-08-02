using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// select按键目录
/// </summary>
public class SelectMenu : BasePanel
{
    [SerializeField] List<BasePanel> selectPanels;//详细 背包 地图 图鉴 卡片 保存 设置 暂停
    [SerializeField] CanvasGroup listCanvas;
    //select按钮
    public void SelectButton()
    {
        if(listCanvas.alpha == 0)
        {
            ShowOrHide(listCanvas, true);
            GameManager.Instance.PauseGame(true);
            UIManager.Instance.ChangePlayerCtrlState(false);
        }
        else
        {
            ShowOrHide(listCanvas, false);
            GameManager.Instance.PauseGame(false);
            UIManager.Instance.ChangePlayerCtrlState(true);
        }
    }

    public void OpenSelectPanel(int num)
    {
        //AudioManager.instance.TapButton();
        OnClose();
        selectPanels[num].OnOpen();
    }

    public override void OnClose()
    {
        base.OnClose();
        ShowOrHide(listCanvas, false);
    }

    /// <summary>
    /// 关掉select菜单列
    /// </summary>
    public void CloseList()
    {
        base.OnOpen();
        ShowOrHide(listCanvas, false);
        GameManager.Instance.PauseGame(false);
    }
}