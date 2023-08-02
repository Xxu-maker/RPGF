using System;
using UnityEngine;
using UnityEngine.UI;
public class PCPanel : BasePanel
{
    [Header("宝可梦详细页面板脚本")]
    [SerializeField] PokeDesPanel pokeDesPanel;//宝可梦详细面板

    [Header("右侧按键面板")]
    [SerializeField] CanvasGroup multiSelectionModeButtonPanelCG;//多选模式按键面板
    [SerializeField] CanvasGroup normalModeButtonPanelCG;//正常模式按键面板
    [SerializeField] CanvasGroup exitPCButtonCG;//PC页面退出按钮的CanvasGroup

    [Header("切换箱子")]
    [SerializeField] CanvasGroup switchBoxTogglesPanelCG;//切换箱子的Toggle面板
    [SerializeField] Text[] boxNameTextArray;//箱子名字

    [Header("盒子")]
    [SerializeField] BoxBase[] pcBoxes;
    [SerializeField] BoxBase[] bagBoxes;
    [SerializeField] PCOnDrag[] onDragSlots;

    private PokemonTeam _pokemonTeam;
    private Action[] pcSelectionPanel = null;
    private string[] pcSelectionBtnText = new string[] { "打开电脑", "取消" };
#region Start
    private void Start()
    {
        foreach(PCOnDrag slot in onDragSlots)
        {
            slot.SetDelegate(DesTipPanel ,SwapPokemon);
        }

        _pokemonTeam = GameManager.Instance.PlayerTeam;

        if(pcSelectionPanel == null)
        {
            pcSelectionPanel = new Action[]
            {
                Open,
                UIManager.Instance.ResumeControl
            };
        }
    }
#endregion
    /// <summary>
    /// 弹出电脑提示框
    /// </summary>
    public void PCSelection()
    {
        AudioManager.Instance.OpenComputer();

        UIManager.Instance.CloseControl();

        UIManager.Instance.SelectionColumn.Set(pcSelectionPanel, pcSelectionBtnText, SelectionColumnPosType.TopRightCorner);
    }

    public void SetBoxName(string[] name)
    {
        int nameLength = name.Length;
        for(int i = 0; i < nameLength; ++i)
        {
            boxNameTextArray[i].text = name[i];
        }
    }

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="pokemons"></param>
    /// <param name="isBox">是否为箱子</param>
    public void SetData(Pokemon[] pokemons, bool isBox)
    {
        BoxBase[] boxes = isBox? pcBoxes : bagBoxes;
        int boxesLength = boxes.Length;
        for(int i = 0; i < boxesLength; ++i)
        {
            boxes[i].SetData(pokemons[i]);
        }
    }

    /// <summary>
    /// 刷新电脑里当前Box和Bag的宝可梦
    /// </summary>
    public void RefreshBoxPokemon()
    {
        SetData(_pokemonTeam.Pokemons, false);
        SetData(_pokemonTeam.GetCurrentBox(), true);
    }
#region 多选模式按键绑定
    public void MultiSelectionMode()//开启多选模式(按键)
    {
        _OpenMultiSelectionPanel(true);
        ShowOrHide(switchBoxTogglesPanelCG, false);
        foreach(BoxBase box in pcBoxes)
        {
            box.ReadySelectMode();
        }
    }

    public void ExitMultiSelectionMode()//退出多选模式(按键)
    {
        _OpenMultiSelectionPanel(false);
        ShowOrHide(switchBoxTogglesPanelCG, true);
        foreach(BoxBase box in pcBoxes)
        {
            box.OffSelectMode();
        }
    }

    private void _OpenMultiSelectionPanel(bool open)
    {
        ShowOrHide(normalModeButtonPanelCG, !open);
        ShowOrHide(multiSelectionModeButtonPanelCG, open);
        ShowOrHide(exitPCButtonCG, !open);
    }

    /// <summary>
    /// 全选//(按键)
    /// </summary>
    public void AllSelect()
    {
        foreach(BoxBase box in pcBoxes)
        {
            box.OnToggle();
        }
    }

    /// <summary>
    /// 取消全选//(按键)
    /// </summary>
    public void CancelAllSelect()
    {
        foreach(BoxBase box in pcBoxes)
        {
            box.CancelModeToggle();
        }
    }

    /// <summary>
    /// 放生已选精灵//(按键)
    /// </summary>
    public void GiveUpPokemon()
    {
        foreach(BoxBase box in pcBoxes)
        {
            if(box.CheckToggle())
            {
                _pokemonTeam.Free(box.ID());
            }
        }
        RefreshBoxPokemon();
    }

    public void MoveToBag()//移入背包//(按键)
    {
        foreach(BoxBase box in pcBoxes)
        {
            if(box.CheckToggle())
            {
                int x = _pokemonTeam.FindVacancyByNumber(0);;
                box.OffToggle();
                if(x != 61)
                {
                    x += 60;
                    SwapPokemon(box.ID(), x);
                }
            }
        }
        RefreshBoxPokemon();
    }
#endregion
#region 交换
    /// <summary>
    /// PCBox和PlayerBag全部范围交换
    /// </summary>
    public void SwapPokemon(int id, int sid)
    {
        //id sid pc的标号范围0 - 59, bag的标号范围60 - 65
        bool idIsBox = id < 60;
        bool sidIsBox = sid < 60;
        int boxIndex = _pokemonTeam.CurrentBox;
        if(idIsBox)//ID是Box
        {
            if(sidIsBox)//PCToPC
            {
                _pokemonTeam.SwapPokemonAndRefresh(boxIndex, id, boxIndex, sid, pcBoxes[id].SetData, pcBoxes[sid].SetData);
            }
            else//PCToBag
            {
                //检查背包是否剩最后一个,只剩一个就不能换
                if(!_pokemonTeam.CanSwap(id)) { return; }

                Pokemon a, b;
                sid -= 60;
                _pokemonTeam.SwapPokemonAndOut(boxIndex, id, 0, sid, out a, out b);
                pcBoxes[id].SetData(a);
                if(_pokemonTeam.SortTeam())//如果队伍排过序, 直接刷新BagBox
                {
                    SetData(_pokemonTeam.Pokemons, false);
                }
                else//无排序定点刷新
                {
                    bagBoxes[sid].SetData(b);
                }
            }
        }
        else//ID不是Box
        {
            //检查背包是否剩最后一个,只剩一个就不能换
            if(!_pokemonTeam.CanSwap(sid)) { return; }

            Pokemon a, b;
            id -= 60;
            if(sidIsBox)//BagToPC
            {
                _pokemonTeam.SwapPokemonAndOut(0, id, boxIndex, sid, out a, out b);
                pcBoxes[sid].SetData(b);
                if(_pokemonTeam.SortTeam())//如果队伍排过序, 直接刷新BagBox
                {
                    SetData(_pokemonTeam.Pokemons, false);
                }
                else
                {
                    bagBoxes[id].SetData(a);
                }
            }
            else//BagToBag
            {
                sid -= 60;
                _pokemonTeam.SwapPokemonAndRefresh(0, id, 0, sid, bagBoxes[id].SetData, bagBoxes[sid].SetData);
            }
        }
    }
#endregion
#region 盒子delegate及selection处理
    private int nowTap;
    /// <summary>
    /// 保存当前点击宝可梦位置， 弹出提示框
    /// </summary>
    public void DesTipPanel(int n)
    {
        if(!_pokemonTeam.HavePokemon(n))
        {
            return;
        }
        UIManager.Instance.SelectionColumn.Set(GetSelectionActions(), selectionBtnText);
        nowTap = n;
    }

    private Action[] selectionActions;
    private string[] selectionBtnText = new string[] { "详细面板", "锁定/解锁" };
    private ref Action[] GetSelectionActions()
    {
        if(selectionActions == null || selectionActions.Length == 0)
        {
            selectionActions = new Action[]
            {
                OpenPokemonDetailPanel,
                LockPokemon,
            };
        }
        return ref selectionActions;
    }

    /// <summary>
    /// 打开详细面板
    /// </summary>
    public void OpenPokemonDetailPanel()
    {
        base.OnClose();
        pokeDesPanel.OpenFromPC();
    }

    /// <summary>
    /// 锁定/解锁宝可梦
    /// </summary>
    public void LockPokemon()
    {
        int i = nowTap;
        if(i < 60)
        {
            _pokemonTeam.GetCurrentBox()[i].LockPokemon(pcBoxes[i].RefreshLockAndSign);
        }
        else
        {
            i -= 60;
            _pokemonTeam.Pokemons[i].LockPokemon(bagBoxes[i].RefreshLockAndSign);
        }
    }
#endregion
#region 按键接口
    /// <summary>
    /// Toggle切换箱子
    /// </summary>
    public void SwitchBox(int i)//因为TeamBox从1开始，i要加一或者加了传
    {
        _pokemonTeam.SetCurrentValue(i);
        SetData(_pokemonTeam.GetCurrentBox(), true);
    }
#endregion
#region 面板显隐
    public void Open()
    {
        AudioManager.Instance.TapPC();
        UIManager.Instance.TeamCirclePanel.Hide();
        base.OnOpen();
        SetBoxName(_pokemonTeam.BoxName);
        RefreshBoxPokemon();
    }

    /// <summary>
    /// 只显示不刷新数据
    /// </summary>
    public void Show()
    {
        base.OnOpen();
    }

    public override void OnClose()
    {
        base.OnClose();
        AudioManager.Instance.CloseComputer();
        UIManager.Instance.ExitPCBox();
    }
#endregion
}