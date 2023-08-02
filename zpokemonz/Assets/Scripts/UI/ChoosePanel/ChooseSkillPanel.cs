using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 选择面板之技能选择
/// </summary>
public class ChooseSkillPanel : BasePanel
{
    [SerializeField] ChoosePanel choosePanel;
    [Header("技能面板")]
    [SerializeField] CanvasGroup exitSkillPanel;//退出
    [SerializeField] CanvasGroup[] skillButtonCG;//按键的画布组
    [SerializeField] Text[] nameTexts;
    [SerializeField] Text[] ppTexts;
    [Header("确认替换技能面板")]
    [SerializeField] BoolSelectionTip selectionTips;
    public Action<bool, int> learnSkillAction;
    private int chooseSkillPos;
    bool inTheBattle;
    bool LearnSkill;

    /// <summary>
    /// 技能加PP面板
    /// </summary>
    public void SetSkillData(List<Skill> skills)
    {
        LearnSkill = false;

        OnOpen();

        int count = skills.Count;
        for(int i = 0; i < count; ++i)
        {
            nameTexts[i].text = skills[i].Base.SkillName;
            ppTexts[i].text = skills[i].GetPPValueString();
            if(skills[i].IsThePPMax())
            {
                OnCover(skillButtonCG[i]);
            }
            else
            {
                ShowOrHide(skillButtonCG[i], true);
            }
        }

        while(count < 4)
        {
            OnCover(skillButtonCG[count]);
            nameTexts[count].text = "-";
            ppTexts[count].text = "-";
            ++count;
        }
    }

    /// <summary>
    /// 替换技能面板
    /// </summary>
    public void SetSkillDataForReplace(List<Skill> skills, SkillBase learn, bool battle = false)
    {
        LearnSkill = true;
        inTheBattle = battle;

        OnOpen();

        int count = skills.Count;
        for(int i = 0; i < count; ++i)
        {
            nameTexts[i].text = skills[i].Base.SkillName;
            ppTexts[i].text = skills[i].GetPPValueString();
            ShowOrHide(skillButtonCG[i], true);
        }

        while(count < 4)
        {
            OnCover(skillButtonCG[count]);
            nameTexts[count].text = "-";
            ppTexts[count].text = "-";
            ++count;
        }

        ShowOrHide(skillButtonCG[4], true);
        nameTexts[4].text = learn.SkillName;
        ppTexts[4].text = string.Concat(learn.PP.ToString(), " / ", learn.PP.ToString());
    }

    public void GetSkillNum(int n)
    {
        if(LearnSkill)
        {
            if(inTheBattle)
            {
                learnSkillAction.Invoke(true, n);
            }
            else
            {
                chooseSkillPos = n;
                selectionTips.SetData("确认替换吗？如果已有四个技能，就会忘掉一个技能!", ConfirmLearnSkill);
            }
        }
        else
        {
            if(inTheBattle)
            {
                chooseSkillPos = n;
                OnClose();
                choosePanel.OnClose();
                UIManager.Instance.ItemHandler.ConfirmAddPP(n);
            }
            else
            {
                UIManager.Instance.ItemHandler.ConfirmAddPP(n);
                ExitSkillPanel();
            }
        }
    }

    /// <summary>
    /// 确认学习技能
    /// </summary>
    /// <param name="yes"></param>
    public void ConfirmLearnSkill(bool yes)
    {
        if(yes)
        {
            UIManager.Instance.ItemHandler.ConfirmReplaceSkill(chooseSkillPos);
            OnClose();
            choosePanel.OnCoverPBox();
        }
    }
    /*public void RefreshSkillBox(Skill skill, int i)
    {
        nameTexts[i].text = skill.Base.SkillName;
        ppTexts[i].text = string.Concat(skill.PP.ToString(), " / ", skill.Base.PP.ToString());
        if(skill.IsThePPMax())
        {
            OnCover(skillButtonCG[i]);
        }
        else
        {
            ShowOrHide(skillButtonCG[i], true);
        }
    }*/

    public void ExitSkillPanel()
    {
        OnClose();
        if(inTheBattle)
        {
            learnSkillAction.Invoke(false, 0);
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();
        ShowOrHide(exitSkillPanel, true);
    }

    public override void OnClose()
    {
        base.OnClose();
        ShowOrHide(exitSkillPanel, false);
    }

    private void OnCover(CanvasGroup canvas)
    {
        canvas.blocksRaycasts = false;
        canvas.interactable = false;
    }
}