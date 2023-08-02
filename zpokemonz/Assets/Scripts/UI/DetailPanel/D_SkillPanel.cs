using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 3技能
/// </summary>
public class D_SkillPanel : BasePanel
{
    [SerializeField] List<Text> skillText;
    [SerializeField] List<Text> pptext;
    [SerializeField] List<Text> detailtext;
    public override void SetData(Pokemon pokemon)//技能名加载
    {
        List<Skill> skills = pokemon.Skill;
        OnOpen();
        int skillTextCount = skillText.Count;
        int skillCount = skills.Count;
        for(int i = 0; i < skillTextCount; ++i)
        {
            if(i < skillCount)
            {
                skillText[i].text = skills[i].Base.SkillName;
                pptext[i].text = skills[i].GetPPValueString();
                //颜色
                pptext[i].color = skills[i].GetPercentColor();
            }
            else
            {
                skillText[i].text = "-";
                pptext[i].text = "-";
                pptext[i].color = Color.black;
            }
        }
    }
}
