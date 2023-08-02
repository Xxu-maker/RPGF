using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦特性/创建全员处理的特性")]
public class AllPlayerAbility : AbilityBase
{
    [SerializeField] Ability_AllPlayerAbilityType allPlayerAbilityType;

    [Header("禁止某个技能")]
    [SerializeField] int[] sid;

    /// <summary>
    /// 检查是否能释放技能
    /// </summary>
    public bool CanToRunSkill(Skill skill)
    {
        if(allPlayerAbilityType == Ability_AllPlayerAbilityType.禁技能)
        {
            int skillID = skill.Base.Sid;
            foreach(int i in sid)
            {
                if(skillID == i)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 天气是否有效
    /// </summary>
    public bool IsWeatherValid()
    {
        return allPlayerAbilityType != Ability_AllPlayerAbilityType.禁天气;
    }
}
public enum Ability_AllPlayerAbilityType{ 禁技能, 禁天气, }