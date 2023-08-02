using Novels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class NovelsManager : MonoBehaviour
{
    public static NovelsManager Instance;

    public Coroutine MainCoroutine;

    public bool IsAcceptConfirm=false;

    public bool IsPlayableOver = false;

    public PlayableDirector CurrentPlayable;

    public Coroutine TimeLineCoroutine;

    public string CacheContent;

    void Awake()
    {
        Instance = this;
        var sr = gameObject.AddComponent<SignalReceiver>();
    }

    public void OnDestory()
    {
        StopMainCoroutine();
            
    }
    
    //暂停主线程
    public void StopMainCoroutine()
    {
        if (MainCoroutine != null)
        {
            StopCoroutine(MainCoroutine);
        }
    }


    public void Install(string chapterPath = null, int sectionIndex = -1)
    {
        MainCoroutine=StartCoroutine(Run(chapterPath,sectionIndex));
    }

 
    public IEnumerator Run(string chapterPath=null,int sectionIndex=-1)
    {
        if (chapterPath == null) 
        {
            //Todo 章节路径
            chapterPath = SaveManager.Instance.Cfg.ChapterName;
        }

        if (sectionIndex == -1)
        {
            sectionIndex = SaveManager.Instance.Cfg.SectionIndex;
        }
        
        var config = Resources.Load<NovelsSectionData>(chapterPath);
       
       
        for (int k = sectionIndex; k < config.EventNodes.Length; k++)
        {
            yield return config.EventNodes[k].Run();
        }
            
        
    }

    public void PauseTimeLine()
    {
        if (CurrentPlayable != null)
        {
            IsPlayableOver = true;
            CurrentPlayable.Pause();
        }
    }

    public void ContineTimeLine()
    {
        if (CurrentPlayable != null)
        {
            IsPlayableOver = false;
            CurrentPlayable.Play();
        }
    }
}
