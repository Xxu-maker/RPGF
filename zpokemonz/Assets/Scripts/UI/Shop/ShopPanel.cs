using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopPanel : BasePanel
{
    [SerializeField] GameObject scroll;

    [SerializeField] Text moneyText;//玩家持有金币Text
    [SerializeField] Text descriptionText;//物品细节Text

    [SerializeField] List<ShopBox> boxes;//Scroll单格
    [SerializeField] CanvasGroup buyPanelCG;//购买弹窗
    [SerializeField] InputField inputField;//购买输入框
    [SerializeField] Slider slider;//购买时的Slider

    private List<ItemBase> items;
    private int itemPos;//选择的item位置
    private Action[] shopSelectionPanel = null;
    private string[] shopSelectionBtnText = new string[] { "购买", "卖出", "取消" };

    private void Start()
    {
        scroll.transform.rotation = Quaternion.identity;
        if(shopSelectionPanel == null)
        {
            shopSelectionPanel = new Action[]
            {
                SetData,
                UIManager.Instance.ResumeControl,
                UIManager.Instance.ResumeControl
            };
        }
    }

    /// <summary>
    /// 商店选择操作页
    /// </summary>
    public void ShopSelection(ref List<ItemBase> itemsBase)
    {
        UIManager.Instance.CloseControl();

        items = itemsBase;

        UIManager.Instance.SelectionColumn.Set(shopSelectionPanel, shopSelectionBtnText, SelectionColumnPosType.TopRightCorner);
    }

    /// <summary>
    /// 设置Scroll
    /// </summary>
    private void SetData()
    {
        for(int i = 0; i < 10; ++i)
        {
            boxes[i].SetData(items[i]);
        }
        moneyText.text = GameManager.Instance.Inventory.Money.ToString();
        OnOpen();
    }

    /// <summary>
    /// 点击购买物后弹窗
    /// </summary>
    public void GetValue(int n)
    {
        itemPos = n;
        descriptionText.text = items[itemPos].Description;

        //Slider范围
        slider.minValue = 0;
        float money = GameManager.Instance.Inventory.Money;
        int price = items[itemPos].Price;
        //算100个比价格 最多一次买100个
        slider.maxValue = price * 100f > money? (money == 0f || money < price? 0f : money / price) : 100f;
        if(slider.value != 0f)
        {
            slider.value = 0f;
            inputField.text = null;
        }

        ShowOrHide(buyPanelCG, true);
    }

    /// <summary>
    /// 确认购买
    /// </summary>
    public void Purchases()
    {
        if(slider.value != 0)
        {
            GameManager.Instance.Inventory.BuyItems(new ItemSlot(items[itemPos], (int) slider.value));
        }
        Cancel();
        moneyText.text = GameManager.Instance.Inventory.Money.ToString();
    }

    public void Cancel()
    {
        ShowOrHide(buyPanelCG, false);
    }

    /// <summary>
    /// 当slider值改变时设置输入框值
    /// </summary>
    public void SetText()
    {
        inputField.text = slider.value.ToString();
    }

    /// <summary>
    /// 当输入文本改变时设置slider值
    /// </summary>
    public void SetSliderValue()
    {
        slider.value = int.Parse(inputField.text);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIManager.Instance.ResumeControl();
    }
}