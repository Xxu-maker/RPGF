using System.Collections.Generic;
using UnityEngine;
public class FosterYard : MonoBehaviour
{
    [SerializeField] List<RuleMove> move;//地图宝可梦随机移动路径列表
    [SerializeField] NPCCtrller prefab;//宝可梦预制体
    [SerializeField] List<NPCCtrller> pokemonPool;
    [SerializeField] List<GameObject> gameObjectPool;
    public void SetPokemon(Pokemon[] pokemons)
    {
        //测试
        if(gameObjectPool != null && gameObjectPool.Count == 4)
        {
            return;
        }

        int length = pokemons.Length;
        for(int i = 0; i < length; ++i)
        {
            if(pokemons[i].Base != null)
            {
                prefab.SetBaseData
                (
                    "斗也的" + pokemons[i].NickName,
                    pokemons[i].Base.ID.ToString(),
                    move[i],
                    pokemons[i].Shiny? "s" : null
                );
                GameObject clone = Instantiate(prefab.gameObject, move[i].position, Quaternion.identity);
                gameObjectPool.Add(clone);
                clone.SetActive(true);
            }
        }
    }
}

[System.Serializable]
/// <summary>
/// 移动路径
/// </summary>
public class RuleMove
{
    public Dialog dialog;
    public Vector2 position;
    public List<Vector2> move;
    public float intervalTime;
}