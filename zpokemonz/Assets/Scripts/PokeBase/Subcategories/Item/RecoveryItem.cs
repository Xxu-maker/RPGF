using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新恢复道具")]
public class RecoveryItem : ItemBase
{
    [Header("回血")]
    [Tooltip("HP或PP恢复值")]
    [SerializeField] int restoreValue;
    [Tooltip("HP PP 恢复满")]
    [SerializeField] bool restoreMax;
    [Tooltip("全恢复 或 四技能PP恢复")]
    [SerializeField] bool restoreAll;

    public override bool Use(Pokemon pokemon)
    {
        if(pokemon.isHPFull || pokemon.isFainted)
        {
            return false;
        }
        return true;
    }

    public override string UseForPokemon(Pokemon pokemon)
    {
        if(restoreAll)//恢复血量及状态
        {
            AudioManager.Instance.HealPokemon();
            pokemon.AllCureItem();
            return pokemon.NickName + "的HP和状态都恢复了!";
        }
        else if(restoreMax)//恢复满血
        {
            pokemon.UpdateHP(pokemon.MaxHP, UpdateHpVoiceType.Cure);
            return pokemon.NickName + "的HP恢复了!";
        }
        else//恢复值
        {
            int lack = pokemon.HP;
            pokemon.UpdateHP(restoreValue, UpdateHpVoiceType.Cure);
            return $"{pokemon.NickName}恢复了{(pokemon.HP - lack).ToString()}点HP!";
        }
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