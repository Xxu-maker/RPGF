using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新状态恢复道具")]
public class ConditionItem : ItemBase
{
    [Header("状态恢复")]
    [SerializeField] ConditionID status;
    [Tooltip("全恢复")]
    [SerializeField] bool restoreAll;
    public override bool Use(Pokemon pokemon)
    {
        if(pokemon.Status == null && pokemon.VolatileStatus == null)//临时状态和持久状态
        {
            return false;
        }
        else
        {
            //检查是否能用
            if(restoreAll || status == pokemon.Status.ConditionID || status == pokemon.VolatileStatus.ConditionID)
            {
                return true;
            }
            return false;
        }
    }

    public override string UseForPokemon(Pokemon pokemon)
    {
        AudioManager.Instance.HealPokemon();
        if(restoreAll)
        {
            pokemon.CureAllStatus();
            return pokemon.NickName + "的所有状态都恢复了!";
        }
        else
        {
            //*前面已经检查是否能用且临时和持久状态不重复可不多判断
            if(pokemon.Status == null)//临时状态
            {
                string statusName = pokemon.VolatileStatus.Name;
                pokemon.CureVolatileStatus();
                return $"{pokemon.NickName}的{statusName}状态恢复了!";
            }
            else//持久状态
            {
                string statusName = pokemon.Status.Name;
                pokemon.CureStatus();
                return $"{pokemon.NickName}的{statusName}状态恢复了!";
            }
        }
    }
}