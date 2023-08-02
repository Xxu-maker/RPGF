using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦特性/创建出场特性")]
public class AppearancesAbility : AbilityBase
{
    [SerializeField] Ability_AppearancesAbilityType appearancesAbilityType;

    [Header("出场天气")]
    [SerializeField] WeatherType weatherType;
    [Header("出场降低对手增益")]
    [SerializeField] StatBoost[] statBoosts;

    /// <summary>
    /// 检查出场增益变化
    /// </summary>
    public string CheckAppearancesEvent(Pokemon pokemon, out WeatherType weather)
    {
        weather = weatherType;

        if(appearancesAbilityType == Ability_AppearancesAbilityType.Weather)
        {
            return $"因为{base.Name}改变了天气!";
        }
        else if(appearancesAbilityType == Ability_AppearancesAbilityType.降低对手增益)
        {
            pokemon.ApplyBoosts(statBoosts);
            return $"因为{base.Name}{statBoosts[0].Stat.ToString()}{(statBoosts[0].Boost < 0? "降低" : "升高")}了!";
        }

        return null;
    }
}
public enum Ability_AppearancesAbilityType{ Weather, 降低对手增益}
public enum WeatherType { 正常, 晴朗, 下雨, 冰雹, 沙暴, 始源之海, 终结之地, 德尔塔气流, 天气失效 }