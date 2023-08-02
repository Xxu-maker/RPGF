using System.Collections.Generic;
public enum InventoryType {NormalItem, Ball, TM, Berry, MegaAndZ, Medicine, KeyItem, None}
public class Inventory : ZSavable
{
    [UnityEngine.SerializeField] float money;
    public float Money => money;
    private void MoneyValueChange(float value) { money = UnityEngine.Mathf.Clamp(money + value, 0, 9000000f);}
    public void BuyItems(ItemSlot itemSlot) { MoneyValueChange(-(itemSlot.Base.Price * itemSlot.Count)); LayInItem(itemSlot); }

    [UnityEngine.SerializeField] Bag[] bags;
    private Dictionary<InventoryType, List<ItemSlot>> inventory = new Dictionary<InventoryType, List<ItemSlot>>();

    void Start()
    {
        inventory.Add(InventoryType.NormalItem, bags[0].Items);
        inventory.Add(InventoryType.Ball, bags[1].Items);
        inventory.Add(InventoryType.Medicine, bags[2].Items);
        inventory.Add(InventoryType.Berry, bags[3].Items);
        inventory.Add(InventoryType.TM, bags[4].Items);
        inventory.Add(InventoryType.KeyItem, bags[5].Items);
        inventory.Add(InventoryType.MegaAndZ, bags[6].Items);
    }

    public List<ItemSlot> GetInventory(InventoryType bagType)
    {
        return inventory[bagType];
    }

    /// <summary>
    /// 获得道具(单个slot)
    /// </summary>
    /// <param name="item"></param>
    public void LayInItem(ItemSlot itemSlot)
    {
        if(itemSlot.Base == null)
        {
            return;
        }

        InventoryType currentBag = InventoryType.NormalItem;
        ItemType type = itemSlot.Base.Type;
        switch(type)
        {
            case ItemType.Ball    :         currentBag = InventoryType.Ball     ; break;

            case ItemType.Medicine:
                                        if(itemSlot.Base.IsBerry)
                                        {
                                            currentBag = InventoryType.Berry    ;
                                        }
                                        else
                                        {
                                            currentBag = InventoryType.Medicine ;
                                        }                                          break;

            case ItemType.TM     :          currentBag = InventoryType.TM       ; break;

            case ItemType.KeyItem:          currentBag = InventoryType.KeyItem  ; break;

            case ItemType.Mega   :          currentBag = InventoryType.MegaAndZ ; break;
        }

        int bagPos = CheckBagHaveItem(itemSlot.Base, currentBag);
        if(bagPos == -1)
        {
            inventory[currentBag].Add(new ItemSlot(itemSlot.Base, itemSlot.Count > 0? itemSlot.Count : 1));
        }
        else
        {
            inventory[currentBag][bagPos].Get(itemSlot.Count > 0? itemSlot.Count : 1);
        }
    }

    /// <summary>
    /// 获得道具(list)
    /// </summary>
    public void LayInItemList(List<ItemSlot> items)
    {
        foreach(ItemSlot item in items)
        {
            LayInItem(item);
        }
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <returns>返回是否用完了</returns>
    public bool UseItem(InventoryType type, int pos, int numOfUses = 1)
    {
        ItemSlot itemSlot = inventory[type][pos];
        itemSlot.Use(numOfUses);
        if(itemSlot.Count == 0)
        {
            inventory[type].Remove(itemSlot);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 检查道具是否存在
    /// </summary>
    /// <param name="checkBase">需要检查的道具</param>
    /// <param name="inventoryType">哪个背包</param>
    /// <returns>-1即为不存在, 其它值为在该背包中的位置</returns>
    private int CheckBagHaveItem(ItemBase checkBase, InventoryType inventoryType)
    {
        int bagCount = inventory[inventoryType].Count;
        for(int i = 0; i < bagCount; ++i)
        {
            if(checkBase == inventory[inventoryType][i].Base)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 检查对应位置道具是否用完
    /// </summary>
    public bool IsItemUsedUp(InventoryType type, int pos)
    {
        return inventory[type][pos].Count == 0;
    }

#region 数据存储
    public override object CaptureState()
	{
        //得和宝可梦一样的存法
        Dictionary<InventoryType, (int, int)[]> z = new Dictionary<InventoryType, (int, int)[]>();
		return inventory;
	}

	public override void RestoreState(object state)
	{
		inventory = (Dictionary<InventoryType, List<ItemSlot>>) state;
	}
#endregion
}