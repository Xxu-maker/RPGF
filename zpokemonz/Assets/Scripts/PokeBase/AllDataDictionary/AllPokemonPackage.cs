using UnityEngine;
[CreateAssetMenu(fileName = "宝可梦存储列表",menuName = "宝可梦/创建新宝可梦列表")]
public class AllPokemonPackage : ScriptableObject
{
    [SerializeField] PokemonBase[] pokemonBases;
    public PokemonBase GetPokemonByID(int id)
    {
        return pokemonBases[id];
    }
    //static Dictionary<int, PokemonBase> pokemons;
    //private static bool AlreadyLoad;
    //public static void Init()
    //{
    //    if(AlreadyLoad)
    //    {
    //        return;
    //    }
    //    pokemons = new Dictionary<int, PokemonBase>();
    //    PokemonBase[] pokemonArray = ResM.Instance.LoadAll<PokemonBase>("PokemonSO/");
    //    foreach(PokemonBase pokemon in pokemonArray)
    //    {
    //        int id = pokemon.ID;
    //        if(pokemons.ContainsKey(id))
    //        {
    //            Debug.LogError($"有两只宝可梦ID相同{id}");
    //            continue;
    //        }
    //        pokemons[id] = pokemon;
    //    }
    //    AlreadyLoad = true;
    //}
    //public static PokemonBase GetPokemonByID(int id)
    //{
    //    if(!pokemons.ContainsKey(id))
    //    {
    //        Debug.LogError($"没有查询到Base{id}");
    //        return null;
    //    }
    //    return pokemons[id];
    //}
}