using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "宝可梦",menuName = "宝可梦/创建新宝可梦")]
[PreferBinarySerialization]//二进制序列化,可以提升大量数据资源文件的读写性能//无法合并到版本控制软件
public class PokemonBase : ScriptableObject
{
    [SerializeField] int id;
    [SerializeField] string pokemonName;

    [TextArea]
    [SerializeField] string description;//描述

    [Header("宝可梦动图设置数据")]
    [SerializeField] PokemonSpriteSetData spriteSetData;
    //体型用于计算技能打击位置
    [SerializeField] PokemonBodyType bodyTypeUsedToCalculateHitPos;

    [Header("属性")]
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [Header("蛋组")]
    [SerializeField] EggGroup eggType1;
    [SerializeField] EggGroup eggType2;

    [Header("特性表")]
    [SerializeField] AbilityBase[] abilities;

    [Header("种族值")]
    [SerializeField] int[] strength;
    [Header("战斗中进化(Mega和极巨化)")]
    [SerializeField] bool canMega;
    [SerializeField] bool canGigantamax;
    [Header("Mega种族值(0号位为Hp不填 但一定要6个, 和普通种族值项对应)")]
    [SerializeField] int[] megaStrength;

    [Header("击败经验")]
    [SerializeField] int expYield;

    [Header("击败努力值")]
    [SerializeField] List<BasePoint> eValue;

    [Header("经验(fest)6, 8, (fer)10, (ser)105986, 125, (sest)164")]
    [SerializeField] GrowthRate growthRate;

    [Header("捕捉概率")]
    [SerializeField] int catchRate = 255;

    [Header("公母比例(1 为 1:1 7为 7:1)")]
    [SerializeField] int ratio;//1 为 1:1 7为 7:1

    [Header("进化")]
    [SerializeField] Evolution evolution;

    [Header("技能表")]
    [SerializeField] List<LearnableSkill> learnableSkill;

    public int ID => id;
    public string Name => pokemonName;
    public string Description => description;

    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;

    public AbilityBase[] Abilities => abilities;

    public int[] Strength => strength;
    public bool CanMega => canMega;
    public bool CanGigantamax => canGigantamax;
    public int[] MegaStrength => megaStrength;

    public PokemonSpriteSetData SpriteSetData => spriteSetData;

    public PokemonBodyType BodyType => bodyTypeUsedToCalculateHitPos;

    public int CatchRate => catchRate;
    public int Ratio => ratio;
    public int ExpYield => expYield;
    public Evolution Evolution => evolution;
    public List<BasePoint> EValue => eValue;
    public GrowthRate GrowthRate => growthRate;

    public List<LearnableSkill> LearnableSkills => learnableSkill;
}

[System.Serializable]
/// <summary>
/// 宝可梦动图设置数据
/// </summary>
public class PokemonSpriteSetData
{
    [Header("普通呼吸动画帧数 0普通 1Mega 2超极巨")]
    [SerializeField] int[] normalCount;
    public int[] NormalCount => normalCount;

    [Header("位置01普通 23Mega 45超极巨(反-正)")]
    [SerializeField] float[] fixY;
    public float[] FixY => fixY;

    [Header("影子位置01普通 23Mega 45超极巨(反-正)")]
    [SerializeField] Vector2[] shadowFix;
    public Vector2[] ShadowFix => shadowFix;
}
//体型
public enum PokemonBodyType{ Small, Medium, Big }

[System.Serializable]
public class PokemonSprite
{
    [SerializeField] List<Sprite> front;
    [SerializeField] List<Sprite> back;
    [SerializeField] List<Sprite> frontShiny;
    [SerializeField] List<Sprite> backShiny;
    public List<Sprite> GetSprite(bool f, bool shiny, bool mega, bool gigantamax)
    {
        if(shiny)
        {
            return f? frontShiny : backShiny;
        }
        else
        {
            return f? front : back;
        }
    }
}

[System.Serializable]
public class LearnableSkill
{
    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;
    public SkillBase Base => skillBase;
    public int Level => level;
}

[System.Serializable]
public struct BasePoint//努力值
{
    [SerializeField] BasePointType effort;
    [SerializeField] int value;
    public BasePointType Type => effort;
    public int Value => value;
}

[System.Serializable]
public struct Evolution//进化
{
    [SerializeField] bool canEvo;
    [SerializeField] bool needHappiness;
    [SerializeField] bool needStone;
    [SerializeField] int stoneID;
    [SerializeField] bool needIllumination;
    [SerializeField] int evoLevel;
    [SerializeField] int evoId;
    public bool CanEvolution => canEvo;
    public bool Happiness => needHappiness;
    public bool Stone => needStone;
    public int StoneID => stoneID;
    public bool Illumination => needIllumination;
    public int EvoLevel => evoLevel;
    public int EvolutionID => evoId;
}

/// <summary>
/// 宝可梦数据分类
/// </summary>
public enum Stat
{
    //物攻,  物防,    特攻,     特防,      速度,
    Attack = 1, Defence, SpAttack, SpDefence, Speed,
    //命中率, 闪避
    Accuracy, Evasion
}

/// <summary>
/// 努力值类型
/// </summary>
public enum BasePointType { HP努力值, 物攻努力值, 物防努力值, 特攻努力值, 特防努力值, 速度努力值 }

/// <summary>
/// 蛋组
/// </summary>
public enum EggGroup
{
    无, 怪兽组, 人型组, 水1组, 水3组, 虫组,
    矿物组, 飞行组, 不定形组, 陆上组, 水2组, 妖精组, 百变怪,
    植物组, 龙组, 未发现组, 性别不明
}

/// <summary>
/// 宝可梦属性
/// </summary>
public enum PokemonType
{
    一般, 火, 水, 草, 电, 冰, 格斗, 毒, 地面, 飞行, 超能力, 虫, 岩石, 幽灵, 龙, 恶, 钢, 妖精, None
}

/// <summary>
/// 属性克制
/// </summary>
public class TypeChart
{
    private static float[][] chart =
    {
        //攻\守              普   火   水   草    电   冰   格   毒   地   飞    超   虫   岩    鬼   龙   恶   钢   妖
        /*普*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f ,0.5f, 0f , 1f , 1f ,0.5f, 1f },
        /*火*/ new float[] { 1f ,0.5f,0.5f, 2f , 1f , 2f , 1f , 1f , 1f , 1f , 1f , 2f ,0.5f, 1f ,0.5f, 1f , 2f , 1f },
        /*水*/ new float[] { 1f , 2f ,0.5f,0.5f, 1f , 1f , 1f , 1f , 2f , 1f , 1f , 1f , 2f , 1f ,0.5f, 1f , 1f , 1f },
        /*草*/ new float[] { 1f ,0.5f, 2f ,0.5f, 1f , 1f , 1f ,0.5f, 2f ,0.5f, 1f ,0.5f, 2f , 1f ,0.5f, 1f ,0.5f, 1f },
        /*电*/ new float[] { 1f , 1f , 2f ,0.5f,0.5f, 1f , 1f , 1f , 0f , 2f , 1f , 1f , 1f , 1f ,0.5f, 1f , 1f , 1f },
        /*冰*/ new float[] { 1f ,0.5f,0.5f, 2f , 1f ,0.5f, 1f , 1f , 2f , 2f , 1f , 1f , 1f , 1f , 2f , 1f ,0.5f, 1f },
        /*格*/ new float[] { 2f , 1f , 1f , 1f , 1f , 2f , 1f ,0.5f, 1f ,0.5f,0.5f,0.5f, 2f , 0f , 1f , 2f , 2f ,0.5f},
        /*毒*/ new float[] { 1f , 1f , 1f , 2f , 1f , 1f , 1f ,0.5f,0.5f, 1f , 1f , 1f ,0.5f,0.5f, 1f , 1f , 0f , 2f },
        /*地*/ new float[] { 1f , 2f , 1f ,0.5f, 2f , 1f , 1f , 2f , 1f , 1f , 1f ,0.5f, 2f , 1f , 1f , 1f , 2f , 1f },
        /*飞*/ new float[] { 1f , 1f , 1f , 2f ,0.5f, 1f , 2f , 1f , 1f , 1f , 1f , 2f ,0.5f, 1f , 1f , 1f ,0.5f, 1f },
        /*超*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 2f , 2f , 1f , 1f ,0.5f, 1f , 1f , 1f , 1f , 0f ,0.5f, 1f },
        /*虫*/ new float[] { 1f ,0.5f, 1f , 2f , 1f , 1f ,0.5f,0.5f, 1f ,0.5f, 2f , 1f , 1f ,0.5f, 1f , 2f ,0.5f,0.5f},
        /*岩*/ new float[] { 1f , 2f , 1f , 1f , 1f , 2f ,0.5f, 1f ,0.5f, 2f , 1f , 2f , 1f , 1f , 1f , 1f ,0.5f, 1f },
        /*鬼*/ new float[] { 0f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f , 1f , 2f , 1f ,0.5f, 1f , 1f },
        /*龙*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f ,0.5f, 0f },
        /*恶*/ new float[] { 1f , 1f , 1f , 1f , 1f , 1f ,0.5f, 1f , 1f , 1f , 2f , 1f , 1f , 2f , 1f ,0.5f, 1f ,0.5f},
        /*钢*/ new float[] { 1f ,0.5f,0.5f, 1f ,0.5f, 2f , 1f , 1f , 1f , 1f , 1f , 1f , 2f , 1f , 1f , 1f ,0.5f, 2f },
        /*妖*/ new float[] { 1f ,0.5f, 1f , 1f , 1f , 1f , 2f ,0.5f, 1f , 1f , 1f , 1f , 1f , 1f , 2f , 2f ,0.5f, 1f }
        //攻/守              普   火   水   草    电   冰   格   毒   地   飞    超   虫   岩    鬼   龙   恶   钢   妖
    };

    /// <summary>
    /// 克制倍数计算
    /// </summary>
    /// <param name="attackType1">攻击技能的属性</param>
    /// <param name="defenceType1">防守方属性1</param>
    /// <param name="defenceType2">防守方属性2</param>
    /// <returns>倍数</returns>
    public static float GetEffectiveness(PokemonType attackType1, PokemonType defenceType1,  PokemonType defenceType2)
    {
        int atk = (int)attackType1;
        return
        (
            attackType1 == PokemonType.None || defenceType1 == PokemonType.None?
            1 : chart[atk][(int)defenceType1]
        )
        *
        (
            attackType1 == PokemonType.None || defenceType2 == PokemonType.None?
            1 : chart[atk][(int)defenceType2]
        );
    }
}

/// <summary>
/// 经验成长
/// </summary>
public enum GrowthRate { Fastest, Fast, Faster, Slower, Slow, Slowest}

/// <summary>
/// 经验表
/// </summary>
public class ExpArray
{
    /// <summary>
    /// 获取当前等级经验
    /// </summary>
    public static int GetExpForLevelAndGrowthRate(int level, GrowthRate growthRate)
    {
        return chart[ level > 100? 99 : level - 1 ][ (int)growthRate ];
    }

    private static int[][] chart =
    {
        //         600000, 800000, 1000000, 1059860, 1250000, 1640000
        //         最快    快       较快     较慢     慢       最慢
        new int[] {0	 , 0	 , 0	  , 0      , 0      , 0	      },//1
        new int[] {15	 , 6	 , 8	  , 9      , 10     , 4	      },//2
        new int[] {52	 , 21	 , 27	  , 57     , 33     , 13	  },//3
        new int[] {122	 , 51	 , 64	  , 96     , 80     , 32	  },//4
        new int[] {237	 , 100	 , 125	  , 135    , 156    , 65	  },//5
        new int[] {406	 , 172	 , 216	  , 179    , 270    , 112	  },//6
        new int[] {637	 , 274	 , 343	  , 236    , 428    , 178	  },//7
        new int[] {942	 , 409	 , 512	  , 314    , 640    , 276	  },//8
        new int[] {1326	 , 583	 , 729	  , 419    , 911    , 393	  },//9
        new int[] {1800	 , 800	 , 1000	  , 560    , 1250   , 540	  },//10
        new int[] {2369	 , 1064	 , 1331	  , 742    , 1663   , 745	  },//11
        new int[] {3041	 , 1382	 , 1728	  , 973    , 2160   , 967	  },//12
        new int[] {3822	 , 1757	 , 2197	  , 1261   , 2746   , 1230	  },//13
        new int[] {4719	 , 2195	 , 2744	  , 1612   , 3430   , 1591	  },//14
        new int[] {5737	 , 2700	 , 3375	  , 2035   , 4218   , 1957	  },//15
        new int[] {6881	 , 3276	 , 4096	  , 2535   , 5120   , 2457	  },//16
        new int[] {8155	 , 3930	 , 4913	  , 3120   , 6141   , 3046	  },//17
        new int[] {9564	 , 4665	 , 5832	  , 3798   , 7290   , 3732	  },//18
        new int[] {11111 , 5487	 , 6859	  , 4575   , 8573   , 4526	  },//19
        new int[] {12800 , 6400	 , 8000	  , 5460   , 10000  , 5440	  },//20
        new int[] {14632 , 7408	 , 9261	  , 6458   , 11576  , 6482	  },//21
        new int[] {16610 , 8518	 , 10648  , 7577   , 13310  , 7666	  },//22
        new int[] {18737 , 9733	 , 12167  , 8825   , 15208  , 9003	  },//23
        new int[] {21012 , 11059 , 13824  , 10208  , 17280  , 10506	  },//24
        new int[] {23437 , 12500 , 15625  , 11735  , 19531  , 12187	  },//25
        new int[] {26012 , 14060 , 17576  , 13411  , 21970  , 14060	  },//26
        new int[] {28737 , 15746 , 19683  , 15244  , 24603  , 16140	  },//27
        new int[] {31610 , 17561 , 21952  , 17242  , 27440  , 18439	  },//28
        new int[] {34632 , 19511 , 24389  , 19411  , 30486  , 20974	  },//29
        new int[] {37800 , 21600 , 27000  , 21760  , 33750  , 23760	  },//30
        new int[] {41111 , 23832 , 29791  , 24294  , 37238  , 26811	  },//31
        new int[] {44564 , 26214 , 32768  , 27021  , 40960  , 30146	  },//32
        new int[] {48155 , 28749 , 35937  , 29949  , 44921  , 33780	  },//33
        new int[] {51881 , 31443 , 39304  , 33084  , 49130  , 37731	  },//34
        new int[] {55737 , 34300 , 42875  , 36435  , 53593  , 42017	  },//35
        new int[] {59719 , 37324 , 46656  , 40007  , 58320  , 46656	  },//36
        new int[] {63822 , 40522 , 50653  , 43808  , 63316  , 50653	  },//37
        new int[] {68041 , 43897 , 54872  , 47846  , 68590  , 55969	  },//38
        new int[] {72369 , 47455 , 59319  , 52127  , 74148  , 60505	  },//39
        new int[] {76800 , 51200 , 64000  , 56660  , 80000  , 66560	  },//40
        new int[] {81326 , 55136 , 68921  , 61450  , 86151  , 71677	  },//41
        new int[] {85942 , 59270 , 74088  , 66505  , 92610  , 78533	  },//42
        new int[] {90637 , 63605 , 79507  , 71833  , 99383  , 84277	  },//43
        new int[] {95406 , 68147 , 85184  , 77440  , 106480 , 91998	  },//44
        new int[] {100237, 72900 , 91125  , 83335  , 113906 , 98415	  },//45
        new int[] {105122, 77868 , 97336  , 89523  , 121670 , 107069  },//46
        new int[] {110052, 83058 , 103823 , 96012  , 129778 , 114205  },//47
        new int[] {115015, 88473 , 110592 , 102810 , 138240 , 123863  },//48
        new int[] {120001, 94119 , 117649 , 109923 , 147061 , 131766  },//49
        new int[] {125000, 100000, 125000 , 117360 , 156250 , 142500  },//50
        new int[] {131324, 106120, 132651 , 125126 , 165813 , 151222  },//51
        new int[] {137795, 112486, 140608 , 133229 , 175760 , 163105  },//52
        new int[] {144410, 119101, 148877 , 141677 , 186096 , 172697  },//53
        new int[] {151165, 125971, 157464 , 150476 , 196830 , 185807  },//54
        new int[] {158056, 133100, 166375 , 159635 , 207968 , 196322  },//55
        new int[] {165079, 140492, 175616 , 169159 , 219520 , 210739  },//56
        new int[] {172229, 148154, 185193 , 179056 , 231491 , 222231  },//57
        new int[] {179503, 156089, 195112 , 189334 , 243890 , 238036  },//58
        new int[] {186894, 164303, 205379 , 199999 , 256723 , 250562  },//59
        new int[] {194400, 172800, 216000 , 211060 , 270000 , 267840  },//60
        new int[] {202013, 181584, 226981 , 222522 , 283726 , 281456  },//61
        new int[] {209728, 190662, 238328 , 234393 , 297910 , 300293  },//62
        new int[] {217540, 200037, 250047 , 246681 , 312558 , 315059  },//63
        new int[] {225443, 209715, 262144 , 259392 , 327680 , 335544  },//64
        new int[] {233431, 219700, 274625 , 272535 , 343281 , 351520  },//65
        new int[] {241496, 229996, 287496 , 286115 , 359370 , 373744  },//66
        new int[] {249633, 240610, 300763 , 300140 , 375953 , 390991  },//67
        new int[] {257834, 251545, 314432 , 314618 , 393040 , 415050  },//68
        new int[] {267406, 262807, 328509 , 329555 , 410636 , 433631  },//69
        new int[] {276458, 274400, 343000 , 344960 , 428750 , 459620  },//70
        new int[] {286328, 286328, 357911 , 360838 , 447388 , 479600  },//71
        new int[] {296358, 298598, 373248 , 377197 , 466560 , 507617  },//72
        new int[] {305767, 311213, 389017 , 394045 , 486271 , 529063  },//73
        new int[] {316074, 324179, 405224 , 411388 , 506530 , 559209  },//74
        new int[] {326531, 337500, 421875 , 429235 , 527343 , 582187  },//75
        new int[] {336255, 351180, 438976 , 447591 , 548720 , 614566  },//76
        new int[] {346965, 365226, 456533 , 466464 , 570666 , 639146  },//77
        new int[] {357812, 379641, 474552 , 485862 , 593190 , 673863  },//78
        new int[] {367807, 394431, 493039 , 505791 , 616298 , 700115  },//79
        new int[] {378880, 409600, 512000 , 526260 , 640000 , 737280  },//80
        new int[] {390077, 425152, 531441 , 547274 , 664301 , 765275  },//81
        new int[] {400293, 441094, 551368 , 568841 , 689210 , 804997  },//82
        new int[] {411686, 457429, 571787 , 590969 , 714733 , 834809  },//83
        new int[] {423190, 474163, 592704 , 613664 , 740880 , 877201  },//84
        new int[] {433572, 491300, 614125 , 636935 , 767656 , 908905  },//85
        new int[] {445239, 508844, 636056 , 660787 , 795070 , 954084  },//86
        new int[] {457001, 526802, 658503 , 685228 , 823128 , 987754  },//87
        new int[] {467489, 545177, 681472 , 710266 , 851840 , 1035837 },//88
        new int[] {479378, 563975, 704969 , 735907 , 881211 , 1071552 },//89
        new int[] {491346, 583200, 729000 , 762160 , 911250 , 1122660 },//90
        new int[] {501878, 602856, 753571 , 789030 , 941963 , 1160499 },//91
        new int[] {513934, 622950, 778688 , 816525 , 973360 , 1214753 },//92
        new int[] {526049, 643485, 804357 , 844653 , 1005446, 1254796 },//93
        new int[] {536557, 664467, 830584 , 873420 , 1038230, 1312322 },//94
        new int[] {548720, 685900, 857375 , 902835 , 1071718, 1354652 },//95
        new int[] {560922, 707788, 884736 , 932903 , 1105920, 1415577 },//96
        new int[] {571333, 730138, 912673 , 963632 , 1140841, 1460276 },//97
        new int[] {583539, 752953, 941192 , 995030 , 1176490, 1524731 },//98
        new int[] {591882, 776239, 970299 , 1027103, 1212873, 1571884 },//99
        new int[] {600000, 800000, 1000000, 1059860, 1250000, 1640000 } //100
    };
}