using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 商店店员
/// </summary>
public class ShopClerk : MonoBehaviour, Interactable
{
    [SerializeField] List<ItemBase> items;

    public void Interact(Transform initiator)
    {
        UIManager.Instance.ShopPanel.ShopSelection(ref items);
    }
}