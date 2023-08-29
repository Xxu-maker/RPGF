// Description: 绑定不同对话组件实现统一控制

using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BindAvgCom : MonoBehaviour
{
    //对话框背景
    private Transform bg;
    //角色名字
    private TextMeshProUGUI name;
   
    private AdvancedText content;
    private ToggleGroup toggleGroup;
    void Start()
    {
        bg= transform.GetChild(0);
        name=transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        content=transform.GetChild(3).GetComponent<AdvancedText>();
    }

    private void OnEnable()
    {
        //UINovelsPanel.Instance.textDialog = content;
        //UINovelsPanel.Instance.textCharaNameLeft = name;
    }
}
