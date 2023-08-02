using UnityEngine;
[System.Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;
    public ItemBase Base => item;
    public int Count => count;

    public ItemSlot(ItemBase _base, int n)
    {
        item = _base;
        count = n;
    }

    public void Get(int x)
    {
        count += x;
    }

    public void Use(int x)
    {
        count -= x;
    }
}