using UnityEngine;
public class AbilityBase : ScriptableObject
{
    [Header("基础信息")]
    [SerializeField] int id;
    [SerializeField] string abilityName;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Ability_TriggerType triggerType;

    public int ID => id;
    /// <summary>
    /// 特性名
    /// </summary>
    public string Name => abilityName;
    public string Description => description;
    public Ability_TriggerType TriggerType => triggerType;
}
public enum Ability_TriggerType
{
    //自己， 对手， 攻击，   防御，    接触， 出场，       回合结束，  系统，
    MySelf, Enemy, Attack, Defence, Touch, Appearances, TurnOver, System,
    //队友，    己方，敌方，   全员，     非己,  属性变化
    Teammates, Our, Enemies, AllPlayer, NotMe, DataChange,
    //不用//天气
    Weather, Ground,//固定加成
}
public enum Ability_RoleChangeType{位置, 状态, 数值属性, 枚举属性, HP, 道具, 触发限制, 其它}
public enum Ability_PosChangeType{场上,上场,击败对手,替换下场,被击败,队伍里,逃跑}
public enum Ability_BuffType{正常,异常,中毒,麻痹,灼伤,冰冻,睡眠,畏缩,混乱,着迷,挑衅,被吸收}
public enum Ability_BuffChangeType{获得,免疫,清除,缩短效果时间,同步给对手}
public enum Ability_SexType{无性别,雄性,雌性,同性,异性}
public enum Ability_AbilityChangeType{对方特性,队友特性,正负电特性}
public enum Ability_HPChangeType{减少,恢复,满HP,一半,四分之一,锁血}
public enum Ability_AboutItemType{使用道具,获得道具,丢失道具,夺取道具,替换道具,使用树果}
public enum Ability_OtherRoleChangeType{偶然触发,必然触发,钢属性逃跑,休息一会,防御比特防高,防御比特防低,免疫对手特性,同步最后伤害}
public enum Ability_HurtType{一般,绝佳,无效,不好}
public enum Ability_CriticalType{击中,暴击,未中}
public enum Ability_HurtEffectsType{生效, 触发提升, 必定触发, 不触发}
public enum Ability_SkillClassType{未分类,必杀,爆炸,接触,声音,反作用,拳,连续,特殊,物理,波导,粉尘,低威力}
public enum Ability_SpecialSkillType{降低能力,威力变强,攻击以外的伤害,最后使出,易击中要害}
public enum Ability_TerrainType{无场地,电气,精神,薄雾,青草}
public enum Ability_OtherType{复制对手特性, 偶遇概率提升,提示对手招式}