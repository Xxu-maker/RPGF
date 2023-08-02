using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新努力值道具")]
public class BasePointItem : ItemBase
{
    [Header("努力值")]
    [SerializeField] BasePointType basePointType;
    [SerializeField] int num;//num对应BasePointType的值
    [SerializeField] bool add;
    public BasePointType BasePointType => basePointType;

    public override bool Use(Pokemon pokemon)
    {
        if(add)
        {
            if(pokemon.BasePoints[num] < 252 && !pokemon.BasePointsWasMax)
            {
                return true;
            }
            return false;
        }
        else
        {
            if(pokemon.BasePoints[num] != 0 || pokemon.TotalBasePointsValue() != 0)
            {
                return true;
            }
            return false;
        }
    }

    public override string UseForPokemon(Pokemon pokemon)
    {
        pokemon.GetBasePoint(num, add? 10 : -10);
        return "对" + pokemon.NickName + "使用了" + base.ItemName;
    }
}