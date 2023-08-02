using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦特性/创建防御相关特性")]
/// <summary>
/// 防守时相关(伤害、状态)
/// </summary>
public class DefenceAbility : AbilityBase
{
    [SerializeField] Ability_DefenceType defenceType;

    [Header("免疫的状态")]
    [SerializeField] ConditionID[] immunityConditionID;

    [Header("攻击无效化的属性")]
    [SerializeField] PokemonType[] typesUsedToCheckDefence;
    [Header("用于防止单项能力被降低")]
    [SerializeField] Stat stat;

    /// <summary>
    /// 检查免疫
    /// </summary>
    public string CheckConditionProtect(ConditionID conditionID)
    {
        if(defenceType == Ability_DefenceType.免疫状态)
        {
            foreach(ConditionID condition in immunityConditionID)
            {
                if(conditionID == condition)
                {
                    return $"因为{base.Name}不会进入{condition.ToString()}状态!";
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 暴击保护检查
    /// </summary>
    /// <returns>true为不能被暴击</returns>
    public bool CheckCritical()
    {
        return defenceType == Ability_DefenceType.不被暴击;
    }

    /// <summary>
    /// 一击必杀检查
    /// </summary>
    public string CheckSpike()
    {
        if(defenceType == Ability_DefenceType.满HP时不被秒并无视一击必杀)
        {
            return $"因为{base.Name}, 没有被击倒!";
        }
        return null;
    }

    /// <summary>
    /// 攻击是否有效
    /// </summary>
    public bool IsAttackEffective(Pokemon pokemon, ref Skill skill, out string message)
    {
        if(defenceType == Ability_DefenceType.受特定属性攻击回血)
        {
            foreach(PokemonType type in typesUsedToCheckDefence)
            {
                if(skill.Base.Type == type)
                {
                    pokemon.UpdateHP((int)(pokemon.MaxHP * 0.25f), UpdateHpVoiceType.Cure);
                    message = $"{pokemon.NickName}因为{base.Name}回复了生命值!";
                    return false;
                }
            }
        }
        else if(defenceType == Ability_DefenceType.受特定属性攻击提升技能威力)
        {
            //技能提升50%威力
        }
        else if(defenceType == Ability_DefenceType.受特定属性攻击无效)
        {
            foreach(PokemonType type in typesUsedToCheckDefence)
            {
                if(skill.Base.Type == type)
                {
                    message = $"{skill.Base.Type}属性攻击对{base.Name}没有效果...";
                    return false;
                }
            }
        }
        else if(defenceType == Ability_DefenceType.受特定属性攻击伤害减半)
        {
            //
        }

        message = null;
        return true;
    }

    /// <summary>
    /// 追加是否有效
    /// </summary>
    /// <returns>false为不能追加</returns>
    public bool IsAddOnValid()
    {
        return defenceType != Ability_DefenceType.被追加保护;
    }

    /// <summary>
    /// 检查是否为效果绝佳招式才能命中
    /// </summary>
    /// <returns>true为效果绝佳招式才能命中</returns>
    public bool CheckIfOnlyTheMostEffectiveSkillCanBeHit()
    {
        return defenceType == Ability_DefenceType.只有效果绝佳的招式才能击中;
    }

    /// <summary>
    /// 能力降低防御检查
    /// </summary>
    public string CheckBoostsDownDefence(ref StatBoost[] statBoosts)
    {
        if(defenceType == Ability_DefenceType.能力不被降低)
        {
            return $"因为{base.Name}能力没有被降低!";
        }
        else if(defenceType == Ability_DefenceType.单项能力不被降低)
        {
            foreach(StatBoost statBoost in statBoosts)
            {
                if(statBoost.Boost < 0 && statBoost.Stat == stat)
                {
                    return $"因为{base.Name}能力没有被降低!";
                }
            }
        }
        return null;
    }
}
public enum Ability_DefenceType
{
    免疫状态, 不被暴击, 满HP时不被秒并无视一击必杀, 受特定属性攻击回血,  受特定属性攻击提升技能威力,
    被追加保护, 只有效果绝佳的招式才能击中, 受特定属性攻击无效, 能力不被降低, 受特定属性攻击伤害减半,
    单项能力不被降低,
}