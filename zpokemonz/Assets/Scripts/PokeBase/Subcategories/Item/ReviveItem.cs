using UnityEngine;
public class ReviveItem : ItemBase
{
    [Header("复活")]
    [SerializeField] bool max;

    public override bool Use(Pokemon pokemon)
    {
        if(pokemon.isFainted)
        {
            return true;
        }
        return false;
    }

    public override string UseForPokemon(Pokemon pokemon)
    {
        if(max)
        {
            pokemon.UpdateHP(pokemon.MaxHP, UpdateHpVoiceType.Cure);
            pokemon.CureAllStatus();
            return pokemon.NickName + "恢复了!";
        }
        else
        {
            //Math.Ceiling向上取整 Math.Floor向下取整
            pokemon.UpdateHP((int)(pokemon.MaxHP * 0.5f), UpdateHpVoiceType.Cure);
            pokemon.CureAllStatus();
            return pokemon.NickName + "恢复了一些!";
        }
    }
}