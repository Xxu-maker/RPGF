using System.Collections.Generic;
using UnityEngine;
public class AllConditionData
{
    private static bool AlreadyLoad;
    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        if(AlreadyLoad)
        {
            return;
        }
        foreach(KeyValuePair<ConditionID, Condition> kvp in Conditions)
        {
            ConditionID conditionID = kvp.Key;
            Condition condition = kvp.Value;
            condition.ConditionID = conditionID;
        }
        AlreadyLoad = true;
    }

    public static Dictionary<ConditionID, Condition> Conditions// { get; set; }
        = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "中毒",
                StartMessage = "中毒了",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(-(int)(pokemon.MaxHP * 0.125));//(1/8)
                    pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "受到了中毒的伤害!"));
                }
            }
        },

        {
            ConditionID.hyp,
            new Condition()
            {
                Name = "剧毒",
                StartMessage = "中了剧毒",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = 1;//中毒回合

                },
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    int d = ((int)(pokemon.MaxHP * HypertoxicValue[pokemon.StatusTime]));
                    if(d < 1)
                    {
                        d = 1;
                    }
                    pokemon.UpdateHP(-d);
                    if(pokemon.StatusTime < 16)
                    {
                        pokemon.StatusTime++;
                    }
                    pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "受到了剧毒的伤害!"));
                }
            }
        },

        {
            ConditionID.brn,
            new Condition()
            {
                Name = "烧伤",
                StartMessage = "烧伤了",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    int s = (int)(pokemon.MaxHP * 0.0625);//(1/16)
                    if(s < 1)
                    {
                        pokemon.UpdateHP(-1);
                    }
                    else
                    {
                        pokemon.UpdateHP(-s);
                    }
                    pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "受到了烧伤的伤害!"));
                }
            }
        },

        {
            ConditionID.par,
            new Condition()
            {
                Name = "麻痹",
                StartMessage = "麻痹了",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "麻痹了, 不能行动!"));
                        return false;
                    }
                    return true;
                }
            }
        },

        {
            ConditionID.frz,
            new Condition()
            {
                Name = "冰冻",
                StartMessage = "冻住了",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "解除冰冻了!"));
                        return true;
                    }
                    pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "冻住了, 不能行动!"));
                    return false;
                }
            }
        },

        {
            ConditionID.slp,
            new Condition()
            {
                Name = "睡眠",
                StartMessage = "睡着了",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = Random.Range(1, 4);//睡1-3回合

                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "醒了!"));
                        return true;
                    }
                    pokemon.StatusTime--;
                    pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "睡着了..."));
                    return false;
                }
            }
        },

        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "混乱",
                StartMessage = "混乱了",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = Random.Range(1, 5);//混乱1-4回合

                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "清醒了!"));
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    if(Random.Range(1, 3) == 1)//50%
                    {
                        return true;
                    }
                    pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "混乱了..."));
                    int damage = (int)(pokemon.MaxHP * 0.125);
                    if(damage < 1)
                    {
                        damage = 1;
                    }
                    pokemon.UpdateHP(-damage);//(1/8)
                    pokemon.StatusChange.Enqueue($"攻击了自己...");
                    return false;
                }
            }
        },

        {
            ConditionID.flinch,
            new Condition()
            {
                Name = "畏缩",
                StartMessage = "畏缩了...",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = 1;//畏缩1回合
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.VolatileStatusTime == 0)
                    {
                        pokemon.CureVolatileStatus();
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "畏缩了..."));
                    return false;
                }
            }
        },

        {
            ConditionID.infatuation,
            new Condition()
            {
                Name = "着迷",
                StartMessage = "着迷了...",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = 1;//着迷1回合
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.value > 0.5f)//50%不能行动
                    {
                        return true;
                    }
                    else
                    {
                        pokemon.StatusChange.Enqueue(string.Concat(pokemon.NickName, "着迷了..."));
                        return false;
                    }
                }
            }
        },
    };

    /// <summary>
    /// 异常状态捕捉加成
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static float GetStatusBonus(Condition condition)
    {
        if(condition == null)
        {
            return 1f;
        }
        else if(condition.ConditionID == ConditionID.slp || condition.ConditionID == ConditionID.frz)
        {
            return 2.5f;
        }
        else if(condition.ConditionID == ConditionID.par || condition.ConditionID == ConditionID.hyp ||
                condition.ConditionID == ConditionID.psn || condition.ConditionID == ConditionID.brn)
        {
            return 1.5f;
        }
        //都不是
        return 1f;
    }

    /// <summary>
    /// 剧毒相应回合的伤害值(index是time)
    /// </summary>
    private static double[] HypertoxicValue = new double[]
    {
        0d     ,           //0
        0.0625d,           //1
        0.125d , 0.1875d,  //2 , 3
        0.25d  , 0.3125d,  //4 , 5
        0.375d , 0.4375d,  //6 , 7
        0.5d   , 0.5625d,  //8 , 9
        0.625d , 0.6875d,  //10, 11
        0.75d  , 0.8125d,  //12, 13
        0.875d , 0.9375d,  //14, 15
    };
}

public enum ConditionID
{
    none, psn,    hyp,        brn,  slp,   par,       frz,   confusion, flinch, infatuation
        //中毒,   剧毒,        烧伤, 睡眠,  麻痹,       冰冻,  混乱,       畏缩,   着迷
        //poison, hypertoxic, burn, sleep, paralysis, frozen
}
/*混乱	在１～４回合内，有时会攻击自己。
着迷	♂的话对♀，♀的话对♂，会变得很难使出招式。
剧毒*	剧毒状态。随着回合的推进，中毒伤害会增加。
恶梦	在睡着的时候，每回合ＨＰ会减少。
瞌睡*	在受到招式攻击的下一回合，变为睡眠状态。
再来一次	受到再来一次的招式后，在３回合内，只能使出【招式名称】。
无特性	特性会消失，变得没有效果。
无理取闹	不能连续使出同样的招式。
回复封锁	在５回合内，无法通过招式、特性或携带的道具回复ＨＰ。
被识破	会被那些原本不会有特定效果的招式击中。无视闪避率提升的效果，都会受到攻击。
定身法	在４回合内，【招式名称】会变得无法使用。
无法逃走	无法逃走或替换。
锁定	在锁定的下一回合，招式必定会击中。
查封	在５回合内，持有物和道具会变得无法使用。
挑衅	变得只能使出给予伤害的招式。
意念移物	在３回合内，一击必杀以外的招式也必定会击中。地面属性的招式会变得无法击中。
诅咒	在每回合结束时减少ＨＰ。
万圣夜	宝可梦会被追加幽灵属性。
灭亡之歌	听了灭亡之歌的全部宝可梦３回合后陷入濒死。
森林诅咒	宝可梦会被追加草属性。
寄生种子	每回合结束时，会被寄生种子夺走ＨＰ，同时回复对手的ＨＰ。
束缚	被紧紧绑住，每一回合都会受到伤害。
未来攻击	被招式攻击的２回合后，会受到伤害。
击落	被击落，掉到地面的状态。
地狱突刺	在２回合内，无法使出发声音的招式。
沥青射击	弱点变为火。
蛸固	每回合降低防御和特防。
碎片	尖锐的碎片扎入了体内，每次行动都会受到伤害。
进攻下降	进攻力变弱，攻击和特攻会降低。
防守下降	防守力变弱，防御和特防会降低。
以下状态为非官方状态
畏缩	在１回合内，无法使出招式。
拍落	持有物被拍落，变得无法使用。
输电	在１回合内，使出的招式都会变成电属性。
粉尘	使用火属性招式时会爆炸并受到伤害。
万众瞩目	在１回合内，对手的攻击全部指向自己。
向己方施加的状态变化
状态名称	效果
易中要害	击中对手要害的几率变高。
怨念	因对手的招式而濒死时，该招式的ＰＰ会变为０。
充电	在充电的下一回合，电属性的招式威力会变为２倍。
蓄力	在蓄力的时候，防御和特防会提高。
电磁飘浮	在５回合内，地面属性的招式会变得无法击中。
祈愿	使用祈愿后的下一回合，ＨＰ会回复。
扎根	在每回合结束时回复ＨＰ。扎根的宝可梦无法替换。
封印	宝可梦使出封印后，其所学会的招式，对手的宝可梦将无法使出。
同命	让自己陷入濒死的对手，也会一同陷入濒死。
忍耐	在２回合内忍受攻击，受到的伤害会２倍返还给对手。
逆鳞	在２～３回合内，乱打一气地进行攻击。大闹一番后自己会陷入混乱。
大闹一番	在２～３回合内，乱打一气地进行攻击。大闹一番后自己会陷入混乱。
花瓣舞	在２～３回合内，乱打一气地进行攻击。大闹一番后自己会陷入混乱。
水流环	在自己身体的周围覆盖用水制造的幕。每回合回复ＨＰ。
幸运咒语	变得不会被对手的招式击中要害。
身体轻量化	速度会大幅度提高，体重也会变轻。
磨砺	在使用磨砺的下一回合，招式必定会击中要害。
热衷	会热衷于最近一次使用的招式。所热衷的招式造成的伤害会增加。同时自己受到的伤害也会增加。
烟幕	让自己被烟雾包裹，对手的招式将变得不易命中。
加倍	摆起强而有力的架势增加造成的伤害。
攻守转换	进攻力和防守力互换。
慢启动	在拿出平时的水准之前，攻击与速度都会减半。
狂猛之力	每次行动时因中毒或碎片等受到的伤害会减少。
伟大之力	每次行动时因中毒或碎片等受到的伤害会减少。
野性之力	所有能力都会提高，且每次行动时因中毒或碎片等受到的伤害会减少。
惊骇之力	所有能力都会提高，且每次行动时因中毒或碎片等受到的伤害会减少。
进攻提升	进攻力变强，攻击和特攻会提高。
防守提升	防守力变强，防御和特防会提高。
进攻下降	进攻力变弱，攻击和特攻会降低。
防守下降	防守力变弱，防御和特防会降低。
以下状态为非官方状态
帮助	在１回合内，招式的威力会变为１．５倍。
替身	可以防住攻击，受到一定的伤害后就会消失。
守住	在１回合内，对方的招式不会命中自己。
变小	身体变小，被特定招式攻击时受到的伤害会变为２倍。
愤怒	受到攻击时，会因愤怒的力量而提高攻击。
魔法反射	受到会降低能力、变成异常状态或状态变化的招式时，会将招式反射回去。
万众瞩目	在１回合内，对手的攻击全部指向自己。
力量戏法	攻击和防御互换。
变身	变成其它宝可梦的样子。
飞翔	飞上了天空，对方的大部分招式不会命中自己。下一回合将进行攻击。
挖洞	潜入了地下，对方的大部分招式不会命中自己。下一回合将进行攻击。
潜水	潜入了水中，对方的大部分招式不会命中自己。下一回合将进行攻击。
暗影潜袭	消失身影了，对方的大部分招式不会命中自己。下一回合将进行攻击。
吃饱	宝可梦吃掉了树果，可以使出打嗝。
向场地施加的状态变化
状态名称	效果
以下状态为向全体场地施加的状态变化
戏法空间	在５回合内，速度慢的宝可梦可以先行动。
魔法空间	在５回合内，变得无法使用持有物。
奇妙空间	在５回合内，宝可梦的防御和特防会互换。
重力	在５回合内，招式会变得容易击中。飘浮特性和飞行属性的宝可梦会被地面招式击中。飞向空中的招式将无法使用。
玩泥巴	在５回合内，电属性的招式威力会减半。
玩水	在５回合内，火属性的招式威力会减半。
吵闹	在３回合内，用骚乱攻击对手。在此期间谁都不能入眠。
妖精之锁	在使用招式的下一回合，所有的宝可梦都无法逃走。
以下状态为天气型状态
大晴天	天气为晴朗状态。火属性的招式威力会提高。水属性的招式威力会降低。
下雨	火属性招式给予的伤害会减少。
沙暴	天气为沙暴状态。岩石、地面和钢属性以外的宝可梦，每回合会受到伤害。岩石属性的特防会变强。
冰雹	天气为冰雹状态。冰属性以外的宝可梦，每回合会受到伤害。
起雾	除必定命中的招式外，所有招式会变得不易命中。
大日照	天气为大日照状态。火属性的招式威力会提高。不会受到水属性招式的攻击。
大雨	天气为大雨状态。水属性的招式威力会提高。不会受到火属性招式的攻击。
乱流	天气为乱流状态。与飞行属性相克且效果绝佳的属性，其招式威力会变弱。
晴朗	草属性宝可梦的速度会提高。
下雪	会变得容易冻伤。陷入瞌睡状态时会变得不易行动。冰属性宝可梦的速度会提高。
以下状态为场地型状态
电气场地	在５回合内，地面上的宝可梦不会变为睡眠状态。电属性的招式威力会提高。
青草场地	在５回合内，地面上的宝可梦会缓缓回复ＨＰ。草属性的招式威力会提高。
薄雾场地	在５回合内，地面上的宝可梦不会陷入异常状态。龙属性招式的伤害也会减半。
精神场地	在５回合内，地面上的宝可梦不会受到先制招式的攻击。超能力属性的招式威力会提高。
以下状态为向对手场地施加的状态变化
撒菱	给予替换出场的宝可梦伤害。飞行属性和飘浮特性的宝可梦不会被击中。
毒菱	让替换出场的宝可梦陷入中毒状态。飞行属性和飘浮特性的宝可梦不会被击中。
隐形岩	替换出场的宝可梦会受到伤害。
黏黏网	降低替换出场的宝可梦的速度。
湿地	在４回合内，速度变为原来的１／４。
火海	在４回合内，火属性以外的宝可梦，每回合受到伤害。
超极巨地狱灭焰	在４回合内对火属性以外的宝可梦造成伤害。
超极巨灰飞鞭灭	在４回合内对草属性以外的宝可梦造成伤害。
超极巨水炮轰灭	在４回合内对水属性以外的宝可梦造成伤害。
超极巨炎石喷发	在４回合内对岩石属性以外的宝可梦造成伤害。
超极巨钢铁阵法	对替换出场的宝可梦造成伤害。
以下状态为向己方场地施加的状态变化
白雾	在５回合内，不会被降低能力。
神秘守护	在５回合内，不会陷入异常状态。
光墙	在５回合内，受到的特殊攻击威力会减半。
反射壁	在５回合内，受到的物理攻击威力会减半。
顺风	在４回合内，出战的宝可梦速度会变为２倍。
彩虹	在４回合内，招式的追加效果变得更易出现。
极光幕	在５回合内，物理与特殊攻击的威力会减半。
以下状态为非官方状态
快速防守	在１回合内，对手的先制招式不会命中我方全员。
广域防守	在１回合内，对手击打我方全员的攻击不会命中。
戏法防守	在１回合内，对手的变化招式不会命中我方全员。
*/