using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "宝可梦",menuName = "背包/创建新背包")]
[PreferBinarySerialization]
/// <summary>
/// 方便测试放东西的背包
/// 真背包详见↓ 1 reference Inventory
/// </summary>
public class Bag : ScriptableObject
{
    [SerializeField] List<ItemSlot> items = new List<ItemSlot>();
    public List<ItemSlot> Items => items;
}