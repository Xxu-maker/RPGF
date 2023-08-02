using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦特性/创建接触相关特性")]
public class TouchAbility : AbilityBase
{
    [SerializeField] Ability_TouchAbilityType touchAbilityType;
    [Header("反伤/反伤状态")]
    [Range(0, 100)]
    [SerializeField] int percent;
    [Tooltip("是否为临时状态")]
    [SerializeField] bool isVolatileStatus;
    [Tooltip("是否为多种状态随机感染")]
    [SerializeField] bool isRandomCondition;
    [SerializeField] ConditionID[] conditions;
    [Range(0f, 1f)]
    [SerializeField] float thornDamage = 0.125f;//1/8

    public string CheckThorn(ref Pokemon pokemon)
    {
        if(touchAbilityType == Ability_TouchAbilityType.给予对手状态)
        {
            if(isVolatileStatus)
            {
                pokemon.SetVolatileStatus(conditions[isRandomCondition? Random.Range(0, 2) : 0]);
            }
            else
            {
                pokemon.SetStatus(conditions[isRandomCondition? Random.Range(0, 2) : 0]);
            }
            return null;
        }
        else if(touchAbilityType == Ability_TouchAbilityType.反伤)
        {
            pokemon.UpdateHP((int) (pokemon.MaxHP * thornDamage), UpdateHpVoiceType.NormalHurt);
            return $"{pokemon.NickName}因{base.Name}受到反伤!";
        }
        return null;
    }
}
public enum Ability_TouchAbilityType{ 反伤, 给予对手状态, }