using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Novels;
using System.Linq;


[InlineEditor]
public class GlobalConfig : SerializedScriptableObject
{
    public static GlobalConfig _instance;

    public static GlobalConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                Reload();
            }
            return _instance;
        }
    }

    public static void Reload()
    {
        var res = Resources.Load<GlobalConfig>("Config/GlobalConfig");

        if (res != null)
        {
            _instance = res;
        }
    }

#if UNITY_EDITOR
    [Button("刷新")]
    public void Fresh() 
    {
        Reload();
        
    }
#endif

    
    [LabelText("当前剧情")]
    [HideInInspector]
    public NovelsChapterData StartNovelName;
    
    [LabelText("剧本章节")]
    public class NovelsChapterData
    {
        [ResourcePath(typeof(NovelsSectionData))]
        public String SectionNodes = String.Empty;
    }
    
   
    [LabelText("出场坐标Pos")]
    public class CharaPosData
    {
        [LabelText("Key")]
        public string PosKey;
        [LabelText("舞台坐标")]
        public Vector3 Pos;
    }
    
    [BoxGroup("舞台配置")]
    [LabelText("舞台坐标配置")]
    public List<CharaPosData> CharaPosList = new List<CharaPosData>();
    
    
    [BoxGroup("文本配置")]
    [LabelText("字符出现间隔")]
    public float CharSpeed = 0.02f;
    [LabelText("对话间隔强制等待时间")]
    public float ForceTextWait = 0.5f;

    [BoxGroup("音效配置")]
    [LabelText("字体出现声音")]
    public AudioClipData SFX_TextAppear = new AudioClipData();
}
