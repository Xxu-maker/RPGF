using System.Collections.Generic;
using UnityEngine;
public class UIManager : SingletonMono<UIManager>
{
    [SerializeField] CanvasGroup canvas;//自由状态UI界面
    [SerializeField] GameObject[] obj;
    [Header("通过CanvasGroup管理")]
    [SerializeField] CanvasGroup ctrlCanvas;//控制类
    [SerializeField] CanvasGroup touchCanvas;//交互类
    [SerializeField] CanvasGroup boxCanvas;//无交互 对话框 maptip
    [Header("通过脚本管理")]
    [SerializeField] SelectMenu select;//select按键目录
    [SerializeField] TeamCirclePanel teamCirclePanel;//右边圆形面板
    [SerializeField] PCPanel pcPanel;//电脑箱子
    [SerializeField] ShopPanel shopPanel;//商店
    [SerializeField] FosterPanel fosterPanel;
    [SerializeField] MessageTip messageTip;
    [SerializeField] SelectionColumn selectionColumn;
    [Header("Select")]
    [SerializeField] MapTip mapTip;
    [SerializeField] List<CanvasGroup> cannotBeUsedInDialogue;
    [SerializeField] ItemHandler itemHandler;
#region public load private
    public TeamCirclePanel TeamCirclePanel => teamCirclePanel;
    public ShopPanel ShopPanel => shopPanel;
    public PCPanel PCPanel => pcPanel;
    public MessageTip MessageTip => messageTip;
    public SelectionColumn SelectionColumn => selectionColumn;
    public MapTip MapTip => mapTip;
    public ItemHandler ItemHandler => itemHandler;
#endregion
    public void Start()
    {
        if(MyData.change)
        {
            obj[0].transform.position = MyData.ctrl;
            obj[1].transform.position = MyData.aButton;
            MyData.change = false;
        }
    }
#region 常用接口控制
    /// <summary>
    /// 战斗结束面板显示
    /// </summary>
    public void ExitBattle()
    {
        OnOpen();
        UpdateCirclePanel();
    }

    /// <summary>
    /// 对话时需要隐藏的
    /// </summary>
    public void DialogCover()
    {
        foreach(CanvasGroup c in cannotBeUsedInDialogue)
        {
            c.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// 对话完成恢复的
    /// </summary>
    public void DialogResume()
    {
        foreach(CanvasGroup c in cannotBeUsedInDialogue)
        {
            c.blocksRaycasts = true;
        }
    }

    /// <summary>
    /// 更新右侧圆圈UI
    /// </summary>
    public void UpdateCirclePanel()
    {
        teamCirclePanel.UpdateData();
    }

    /// <summary>
    /// 恢复正常控制类UI
    /// </summary>
    public void ResumeControl()
    {
        ShowOrHide(ctrlCanvas, true);
        teamCirclePanel.Resume();
        select.OnOpen();
    }

    /// <summary>
    /// 关掉正常控制类UI
    /// </summary>
    public void CloseControl()
    {
        ShowOrHide(ctrlCanvas, false);
        teamCirclePanel.Hide();
        select.OnClose();
    }

    /// <summary>
    /// 返回正常控制
    /// </summary>
    public void BackCtrlPanel()
    {
        ShowOrHide(touchCanvas, true);
        ChangePlayerCtrlState(true);
        select.CloseList();
    }

    /// <summary>
    /// 改变切换控制状态 true正常移动 false关闭十字键和A等
    /// </summary>
    /// <param name="open"></param>
    public void ChangePlayerCtrlState(bool open)
    {
        ShowOrHide(ctrlCanvas, open);
        ShowOrHide(boxCanvas, open);
        if(!open)
        {
            teamCirclePanel.Hide();
        }
        else
        {
            teamCirclePanel.Resume();
        }
    }
#endregion
#region 其它接口
    public void OpenBag() { select.OpenSelectPanel(1); }

    /// <summary>
    /// 关闭电脑箱子
    /// </summary>
    public void ExitPCBox()
    {
        ShowOrHide(boxCanvas, true);
        teamCirclePanel.Resume();
        ResumeControl();
    }

    /// <summary>
    /// 打开牧场Panel
    /// </summary>
    public void OpenFosterPanel()
    {
        CloseControl();
        fosterPanel.SetData();
    }
#endregion
#region BasePanel
    public void OnOpen()//打开
    {
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        canvas.interactable = true;
    }

    public void OnClose()//退出
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
    }

    /// <summary>
    /// CanvasGroup开关
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="open"></param>
    public void ShowOrHide(CanvasGroup canvas, bool open)
    {
        canvas.alpha = open? 1 : 0;
        canvas.interactable = open;
        canvas.blocksRaycasts = open;
    }
#endregion
}