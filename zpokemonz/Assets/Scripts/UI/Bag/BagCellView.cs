using UnityEngine;
using UnityEngine.UI;
using ZUI.ZScroller;
namespace ZUI.BagScroll
{
    public delegate void BagCellClickedDelegate(int cellIndex);
    public class BagCellView : ScrollCellView
    {
        [SerializeField] Image itemImage;
        [SerializeField] Text nameText;
        [SerializeField] Text itemNumberText;
        public void SetData(ItemSlot item, BagCellClickedDelegate _delegate)
        {
            if(bagCellClickedDelegate == null)
            {
                bagCellClickedDelegate = _delegate;
            }
            itemImage.sprite = item.Base.ItemSprite;
            nameText.text = item.Base.ItemName;
            itemNumberText.text = item.Count.ToString();
        }

        public override void RefreshHoldNum(int hold)
        {
            itemNumberText.text = hold.ToString();
        }

        private BagCellClickedDelegate bagCellClickedDelegate;
        /// <summary>
        /// 点击事件
        /// </summary>
        public void CellButton_OnClick()
        {
            bagCellClickedDelegate(cellIndex);
        }
    }
}