using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦特性/创建回合结束处理特性")]
public class TurnOverAbility : AbilityBase
{
    [SerializeField] Ability_TurnOverAbilityType turnOverAbilityType;

    [Header("每回合结束改变增益")]
    [Tooltip("勾选为随机增减一项属性")]
    [SerializeField] bool random;
    [SerializeField] StatBoost[] statBoosts;
    /// <summary>
    /// 回合结束增减益
    /// </summary>
    public bool StatBoostPerTurn(Pokemon pokemon)
    {
        if(turnOverAbilityType == Ability_TurnOverAbilityType.每回合结束改变增益)
        {
            if(random)//心情不定随机
            {
                int n = Random.Range(0, 5);
                StatBoost statBoosts1 = new StatBoost((Stat)n, 1);//随机第一个

                int m = Random.Range(0, 5);
                //如果与第一个相等
                while(n == m)
                {
                    m = Random.Range(1, 5);
                }

                StatBoost statBoosts2 = new StatBoost((Stat)m, -1);

                pokemon.ApplyBoosts(new StatBoost[]{statBoosts1, statBoosts2});
                return true;
            }

            //正常变化增益
            pokemon.ApplyBoosts(statBoosts);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 概率治愈异常状态
    /// </summary>
    public string ChanceToHealCondition(ref Pokemon pokemon)
    {
        if(turnOverAbilityType == Ability_TurnOverAbilityType.几率治愈异常状态)
        {
            if(Random.Range(0, 100) <= 33)//1/3
            {
                pokemon.CureAllStatus();
                return $"因为{base.Name}异常状态恢复了!";
            }
        }
        return null;
    }
}
public enum Ability_TurnOverAbilityType
{
    每回合结束改变增益, 几率治愈异常状态,
}