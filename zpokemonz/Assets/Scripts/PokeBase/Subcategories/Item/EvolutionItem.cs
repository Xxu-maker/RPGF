using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建新进化道具")]
public class EvolutionItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return base.Use(pokemon);
    }
}