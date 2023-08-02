using System;
using UnityEngine;
using ZUI.BagScroll;
public class ItemHandler : MonoBehaviour
{
    [SerializeField] BagScroller bagPanel;
    [SerializeField] ChoosePanel choosePanel;
    private PokemonTeam pokemonTeam;
    private Inventory playerInventory;
    public event Action<ItemBase, string, BattleAction> UseItemInBattleAction;
    public event Action<string> OnShowDescription;
    public event Action<int, bool> OnRefreshCellView;
#region 记录的值
    private int itemCellIndex;
    private ItemBase useItem;
    private int useForPokemonPos;
#endregion
    private void Start()
    {
        pokemonTeam = GameManager.Instance.PlayerTeam;
        playerInventory = GameManager.Instance.Inventory;

        selectionActions = new Action[]
        {
            ConfirmationOfUse,
            null,
            ConfirmationOfCarrying,
            null
        };
    }

    private Action[] selectionActions = null;
    private string[] selectionBtnText = new string[] { "使用", "使用多个", "携带", "返回" };
    private enum OpenAction { Use, Carry, Sale }
    private OpenAction currentState;

    /// <summary>
    /// 打开提示选择面板
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pos"></param>
    public void OpenTipPanel(ItemBase item, int cellIndex)
    {
        //if(sale)
        //详细信息
        itemCellIndex = cellIndex;
        useItem = item;

        OnShowDescription.Invoke(item.Description);

        //选择栏面板
        UIManager.Instance.SelectionColumn.Set(selectionActions, selectionBtnText);
    }

    /// <summary>
    /// SelectionTip -> 确认使用
    /// </summary>
    public void ConfirmationOfUse()
    {
        //检查能否在当前状态使用
        if(GameManager.Instance.BattleState)
        {
            if(!useItem.UseInBattle)  { return; }
        }
        else
        {
            if(!useItem.UseInFreedom) { return; }
        }

        currentState = OpenAction.Use;

        choosePanel.SetData(pokemonTeam.Pokemons, useItem);
    }

    /// <summary>
    /// SelectionTip -> 选择确认携带
    /// </summary>
    public void ConfirmationOfCarrying()
    {
        if(GameManager.Instance.BattleState)
        {
            return;
        }

        currentState = OpenAction.Carry;

        choosePanel.CarryOnItemPanel(pokemonTeam.Pokemons);
    }

    /// <summary>
    /// SelectionTip -> 使用球类道具
    /// </summary>
    public void ThrowBallInTheBattle()
    {
        //currentState = OpenAction.Use;
        playerInventory.UseItem(bagPanel.CurrentBagType, itemCellIndex);
        bagPanel.OnClose();
        UseItemInBattleAction.Invoke(useItem, null, BattleAction.ThrowBall);
    }

    public void ChooseValue(int value)
    {
        if(playerInventory.GetInventory(bagPanel.CurrentBagType)[itemCellIndex].Count == 0)
        {
            UIManager.Instance.MessageTip.Tip("道具用完了。。");
            choosePanel.OnClose();
            return;
        }

        Pokemon pokemon = pokemonTeam.Pokemons[value];
        if(currentState == OpenAction.Carry)
        {
            //携带并储存原来的物品
            playerInventory.LayInItem(pokemon.CarryOnItem(useItem));

            OnRefreshCellView.Invoke(itemCellIndex, playerInventory.UseItem(bagPanel.CurrentBagType, itemCellIndex));
            choosePanel.CarryOnItemPanel(pokemonTeam.Pokemons);
            return;
        }

        ItemType type = useItem.Type;
        switch(type)
        {
            //恢复 状态 复活
            case ItemType.Medicine: case ItemType.Condition: case ItemType.Revive:

                if(GameManager.Instance.BattleState)
                {
                    bagPanel.OnClose();
                    UseItemInBattleAction.Invoke(useItem, useItem.UseForPokemon(pokemon), BattleAction.OtherItem);
                    choosePanel.OnClose();
                }
                else
                {
                    useItem.UseForPokemon(pokemon);
                    choosePanel.RefreshBox(value);//更新选择盒子
                    UIManager.Instance.UpdateCirclePanel();
                }

            break;

            //加PP值
            case ItemType.AddPP:

                useForPokemonPos = value;
                choosePanel.ChooseSkillPanel.SetSkillData(pokemon.Skill);

            return;

            //努力值
            case ItemType.BasePoint:

                choosePanel.RefreshBox(value, true);//更新选择盒子
                UIManager.Instance.MessageTip.Tip(useItem.UseForPokemon(pokemonTeam.Pokemons[value]));

            break;

            //技能
            case ItemType.TM:

                useForPokemonPos = value;
                choosePanel.ChooseSkillPanel.SetSkillDataForReplace(pokemon.Skill, (useItem as TM).SKill);

            return;

            //薄荷
            case ItemType.Mint:

                pokemon.ChangeNature(useItem.ID);
                choosePanel.OnClose();
                UIManager.Instance.MessageTip.Tip(string.Concat(pokemon.NickName, "使用了", useItem.ItemName));

            break;

            case ItemType.Evolution:

                //

            break;
        }
        OnRefreshCellView.Invoke(itemCellIndex, playerInventory.UseItem(bagPanel.CurrentBagType, itemCellIndex));
    }

    /// <summary>
    /// 确认加PP
    /// </summary>
    /// <param name="n"></param>
    public void ConfirmAddPP(int n)
    {
        bool usedUp = playerInventory.UseItem(bagPanel.CurrentBagType, itemCellIndex);

        if(GameManager.Instance.BattleState)
        {
            bagPanel.OnClose();
            UseItemInBattleAction.Invoke
            (
                useItem,
                (useItem as RecoveryItem).AddPP(pokemonTeam.Pokemons[useForPokemonPos], n),
                BattleAction.OtherItem
            );
        }
        else
        {
            UIManager.Instance.MessageTip.Tip((useItem as RecoveryItem).AddPP(pokemonTeam.Pokemons[useForPokemonPos], n));
            OnRefreshCellView.Invoke(itemCellIndex, usedUp);
        }
    }

    /// <summary>
    /// 确认替换技能
    /// </summary>
    /// <param name="n"></param>
    public void ConfirmReplaceSkill(int n)
    {
        OnRefreshCellView.Invoke(itemCellIndex, playerInventory.UseItem(bagPanel.CurrentBagType, itemCellIndex));
        UIManager.Instance.MessageTip.Tip((useItem as TM).LearnSkill(pokemonTeam.Pokemons[useForPokemonPos], n));
    }
}