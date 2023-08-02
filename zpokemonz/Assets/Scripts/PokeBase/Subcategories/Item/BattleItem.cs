using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新战斗道具")]
public class BattleItem : ItemBase
{
    [Header("战斗道具")]
    [SerializeField] BattleItemType battleItemType;//战斗道具类型
    [SerializeField] PokemonType skillType;//技能属性
    [SerializeField] SkillCategory skillCategory;//技能攻击类型(物理/特殊/变化)
    [SerializeField] StatBoost statBoost;

    /// <summary>
    /// 技能威力提升检查
    /// </summary>
    public int SkillPowerUpCheck(Skill skill)
    {
        if(battleItemType == BattleItemType.SkillType && skillType == skill.Base.Type)
        {
            //技能属性加成
            return (int)(skill.Base.Power * 1.2f);
        }
        else if(battleItemType == BattleItemType.AttackType && skillCategory == skill.Base.Category)
        {
            if(skillCategory == SkillCategory.Status)//变化不加成
            {
                return 0;
            }
            //技能攻击类型加成
            return (int)(skill.Base.Power * 1.1f);
        }
        return skill.Base.Power;
    }
}
public enum BattleItemType
{
    SkillType, AttackType, Cure, Protect, Weather, Train, Money
}