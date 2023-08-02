using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦特性/创建对手全员相关特性")]
public class EnemiesAbility : AbilityBase
{
    [SerializeField] Ability_EnemiesAbilityType enemiesAbilityType;
    [Header("禁止逃跑")]
    [Tooltip("勾选为全属性不能逃走")]
    [SerializeField] bool allType;
    [SerializeField] PokemonType typeUsedToCheckEscape;//可以设定单一禁止逃跑属性

    /// <summary>
    /// 检查是否能逃跑
    /// </summary>
    public bool CheckEscape(PokemonType type1, PokemonType type2)
    {
        if(enemiesAbilityType == Ability_EnemiesAbilityType.禁止逃跑)
        {
            return !allType || (type1 != typeUsedToCheckEscape && type2 != typeUsedToCheckEscape);
        }
        return true;
    }
}
public enum Ability_EnemiesAbilityType{ 禁止逃跑, }