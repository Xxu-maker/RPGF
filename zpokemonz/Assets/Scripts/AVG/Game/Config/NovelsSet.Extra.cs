using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Novels;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;

#region Anim的角色控制管理 目前已经弃用

[Serializable]
[Obsolete]//标记该方法已弃用
public class CurrentControlPlayAnim : INovelsSet
{
    
    public string CharaKey;
    public string AnimName;
    public bool IsLoop;
    public float WaitTime;

    public IEnumerator Run() {

        if (StoryScenePrefab.Instance != null && StoryScenePrefab.Instance.CharaMap.ContainsKey(CharaKey)) 
        {
            StoryScenePrefab.Instance.CharaMap[CharaKey].Play(AnimName, IsLoop);
        }
        yield return new WaitForSeconds(WaitTime);
    }
}


[Serializable]
[Obsolete]//标记该方法已弃用
public class ShowCharaSet : INovelsSet
{
   
    [ValueDropdown("_dropItemList")]
    [LabelText("提示Key")]
    [HideIf("@State== CharaShowData.EEffType.Close")]
    [InlineButton("FreshItem")]
    public string CharaKey;
    [LabelText("位置")]
    public CharaShowData.EPosType PosType;
    [LabelText("X翻转")]
    public bool IsFlipX=false;
    [LabelText("图片效果")]
    public CharaShowData.EEffType State = CharaShowData.EEffType.Show;

    private IEnumerable<string> _dropItemList = GlobalConfig.Instance.NovelsCharas.Select(o => o.Key);

    public void FreshItem()
    {
        _dropItemList = GlobalConfig.Instance.NovelsCharas.Select(o => o.Key);
    }


    public IEnumerator Run()
    {
        yield break;
    }

}

[Serializable]
[Obsolete]//标记该方法已弃用
public class PopupClose : INovelsSet
{
    public IEnumerator Run()
    {
        UIPopupWindow.Instance.CloseAll();
        yield break;
    }
}


[Serializable]
[Obsolete]//标记该方法已弃用
public class CheckItem : INovelsSet
{
    public int CheckCount;

    public IEnumerator Run()
    {
        while (true)
        {
            if (CheckCount <= CharaterData.CollectItemCount)
            {
                break;
            }

            yield return null;
        }
    }
}


[Serializable]
[Obsolete]//标记该方法已弃用
public class SetCharaterControl : INovelsSet
{
    public bool IsActive = false;

    public IEnumerator Run()
    {
        if (CharacterControl.Instance != null) 
        {
            if (IsActive)
            {
                CharacterControl.Instance.State = CharacterControl.EState.Move;
            }
            else 
            {
                CharacterControl.Instance.State = CharacterControl.EState.Stop;
            }
        }

        yield break;
    }
}


[Serializable]
[Obsolete]//标记该方法已弃用
public class CharaExploreActive : INovelsSet
{
    public bool IsActive;

    public IEnumerator Run()
    {
        CharaterData.IsExplore = IsActive;

        if (CharacterControl.Instance!=null)
        {
            if (IsActive)
            {
                CharacterControl.Instance.State = CharacterControl.EState.Move;
            }
            else
            {
                CharacterControl.Instance.State = CharacterControl.EState.Stop;
            }
        }
        yield break;
    }

}

[Serializable]
[Obsolete]//标记该方法已弃用
public class WaitCharaExploreOver : INovelsSet
{
    public IEnumerator Run()
    {
        CharaterData.IsExplore = true;

        if (CharacterControl.Instance != null)
        {
            CharacterControl.Instance.State = CharacterControl.EState.Move;
        }

        while (CharaterData.IsExplore) 
        {
            yield return null;
        }

        if (CharacterControl.Instance!=null)
        {
            CharacterControl.Instance.State = CharacterControl.EState.Stop;
        }
    }
}
#endregion

#region TimeLine 3D剧情使用
[Serializable]
public class OverConditionSet : INovelsSet
{ 
    [LabelText("结束检测")]
    public List<INovelsCheck> OverCheck = new List<INovelsCheck>();

    public virtual IEnumerator Run()
    {
        bool isOver = true;
        do
        {
            isOver = true;
            for (int i = 0; i < OverCheck.Count; i++)
            {
                if (!OverCheck[i].Check())
                {
                    isOver = false;
                }
            }

            if (isOver)
            {
                break;
            }

            yield return null;

        } while (!isOver);
    }
}


[Serializable]
public class PlayTimeline : INovelsSet
{
    [LabelText("加载的剧编")]
    public PlayableDirector Playable;
    [LabelText("加载后播放")]
    [ShowIf(("@Playable!=null"))]
    public bool IsAwakePlay=false;

    public OverConditionSet OverCheck;

    public PlayTimeline()
    {
        OverCheck = new OverConditionSet();
        OverCheck.OverCheck.Add(new PlayableOver());
    }

    public IEnumerator Run()
    {
        if (Playable != null)
        {
            if (NovelsManager.Instance.CurrentPlayable != null)
            {
                GameHelper.Recycle(NovelsManager.Instance.CurrentPlayable.gameObject);
            }

            var obj = GameObject.Instantiate(Playable.gameObject);
            NovelsManager.Instance.CurrentPlayable = obj.GetComponent<PlayableDirector>();
        }

        if (Playable == null || IsAwakePlay)
        {
            NovelsManager.Instance.ContineTimeLine();
        }
 
        if (OverCheck != null)
        {
            yield return OverCheck.Run();
        }
    }
}



[LabelText("加载场景")]
[Serializable]
public class LoadScene : INovelsSet
{
    [LabelText("加载的场景")]
    public string SceneId;
 
    public IEnumerator Run()
    {
        yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, 0);

        if (AVGManager.Instance.CurrentScene != null)
        {
            GameHelper.Recycle(AVGManager.Instance.CurrentScene.gameObject);
        }

        var obj = GameHelper.Alloc<GameObject>("EventScene/Scene_" + SceneId);
        AVGManager.Instance.CurrentScene = obj.GetComponent<StoryScenePrefab>();
        NovelsManager.Instance.CurrentPlayable = AVGManager.Instance.CurrentScene.PlayableDirector;
    }
}

[LabelText("离开场景")]
[Serializable]
public class LeaveScene : INovelsSet
{
    [LabelText("离场时间")]
    public float FadeTime = 0.5f;

    public IEnumerator Run()
    {
        yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, FadeTime);

        if (GameManager.Instance.CurrentScene != null)
        {
            GameHelper.Recycle(AVGManager.Instance.CurrentScene.gameObject);
        }

    }
}

[LabelText("设置当前场景切换舞台")]
[Serializable]
public class SetSceneStage : INovelsSet
{
    public bool IsBlackFade = false;
    [ShowIf("@IsBlackFade")]
    public float FadeTime = 0.5f;

    public ESceneType Stage;

    public IEnumerator Run()
    {
        if (IsBlackFade) 
        {
            yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, FadeTime);
        }

        if (SceneSwitchManager.Instance!=null)
        {
            SceneSwitchManager.Instance.SetScene(Stage);
        }

        if (IsBlackFade)
        {
            yield return UINovelsPanel.Instance.BlackLeave(FadeTime);
        }
        yield break;
    }
}


[LabelText("角色进门")]
[Serializable]
public class SetPlayerEnterDoor: INovelsSet
{
    public IEnumerator Run()
    {
        if (CharacterControl.Instance != null) 
        {
            yield return CharacterControl.Instance.EnterDoor();
        }
    }
}


[Serializable]
public class PlayEvent : INovelsSet
{
    [LabelText("加载的剧编")]
    public PlayableAsset Asset;
    [LabelText("加载后播放")]
    [ShowIf(("@Asset!=null"))]
    public bool IsAwakePlay = false;

    public OverConditionSet OverCheck;

    public PlayEvent()
    {
        OverCheck = new OverConditionSet();
        OverCheck.OverCheck.Add(new PlayableOver());
    }

    public IEnumerator Run()
    {
        if (Asset != null)
        {
            AVGManager.Instance.CurrentScene.PlayableDirector.playableAsset = Asset;
        }

        if (Asset == null || IsAwakePlay)
        {
            NovelsManager.Instance.ContineTimeLine();
        }

        if (OverCheck != null)
        {
            yield return OverCheck.Run();
        }
    }
}


[Serializable]
public class ClearTimeLine: INovelsSet
{
    public IEnumerator Run()
    {
        if (NovelsManager.Instance.CurrentPlayable != null)
        {
            GameHelper.Recycle(NovelsManager.Instance.CurrentPlayable.gameObject);
        }
        yield break;
    }
}

#endregion


#region Spine4.1 剧情角色使用

[Serializable]
[LabelText("预加载Spine资源")]
public class PreLoadSpineAsset: INovelsSet
{
    [LabelText("加载的Spine资源")]
    [ResourcePath(typeof(Spine.Unity.SkeletonDataAsset))]
    public List<String> SpineNodes = new List<string>();
    
    public IEnumerator Run()
    {
        //Todo 黑屏进入
        yield return UINovelsPanel.Instance.BlackEnter(EBlackType.Black, 1f);
        
        foreach (var path in SpineNodes)
        {
            yield return SpineManager.Instance.PreLoadSpine(path);
        }
        
        //黑屏离开
        yield return UINovelsPanel.Instance.BlackLeave( 1f);
    }

}

[Serializable]
[LabelText("Spine角色位置")]
public class SetSpinePos : INovelsSet
{
    [ResourcePath(typeof(Spine.Unity.SkeletonDataAsset))]
    [LabelText("Spine角色")]
    public string SpineName;
    
    [ValueDropdown("_dropItemList")]
    [LabelText("起始位置")]
    [InlineButton("FreshItem")]
    [OnValueChanged("FreshItem")]
    public string StartPos;

    [ValueDropdown("_dropItemList")]
    [LabelText("最终位置")]
    [InlineButton("FreshItem")]
    [OnValueChanged("FreshItem")]
    public string EndPos;
    
    [LabelText("起始透明度")]
    [Range(0,1)]
    public float StartAlpha = 0;
    
    [LabelText("最终透明度")]
    [Range(0,1)]
    public float EndAlpha = 1;

    [LabelText("完成时间")]
    public float time = 1;
    
    private Vector3 StartPosVec3;
    private Vector3 EndPosVec3;
    
    IEnumerable<string> _dropItemList;
    
    private void FreshItem()
    {
        _dropItemList = GlobalConfig.Instance.CharaPosList.Select(x => x.PosKey);
        StartPosVec3= GlobalConfig.Instance.CharaPosList.Find(x => x.PosKey == StartPos).Pos;
        EndPosVec3= GlobalConfig.Instance.CharaPosList.Find(x => x.PosKey == EndPos).Pos;
    }
    
    public IEnumerator Run()
    {
        //Todo 缓动DT实现，可能角色透明底会分层，后续用RT实现
        var spine = SpineManager.Instance.GetSpine(SpineName);
        spine.transform.localPosition = StartPosVec3;
        SkeletonAnimation skeletonAnimation = spine.GetComponent<SkeletonAnimation>();
        skeletonAnimation.skeleton.A = StartAlpha;
       
        spine.transform.DOLocalMove(EndPosVec3, time);
        DOTween.To(
                () => StartAlpha, //起始值
                x =>
                {
                    skeletonAnimation.skeleton.A = x; //变化值
                },
                EndAlpha, //终点值
                time) //持续时间
            .SetEase(Ease.InCirc) //缓动类型
            .SetUpdate(false); //Time.Scale影响
        
        yield return new WaitForSeconds(time);
    }
}

[Serializable]
[LabelText("Spine角色动作")]
public class SetSpineAction : INovelsSet
{
    [ResourcePath(typeof(Spine.Unity.SkeletonDataAsset))]
    [OnValueChanged("FreshItem")]
    [LabelText("Spine资源")]
    public string SpineAsset;
    
    [ValueDropdown("_dropItemList")]
    [LabelText("Spine动画")]
    [ShowIf("@SpineAsset!=null")]
    public string ActionName;
    
    IEnumerable<string> _dropItemList;
    
    private void FreshItem()
    {
        SkeletonDataAsset spine = GameHelper.Alloc<SkeletonDataAsset>(SpineAsset);

        _dropItemList = spine.GetSkeletonData(true).Animations.Select(x => x.Name);
        
    }
    
    public IEnumerator Run()
    {
        
        yield return  null;
    }
}


#endregion