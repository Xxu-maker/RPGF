using UnityEngine;
[CreateAssetMenu(menuName = "宝可梦道具/创建普通无特殊信息道具")]
public class ItemBase : ScriptableObject
{
    [Header("基础信息")]
    [SerializeField] string itemName;
    [SerializeField] Sprite itemSprite;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] ItemType itemType;
    [SerializeField] int price;
    [Tooltip("战斗中一次性消耗道具")]
    [SerializeField] bool isDisposableItem;
    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public string Description => description;
    public ItemType Type => itemType;
    public int Price => price;
    public bool IsDisposableItem => isDisposableItem;

    [Tooltip("球种编号, 薄荷性格float索引, 其它道具排序")]
    [SerializeField] int id;
    public int ID => id;

    [SerializeField] ItemsUseRules itemsUseRules;
    public bool UseInBattle  => itemsUseRules == ItemsUseRules.Battle  || itemsUseRules == ItemsUseRules.All;
    public bool UseInFreedom => itemsUseRules == ItemsUseRules.Freedom || itemsUseRules == ItemsUseRules.All;
    [SerializeField] bool isBerry;
    public bool IsBerry => isBerry;

    /// <summary>
    /// 检查宝可梦能否使用该道具
    /// </summary>
    /// <param name="pokemon">要检查的宝可梦</param>
    /// <returns>返回bool值</returns>
    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    public virtual string UseForPokemon(Pokemon pokemon)
    {
        return null;
    }
}
public enum ItemType
{
    Ball, Mint, Medicine, AddPP, BasePoint, Condition, Revive, PPMaximum, TM, KeyItem,
    Fossil, Treasure, BattleItem, Evolution, Ability, Mega, Z
}
public enum Classification
{
    Normal, Medicine, Berry, Ball, TM, MegaAZ, Key
}
public enum ItemsUseRules{ All, Battle, Freedom, None }