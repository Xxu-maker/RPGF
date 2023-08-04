using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class AVGManager : MonoSingleton<AVGManager>
{
    public static AVGManager Instance;

    [LabelText("当前剧情名")] 
    [ReadOnly]
    public GlobalConfig.NovelsChapterData AVGNamel;
    [LabelText("当前BGM")] 
    public AudioClipData BGM;
    
    //[LabelText("是否是剧情模式")]
    //public bool IsTimelineMode;
    
    [HideInInspector]
    public  StoryScenePrefab CurrentScene;

    public void Awake()
    {
        Instance = this;

        SaveManager.CreateInstance();
        GameHelper.Alloc<GameObject>("Prefabs/Base/UIRoot");
        //外层初始化完成
        //GameHelper.Alloc<GameObject>("Prefabs/Base/AudioManager");
        //Todo 初始化剧情UI模块
        GameHelper.Alloc<GameObject>("Prefabs/UI/Novels/UINovelsPanel").transform.RestTransform(UIRoot.Instance.Trans_NovelsPoint);

        //UINovelsPanel.Instance.gameObject.SetActive(true);
       
        //Todo 初始化剧情存档
        //SaveManager.Instance.Initialize();
    }

    public void Start()
    {
        SaveManager.Instance.Cfg.ChapterName = GlobalConfig.Instance.StartNovelName.SectionNodes;
        SaveManager.Instance.Cfg.SectionIndex = 0;
        //Todo 初始化剧情管理器
        _CreateNovelsMgr();
    }

   
 
    public void EndAVGScene()
    {
        //Todo 销毁剧情管理器å
        SceneManager.UnloadSceneAsync("Scene/AVG/AVG");
    }
    

    public void ContinueGame()
    {
        SaveManager.Instance.Load();
        _ClearNovelsMgr();
        _CreateNovelsMgr();
    }

    private void _CreateNovelsMgr()
    {
        var novelsMgr = new GameObject("NovelsManager").AddComponent<NovelsManager>();
        novelsMgr.transform.RestTransform(UIRoot.Instance.Trans_NovelsPoint);;
        novelsMgr.Install();

    }

    private void _ClearNovelsMgr()
    {
        if (NovelsManager.Instance != null)
            GameHelper.Recycle(NovelsManager.Instance.gameObject);
    }


 
}
