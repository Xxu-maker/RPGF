using Novels;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks.Triggers;

public interface INovelsSet 
{
    IEnumerator Run();
}

#region 基础组件
[Serializable]
[LabelText("黑屏")]
public class BlackScreen : INovelsSet
{
    [LabelText("是否开启")]
    public bool IsShow;
    [LabelText("过渡时间")]
    public float FadeTime;

    public IEnumerator Run()
    {
        if (IsShow)
        {
            yield return UINovelsPanel.Instance.BlackEnter( EBlackType.Black,FadeTime);
        }
        else
        {
            yield return UINovelsPanel.Instance.BlackLeave( FadeTime);
        }
    }
}


[Serializable]
[LabelText("延迟")]
public class Delay : INovelsSet
{
    [LabelText("延迟时间")]
    public float DelayTime = 0;

    public IEnumerator Run()
    {
        yield return new WaitForSeconds(DelayTime);
    }
}


[Serializable]
[LabelText("返回游戏主场景")]
public class BackGameScene : INovelsSet
{
    public IEnumerator Run()
    {
        yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, 1f);

        //if (GameManager.Instance.CurrentScene != null)
        //{
            //GameHelper.Recycle(AVGManager.Instance.CurrentScene.gameObject);
        //}

        AVGManager.Instance.EndAVGScene();
    }
}

[Serializable]
[LabelText("跳转下一个剧情片段")]
public class JumpNextAVGPlot : INovelsSet
{
    [LabelText("跳转剧情片段")]
    public GlobalConfig.NovelsChapterData StartNovelName;
    
    public IEnumerator Run()
    {
        yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, 1f);


        NovelsManager.Instance.StopMainCoroutine();
        NovelsManager.Instance.Install(StartNovelName.SectionNodes,0);
    }
}


[Serializable]
[LabelText("音效/BGM")]
public class AudioSet: INovelsSet
{
    [LabelText("设置BGM")]
    public bool IsSetBgm = false;
    [LabelText("背景音乐")]
    [ShowIf(@"IsSetBgm")]
    public AudioClipData Bgm = new AudioClipData();
    [LabelText("音效列表")]
    public List<AudioClipData> AudioDatas = new List<AudioClipData>();

    public IEnumerator Run()
    {
        if (IsSetBgm)
        {
            if (Bgm == null || string.IsNullOrEmpty(Bgm.AudioRes))
            {
                AudioManager.Instance.SetBgmFadeOut(1);
            }
            else
            {
                if (AudioManager.Instance.CurrentBgm != Bgm.AudioRes)
                {
                    AudioManager.Instance.SetBgmFadeInOut(Bgm, 3);
                }
            }
        }

        if (AudioDatas != null)
        {
            for (int i = 0; i < AudioDatas.Count; i++)
            {
                AudioManager.Instance.PlaySound(AudioDatas[i], AudioGroupType.Effect);
            }
        }
        yield break;
    }

}


[Serializable]
[LabelText("显示/隐藏2D背景图")]
public class ShowHide2DBackground : INovelsSet
{
    [LabelText("是否显示")]
    public bool IsShow;
    [LabelText("背景图")]
    [ShowIf("@IsShow")]
    public Sprite SpriteData;

    public IEnumerator Run()
    {
        if (IsShow)
        {
            UINovelsPanel.Instance.Show2DBackground(SpriteData);
        }
        else
        {
            UINovelsPanel.Instance.Hide2DBackground();
        }
        yield break;
    }
}



#endregion