using UnityEngine;
using UnityEngine.UI;
public class ShopBox : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text nameText;
    [SerializeField] Text unitPriceText;

    public void SetData(ItemBase itemBase)
    {
        icon.sprite = itemBase.ItemSprite;
        nameText.text = itemBase.ItemName;
        unitPriceText.text = itemBase.Price.ToString();
    }
}