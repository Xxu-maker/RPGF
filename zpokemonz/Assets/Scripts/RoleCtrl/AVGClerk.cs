using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 大剧情触发器
/// </summary>
public class AVGClerk : SerializedMonoBehaviour, Interactable
{
    [LabelText("剧情名")]
    public GlobalConfig.NovelsChapterData NovelName;
    
    public void Interact(Transform initiator)
    {
        
        GlobalConfig.Instance.StartNovelName= NovelName;
        GameManager.Instance.LoadAVGScene();
    }
}

