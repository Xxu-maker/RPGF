using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExcelData;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Startup : SingletonMono<Startup>
{
    public ConfigMgrSObj TableData;

    [HideInInspector] 
    public Dictionary<string, Language> DicLanguages;
    private void Start()
    {
        DontDestroyOnLoad(this);
        
        //框架启动层部署
        
        //本地化表
        DicLanguages = TableData.languages.ToDictionary(Key => Key.key, Value => Value);
        
        
        //其它配置表

    }

    
}
