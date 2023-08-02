using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

[AutoCreateSingleton]
public class SpineManager : SingletonMono<SpineManager>
{
     /// <summary>
    /// 播放Spine动画
    /// </summary>
    /// <param name="spine"></param>
    /// <param name="trackIndex"></param>
    /// <param name="animName"></param>
    /// <param name="loop"></param>
    /// <param name="skinName"></param>
    /// <param name="callBack"></param>
    public static void PlayAnim(SkeletonAnimation spine, int trackIndex, string animName, bool loop, string skinName = "", Action callBack = null)
    {
        Spine.Skeleton skeleton = spine.skeleton;
        Spine.AnimationState state = spine.AnimationState;
 
        if (spine != null)
        {
            if (!string.IsNullOrEmpty(skinName))
                skeleton.SetSkin(skinName);
 
            state.SetAnimation(trackIndex, animName, loop);
        }
 
        AnimationState.TrackEntryDelegate animCallBack = null;
 
        animCallBack = delegate
        {
            if (callBack != null)
            {
                callBack.Invoke();
            }
 
            state.Complete -= animCallBack;
 
            animCallBack = null;
        };
 
        state.Complete += animCallBack;
    }
 
    /// <summary>
    /// 播放Spine动画（UI）
    /// </summary>
    /// <param name="spine"></param>
    /// <param name="trackIndex"></param>
    /// <param name="animName"></param>
    /// <param name="loop"></param>
    /// <param name="skinName"></param>
    /// <param name="callBack"></param>
    public static void PlayAnim(SkeletonGraphic spine, int trackIndex, string animName, bool loop, string skinName = "", Action callBack = null)
    {
        Spine.Skeleton skeleton = spine.Skeleton;
        Spine.AnimationState state = spine.AnimationState;
 
        if (spine != null)
        {
            if (!string.IsNullOrEmpty(skinName))
                skeleton.SetSkin(skinName);
 
            state.SetAnimation(trackIndex, animName, loop);
        }
 
        AnimationState.TrackEntryDelegate animCallBack = null;
 
        animCallBack = delegate
        {
            if (callBack != null)
            {
                callBack.Invoke();
            }
 
            state.Complete -= animCallBack;
 
            animCallBack = null;
        };
 
        state.Complete += animCallBack;
    }
 
    /// <summary>
    /// 停止Spine动画
    /// </summary>
    /// <param name="spine"></param>
    /// <param name="trackIndex"></param>
    /// <param name="mixDuration"></param>
    public static void StopAnim(SkeletonAnimation spine, int trackIndex, float mixDuration)
    {
        Spine.AnimationState state = spine.AnimationState;
 
        state.SetEmptyAnimation(trackIndex, mixDuration);
    }
 
    /// <summary>
    /// 停止Spine动画（UI）
    /// </summary>
    /// <param name="spine"></param>
    /// <param name="trackIndex"></param>
    /// <param name="mixDuration"></param>
    public static void StopAnim(SkeletonGraphic spine, int trackIndex, float mixDuration)
    {
        Spine.AnimationState state = spine.AnimationState;
 
        state.SetEmptyAnimation(trackIndex, mixDuration);
    }
    
    //缓存Spine字典
    private Dictionary<string,GameObject> _spineDic = new Dictionary<string, GameObject>();
    
    //预加载Spine到字典
    public IEnumerator  PreLoadSpine(string spineName)
    {
        if (_spineDic.ContainsKey(spineName))
        {
            yield return null;
        }
        else
        {
            GameObject spine = GameHelper.Alloc<GameObject>( spineName);
            spine.transform.RestTransform(UINovelsPanel.Instance.Spine_Root);
            //Todo 透明度设置为0
            SkeletonAnimation skeletonAnimation = spine.GetComponent<SkeletonAnimation>();
            skeletonAnimation.Skeleton.A = 0;
            _spineDic.Add(spineName,spine);
            yield return null;
        }
    }
    
    //获取缓存中的spine
    public GameObject GetSpine(string spineName)
    {
        if (_spineDic.ContainsKey(spineName))
        {
            return _spineDic[spineName];
        }
        else
        {
            GameObject spine = GameHelper.Alloc<GameObject>("Prefabs/Spine/" + spineName);
            spine.transform.RestTransform(UINovelsPanel.Instance.Spine_Root);
            //Todo 透明度设置为0
            SkeletonAnimation skeletonAnimation = spine.GetComponent<SkeletonAnimation>();
            skeletonAnimation.Skeleton.A = 0;
            _spineDic.Add(spineName,spine);
            return spine;
        }
    }
    
    //清空字典
    public void ClearSpineDic()
    {
        _spineDic.Clear();
    }
}

