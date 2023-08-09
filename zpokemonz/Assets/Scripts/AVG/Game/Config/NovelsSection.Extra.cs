using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Novels
{
    [LabelText("黑屏")]
    [Serializable]
    public class 黑屏文本: IContent
    {
        [TabGroup("展示内容")]
        [TextArea]
        [LabelText("内容文本")]
        public string Str;
        [TabGroup("功能设置")]
        [LabelText("点击下一步")]
        public bool IsNeedClick = true;
        [TabGroup("功能设置")]
        [LabelText("结束后关闭")]
        public bool IsClose = false;
        [TabGroup("功能设置")]
        [LabelText("进入时间")]
        public float EnterFade = 0.5f;
        [TabGroup("功能设置")]
        [LabelText("停留时间")]
        public float StayTime = 1;
        [TabGroup("功能设置")]
        [LabelText("结束时间")]
        public float LeaveFade = 0.5f;

        [TabGroup("文本设置")]
        [LabelText("字体出现效果")]
        public ETextEffect TextEffect = ETextEffect.Fade;
        [TabGroup("文本设置")]
        [LabelText("效果时间")]
        public float EffectTime = 0.5f;
        [TabGroup("文本设置")]
        [LabelText("背景类型")]
        public EBlackType BGType = EBlackType.Black;

        public string GetStr()
        {
            return Str;
        }

        EContentType IContent.GetType()
        {
            return EContentType.Black;
        }


#if UNITY_EDITOR
        [Button("添加新节点")]
        public void AddNewContent()
        {
            var item = UnityEditor.Selection.GetFiltered<SceneItem>(UnityEditor.SelectionMode.Unfiltered);

            if (item.Length > 0)
            {
                var sceneItem = item[0] as SceneItem;
                for (int i = 0; i < sceneItem.TriggerPlot.EventNodes.Length; i++)
                {
                    var nodeData = sceneItem.TriggerPlot.EventNodes[i] as NovelsNodeData;
                    if (nodeData != null && nodeData.Contents.Contains(this))
                    {
                        nodeData.AddNewContent(this, MemberwiseClone());
                        return;
                    }
                }
            }

            var section = UnityEditor.Selection.GetFiltered<NovelsSectionData>(UnityEditor.SelectionMode.Unfiltered);

            if (section.Length > 0)
            {
                var data = section[0] as NovelsSectionData;

                for (int i = 0; i < data.EventNodes.Length; i++)
                {
                    var nodeData = data.EventNodes[i] as NovelsNodeData;
                    if (nodeData != null && nodeData.Contents.Contains(this))
                    {
                        nodeData.AddNewContent(this, MemberwiseClone());
                        return;
                    }
                }
            }

            for (int i = 0; i < NovelsConfig.Instance.Chapters.Count; i++)
            {
                for (int j = 0; j < NovelsConfig.Instance.Chapters[i].SectionNodes.Count; j++)
                {
                    var config = Resources.Load<NovelsSectionData>(NovelsConfig.Instance.Chapters[i].SectionNodes[j]);
                    for (int k = 0; k < config.EventNodes.Length; k++)
                    {
                        var nodeData = config.EventNodes[k] as NovelsNodeData;

                        if (nodeData != null && nodeData.Contents.Contains(this))
                        {
                            nodeData.AddNewContent(this, MemberwiseClone());
                            return;
                        }
                    }
                }
            }

        }
#endif
    }
    
}
