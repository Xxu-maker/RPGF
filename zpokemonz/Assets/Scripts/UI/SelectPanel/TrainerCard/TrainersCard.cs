using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TrainersCard : BasePanel
{
    [Header("右侧栏Toggle面板")]
    [SerializeField] CanvasGroup toggleListPanel;

    [Header("右下角Toggle")]
    [SerializeField] Toggle bottomRightToggle;

    [Header("右侧Toggle列表")]
    [SerializeField] Toggle[] rightToggles;

    [Header("右侧Toggle原始位置表")]
    [SerializeField] RectTransform[] rightTogglesTrans;

    [Header("右侧Toggle动画目标位置表")]
    [SerializeField] RectTransform[] targetTrans;

    [Header("信息卡设置面板")]
    [SerializeField] CardSettingPanel cardSettingPanel;

    [Header("徽章滚动面板")]
    [SerializeField] CardRobbinPanel cardRobbinPanel;

    [Header("信息卡数据面板")]
    [SerializeField] CardDataPanel cardDataPanel;

    [Header("卡首页信息")]
    [SerializeField] CanvasGroup baseMessageCG;
    [SerializeField] Image playerImage;
    [SerializeField] Text nameText;
    [SerializeField] Text battlePointText;
    [SerializeField] Text timeText;

    private Vector3 originButtonPos;

    void Start()
    {
        bottomRightToggle.onValueChanged.AddListener((bool isOn) => OpenTogglePanel(isOn));
        rightToggles[0].onValueChanged.AddListener((bool isOn) => OpenRobbinsPanel(isOn));
        originButtonPos = rightTogglesTrans[0].localPosition;
    }

    private void OpenTogglePanel(bool isOn)
    {
        ShowOrHide(toggleListPanel, isOn);
        if(isOn)
        {
            rightTogglesTrans[0].DOLocalMoveY(targetTrans[0].localPosition.y, 0.2f);
            rightTogglesTrans[1].DOLocalMoveY(targetTrans[1].localPosition.y, 0.4f);
            rightTogglesTrans[2].DOLocalMoveY(targetTrans[2].localPosition.y, 0.4f);
        }
        else
        {
            rightTogglesTrans[0].localPosition = originButtonPos;
            rightTogglesTrans[1].localPosition = originButtonPos;
            rightTogglesTrans[2].localPosition = originButtonPos;
            if(rightToggles[0].isOn)
            {
                rightToggles[0].isOn = false;
            }
        }
    }

    public void ExitPanel()
    {
        OnClose();
        UIManager.Instance.BackCtrlPanel();
    }

    //Toggle
    private void OpenSettingPanel(bool isOn)
    {
        //
    }
    private void OpenRobbinsPanel(bool isOn)
    {
        if(isOn)
        {
            cardRobbinPanel.OnOpen();
            ShowOrHide(baseMessageCG, false);
        }
        else
        {
            cardRobbinPanel.OnClose();
            ShowOrHide(baseMessageCG, true);
        }
    }
    private void OpenDataPanel(bool isOn)
    {
        //
    }
}