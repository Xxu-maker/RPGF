using UnityEngine;
[CreateAssetMenu(fileName = "宝可梦技能", menuName = "宝可梦技能/创建新技能")]
public class SkillBase : ScriptableObject
{
    [SerializeField] string skillName;
    [SerializeField] int sid;
    [TextArea][SerializeField] string description;
    [SerializeField] SkillType skillType;//技能分类

    [SerializeField] SkillAnimatorType skillAnimatorType;
    [SerializeField] SkillParticleHandler skillAnim;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] float accuracy;//命中率
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int maxPP;//最大能达到的PP
    [SerializeField] byte priority;
    [SerializeField] SkillTarget target;
    [SerializeField] SkillCategory category;

    [Header("追加(add-on勾为追加, 否则为特殊技能效果)")]
    [Tooltip("附加必须勾选")]
    [SerializeField] bool addOn;
    [Tooltip("追加命中率")]
    [SerializeField] int addOnPercent;
    [SerializeField] SkillEffects effects;

    public string SkillName => skillName;
    public string Description => description;
    public SkillType SkillType => skillType;
    public SkillAnimatorType AnimatorType => skillAnimatorType;
    public SkillParticleHandler ParticleAnim => skillAnim;
    public int Sid => sid;
    public PokemonType Type => type;
    public int Power => power;
    public float Accuracy => accuracy;
    public bool AlwaysHits => alwaysHits;
    public int PP => pp;
    public int MaxPP => maxPP;
    public byte Priority => priority;
    public SkillCategory Category => category;
    public SkillTarget Target => target;

    /// <summary>
    /// 是否有追加效果
    /// </summary>
    public bool AddOn => addOn;
    /// <summary>
    /// 追加是否命中
    /// </summary>
    public bool IsAddOnValid(bool percentUp = false) => Random.Range(0, 100) <= (percentUp? addOnPercent + addOnPercent : addOnPercent);
    /// <summary>
    /// add-on勾为追加, 否则为特殊技能效果
    /// </summary>
    public SkillEffects Effects => effects;
}

[System.Serializable]
public class SkillEffects
{
    [SerializeField] StatBoost[] boosts;//增减益
    [SerializeField] ConditionID status;//状态
    [SerializeField] ConditionID volatileStatus;//临时状态
    [SerializeField] SkillTarget effectTarget;//目标

    public StatBoost[] Boosts => boosts;
    public ConditionID Status => status;
    public ConditionID VolatileStatus => volatileStatus;
    public SkillTarget Target => effectTarget;
}

[System.Serializable]
public struct StatBoost
{
    [SerializeField] Stat stat;
    [SerializeField] int boost;
    public Stat Stat => stat;
    public int Boost => boost;

    public StatBoost(Stat _stat, int _boost)
    {
        stat = _stat;
        boost = _boost;
    }
}

/// <summary>
/// 技能类别
/// </summary>
public enum SkillCategory{ Physical, Special, Status }

public enum SkillTarget{ Foe, Self }

public enum SkillAnimatorType { None, Image, Particle }

public enum SkillType
{
    接触类,一击必杀, 互换类,使用后下一回合自己将无法动弹的招式,先制招式,压击类招式,反作用力伤害,可以在对战外使用,合体招式,
    吸取Hp,和后备宝可梦进行替换,啃咬类,回复Hp,固定伤害,声音,复制类,多回合攻击,容易击中要害,对入场宝可梦生效,
    强制拉对手的后备宝可梦上场的招式,心灵攻击类招式,拳类招式,攻击必定会命中,波动和波导类招式,球和弹类招式,
    目标特性改变的招式,空间招式,粉末类招式,绑紧招式,蓄力的招式,解除冰冻状态的招式,识破类招式,跳舞招式,连续招式,
    追加或改变属性的招式,造成自身陷入濒死状态的招式,防住类招式,Z招式
}