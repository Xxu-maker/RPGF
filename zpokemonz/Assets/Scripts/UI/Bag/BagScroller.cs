using UnityEngine;
using System.Collections.Generic;
using ZUI.ZScroller;
using ZUI.BagToggle;
using UnityEngine.UI;
namespace ZUI.BagScroll
{
    public class BagScroller : BasePanel, IScrollDelegate
    {
        [SerializeField] Scroller scroller;
        [SerializeField] Text descriptionText;
        [SerializeField] ScrollCellView cellViewPrefab;
        [SerializeField] BagToggleGroup bagToggleGroup;

        private ItemHandler itemHandler;
        private List<ItemSlot> _data;
        private Inventory playerInventory;
        private InventoryType currentBagType = InventoryType.None;
        public InventoryType CurrentBagType => currentBagType;
        private bool isOpen = false;

        void Start()
        {
            scroller.Delegate = this;
            playerInventory = GameManager.Instance.Inventory;
            itemHandler = UIManager.Instance.ItemHandler;

            itemHandler.OnShowDescription += ShowDescription;
            itemHandler.OnRefreshCellView += RefreshBagCellView;
        }

        public void LoadData(InventoryType loadBagType)
        {
            if(isOpen && currentBagType == loadBagType)
            {
                return;
            }
            currentBagType = loadBagType;

            //获取当前背包物品
            _data = playerInventory.GetInventory(loadBagType);

            if(descriptionText.text != null)
            {
                descriptionText.text = null;
            }

            scroller.ReloadData();
        }

    #region EnhancedScroller Handlers

        public int GetNumberOfCells() => _data.Count;

        public ScrollCellView GetCellView(Scroller scroller, int dataIndex)
        {
            BagCellView cellView = scroller.GetCellView(cellViewPrefab) as BagCellView;
            cellView.SetData(_data[dataIndex], CellButtonClicked);
            return cellView;
        }

    #endregion
    #region CellView相关
        /// <summary>
        /// CellView点击后delegate
        /// </summary>
        public void CellButtonClicked(int cellIndex)
        {
            //Debug.Log($"Clicked! cellIndex:{cellIndex}");
            ItemBase itemBase = playerInventory.GetInventory(currentBagType)[cellIndex].Base;
            descriptionText.text = itemBase.Description;
            itemHandler.OpenTipPanel(itemBase, cellIndex);
        }

        /// <summary>
        /// 物品简介显示
        /// </summary>
        /// <param name="str"></param>
        public void ShowDescription(string str)
        {
            descriptionText.text = str;
        }

        /// <summary>
        /// 刷新BagUI格子
        /// </summary>
        /// <param name="i"></param>
        public void RefreshBagCellView(int cellIndex, bool usedUp)
        {
            if(usedUp)
            {
                //用完了
                scroller.RefreshList(cellIndex);
            }
            else
            {
                //普通刷新
                scroller.RefreshCellView(cellIndex, _data[cellIndex].Count);
            }
        }
    #endregion
    #region 显隐
        public override void OnOpen()
        {
            base.OnOpen();
            isOpen = true;
            scroller.OnAddListener();
            LoadData(InventoryType.NormalItem);
            bagToggleGroup.SwitchCurrentToggle(0);
        }

        public override void OnClose()
        {
            base.OnClose();
            isOpen = false;

            scroller.OnRemoveListener();
            UIManager.Instance.BackCtrlPanel();
            bagToggleGroup.Reset();
        }
    #endregion
    }
}