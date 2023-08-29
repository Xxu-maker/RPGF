using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Novels;
[System.Serializable]
public class Script
{
    public string Sign;
    //默认通用文本
    public NovelsScriptsType action= NovelsScriptsType.DialogContent;
    public List<string> Value;
    
}
public enum NovelsScriptsType
{
    DialogContent,//对话内容
    BlackScreenContent,//黑屏内容
    SelectContent,//选择内容
}

