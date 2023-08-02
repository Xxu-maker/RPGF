using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新Mega道具")]
public class MegaItem : ItemBase
{
    [Header("Mega")]
    [SerializeField] int megaID;
    public override bool Use(Pokemon pokemon)
    {
        if(pokemon.Base.ID == megaID)
        {
            return true;
        }
        return false;
    }
}