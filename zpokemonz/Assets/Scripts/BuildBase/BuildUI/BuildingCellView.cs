using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZUI.ZScroller;

public class BuildingCellView : ScrollCellView
{
    public delegate void BagCellClickedDelegate(int cellIndex);
    [SerializeField] Image itemImage;
    [SerializeField] Text nameText;
    [SerializeField] Text itemNumber;

    public void SetData(ItemSlot item, BagCellClickedDelegate _delegate)
    {
        bagCellClickedDelegate = _delegate;
        itemImage.sprite = item.Base.ItemSprite;
    }

    public override void RefreshHoldNum(int hold)
    {
        itemNumber.text = hold.ToString();
    }

    public BagCellClickedDelegate bagCellClickedDelegate;

    /// <summary>
    /// 点击事件
    /// </summary>
    public void CellButton_OnClick()
    {
        bagCellClickedDelegate(cellIndex);
    }
}