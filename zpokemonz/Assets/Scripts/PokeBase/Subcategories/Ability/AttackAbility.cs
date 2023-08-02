using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦特性/创建新攻击特性")]
/// <summary>
/// 进攻时伤害相关
/// </summary>
public class AttackAbility : AbilityBase
{
    [SerializeField] Ability_AttackType attackType;

    [Header("通用概率")]
    [Range(0, 100)]
    [SerializeField] int percent;

    [Header("概率对对手触发异常状态")]
    [SerializeField] bool isVolatileCondition;//是否为不稳定的状态
    [SerializeField] ConditionID conditionID;

    [Header("HP百分比提升相应属性技能威力")]
    [Range(0f, 1f)]
    [SerializeField] float hpPercentUsedToCheck;
    [SerializeField] PokemonType typeUsedToCheckBoost;

    [Header("会心一击特性")]
    [Tooltip("暴击率(4.16f)1/24 (12.5)1/8 (50)1/2 (100)1/1")]
    [SerializeField] float criticalPercent = 4.16f;
    [SerializeField] float damage = 1.5f;

    [Header("永久提升(Always)")]//***********
    [SerializeField] Stat _stat;
    [SerializeField] float addition;
    [SerializeField] StatsChange[] statsChanges;//有两项永久变化，没写

    [Header("天气加成")]
    [SerializeField] WeatherType weatherTypeUsedToCheckStatsBonus;
    //还有属性直接变化，不过boosts
    public float GetAddition(Stat stat, int value)
    {
        return _stat == stat? value * addition : value;
    }
    /// <summary>
    /// 固定加成
    /// </summary>
    public (Stat stat, float multiplier) GetAddition2()
    {
        if(attackType == Ability_AttackType.固定属性加成)
        {
            return (_stat, addition);
        }
        return (_stat, 0);
    }

    /// <summary>
    /// 检查特性追加状态(处理完技能后的追加)
    /// </summary>
    public bool CheckAbilityAddOnCondition(ref Pokemon pokemon)
    {
        if(attackType == Ability_AttackType.概率对对手触发异常状态)
        {
            if(Random.Range(0, 100) < percent)
            {
                //不稳定异常状态
                if(isVolatileCondition)
                {
                    pokemon.SetVolatileStatus(conditionID);
                }
                else//正常异常状态
                {
                    pokemon.SetStatus(conditionID);
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 检查是否会心
    /// </summary>
    /// <returns>返回会心伤害系数</returns>
    public float GetCritical(out bool isActive)
    {
        //4.16f暴击率1/24 (12.5)1/8 (50)1/2 (100)1/1
        //触发反馈(暴击率大于正常4.16或暴击伤害大于1.5)
        if(attackType == Ability_AttackType.暴击效果)
        {
            isActive = criticalPercent > 4.16f || damage > 1.5f;
            return UnityEngine.Random.value * 100f <= criticalPercent? damage : 1f;
        }
        isActive = false;
        return 1f;
    }

    /// <summary>
    /// 追加效果命中提升
    /// </summary>
    public bool CheckAddOnPercentUp()
    {
        return attackType == Ability_AttackType.追加命中提升;
    }

    /// <summary>
    /// 技能伤害加成
    /// </summary>
    public int SkillPowerAddition(ref Pokemon pokemon, ref Skill skill, out bool isActive)
    {
        if(attackType == Ability_AttackType.HP低于百分之33相应技能威力提升百分之50 && pokemon.HPPercent <= hpPercentUsedToCheck && skill.Base.Type == typeUsedToCheckBoost)
        {
            isActive = true;
            return (int)(skill.Base.Power * 1.5f);
        }
        isActive = false;
        return skill.Base.Power;
    }
}
public enum Ability_AttackValueType
{
    攻, 防, 特攻, 特防, 速度, 命中, 闪避,
    体重, 身高, 随机能力, 全部能力, 招式pp
}
public enum Ability_AttackType
{
    概率对对手触发异常状态, 固定属性加成, HP低于百分之33相应技能威力提升百分之50, 追加命中提升,
    招式属性, 招式类别, 天气加成, 暴击效果,
    威力效果,  追加效果, 概率触发, 任意攻击, 特定效果
}
public enum Ability_SkillAttackType
{
    一般, 火, 水, 草, 电, 冰, 格斗, 毒, 地面, 飞行, 超能力, 虫, 岩石, 幽灵, 龙, 恶, 钢, 妖精, 全属性,
    对手招式属性, 自己招式属性, 对手属性, 自身属性
}
public enum Ability_EnumAttackType{ 性别, 属性, 特性 }
[System.Serializable]
public struct StatsChange
{
    public Stat stat;
    public float addition;
}