using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExcelData;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class GameTableData : SingletonMono<GameTableData>
{
    public GameTableSO TableData;

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
    
    /// <summary>
    /// 根据当前选择的语言自动返回对应的本地翻译
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetLocalLanguage(string key)
    {
        switch (GameManager.Instance.language)
        {
            case GameManager.Language.CN:
                return DicLanguages[key].CN;
            case GameManager.Language.HK:
                return DicLanguages[key].HK;
            case GameManager.Language.EN:
                return DicLanguages[key].EN;
            case GameManager.Language.JP:
                return DicLanguages[key].JP;
        }
        return key;
    }
    
}
