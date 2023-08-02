using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class BattleDialogBox : BasePanel
{
    [SerializeField] BattleSystem _bs;
    [Header("战斗文本")]
    [SerializeField] Text dialogText;
    [Header("选择面板")]
    [SerializeField] CanvasGroup choosePanel;
    [Header("Mega和极巨化画布组 及 按键变换图")]
    [SerializeField] CanvasGroup mega;
    [SerializeField] CanvasGroup dynamax;
    [SerializeField] Image megaImage;
    [SerializeField] Sprite[] megaSprites;
    [Header("技能面板")]
    [SerializeField] CanvasGroup skillPanel;
    [Header("选择提示面板")]
    [SerializeField] BoolSelectionTip selectionTips;
    [Header("技能信息Slots相关")]
    [SerializeField] BattleSkillUISlot[] skillSlots;
    [SerializeField] Sprite[] skillSlotTypeSprites;
    [SerializeField] Sprite[] effectivenessSprites;
#region 文本显示
    /// <summary>
    /// 战斗文本显示框
    /// </summary>
    public async UniTask TypeDialog(string dialog, bool delay = true)
    {
        dialogText.text = null;
        char[] letters = dialog.ToCharArray();
        foreach(char letter in letters)
        {
            dialogText.text += letter;
            await UniTask.Delay(20);
        }
        await UniTask.Delay(delay? 1000 : 200);//结束等待1s
    }
#endregion
#region 更新技能信息
    /// <summary>
    /// 更新技能栏
    /// </summary>
    /// <param name="target"></param>
    /// <param name="skill"></param>
    public void SetSkillData(List<Skill> skill, PokemonType type1, PokemonType type2)//技能名加载//还要做判断敌方属性克制
    {
        int skillCount = skill.Count;
        for(int i = 0; i < 4; ++i)
        {
            if(i < skillCount)
            {
                float k = TypeChart.GetEffectiveness(skill[i].Base.Type, type1, type2);
                skillSlots[i].SetData
                (
                    skill[i].Base.SkillName,
                    skill[i].GetPPValueString(),
                    ref skill[i].Base.Category == SkillCategory.Status? ref skillSlotTypeSprites[18] : ref skillSlotTypeSprites[(int) skill[i].Base.Type],
                    k == 1f? effectivenessSprites[3] : k > 1f? effectivenessSprites[0] : k == 0f? effectivenessSprites[2] : effectivenessSprites[1]
                );
            }
            else
            {
                skillSlots[i].OnClose();
            }
        }
    }

    /// <summary>
    /// 使用技能后更新信息
    /// </summary>
    /// <param name="i"></param>
    /// <param name="skill"></param>
    public void RefreshSkillData(int i, Skill skill, PokemonType type1, PokemonType type2)//更新技能信息
    {
        //ppText[i].color = skill.GetPercentColor();
        skillSlots[i].Refresh(skill.GetPPValueString());
    }
#endregion
#region 接口
    /// <summary>
    /// 打开战斗选择栏(战斗 背包 道具 逃跑)
    /// </summary>
    public void OpenChoosePanel()
    {
        ShowOrHide(choosePanel, true);
    }

    /// <summary>
    /// 关闭战斗选择栏
    /// </summary>
    public void CloseChoosePanel()
    {
        ShowOrHide(choosePanel, false);
    }

    /// <summary>
    /// 关闭所有按键
    /// </summary>
    public void CloseButton()
    {
        //关掉战斗和技能按钮
        ShowOrHide(choosePanel, false);
        ShowOrHide(skillPanel, false);
    }

    /// <summary>
    /// 战斗按键
    /// </summary>
    public void AttackButton()
    {
        ShowOrHide(choosePanel, false);
        ShowOrHide(skillPanel, true);
    }

    /// <summary>
    /// 从技能栏返回战斗选择栏
    /// </summary>
    public void BackBattleSelection()
    {
        ShowOrHide(choosePanel, true);
        ShowOrHide(skillPanel, false);
    }

    public void Mega()
    {
        mega.blocksRaycasts = false;
        megaImage.sprite = megaSprites[1];
    }

    public void Dynamax()
    {
        dynamax.blocksRaycasts = false;
    }

    public void EndBattle()
    {
        dialogText.text = null;
        megaImage.sprite = megaSprites[0];
        mega.blocksRaycasts = true;
        dynamax.blocksRaycasts = true;
    }

    private string f = "是否继续战斗?";
    private string s = "确定替换这只宝可梦吗?";
    /// <summary>
    /// 宝可梦替换选择完成
    /// </summary>
    public void SwitchChooseFinish()//(SelectionDelegate _delegate)
    {
        //selectionTips.SetData(s, _delegate);
        selectionTips.SetData(s, _bs.ConfirmSwitch);
    }

    /// <summary>
    /// 继续战斗
    /// </summary>
    public void ContinueBattle(System.Action<bool> _action)
    {
        selectionTips.SetData(f, _action);
    }
#endregion
}