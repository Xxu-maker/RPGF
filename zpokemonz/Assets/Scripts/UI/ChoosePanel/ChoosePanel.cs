using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 对带在身上的宝可梦进行选择
/// </summary>
public class ChoosePanel : BasePanel
{
    [SerializeField] CanvasGroup exitPanelCG;
    [SerializeField] List<PokeChBox> pokeBox;
    [SerializeField] BattleDialogBox battleDialog;

    [Header("选择技能面板")]
    [SerializeField] ChooseSkillPanel chooseSkillPanel;
    public ChooseSkillPanel ChooseSkillPanel => chooseSkillPanel;

    private int choosePos;
    public int ChoosePos => choosePos;
    bool isSwitch;
    bool isFainted;
    bool inTheBattle;

    /// <summary>
    /// 打开宝可梦选择面板
    /// </summary>
    public void SetData(Pokemon[] pokemons, ItemBase item)
    {
        inTheBattle = GameManager.Instance.BattleState;
        ItemType itemType = item.Type;
        switch(itemType)
        {
            //球
            case ItemType.Ball:

                UIManager.Instance.ItemHandler.ThrowBallInTheBattle();

            return;

            //恢复//状态
            case ItemType.Medicine: case ItemType.Condition:

                for(int i = 0; i < 6; ++i)
                {
                    if(pokemons[i].Base != null)
                    {
                        pokeBox[i].SetData(pokemons[i], item.Use(pokemons[i]));
                    }
                    else
                    {
                        pokeBox[i].OnClose();
                    }
                }

            break;

            //复活
            case ItemType.Revive:

                for(int i = 0; i < 6; ++i)
                {
                    if(pokemons[i].Base != null)
                    {
                        pokeBox[i].SetData(pokemons[i], pokemons[i].isFainted);
                    }
                    else
                    {
                        pokeBox[i].OnClose();
                    }
                }

            break;

            //其它
            case ItemType.AddPP: case ItemType.Mint: case ItemType.PPMaximum:

                for(int i = 0; i < 6; ++i)
                {
                    if(pokemons[i].Base != null)
                    {
                        pokeBox[i].SetData(pokemons[i]);
                    }
                    else
                    {
                        pokeBox[i].OnClose();
                    }
                }

            break;

            //努力值
            case ItemType.BasePoint:

                BasePointItem bpItem = item as BasePointItem;
                for(int i = 0; i < 6; ++i)
                {
                    if(pokemons[i].Base != null)
                    {
                        pokeBox[i].SetBasePointData(pokemons[i], bpItem.Use(pokemons[i]), bpItem.BasePointType);
                    }
                }

            break;

            //技能机
            case ItemType.TM:

                for(int i = 0; i < 6; ++i)
                {
                    if(pokemons[i].Base != null)
                    {
                        pokeBox[i].SetData(pokemons[i], item.Use(pokemons[i]));
                    }
                    else
                    {
                        pokeBox[i].OnClose();
                    }
                }

            break;

            case ItemType.Evolution:
                //
            break;
        }

        OnOpen();
        ShowOrHide(exitPanelCG, true);
    }


    /// <summary>
    /// 替换用的面板 传入宝可梦, 是否为濒死替换, 当前场上宝可梦
    /// </summary>
    public void SetDataForSwitch(Pokemon[] pokemons, bool isFaintedSwitch, int coverIndex)
    {
        isSwitch = true;
        inTheBattle = true;
        isFainted = isFaintedSwitch;
        OnOpen();
        for(int i = 0; i < 6; ++i)
        {
            if(pokemons[i].Base != null)
            {
                pokeBox[i].SetData(pokemons[i], pokemons[i].isActive);
            }
            else
            {
                pokeBox[i].OnClose();
            }
        }

        ShowOrHide(exitPanelCG, !isFaintedSwitch);//如果是濒死替换，不开关闭
        pokeBox[coverIndex].OnCover();//替换时的当前场上的宝可梦 关闭交换
    }


    /// <summary>
    /// 携带道具面板
    /// </summary>
    public void CarryOnItemPanel(Pokemon[] pokemons)
    {
        OnOpen();
        ShowOrHide(exitPanelCG, true);
        for(int i = 0; i < 6; ++i)
        {
            if(pokemons[i].Base != null)
            {
                pokeBox[i].CarryOnItem(pokemons[i]);
            }
            else
            {
                pokeBox[i].OnClose();
            }
        }
    }

    /// <summary>
    /// 按键位置使用道具
    /// </summary>
    public void GetNum(int n)
    {
        choosePos = n;
        if(isSwitch)
        {
            battleDialog.SwitchChooseFinish();
            if(!isFainted)
            {
                ShowOrHide(exitPanelCG, false);
            }
            base.OnClose();
            isSwitch = false;
            return;
        }
        UIManager.Instance.ItemHandler.ChooseValue(n);
    }

    /// <summary>
    /// 刷新选择格子
    /// </summary>
    /// <param name="i"></param>
    public void RefreshBox(int i, bool basePoint = false)
    {
        pokeBox[i].Refresh(basePoint);
    }

    /// <summary>
    /// 退出本面板
    /// </summary>
    public void ExitPanel()
    {
        OnClose();
        if(inTheBattle)
        {
            battleDialog.BackBattleSelection();
        }
    }

    public void OnCoverPBox()
    {
        pokeBox[choosePos].OnCover();
    }

    public void PauseTouch()
    {
        Canvas.blocksRaycasts = false;
        ShowOrHide(exitPanelCG, false);
    }

    public override void OnClose()
    {
        base.OnClose();
        ShowOrHide(exitPanelCG, false);
    }

    public void Resume()
    {
        Canvas.blocksRaycasts = true;
        ShowOrHide(exitPanelCG, true);
    }

    private void OnCover(CanvasGroup canvas)
    {
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
    }
}