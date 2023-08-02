using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] ItemSlot carryonItem;
    [SerializeField] bool isMega;
    [SerializeField] bool isDynamax;
    [SerializeField] bool isShiny;
    [SerializeField] int[] iv;
    [SerializeField] int[] basePoints;
    [SerializeField] Nature nature;
    [SerializeField] AbilityBase ability;
    [SerializeField] string nickName;
    [SerializeField] bool lockPKM;
    private bool basePointsWasMax;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        if(iv == null)
        {
            Wild();
        }
        Init();
    }
#region Public Load Private
    public string NickName => nickName == null || nickName.Length == 0? _base.Name : nickName;
    public PokemonBase Base => _base;
    public int Level => level;
    public ItemBase ItemBase => carryonItem == null? null : carryonItem.Base;
    public Nature Nature => nature;
    public AbilityBase Ability => ability;
    public bool Mega => isMega;
    public bool Dynamax => isDynamax;
    public bool Shiny => isShiny;
    public int[] IV => iv;
    public int[] BasePoints => basePoints;
    public bool BasePointsWasMax => basePointsWasMax;
    public bool Lock => lockPKM;

    public float HPPercent => (float) HP / MaxHP;
    /// <summary>
    /// HP > 0
    /// </summary>
    public bool isActive => HP > 0;
    /// <summary>
    /// HP == 0
    /// </summary>
    public bool isFainted => HP == 0;
    /// <summary>
    /// HP == MaxHP
    /// </summary>
    public bool isHPFull => HP == MaxHP;
    public int MaxHP { get; private set; }
    public int PAttack => GetStat(Stat.Attack);
    public int PDefence => GetStat(Stat.Defence);
    public int SAttack => GetStat(Stat.SpAttack);
    public int SDefence => GetStat(Stat.SpDefence);
    public int Speed => GetStat(Stat.Speed);
#endregion
#region 无设置数据
    [NonSerialized] public int Exp;
    [NonSerialized] public int HP;
    [NonSerialized] public List<Skill> Skill;
    [NonSerialized] public Skill CurrentSkill;

    //没有临时状态的属性值
    private List<int> Stats;
    //属性提升等级
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    //状态
    public Condition Status { get; private set; }
    //状态持续时间
    [NonSerialized] public int StatusTime;
    //不稳定状态
    public Condition VolatileStatus { get; private set; }
    //不稳定状态持续时间
    [NonSerialized] public int VolatileStatusTime;
    //状态变化文本
    public Queue<string> StatusChange{ get; private set; }
    private Queue<string> damageMessage;

    public event Action OnHPChanged;
    public event Action OnMaxHPChanged;
    public event Action OnStatusChanged;
    [NonSerialized] public bool OnLevelChanged;

    private bool abilityWasActive = false;
    public bool AbilityWasActive => abilityWasActive;
    public void ChangeAbilityState(bool value) {if(abilityWasActive != value) {abilityWasActive = value;}}
    public void ResetAbilityState() {ChangeAbilityState( ability == null );}

#endregion
#region 初始化
    public void Init()
    {
        nature = AllNatureData.GetNature();//性格

        Skill = new List<Skill>();//生成技能
        List<LearnableSkill> skills = Base.LearnableSkills;
        foreach(LearnableSkill skill in skills)
        {
            if(skill.Level <= Level)
            {
                Skill.Add(new Skill(skill.Base));
            }

            if(Skill.Count >= 4)
            {
                break;
            }
        }

        Exp = ExpArray.GetExpForLevelAndGrowthRate(Level, _base.GrowthRate);

        //暂时
        if(iv.Length < 6)
        {
            iv = new int[6]
            {
                UnityEngine.Random.Range(0,32), UnityEngine.Random.Range(0,32),
                UnityEngine.Random.Range(0,32), UnityEngine.Random.Range(0,32),
                UnityEngine.Random.Range(0,32), UnityEngine.Random.Range(0,32)
            };
        }

        CalculateStats();//设置数值
        SetHpValue();//设置hp

        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack  , 0}, {Stat.Defence  , 0},
            {Stat.SpAttack, 0}, {Stat.SpDefence, 0},
            {Stat.Speed   , 0},
            {Stat.Accuracy, 0}, {Stat.Evasion  , 0}
        };

        HP = MaxHP;
        StatusChange  = new Queue<string>();
        damageMessage = new Queue<string>();
        Status = null;
        VolatileStatus = null;
        ResetAbilityState();
    }
#endregion
#region 基础数值
    /// <summary>
    /// 计算基础属性
    /// </summary>
    private void CalculateStats()
    {
        //计算公式
        //HP = ((种族值 * 2 + 个体值 + 努力值 * 0.25f) * 等级) * 0.01f) + 10 + 等级
        //其它 = (((种族值 * 2 + 个体值 + 努力值 * 0.25f) * 等级) * 0.01f) + 5) * 性格修正
        if(Stats == null) { Stats = new List<int>(){ 0,0,0,0,0,0 }; }

        int up = nature.Up;
        int[] baseValue = isMega? Base.MegaStrength : Base.Strength;
        if(up == 6)
        {
            for(int i = 1; i < 6; ++i)
            {
                Stats[i] = Mathf.FloorToInt((((baseValue[i] * 2 + iv[i] + basePoints[i] * 0.25f) * Level) * 0.01f) + 5f);
            }
        }
        else
        {
            int down = nature.Down;
            for(int i = 1; i < 6; ++i)
            {
                float value = (((baseValue[i] * 2 + iv[i] + basePoints[i] * 0.25f) * Level) * 0.01f) + 5f;
                if(up == i)
                {
                    value *= 1.1f;
                }
                else if(down == i)
                {
                    value *= 0.9f;
                }
                Stats[i] = (Mathf.FloorToInt(value));
            }
        }
    }

    /// <summary>
    /// 设置HP值
    /// </summary>
    void SetHpValue()
    {
        MaxHP = Mathf.FloorToInt(((Base.Strength[0] * 2 + iv[0] + basePoints[0] * 0.25f) * Level) * 0.01f) + 10 + Level;
    }

    public void LevelUpUpdate()
    {
        if(OnLevelChanged)
        {
            OnLevelChanged = false;
            CalculateStats();//设置数值
            SetHpValue();//设置hp
        }
    }
#endregion
#region 野生精灵创建数据
    /// <summary>
    /// 野生宝可梦随机属性
    /// </summary>
    public void Wild()
    {
        nature = AllNatureData.GetNature();
        iv = new int[6]
        {
            UnityEngine.Random.Range(0,32), UnityEngine.Random.Range(0,32),
            UnityEngine.Random.Range(0,32), UnityEngine.Random.Range(0,32),
            UnityEngine.Random.Range(0,32), UnityEngine.Random.Range(0,32)
        };
        basePoints = new int[6] { 0,0,0,0,0,0 };
    }
#endregion
#region 战斗时属性及增益
    /// <summary>
    /// 战斗中临时增益状态
    /// </summary>
    /// <param name="statBoosts"></param>
    public void ApplyBoosts(StatBoost[] statBoosts)
    {
        foreach(StatBoost statBoost in statBoosts)
        {
            Stat stat = statBoost.Stat;
            int boost = statBoost.Boost;
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            StatusChange.Enqueue
            (
                string.Concat
                (
                    Base.Name,
                    MyData.statsString[(int)stat],
                    boost > 0? "上升到了" : "下降到了",
                    StatBoosts[stat].ToString(),
                    "级!",
                    StatBoosts[stat] == 6?  "不会再上升了!" : null,
                    StatBoosts[stat] == -6? "不会再下降了!" : null
                )
            );
        }
    }

    /// <summary>
    /// 重置增益
    /// </summary>
    private void ResetStatBoost()
    {
        if(StatBoosts != null)
        {
            StatBoosts[Stat.Attack   ] = 0;
            StatBoosts[Stat.Defence  ] = 0;
            StatBoosts[Stat.SpAttack ] = 0;
            StatBoosts[Stat.SpDefence] = 0;
            StatBoosts[Stat.Speed    ] = 0;
            StatBoosts[Stat.Accuracy ] = 0;
            StatBoosts[Stat.Evasion  ] = 0;
        }
        else
        {
            StatBoosts = new Dictionary<Stat, int>()
            {
                {Stat.Attack  , 0}, {Stat.Defence  , 0},
                {Stat.SpAttack, 0}, {Stat.SpDefence, 0},
                {Stat.Speed   , 0},
                {Stat.Accuracy, 0}, {Stat.Evasion  , 0}
            };
        }
    }

    /// <summary>
    /// 战斗时属性计算
    /// </summary>
    int GetStat(Stat stat)
    {
        int boost = StatBoosts[stat];
        return boost >= 0?
            Mathf.FloorToInt(Stats[(int)stat] * MyData.BoostValues[boost]) :
            Mathf.FloorToInt(Stats[(int)stat] / MyData.BoostValues[-boost]);
    }
#endregion
#region 努力值
    /// <summary>
    /// 努力值之和
    /// </summary>
    /// <returns></returns>
    public int TotalBasePointsValue()
    {
        return basePoints[0]+basePoints[1]+basePoints[2]+basePoints[3]+basePoints[4]+basePoints[5];
    }

    /// <summary>
    /// 加努力值，首位索引 后面值
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void GetBasePoint(int index, int value)
    {
        basePoints[index] = Mathf.Clamp(basePoints[index] + value, 0, 252);
        int x = TotalBasePointsValue();
        if(x >= 510)
        {
            basePoints[index] -= x - 510;
            basePointsWasMax = true;
        }
    }
#endregion
#region 升级及技能学习
    /// <summary>
    /// 检查升级
    /// </summary>
    /// <returns></returns>
    public bool CheckForLevelUp()
    {
        if(Exp > ExpArray.GetExpForLevelAndGrowthRate(level + 1, _base.GrowthRate))
        {
            ++level;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获得当前等级学习技能
    /// </summary>
    /// <returns></returns>
    public LearnableSkill GetLearnSkillAtCurrentLevel()
    {
        List<LearnableSkill> learns = Base.LearnableSkills;
        foreach(LearnableSkill learn in learns)
        {
            if(learn.Level == level)
            {
                return learn;
            }
        }
        return null;
    }

    /// <summary>
    /// 学习技能
    /// </summary>
    /// <param name="skillBase"></param>
    public void LearnSkill(SkillBase skillBase)
    {
        Skill.Add(new Skill(skillBase));
    }
#endregion
#region 伤害计算
    /// <summary>
    /// 伤害计算
    /// </summary>
    /// <param name="skill">释放的技能</param>
    /// <param name="attacker">发起攻击的宝可梦</param>
    /// <returns></returns>
    public Queue<string> TakeDamage(Skill skill, Pokemon attacker)
    {
        //固定伤害直接扣,不计算
        //(((2 * 己方等级 + 10) / 250) * 攻击 / 防御 * 威力 + 2) * 加成
        //攻击方攻击 和 防守方防御 (如果攻击方招式为欺诈时，攻击取防御方的攻击)
        //其他加成 是指特性加成、道具加成、天气和状态加成、目标数量加成等的乘积。

        UpdateHpVoiceType voiceType = UpdateHpVoiceType.NormalHurt;
        AbilityBase attackerAbility = attacker.Ability;
        //如果不为DataChange则为true不用判断
        bool isAttackerAbilityActive = attacker.AbilityWasActive? true : attackerAbility.TriggerType != Ability_TriggerType.Attack;
        //如果不为Defence则为true不用判断
        bool isDefenderAbilityActive = abilityWasActive? true : ability.TriggerType != Ability_TriggerType.Defence;
        //防守方特性检查//***一部分
        if(!isDefenderAbilityActive)
        {
            string message;
            if(!(ability as DefenceAbility).IsAttackEffective(this, ref skill, out message))
            {
                damageMessage.Enqueue(message);
                abilityWasActive = true;
                return damageMessage;
            }
        }

        //克制系数计算
        float typeCoefficient = TypeChart.GetEffectiveness(skill.Base.Type, Base.Type1, Base.Type2);

        //(2 * 攻方等级 + 10) / 250)
        float a = (2 * attacker.Level + 10) * 0.004f;
        //攻防
        bool sp = skill.Base.Category == SkillCategory.Special;//判断是否为特攻技能
        float attack  = sp? attacker.SAttack : attacker.PAttack;
        float defense = sp? SDefence         : PDefence;

        //攻方属性和技能属性是否相同(属性一致加成值为1.5,否则为1)
        PokemonType skillType = skill.Base.Type;
        float sameType = attacker.Base.Type1 == skillType || attacker.Base.Type2 == skillType? 1.5f : 1f;

        //if(isAttackerAbilityActive && attackerAbility.TriggerType == A)
        bool wasActive = false;
        //技能伤害
        int power = isAttackerAbilityActive? skill.Base.Power : (attackerAbility as AttackAbility).SkillPowerAddition(ref attacker, ref skill, out wasActive);
        if(wasActive)
        {
            isAttackerAbilityActive = true;
            attacker.ChangeAbilityState(true);
        }

        //道具加成检查
        if(attacker.ItemBase != null && attacker.ItemBase.Type == ItemType.BattleItem)
        {
            power = (attacker.ItemBase as BattleItem).SkillPowerUpCheck(skill);
        }

        //会心伤害加成(1f为不暴击)
        float critical = 1f;

        //攻方宝可梦特性加成
        if(!isAttackerAbilityActive)
        {
            //暴击加成(守方没有不被暴击特性)
            if((!isDefenderAbilityActive && !(ability as DefenceAbility).CheckCritical()))
            {
                wasActive = false;
                //特性加成会心
                critical = (attackerAbility as AttackAbility).GetCritical(out wasActive);
                if(wasActive)
                {
                    attacker.ChangeAbilityState(isAttackerAbilityActive = true);
                }
            }
            else
            {
                ChangeAbilityState(isDefenderAbilityActive = true);
            }
        }
        else
        {
            if((!isDefenderAbilityActive && !(ability as DefenceAbility).CheckCritical()))
            {
                //正常会心
                critical = UnityEngine.Random.value * 100f <= 4.16f? 1.5f : 1f;
            }
            else
            {
                ChangeAbilityState(isDefenderAbilityActive = true);
            }
        }

        //会心文本
        if(critical > 1f) { damageMessage.Enqueue("会心一击!"); }

        //克制提示文本及音效
        if(typeCoefficient > 1f)
		{
			damageMessage.Enqueue("效果拔群!");
            voiceType = UpdateHpVoiceType.WellHurt;
		}
		else if(typeCoefficient == 0f)
		{
			damageMessage.Enqueue("完全没有效果...");
            voiceType = UpdateHpVoiceType.None;
		}
		else if(typeCoefficient < 1f)
		{
			damageMessage.Enqueue("不是很有效..");
		    voiceType = UpdateHpVoiceType.BadHurt;
		}

        float d = a * ((float) attack / defense) * power + 2;

        //加成 = 属性一致加成 × 属性相克倍率 × 击中要害的倍率 × 其他加成 × 随机数(属于[0.85, 1])
        float modifiers = sameType * typeCoefficient * critical * UnityEngine.Random.Range(0.85f, 1f);

        //向下取整, 但至少会造成1HP的伤害
        int damage = Mathf.FloorToInt(d * modifiers);
        if(damage == 0) { damage = 1; }

        //防守方特性检查//有可能需要在damage算出来之前处理
        if(!isDefenderAbilityActive)
        {
            //不能满血秒杀
            if(isHPFull && damage >= MaxHP)
            {
                isDefenderAbilityActive = true;
                string message = (ability as DefenceAbility).CheckSpike();
                if(message != null)
                {
                    if(skill.Base.SkillType == SkillType.一击必杀)
                    {
                        damageMessage.Enqueue($"{NickName}因为{ability.Name}, 一击必杀无效了!");
                        damage = 0;
                    }
                    else
                    {
                        damageMessage.Enqueue(message);
                        damage = MaxHP - 1;
                    }
                    ChangeAbilityState(isDefenderAbilityActive = true);
                }
            }
        }
        //else if(气息腰带)

        //特殊伤害结算检查
		if(damage == 0 && voiceType != UpdateHpVoiceType.None)
        {
            voiceType = UpdateHpVoiceType.None;
        }

        UpdateHP(-damage, voiceType);
        return damageMessage;
    }
#endregion
    /// <summary>
    /// 更新HP
    /// </summary>
    /// <param name="value">(传正值加血 负值扣血)</param>
    /// <param name="voiceID">伤害声>= 0 && < 4, 治疗 -1, </param>
    public void UpdateHP(int value, UpdateHpVoiceType voiceType = UpdateHpVoiceType.NormalHurt)
    {
        switch(voiceType)
        {
            case UpdateHpVoiceType.BadHurt:    AudioManager.Instance.HitSource(0) ; break;
            case UpdateHpVoiceType.NormalHurt: AudioManager.Instance.HitSource(1) ; break;
            case UpdateHpVoiceType.WellHurt:   AudioManager.Instance.HitSource(2) ; break;
            case UpdateHpVoiceType.Cure:       AudioManager.Instance.HealPokemon(); break;
        }

        HP = Mathf.Clamp(HP + value, 0, MaxHP);
        OnHPChanged?.Invoke();
    }

    /// <summary>
    /// 状态设置
    /// </summary>
    /// <param name="conditionid"></param>
    public void SetStatus(ConditionID conditionID)
    {
        //已有状态检查
        if(Status != null)
        {
            StatusChange.Enqueue($"{NickName}已经{Status.StartMessage}!");
            return;
        }

        //特性保护检查
        string str = ability != null? ability.TriggerType == Ability_TriggerType.Defence?
            (ability as DefenceAbility).CheckConditionProtect(conditionID) : null : null;
        if(str != null)
        {
            StatusChange.Enqueue(str);
            return;
        }

        //属性判断
        switch(conditionID)
        {
            case ConditionID.psn: case ConditionID.hyp:

                if(Base.Type1 == PokemonType.毒 || Base.Type2 == PokemonType.毒) { StatusChange.Enqueue("没有效果..."); return; }

            break;

            case ConditionID.brn:

                if(Base.Type1 == PokemonType.火 || Base.Type2 == PokemonType.火) { StatusChange.Enqueue("没有效果..."); return; }

            break;

            case ConditionID.frz:

                if(Base.Type1 == PokemonType.冰 || Base.Type2 == PokemonType.冰) { StatusChange.Enqueue("没有效果..."); return; }

            break;

            case ConditionID.par:

                if(Base.Type1 == PokemonType.电 || Base.Type2 == PokemonType.电) { StatusChange.Enqueue("没有效果..."); return; }

            break;
        }

        //查看是否特性免疫状态
        if(ability != null && ability.TriggerType == Ability_TriggerType.Defence)
        {
            string message = (ability as DefenceAbility).CheckConditionProtect(conditionID);
            if(message != null)
            {
                StatusChange.Enqueue(NickName + message);
                return;
            }
        }

        Status = AllConditionData.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChange.Enqueue($"{NickName}{Status.StartMessage}!");
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// 临时状态设置
    /// </summary>
    /// <param name="conditionid"></param>
    public void SetVolatileStatus(ConditionID conditionID)
    {
        //已有状态检查
        if(VolatileStatus != null)
        {
            StatusChange.Enqueue
            (
                string.Concat(NickName, "已经", VolatileStatus.StartMessage, "!")
            );
            return;
        }

        //特性保护检查
        string str = ability != null? ability.TriggerType == Ability_TriggerType.Defence?
            (ability as DefenceAbility).CheckConditionProtect(conditionID) : null : null;
        if(str != null)
        {
            StatusChange.Enqueue(str);
            return;
        }

        VolatileStatus = AllConditionData.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChange.Enqueue(string.Concat(Base.Name, VolatileStatus.StartMessage));
    }

    /// <summary>
    /// 判断是否能行动，检查状态
    /// </summary>
    /// <returns>bool值</returns>
    public bool OnBeforeMove()
    {
        bool canPerformMove = true;

        if(Status?.OnBeforeMove != null)
        {
            if(!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        if(VolatileStatus?.OnBeforeMove != null)
        {
            if(!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    /// <summary>
    /// 战斗结束事件处理
    /// </summary>
    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
        if(isMega)
        {
            EndMega();
        }
        if(isDynamax)
        {
            EndDynamax();
        }
    }
#region 战斗中进化
    /// <summary>
    /// 查看是否能mega进化
    /// </summary>
    /// <returns>true直接进化算值 false无变化</returns>
    public bool CanMega()
    {
        if(_base.CanMega && carryonItem.Base != null && carryonItem.Base.Use(this))
        {
            isMega = true;
            CalculateStats();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 结束Mega
    /// </summary>
    private void EndMega()
    {
        isMega = false;
        CalculateStats();
    }

    /// <summary>
    /// 极巨化
    /// </summary>
    public void DynamaxEvolution()
    {
        isDynamax = true;
        MaxHP += MaxHP;
        OnMaxHPChanged?.Invoke();
        UpdateHP(HP, UpdateHpVoiceType.None);
    }

    /// <summary>
    /// 结束极巨化
    /// </summary>
    public void EndDynamax()
    {
        isDynamax = false;
        MaxHP = (int)(MaxHP * 0.5f);
        OnMaxHPChanged?.Invoke();
        if(HP != 1)
        {
            UpdateHP((int)(HP * 0.5f), UpdateHpVoiceType.None);
        }
    }
#endregion
    /// <summary>
    /// 随机技能
    /// </summary>
    /// <returns></returns>
    public Skill GetRandomSkills()
    {
        int skillCount = Skill.Count;
        int r = UnityEngine.Random.Range(0, skillCount);
        if(Skill[r].CheckIfPPIsGreaterThanZero())
        {
            return Skill[r];
        }
        else
        {
            for(int i = 0; i < skillCount; ++i)
            {
                if(Skill[i].CheckIfPPIsGreaterThanZero())
                {
                    return Skill[i];
                }
            }
        }
        return null;
        //debug没有写所有技能0pp的情况
    }

    /// <summary>
    /// 进化
    /// </summary>
    public void Evolution()
    {
        int id = _base.Evolution.EvolutionID;
        _base = AllPokemon.GetPokemonByID(id);
        CalculateStats();
        SetHpValue();
    }

    /// <summary>
    /// 替换之后处理
    /// </summary>
    public void ReplacementsAfter()
    {
        //
        CureVolatileStatus();
        ResetAbilityState();
    }

    /// <summary>
    /// 锁定/解锁宝可梦Lock = !Lock
    /// </summary>
    public void LockPokemon(Action<bool> _action)
    {
        lockPKM = !lockPKM;
        _action.Invoke(lockPKM);
    }
#region 道具相关
    /// <summary>
    /// 宝可梦中心的治疗
    /// </summary>
    public void OnCureAll()
    {
        Status = null;
        VolatileStatus = null;

        if(HP != MaxHP)
        {
            HP = MaxHP;
        }

        foreach(Skill skill in Skill)
        {
            skill.ResetPP();
        }
    }

    /// <summary>
    /// 治疗状态
    /// </summary>
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// 治疗临时状态
    /// </summary>
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// 治疗所有状态
    /// </summary>
    public void CureAllStatus()
    {
        VolatileStatus = null;
        Status = null;
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// 替换恢复
    /// </summary>
    public void InsteadCure()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

    /// <summary>
    /// 全复药
    /// </summary>
    public void AllCureItem()
    {
        Status = null;
        HP = MaxHP;
        OnHPChanged?.Invoke();
        if(VolatileStatus != null && VolatileStatus.ConditionID == ConditionID.confusion)//治疗混乱
        {
            VolatileStatus = null;
        }
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// 回复药
    /// </summary>
    public void RestoreAll()
    {
        Status = null;
        VolatileStatus = null;
        HP = MaxHP;
        OnHPChanged?.Invoke();
        OnStatusChanged?.Invoke();
    }

    /// <summary>
    /// 携带道具
    /// </summary>
    /// <param name="item"></param>
    public ItemSlot CarryOnItem(ItemBase item)
    {
        ItemSlot old = carryonItem;
        carryonItem = new ItemSlot(item, 1);
        return old;
    }

    /// <summary>
    /// 薄荷改性格加成
    /// </summary>
    /// <param name="v"></param>
    public void ChangeNature(int v)
    {
        nature = AllNatureData.Instead(nature.Name, v);
    }
#endregion
#region 存取数据
    public Pokemon(PokemonSaveData saveData)
    {
        if(saveData == null)
        {
            return;
        }
        _base = AllPokemon.GetPokemonByID(saveData.id);
        nickName = saveData.name;
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;
        if(saveData.statusId != null)
        {
            Status = AllConditionData.Conditions[saveData.statusId.Value];
        }
        else
        {
            Status = null;
        }
        isShiny = saveData.shiny;
        iv = saveData.individual;
        basePoints = saveData.basePoint;
        basePointsWasMax = saveData.basePointsWasMax;
        nature = saveData.nature;

        Skill = new List<Skill>();
        foreach(SkillSaveData skill in saveData.skills)
        {
            Skill.Add(new Skill(skill));
        }
        lockPKM = saveData.lockPKM;

        CalculateStats();//初始化
        SetHpValue();

        /*AbilityBase[] abilities = _base.Abilities;
        int x = abilities.Length;
        int abilityID = saveData.abilityID;
        for(int i = 0; i < x; ++i)
        {
            if(abilityID == abilities[i].ID)
            {
                ability = abilities[i];
                break;
            }
        }*/

        StatusChange = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }
    public PokemonSaveData GetSaveData()
    {
        if(Base == null)
        {
            return null;
        }
        int x = Skill.Count;
        List<SkillSaveData> skillSaveDatas = new List<SkillSaveData>();
        for(int i = 0; i < x; ++i)
        {
            skillSaveDatas.Add(Skill[i].GetSaveData());
        }
        PokemonSaveData saveData = new PokemonSaveData()
        {
            name = nickName == null? null : nickName.Length == 0? null : nickName,
            id = Base.ID,
            //abilityID = ability.ID,
            hp = HP,
            level = Level,
            exp = Exp,
            statusId = Status?.ConditionID,
            shiny = Shiny,
            individual = IV,
            basePoint = basePoints,
            basePointsWasMax = basePointsWasMax,
            nature = Nature,
            skills = skillSaveDatas,
            lockPKM = lockPKM
        };
        return saveData;
    }
#endregion
}

[System.Serializable]
public class PokemonSaveData
{
    public string name;
    public int id;
    public int abilityID;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public bool shiny;
    public int[] individual;
    public int[] basePoint;
    public bool basePointsWasMax;
    public Nature nature;
    public List<SkillSaveData> skills;
    public bool lockPKM;
}

public enum UpdateHpVoiceType{ None, BadHurt, NormalHurt, WellHurt, Cure}