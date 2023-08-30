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
[LabelText("背景图")]
public class SetBackground : INovelsSet
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

[Serializable]
[LabelText("人物立绘")]
[InlineProperty(LabelWidth = 100)]
public  class ShowCharaSet : INovelsSet
{
    [TabGroup("基础设置")]
    [LabelText("图片效果")]
    public CharaShowData.EEffType State = CharaShowData.EEffType.Show;
    
    [TabGroup("基础设置")]
    [LabelText("立绘纹理")]
    [HideIf("@State== CharaShowData.EEffType.Close")]
    public Sprite image;
    
    [TabGroup("基础设置")]
    [LabelText("立绘名字")] 
    [HideIf("@State== CharaShowData.EEffType.Close")]
    public string name;

    [TabGroup("立绘设置")]
    [LabelText("位置")]
    public CharaShowData.EPosType PosType;
    [TabGroup("立绘设置")]
    [LabelText("X翻转")]
    public bool IsFlipX = false;
   
    [TabGroup("立绘设置")]
    [LabelText("立绘大小")]
    public Vector2 Size=new Vector2(640,720);
    
    

    public IEnumerator Run()
    {
        //Todo 负值给UI字典
        UINovelsPanel.Instance.lastShowPos = PosType;
        UINovelsPanel.Instance._currentShowDict[PosType].RoleSprite = image;
        UINovelsPanel.Instance._currentShowDict[PosType].State = State;
        UINovelsPanel.Instance._currentShowDict[PosType].IsFlipX = IsFlipX;
        UINovelsPanel.Instance._currentShowDict[PosType].Name = name;
        UINovelsPanel.Instance._currentShowDict[PosType].TextureSize = Size;
            
        
        switch (PosType)
        {
            case CharaShowData.EPosType.Left:
                if (UINovelsPanel.Instance._currentShowDict[CharaShowData.EPosType.Right].State == CharaShowData.EEffType.Show)
                {
                    UINovelsPanel.Instance._currentShowDict[CharaShowData.EPosType.Right].State = CharaShowData.EEffType.Dark;
                    UINovelsPanel.Instance.animRight.SetInteger(UINovelsPanel.State, (int)CharaShowData.EEffType.Dark);

                }

                UINovelsPanel.Instance.animLeft.SetInteger(UINovelsPanel.State, (int)State);

                break;
            case CharaShowData.EPosType.Right:
                if (UINovelsPanel.Instance._currentShowDict[CharaShowData.EPosType.Left].State == CharaShowData.EEffType.Show)
                {
                    UINovelsPanel.Instance._currentShowDict[CharaShowData.EPosType.Left].State = CharaShowData.EEffType.Dark;
                    UINovelsPanel.Instance.animLeft.SetInteger(UINovelsPanel.State, (int)CharaShowData.EEffType.Dark);
                }

                UINovelsPanel.Instance.animRight.SetInteger(UINovelsPanel.State, (int)State);
                break;
            case CharaShowData.EPosType.Center:
                UINovelsPanel.Instance.SetCharaClose(CharaShowData.EPosType.Left);
                UINovelsPanel.Instance.SetCharaClose(CharaShowData.EPosType.Right);
                UINovelsPanel.Instance.animCenter.SetInteger(UINovelsPanel.State, (int)State);
                break;
        }

        
        UINovelsPanel.Instance.imageSwitchDialogBg.SetImage(PosType == CharaShowData.EPosType.Left ? 0 : 1);
        

        UINovelsPanel.Instance.FreshSprite();
        

    
        yield return new WaitForSeconds(0.3f);

    }
    
    
    public class CharaShowData
    {
        
        public enum EEffType
        {
            [LabelText("关闭")]
            Close = 0,
            [LabelText("显示")]
            Show = 1,
            [LabelText("压暗")]
            Dark = 2,
        }

        public enum EPosType
        {
            None = -1,
            [LabelText("左侧")]
            Left = 0,
            [LabelText("右侧")]
            Right = 1,
            [LabelText("居中")]
            Center = 2,
        }
        

        public bool IsFlipX = false;

        public Sprite RoleSprite = null;

        public string Name;

        public EEffType State;
        public Vector2 TextureSize { get; set; }
    }


}

#endregion