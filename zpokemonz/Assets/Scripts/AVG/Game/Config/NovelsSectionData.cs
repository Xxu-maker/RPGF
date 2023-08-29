using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Novels;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ExcelData;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Playables;
using Text = UnityEngine.UI.Text;

namespace Novels
{

    public interface INovelsNode
    {
        IEnumerator Run();
    }


    public interface IContent
    {
        EContentType GetType();
        string GetStr();
        
        //回忆数据
        void ReCall();
    }

    public enum EContentType
    {
        None = -1,
        Black,
        Dialog,
        Audio,
        Select,
        Event,
        Popup
    }

    [LabelText("黑屏")]
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public class BlackScreenContent : IContent
    {
        [TabGroup("展示内容")] [TextArea] [LabelText("内容文本")]
        public string Str;

        [TabGroup("功能设置")] [LabelText("点击下一步")]
        public bool IsNeedClick = true;

        [TabGroup("功能设置")] [LabelText("结束后关闭")]
        public bool IsClose = false;

        [TabGroup("功能设置")] [LabelText("进入时间")] public float EnterFade = 0.5f;
        [TabGroup("功能设置")] [LabelText("停留时间")] public float StayTime = 1;
        [TabGroup("功能设置")] [LabelText("结束时间")] public float LeaveFade = 0.5f;

        [TabGroup("文本设置")] [LabelText("字体出现效果")]
        public ETextEffect TextEffect = ETextEffect.Fade;

        [TabGroup("文本设置")] [LabelText("效果时间")] public float EffectTime = 0.5f;
        [TabGroup("文本设置")] [LabelText("背景类型")] public EBlackType BGType = EBlackType.Black;

        public string GetStr()
        {
            return Str;
        }

        public void ReCall()
        {
            
        }

        EContentType IContent.GetType()
        {
            return EContentType.Black;
        }



    }


    [LabelText("对话")]
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public class DialogueContent : IContent
    {
        
        //[TabGroup("展示内容")] [LabelText("名字显示")] public string CharaName;
        
        [TabGroup("展示内容")] [TextArea] [ValueDropdown("_dropItemList")][LabelText("内容文本")][InlineButton("FreshItem")][OnValueChanged("ChangeValue")]
        public string Str;
        
        [TabGroup("展示内容")] [LabelText("简体")] [ReadOnly]
        public string CN;
        [TabGroup("展示内容")] [LabelText("繁体")] [ReadOnly]
        public string HK;
        [TabGroup("展示内容")] [LabelText("英文")] [ReadOnly]
        public string EN;
        [TabGroup("展示内容")] [LabelText("日文")] [ReadOnly]
        public string JP;
        
        [TabGroup("文本设置")] [LabelText("字体出现效果")]
        public ETextEffect TextEffect = ETextEffect.FollowFade;

        [TabGroup("文本设置")] [LabelText("效果时间")] public float EffectTime = 0;

        [TabGroup("文本设置")] [LabelText("离开等待时间")]
        public float LeaveWait = 0;
        

        [TabGroup("功能设置")] [LabelText("对话框类型")]
        public EDialogType DialogBoxType = EDialogType.DialogBox;

        [TabGroup("功能设置")] [LabelText("换行显示")] public bool IsNextLine = true;

        [TabGroup("功能设置")] [LabelText("点击下一步")]
        public bool IsNeedClick = true;

        [TabGroup("功能设置")] [LabelText("进入清理文本")]
        public bool IsClearCache = true;

        [TabGroup("功能设置")] [LabelText("结束后关闭对话框")]
        public bool IsCloseDialog = false;
        

        IEnumerable<string> _dropItemList;

        
#if UNITY_EDITOR
        private void FreshItem()
        {
            var DicLanguages = AssetDatabase.LoadAssetAtPath<GameTableSO>("Assets/Resources/Config/GameTable.asset");
            var dicLanguages = DicLanguages.languages.ToDictionary(Key => Key.key, Value => Value);
            _dropItemList = dicLanguages.Select(o => o.Key);
        }
        
        private void ChangeValue()
        {
            var DicLanguages = AssetDatabase.LoadAssetAtPath<GameTableSO>("Assets/Resources/Config/GameTable.asset");
            var dicLanguages = DicLanguages.languages.ToDictionary(Key => Key.key, Value => Value);
            if (dicLanguages.ContainsKey(Str))
            {
                CN = dicLanguages[Str].CN;
                HK= dicLanguages[Str].HK;
                EN= dicLanguages[Str].EN;
                JP= dicLanguages[Str].JP;
            }
            
        }
#endif

        public string GetStr()
        {
            return Str;
        }

        public void ReCall()
        {
            RecallConfig data = new RecallConfig();
            data.Datas.Add(GetStr());
            data.type = (int)EContentType.Dialog;
            //data.roleName = CharaName;
            
            if (!SaveManager.Instance.Cfg.MemoryData.Contains(data))
            {
                SaveManager.Instance.Cfg.MemoryData.Add(data);

            }
        }

        EContentType IContent.GetType()
        {
            return EContentType.Dialog;
        }





    }

    [LabelText("音效")]
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public class SEAudioContent : IContent
    {


        [LabelText("等待音效结束")] public bool IsWait = false;
        [LabelText("音效配置")] public AudioClipData Data;

        public string GetStr()
        {
            return "";
        }

        public void ReCall()
        {
            throw new NotImplementedException();
        }

        EContentType IContent.GetType()
        {
            return EContentType.Audio;
        }
    }

    [LabelText("事件")]
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public class EventContent : IContent
    {
        [Title("事件文本")] [GUIColor("@Color.red")] [ValueDropdown("_dropItemList")] [InlineButton("FreshItem")]
        public string Str;

        //IEnumerable<string> _dropItemList = GlobalConfig.Instance.EventMap.Select(o => o.Key);

        public void FreshItem()
        {
            //_dropItemList = GlobalConfig.Instance.EventMap.Select(o => o.Key);
        }

        public string GetStr()
        {
            return Str;
        }

        public void ReCall()
        {
           
        }

        EContentType IContent.GetType()
        {
            return EContentType.Event;
        }
    }

    [LabelText("弹窗")]
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public class PopupContent : IContent
    {
        [LabelText("弹窗数据")] public PopupWindowData Data;

        public string GetStr()
        {
            return "";
        }

        public void ReCall()
        {
            
        }

        EContentType IContent.GetType()
        {
            return EContentType.Popup;
        }

    }

    [LabelText("选项")]
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public class SelectContent : IContent
    {
       
        [TabGroup("内容设置")]
        [LabelText("选项按钮数据")]
        [InlineProperty(LabelWidth = 100)]
        public List<ButtonSelectData> Datas;
        

        [TabGroup("功能设置")]
        [LabelText("选项按钮样式")]
        public EButtonType type = EButtonType.Normal;

        [TabGroup("功能设置")]
        [LabelText("选择后清理舞台(对话框和人)")] public bool IsClearStage;

        [Flags]
        public enum EButtonType
        {
            [LabelText("通用")]
            Normal,
            
        }
        
        [HideInInspector]
        [LabelText("当前选项下标")]
        public int BtnIndex;
        
        public EContentType GetType()
        {
            return EContentType.Select;
        }

        public string GetStr()
        {
            return "";
        }

        public void ReCall()
        {
            RecallConfig data = new RecallConfig();
            data.type = (int)EContentType.Select;
            data.CurrentSelect = BtnIndex;
            foreach (var item in Datas)
            {
                data.Datas.Add(item.Content);
            }

            SaveManager.Instance.Cfg.MemoryData.Add(data);
        }


        public class ButtonSelectData
        {
            [LabelText("选项显示文本")]
           
            public string Content;
            [LabelText("片段名")]
            [ResourcePath(typeof(NovelsSectionData))]
            public string SectionName;
            
        }
    
    }

    [Title("文本节点(保存回忆数据，需要点击才能继续)")]
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public class NovelsNodeData : INovelsNode
    {
        [TabGroup("文本段落")] public IContent Content;


        public IEnumerator Run()
        {
            while (NovelsManager.Instance == null || UINovelsPanel.Instance == null)
            {
                yield return null;
            }

            var cacheContent = NovelsManager.Instance.CacheContent;

            var lastType = EContentType.None;

            var currentType = Content.GetType();

            //全屏文本或文本内容类型切换时清理上一次的文本
            if (currentType == EContentType.Black || (lastType != EContentType.None && currentType != lastType))
            {
                NovelsManager.Instance.CacheContent = cacheContent = "";
                UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
            }

            lastType = currentType;

            var str = Content.GetStr() + "";

            switch (currentType)
            {
                //通用文本框逻辑
                case EContentType.Dialog:
                {
                    var dialogContent = Content as DialogueContent;
                    if (dialogContent == null)
                    {
                        //Todo 异常处理
                        yield break;
                    }

                    //文本框进入
                    yield return UINovelsPanel.Instance.DialogEnter(dialogContent.DialogBoxType);


                    //清理文本
                    NovelsManager.Instance.CacheContent = cacheContent = "";
                    UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);


                    if (dialogContent.IsNextLine)
                    {
                        if (!string.IsNullOrEmpty(cacheContent))
                        {
                            cacheContent += "\n";
                        }
                    }

                    //文本特效
                    switch (dialogContent.TextEffect)
                    {
                        case ETextEffect.None:
                        {
                            cacheContent += str;
                            UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                        }
                            break;
                        case ETextEffect.Step:
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                var charArr = str.ToCharArray();
                                var charIndex = 0;
                                while (charIndex < charArr.Length)
                                {
                                    UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog,
                                        cacheContent + str.Substring(0, charIndex));
                                    charIndex += 1;
                                    //AudioManager.Instance.PlaySound(GlobalConfig.Instance.SFX_TextAppear,AudioGroupType.Effect);
                                    if (dialogContent.EffectTime != 0)
                                    {
                                        yield return new WaitForSeconds(dialogContent.EffectTime);
                                    }
                                    else
                                    {
                                        yield return new WaitForSeconds((float)SaveManager.Instance.Cfg.CharSpeed);
                                    }
                                }

                                cacheContent += str;
                                UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                            }
                        }
                            break;
                        case ETextEffect.Fade:
                            cacheContent += str;
                            UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                            yield return UINovelsPanel.Instance.TextFadeIn(UINovelsPanel.EShowType.Dialog,
                                dialogContent.EffectTime);
                            break;
                        case ETextEffect.FollowFade:
                            cacheContent += str;
                            UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.Dialog, cacheContent);
                            yield return UINovelsPanel.Instance.TextFollowFade(UINovelsPanel.EShowType.Dialog, cacheContent, (float)SaveManager.Instance.Cfg.CharSpeed);
                            break;
                    }


                    //点击下一步
                    if (dialogContent != null && dialogContent.IsNeedClick && !SaveManager.Instance.Cfg.IsSkip)
                    {
                        NovelsManager.Instance.IsAcceptConfirm = false;
                        UINovelsPanel.Instance.imageNextStepTip.gameObject.SetActive(true);

                        while (true)
                        {
                            if (NovelsManager.Instance.IsAcceptConfirm)
                            {
                                NovelsManager.Instance.IsAcceptConfirm = false;
                                UINovelsPanel.Instance.imageNextStepTip.gameObject.SetActive(false);
                                break;
                            }

                            yield return null;
                        }
                    }

                    var tmpWait = dialogContent.LeaveWait;
                    if (tmpWait == 0)
                    {
                        tmpWait = (float)SaveManager.Instance.Cfg.ForceTextWait / 1000;
                    }

                    yield return new WaitForSeconds(tmpWait);

                    //关闭对话框
                    if (dialogContent.IsCloseDialog)
                    {
                        yield return UINovelsPanel.Instance.DialogLeave();
                    }
                    
                }
                    break;
                //黑屏逻辑
                case EContentType.Black:
                {
                    var blackContent = Content as BlackScreenContent;
                    if (blackContent == null)
                    {
                        //Todo 异常处理
                        yield break;
                    }

                    yield return UINovelsPanel.Instance.BlackEnter(blackContent.BGType, blackContent.EnterFade);

                    switch (blackContent.TextEffect)
                    {
                        case ETextEffect.None:
                        {
                            cacheContent += str;
                            UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.BlackScreen, cacheContent);
                        }
                            break;
                        case ETextEffect.Step:
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                var charArr = str.ToCharArray();
                                var charIndex = 0;
                                while (charIndex < charArr.Length)
                                {
                                    UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.BlackScreen,
                                        str.Substring(0, charIndex));
                                    charIndex += 1;
                                    AudioManager.Instance.PlaySound(GlobalConfig.Instance.SFX_TextAppear,
                                        AudioGroupType.Effect);
                                    yield return new WaitForSeconds(
                                        (float)SaveManager.Instance.Cfg.CharSpeed / 1000);
                                }
                            }
                        }
                            break;
                        case ETextEffect.Fade:
                            cacheContent += str;
                            UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.BlackScreen, cacheContent);
                            yield return UINovelsPanel.Instance.TextFadeIn(UINovelsPanel.EShowType.BlackScreen,
                                blackContent.EffectTime);
                            break;
                    }

                    UINovelsPanel.Instance.SetContent(UINovelsPanel.EShowType.BlackScreen, str);

                    if (blackContent.IsNeedClick && !SaveManager.Instance.Cfg.IsSkip)
                    {
                        while (true)
                        {
                            if (NovelsManager.Instance.IsAcceptConfirm)
                            {
                                NovelsManager.Instance.IsAcceptConfirm = false;
                                break;
                            }

                            yield return null;
                        }
                    }

                    if (blackContent.StayTime > 0)
                    {
                        yield return new WaitForSeconds(blackContent.StayTime);
                    }

                    if (blackContent.IsClose)
                    {
                        yield return UINovelsPanel.Instance.BlackLeave(blackContent.LeaveFade);
                    }

                    yield return new WaitForSeconds((float)SaveManager.Instance.Cfg.ForceTextWait / 1000);
                    NovelsManager.Instance.IsAcceptConfirm = false;
                }
                    break;
                //音效逻辑
                case EContentType.Audio:
                {
                    var audioContent = Content as SEAudioContent;
                    var lenth = AudioManager.Instance.PlaySound(audioContent.Data, AudioGroupType.Effect);
                    if (audioContent.IsWait)
                    {
                        yield return new WaitForSeconds(lenth);
                    }
                }
                    break;
                //选项逻辑
                case EContentType.Select:
                    {
                        //关闭快进自动
                        var selectContent = Content as SelectContent;
                        UINovelsPanel.Instance.ResetAvgBtn();
                        UINovelsPanel.Instance.buttonIsolateGroup[2].gameObject.SetActive(true);
                        if (selectContent == null)
                        {
                            break;
                        }
                        switch (selectContent.type)
                        {
                            case SelectContent.EButtonType.Normal:
                                {
                                    UINovelsPanel.Instance.ClearSelect();
                                    UINovelsPanel.Instance.choiceMask.gameObject.SetActive(true);
                                    for (var j = 0; j < selectContent.Datas.Count; j++)
                                    {
                                        var btn = UINovelsPanel.Instance.buttonSelectGroup[j];
                                        btn.transform.Find("Text").GetComponent<Text>().text = selectContent.Datas[j].Content;
                                        btn.gameObject.SetActive(true);
                                         
                                        var index = j;
                                        btn.onClick.AddListener(() =>
                                        {

                                            //添加回忆 选项回忆
                                            selectContent.BtnIndex = index;
                                            selectContent.ReCall();

                                            if (selectContent.IsClearStage == true)
                                            {
                                                UINovelsPanel.Instance.isDialogShow = false;
                                                UINovelsPanel.Instance.animDialog.gameObject.SetActive(false);
                                               
                                                NovelsManager.Instance.CacheContent = "";
                                                UINovelsPanel.Instance.Clear();
                                                UINovelsPanel.Instance.ClearStageRole();
                                            }
                                           
                                            
                                            UINovelsPanel.Instance.ClearSelect();
                                            UINovelsPanel.Instance.choiceMask.gameObject.SetActive(false);
                                            NovelsManager.Instance.Install(selectContent.Datas[index].SectionName, 0);
                                            
                                        });
                                    }
                                    NovelsManager.Instance.OnDestory();
                                }
                                break;

                        }
                    }
                    break;
                //事件逻辑(根据事件触发其它小游戏玩法,暂定)
                case EContentType.Event:
                    
                    break;


            }
        }
        
    }


    [Title("并行节点(不保存数据，完成action后直接结束)")]
    [InlineEditor]
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public class SetNodeData : INovelsNode
    {
        public INovelsSet SetData;

        public IEnumerator Run()
        {
            yield return SetData.Run();
        }

    }

    [LabelText("章节段落")]
    [InlineEditor]
    [CreateAssetMenu(fileName = "NovelsSceneData", menuName = "Game/NovelsSceneData")]
    public class NovelsSectionData : SerializedScriptableObject
    {

        [LabelText("事件列表")] public INovelsNode[] EventNodes = new INovelsNode[0];


        public IEnumerator Run()
        {
            for (int i = 0; i < EventNodes.Length; i++)
            {
                yield return EventNodes[i].Run();
            }
        }

    }
}

