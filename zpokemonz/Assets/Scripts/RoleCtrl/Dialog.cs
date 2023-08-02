using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
/// <summary>
/// 对话信息
/// </summary>
public class Dialog
{
    [SerializeField] List<string> lines;
    public List<string> Lines => lines;

    public Dialog(List<string> text)
    {
        lines = text;
    }
}