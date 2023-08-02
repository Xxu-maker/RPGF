using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新PP道具")]
public class PPItem : ItemBase
{
    [Header("PP值")]
    [Tooltip("HP或PP恢复值")]
    [SerializeField] int restoreValue;
    [Tooltip("HP PP 恢复满")]
    [SerializeField] bool restoreMax;
    [Tooltip("全恢复 或 四技能PP恢复")]
    [SerializeField] bool restoreAll;

    public override bool Use(Pokemon pokemon)
    {
        foreach(Skill skill in pokemon.Skill)
        {
            if(skill.IsThePPMax())
            {
                return false;
            }
        }
        return true;
    }

    public string AddPP(Pokemon pokemon, int n)
    {
        List<Skill> skills = pokemon.Skill;
        if(restoreMax)
        {
            if(restoreAll)
            {
                foreach(Skill skill in skills)
                {
                    skill.ResetPP();
                }
                return pokemon.NickName + "所有技能的PP都恢复了!";
            }
            else
            {
                pokemon.Skill[n].ResetPP();
                return $"{pokemon.NickName}的{pokemon.Skill[n].Base.SkillName}PP恢复了!";
            }
        }
        else
        {
            if(restoreAll)
            {
                foreach(Skill skill in skills)
                {
                    skill.PPValueChange(restoreValue);
                }
                return $"{pokemon.NickName}所有技能的PP都恢复{restoreValue.ToString()}点!";
            }
            else
            {
                pokemon.Skill[n].PPValueChange(restoreValue);
                return $"{pokemon.NickName}的{pokemon.Skill[n].Base.SkillName}PP恢复了{restoreValue.ToString()}点!";
            }
        }
    }
}
