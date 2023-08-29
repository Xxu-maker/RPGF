using Novels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class NovelsManager : MonoBehaviour
{
    public static NovelsManager Instance;

    public Coroutine MainCoroutine;

    public bool IsAcceptConfirm=false;

    public bool IsPlayableOver = false;

    public PlayableDirector CurrentPlayable;

    public Coroutine TimeLineCoroutine;

    public string CacheContent;
    
    public bool IsFast = false;

    public bool BtnIsOn = false;
    //流程暂停
    public bool DisableProcesses=false;
    
    public bool IsSkipBottonConfirm { get; set; }
    public bool IsBiYan { get; set; }

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
        if (MainCoroutine != null)
        {
            StopCoroutine(MainCoroutine);
        }
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
    
    public void ToggleGrounp(Toggle auto, Toggle fast)
    {
        if (SaveManager.Instance.Cfg.ForceTextWait == SaveManager.Instance.Cfg.AutoReadWaitTime)
        {
            auto.isOn = true;
            auto.GetComponent<Animator>().Play("Selected");
        }
        if (UnityEngine.Time.timeScale >= 1 && BtnIsOn == true)
        {
            if (fast.gameObject.activeSelf == true)
            {
                fast.isOn = true;
                fast.GetComponent<Animator>().Play("Selected");
            }
        }
    }
    
}
