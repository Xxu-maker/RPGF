using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;


public enum ETextEffect
{
    None,
    [LabelText("逐步")]
    Step,
    [LabelText("淡入")]
    Fade,
    [LabelText("尾随渐入")]
    FollowFade

}

public enum EDialogType
{
    [LabelText("对话文本")]
    DialogBox = 0,
    [LabelText("黑底提示")]
    Black = 1,
 

}

public enum EBlackType
{
    [LabelText("背景黑幕")]
    Black = 0,
    [LabelText("背景透明")]
    Alpha = 1,
}


public class UINovelsPanel : SerializedMonoBehaviour
{
    public static UINovelsPanel Instance;

    [TabGroup("界面元素-对话框")]
    [LabelText("对话框动画")]
    public Animator animDialog;
    
    [TabGroup("界面元素-对话框")]
    [LabelText("左侧")]
    public Animator animLeft;
    [TabGroup("界面元素-对话框")]
    [LabelText("右侧")]
    public Animator animRight;
    [TabGroup("界面元素-对话框")]
    [LabelText("中间")]
    public Animator animCenter;
    
    [TabGroup("界面元素-对话框")]
    public Image imageCharaCenter;
    [TabGroup("界面元素-对话框")]
    public Image imageCharaLeft;
    [TabGroup("界面元素-对话框")]
    public Image imageCharaRight;
    
    [TabGroup("界面元素-对话框")]
    public UIImageSwitch imageSwitchDialogBg;
    
    [TabGroup("界面元素-对话框")]
    public AdvancedText textDialog;
    [TabGroup("界面元素-对话框")]
    public Image imageNextStepTip;
    [TabGroup("界面元素-对话框")]
    public Transform transCharaNameLeft;
  
    [TabGroup("界面元素-对话框")]
    public TextMeshProUGUI textCharaNameLeft;
    
    [TabGroup("界面元素-对话框")]
    public GameObject Title;


    [TabGroup("界面元素-黑屏")]
    public UIImageSwitch imageSwitchBlackBg;
    [TabGroup("界面元素-黑屏")]
    public Text textContent;
    [TabGroup("界面元素-黑屏")]
    public CanvasGroup canvasGroupBlack;
    [TabGroup("界面元素-黑屏")]
    public Image choiceMask;

    [TabGroup("界面元素-功能按钮")]
    [LabelText("控制组")]
    public ToggleGroup[] controlGroupList;
    [TabGroup("界面元素-功能按钮")]
    [LabelText("控制组")]
    public ToggleGroup controlGroup;
    [TabGroup("界面元素-功能按钮")]
    [LabelText("快进按钮")]
    public Toggle buttonQuick;
    public Image buttonQuickTag;
    [TabGroup("界面元素-功能按钮")]
    [LabelText("自动按钮")]
    public Toggle buttonAuto;
    public Image buttonAutoTag;
    // [TabGroup("界面元素-功能按钮")] 
    // [LabelText("跳过按钮")]
    // public Toggle buttonSkip;
    [TabGroup("界面元素-功能按钮")]
    [LabelText("回忆按钮")]
    public Toggle buttonReCall;
    // [TabGroup("界面元素-功能按钮")] 
    // [LabelText("菜单按钮")]
    // public Toggle buttonMenu;
    // [TabGroup("界面元素-功能按钮")] 
    // [LabelText("隐藏按钮")]
    // public Toggle buttonEyes;
    [TabGroup("界面元素-功能按钮")]
    [LabelText("未完待续界面跳过按钮")]
    public Button toBeContinued;
    [FormerlySerializedAs("buttonSelect")]
    [TabGroup("界面元素-功能按钮")]
    [AssetSelector(FlattenTreeView = false)]
    [LabelText("选项按钮组")]
    public List<Button> buttonSelectGroup;

    [LabelText("阻断监听按钮组")]
    public List<Button> buttonIsolateGroup;

    [TabGroup("事件层")]
    [LabelText("选项")]
    public Transform Choice;

    [TabGroup("事件层")]
    public GameObject mask;
    
    [TabGroup("事件层")]
    [LabelText("人物spine动画挂点")]
    public Transform Spine_Root;
    


    [TabGroup("数据层")]
    public float fadeTime = 0.3f;
    [TabGroup("数据层")]
    private bool IsShow => isBlackShow || isDialogShow;
    

    [TabGroup("数据层")]
    public bool isBlackShow = false;
    [TabGroup("数据层")]
    public bool isDialogShow = false;
    
    public Image bgImage;
    
    public enum EShowType
    {
        None,
        [LabelText("对话框")]
        Dialog = 1,
        [LabelText("黑屏")]
        BlackScreen = 2,
    }

    
    [TabGroup("数据层")]
    public EShowType currentShowType = EShowType.None;
    [TabGroup("数据层")]
    public Dictionary<ShowCharaSet.CharaShowData.EPosType, ShowCharaSet.CharaShowData> _currentShowDict = new Dictionary<ShowCharaSet.CharaShowData.EPosType, ShowCharaSet.CharaShowData>()
    {
        { ShowCharaSet.CharaShowData.EPosType.Left, new ShowCharaSet.CharaShowData() },
        { ShowCharaSet.CharaShowData.EPosType.Right, new ShowCharaSet.CharaShowData() },
        { ShowCharaSet.CharaShowData.EPosType.Center, new ShowCharaSet.CharaShowData() },
    };


    [TabGroup("数据层")]
    public ShowCharaSet.CharaShowData.EPosType lastShowPos = ShowCharaSet.CharaShowData.EPosType.None;
    
    public static readonly int State = Animator.StringToHash("State");
    public static readonly int Show = Animator.StringToHash("IsShow");

    private void Awake()
    {
        Instance = this;

        Clear();
        //普通对话框自动快进按钮监听事件
        buttonQuick.onValueChanged.AddListener((bool value) =>
        {
            //Todo:播放 关闭效果音
            //AudioManager.Instance.PlayCommandClose();
            if (value)
            {
                NovelsManager.Instance.BtnIsOn = true;
                NovelsManager.Instance.IsAcceptConfirm = true;
                SaveManager.Instance.Cfg.ForceTextWait = GlobalConfig.Instance.ForceTextWait;
                if (SaveManager.Instance.Cfg.isHaveRead == 0)
                {
                    Time.timeScale = NovelsManager.Instance.IsFast ? (float)SaveManager.Instance.Cfg.GameSpeed : 1;
                }
                else
                {
                    Time.timeScale = (float)SaveManager.Instance.Cfg.GameSpeed;
                }
                SaveManager.Instance.Cfg.IsSkip = true;
                ChangeFunctionBtnState(buttonQuick, value, buttonQuickTag);
            }
            else
            {
                NovelsManager.Instance.BtnIsOn = false;
                Time.timeScale = 1;
                SaveManager.Instance.Cfg.IsSkip = false;
                ChangeFunctionBtnState(buttonQuick, value, buttonQuickTag);
            }
            //GameData.Instance.IsFast = value;
        });
        buttonAuto.onValueChanged.AddListener((bool value) =>
        {
            //AudioManager.Instance.PlayCommandClose();
            if (value)
            {
                NovelsManager.Instance.IsAcceptConfirm = true;
                SaveManager.Instance.Cfg.ForceTextWait = SaveManager.Instance.Cfg.AutoReadWaitTime;
                SaveManager.Instance.Cfg.IsSkip = true;
                ChangeFunctionBtnState(buttonAuto, value, buttonAutoTag);

            }
            else
            {
                NovelsManager.Instance.IsAcceptConfirm = false;
                SaveManager.Instance.Cfg.ForceTextWait = GlobalConfig.Instance.ForceTextWait;
                SaveManager.Instance.Cfg.IsSkip = false;
                ChangeFunctionBtnState(buttonAuto, value, buttonAutoTag);
            }
            //GameData.Instance.IsAuto=value;
            Time.timeScale = 1;
        });
        
        //设置回忆按钮监听
        SetRecallBtnListen(buttonReCall.gameObject);
        
        //阻断监听按钮事件
        foreach (var item in buttonIsolateGroup)
        {

            InputListenerManager.RegisterInputEvent(item.gameObject, new InputCallback()
            {
                ClickCallBack = () =>
                {
                    Debug.Log("阻断下一步监听");
                }
            }, InputListenerManager.PriorityType.UITigger);

        }
        //添加选项按钮效果
        foreach (var item in buttonSelectGroup)
        {
            UIButtonTextChange.ColorChange(item.gameObject, ButtonType.CHOICE);
        }

        InputListenerManager.RegisterInputEvent(toBeContinued.gameObject, new InputCallback()
        {
            ClickCallBack = () =>
            {
                NovelsManager.Instance.IsSkipBottonConfirm = true;
            }
        }, InputListenerManager.PriorityType.UITigger);
        var input = new InputCallback()
        {
            ClickCallBack = () =>
            {
                Invoke("NextStep", 0.1f);
                ResetAvgBtn();
            }
        };
        InputListenerManager.RegisterInputEvent(typeof(UIConfirmBlock), input, InputListenerManager.PriorityType.UI);
        //EventCenter.AddEventListener(Notification_Type.HaveRead, ChangeGameSpeed);


    }
   
    /// <summary>
    /// 回忆按钮添加监听
    /// </summary>
    /// <param name="btnObj"></param>
    private void SetRecallBtnListen(GameObject btnObj)
    {
        InputListenerManager.RegisterInputEvent(btnObj, new InputCallback()
        {

            ClickCallBack = () =>
            {
                //AudioManager.Instance.PlayCommandClose();
                ResetAvgBtn();
                //Todo   打开回忆界面
                UIRecallPanel.Instance.OnOpen();
            }
        }, InputListenerManager.PriorityType.UITigger);
    }
    public void TouchDeplay()
    {
        //若当前为自动状态，不立刻切换下一句
        if (SaveManager.Instance.Cfg.IsSkip)
        {
            NovelsManager.Instance.IsAcceptConfirm = false;
        }
        else NovelsManager.Instance.IsAcceptConfirm = true;
        UINovelsPanel.Instance.buttonIsolateGroup[3].gameObject.SetActive(false);
    }
    //下一步监听
    public void NextStep()
    {
        if (NovelsManager.Instance.IsBiYan == false)
        {
            UINovelsPanel.Instance.buttonIsolateGroup[3].gameObject.SetActive(true);
            //NovelsManager.Instance.IsAcceptConfirm = true;
            Invoke("TouchDeplay", 0.06f);
        }
        else
        {
            NovelsManager.Instance.IsBiYan = false;
            animDialog.SetBool(UINovelsPanel.Show, true);
            controlGroup.gameObject.SetActive(true);
           
            Choice.gameObject.SetActive(true);
        }
    }

    //初始化功能按钮状态
    public void ResetAvgBtn()
    {
        Time.timeScale = 1;
        buttonQuick.isOn = false;
        buttonAuto.isOn = false;
       
        buttonQuick.OnDeselect(null);
        buttonAuto.OnDeselect(null);
        buttonQuick.gameObject.GetComponent<Animator>().Play("Normal");
        buttonAuto.gameObject.GetComponent<Animator>().Play("Normal");
       
        SaveManager.Instance.Cfg.IsSkip = false;
        SaveManager.Instance.Cfg.ForceTextWait = GlobalConfig.Instance.ForceTextWait;
    }
    private void OnDestory()
    {
        Instance = null;
        InputListenerManager.UnInputRegister(typeof(UIConfirmBlock));
       
    }
    public void Clear()
    {
        textDialog.text = "";
        textContent.text = "";
        
        controlGroup.gameObject.SetActive(false);

    }
    //设置文本内容
    public void SetContent(EShowType tp, string str)
    {
        textDialog.text = null;
        switch (tp)
        {
            case EShowType.Dialog:
                textDialog.text = str;
                break;
            case EShowType.BlackScreen:
                textDialog.text = str;
                break;
        }
    }
    /*闭眼特效（清屏）*/
    public void clearScene()
    {
        //avg模块
        if (gameObject.activeInHierarchy)
        {
            NovelsManager.Instance.IsBiYan = true;
            //animDialog.gameObject.SetActive(false);
            animDialog.SetBool(UINovelsPanel.Show, false);
            controlGroup.gameObject.SetActive(false);
            Choice.gameObject.SetActive(false);
            UINovelsPanel.Instance.buttonIsolateGroup[2].gameObject.SetActive(false);
           
        }
        
    }
    /*文字特效*/
    public IEnumerator TextFadeIn(EShowType tp, float fadeTimes)
    {
        switch (tp)
        {
            case EShowType.Dialog:
                {
                    var col = textDialog.color;
                    col.a = 0;
                    textDialog.color = col;
                    textDialog.DOFade(1, fadeTime);
                }
                break;
            case EShowType.BlackScreen:
                {
                    var col = textContent.color;
                    col.a = 0;
                    textContent.color = col;
                    textContent.DOFade(1, fadeTime);
                }
                break;
        }

        yield return new WaitForSeconds(fadeTime);
    }

    /*文字特效*/
    public IEnumerator TextFollowFade(EShowType tp, string content, float speed)
    {
        switch (tp)
        {
            case EShowType.Dialog:
                {
                    textDialog.characterName = textCharaNameLeft.text;
                    yield return textDialog.ShowTextByTyping(content);
                }
                break;
            case EShowType.BlackScreen:
                {
                    yield return textDialog.ShowTextByTyping(content);
                }
                break;
          
        }

        yield return new WaitForSeconds(fadeTime);
    }
  
    
    private void _SetBlack(bool isShow, float fadeTimes)
    {
        canvasGroupBlack.DOFade(isShow ? 1 : 0, fadeTime);
    }

    public void ClearSelect()
    {
        foreach (var btn in buttonSelectGroup)
        {
            btn.onClick.RemoveAllListeners();
            btn.gameObject.SetActive(false);
        }

    }

    public IEnumerator BlackEnter(EBlackType tp = EBlackType.Black, float fadeTimes = -1)
    {

        if (-1.0 == fadeTimes)
        {
            fadeTimes = this.fadeTime;
        }
        imageSwitchBlackBg.SetImage((int)tp);
        _SetBlack(true, fadeTimes);
        isBlackShow = true;
        yield return new WaitForSeconds(fadeTimes);
    }

    public IEnumerator BlackLeave(float fadeTimes = -1)
    {
        if (fadeTimes == -1.0)
        {
            fadeTimes = this.fadeTime;
        }

        if (!string.IsNullOrEmpty(textContent.text))
        {
            textContent.DOFade(0, fadeTimes);
            yield return new WaitForSeconds(fadeTimes);
        }

        _SetBlack(false, fadeTimes);
        isBlackShow = false;
        yield return new WaitForSeconds(fadeTimes);
        textContent.text = "";
        textContent.color = Color.white;
    }
    public void DialogAVG(EDialogType tp = EDialogType.DialogBox)
    {
        imageNextStepTip.gameObject.SetActive(false);
      
      
        if (tp == EDialogType.DialogBox)
        {
            imageSwitchDialogBg.SetImage(0);
            controlGroup = controlGroupList[0];
            buttonIsolateGroup[3].gameObject.SetActive(false);
            buttonIsolateGroup[1].gameObject.SetActive(true);
        }
        if (tp == EDialogType.Black)
        {
            imageSwitchDialogBg.SetImage(2);
        }
        
        
        isDialogShow = true;
        animDialog.SetBool(Show, true);
        //显示功能按钮
        controlGroup.gameObject.SetActive(true);
        //UIManager.openPanel("UI_Main_R");
    }
    /// <summary>
    /// 切换对话框
    /// </summary>
    /// <param name="tp"></param>
    /// <returns></returns>
    public IEnumerator DialogEnter(EDialogType tp = EDialogType.DialogBox)
    {
        //buttonIsolateGroup[2].gameObject.SetActive(false);//阻断关闭
        imageNextStepTip.gameObject.SetActive(false);
       
        if (tp == EDialogType.DialogBox)
        {
            imageSwitchDialogBg.SetImage(0);
            controlGroup = controlGroupList[0];
            buttonIsolateGroup[3].gameObject.SetActive(false);
            buttonIsolateGroup[1].gameObject.SetActive(true);
        }
        if (tp == EDialogType.Black)
        {
            imageSwitchDialogBg.SetImage(2);
        }
        
        isDialogShow = true;
        animDialog.SetBool(Show, true);
        //显示功能按钮
        controlGroup.gameObject.SetActive(true);
        yield break;
    }

    public IEnumerator DialogLeave()
    {
        //isDialogShow = false;
        animDialog.SetBool(Show, false);
        controlGroup.gameObject.SetActive(false);
        yield return new WaitForSeconds(fadeTime);
        NovelsManager.Instance.CacheContent = "";
        Clear();
        //buttonIsolateGroup[2].gameObject.SetActive(true);//开启阻断
    }
    /// <summary>
    /// 隐藏舞台上所有角色
    /// </summary>
    /// <returns></returns>
    public IEnumerator ClearStageRole()
    {
        yield return new WaitForSeconds(fadeTime);
        foreach (var anim in Spine_Root.GetComponentsInChildren<Transform>())
        {

            anim.gameObject.SetActive(false);
        }
        Spine_Root.gameObject.SetActive(true);
    }
    

    public void ChangeFunctionBtnState(Toggle toggle, bool isOn, Image btnTag = null)
    {
        if (btnTag != null)
        {
            if (isOn)
            {
                toggle.OnSelect(null);
                toggle.GetComponent<Animator>().Play("Selected");
                btnTag.sprite = Resources.Load<Sprite>("Texture/UI/dialog/Dia_imgTitleBg_small的副本_sel");
                btnTag.transform.Find("Text").GetComponent<Text>().color = new Color32(254, 246, 225, 255);
            }
            else
            {
                toggle.OnDeselect(null);
                toggle.GetComponent<Animator>().Play("Highlighted");
                btnTag.sprite = Resources.Load<Sprite>("Texture/UI/dialog/Dia_imgTitleBg_small的副本");
                btnTag.transform.Find("Text").GetComponent<Text>().color = new Color32(146, 45, 4, 204);
            }
        }
        else
        {
            if (isOn)
            {
                toggle.OnSelect(null);
                toggle.GetComponent<Animator>().Play("Selected");
            }
            else
            {
                toggle.OnDeselect(null);
                toggle.GetComponent<Animator>().Play("Highlighted");
            }
        }

    }
    private void Update()
    {
        //ToggleGrounp(buttonAuto, buttonQuick);
        //ToggleGrounp(cg_ButtonAuto, cg_ButtonQuick);
        //ChangeGameSpeed();
    }
    private void ChangeGameSpeed()
    {
        if (buttonQuick.isOn)
        {
            Time.timeScale = NovelsManager.Instance.IsFast ? (float)SaveManager.Instance.Cfg.GameSpeed : 1;
        }
    }

    public bool isFast()
    {
        if (buttonQuick.IsActive())
        {
            return buttonQuick.isOn;
        }
       
        return false;
    }

    public void LateUpdate()
    {
        //Todo 空格下一句
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsShow)
            {
                NovelsManager.Instance.IsAcceptConfirm = true;
            }
        }

        
    }

    //显示背景图
    public void Show2DBackground(Sprite spriteData)
    {
        bgImage.sprite = spriteData;
    }
    
    //隐藏背景图
    public void Hide2DBackground()
    {
        //bgImage.sprite = spriteData;
    }
    
    public void FreshSprite()
    {
        foreach (var info in _currentShowDict)
        {

            if (info.Value.RoleSprite==null)
            {

                continue;
            }
            

            Image setImg = null;
            switch (info.Key)
            {
                case ShowCharaSet.CharaShowData.EPosType.Left:
                    {
                        setImg = imageCharaLeft;
                        if (info.Value.IsFlipX)
                        {
                            setImg.transform.localScale = new Vector3(-1, 1, 1);
                        }
                        else
                        {
                            setImg.transform.localScale = new Vector3(1, 1, 1);
                        }

                        var isOpenName = !string.IsNullOrEmpty(info.Value.Name);
                        transCharaNameLeft.gameObject.SetActive(isOpenName);
                        if (isOpenName)
                        {
                            textCharaNameLeft.text = info.Value.Name;
                        }
                    }


                    break;
                case ShowCharaSet.CharaShowData.EPosType.Right:
                    {
                        setImg = imageCharaRight;
                        if (info.Value.IsFlipX)
                        {
                            setImg.transform.localScale = new Vector3(1, 1, 1);
                        }
                        else
                        {
                            setImg.transform.localScale = new Vector3(-1, 1, 1);
                        }

                        var isOpenName = !string.IsNullOrEmpty(info.Value.Name);
                        transCharaNameLeft.gameObject.SetActive(isOpenName);
                        if (isOpenName)
                        {
                            textCharaNameLeft.text = info.Value.Name;
                        }
                    }
                    break;
                case ShowCharaSet.CharaShowData.EPosType.Center:
                    setImg = imageCharaCenter;
                    break;
                case ShowCharaSet.CharaShowData.EPosType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (setImg != null)
            {
               
                setImg.sprite = info.Value.RoleSprite;
                setImg.rectTransform.sizeDelta = info.Value.TextureSize;
            }
        }
    }
    
    
    public void SetCharaClose(ShowCharaSet.CharaShowData.EPosType posType)
    {
        _currentShowDict[posType].RoleSprite = null;
        _currentShowDict[posType].State = ShowCharaSet.CharaShowData.EEffType.Close;
        switch (posType)
        {
            case ShowCharaSet.CharaShowData.EPosType.Left:
                animLeft.SetInteger(State, (int)ShowCharaSet.CharaShowData.EEffType.Close);
                break;
            case ShowCharaSet.CharaShowData.EPosType.Right:
                animRight.SetInteger(State, (int)ShowCharaSet.CharaShowData.EEffType.Close);
                break;
            case ShowCharaSet.CharaShowData.EPosType.Center:
                animCenter.SetInteger(State, (int)ShowCharaSet.CharaShowData.EEffType.Close);
                break;
        }
    }
}