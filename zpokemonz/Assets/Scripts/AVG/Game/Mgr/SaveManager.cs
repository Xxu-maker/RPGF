using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using LitJson;

public class RecallConfig
{
    public int type = 0;
    public int CurrentSelect;
    public string roleName;
    public List<string> Datas = new List<string>();
}

public class SaveConfig
{
    [LabelText("章节名")]
    public string ChapterName;
    [LabelText("段落坐标")]
    public int SectionIndex;

    public int BgmVolume;
    
    public bool IsSkip = false;

    //游戏速度
    public float GameSpeed { get; set; }
    
    [LabelText("文字速度")]
    public double CharSpeed=0.02f;
    [HideInInspector]
    public double RealCharSpeed;
    [LabelText("自动阅读速度")]
    public double AutoReadWaitTime;
    [HideInInspector]
    public double RealAutoReadWaitTime;
    public double ForceTextWait;
    
    [LabelText("回忆数据")]
    public List<RecallConfig> MemoryData = new List<RecallConfig>();
    
    [LabelText("是否已读")]
    public int isHaveRead =1;
}

[AutoCreateSingleton]
public class SaveManager:Singleton<SaveManager>
{
    public SaveConfig Cfg=new SaveConfig();

    public bool IsHasSave 
    {
        get 
        {
            return PlayerPrefs.HasKey("Save");
        }
    }

    protected override void Initialize()
    {
        if (IsHasSave)
        {
            Load();
           
        }
        else 
        {
            Cfg = new SaveConfig();
            Cfg.IsSkip = false;
            Cfg.BgmVolume = 100;
            Cfg.CharSpeed = GlobalConfig.Instance.CharSpeed;
            Cfg.ForceTextWait = GlobalConfig.Instance.ForceTextWait;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("Save", JsonMapper.ToJson(Cfg));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Save"))
        {
            var str = PlayerPrefs.GetString("Save");
            Cfg = JsonMapper.ToObject<SaveConfig>(str);
        }
    }
}
