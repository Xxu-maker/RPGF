using System.Collections.Generic;
using UnityEngine;
public class AllItem
{
    static Dictionary<int, ItemBase> items;
    private static bool AlreadyLoad;
    public static void Init()
    {
        if(AlreadyLoad)
        {
            return;
        }
        items = new Dictionary<int, ItemBase>();
        ItemBase[] itemsArray = ResM.Instance.LoadAll<ItemBase>("SkillSO/");
        foreach(ItemBase item in itemsArray)
        {
            int id = item.ID;
            if(items.ContainsKey(id))
            {
                Debug.LogError($"有两个技能的ID相同{id}");
                continue;
            }
            items[id] = item;
        }
        AlreadyLoad = true;
    }
    public static ItemBase GetPokemonByID(int id)
    {
        if(!items.ContainsKey(id))
        {
            Debug.LogError($"没有查询到Base{id}");
            return null;
        }
        return items[id];
    }
}
