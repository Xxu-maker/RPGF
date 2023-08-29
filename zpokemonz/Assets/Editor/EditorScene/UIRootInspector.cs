using System.Collections;
using System.Collections.Generic;
using Novels;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIRoot))]
public class TestInspector : Editor
{
    [ResourcePath(typeof(NovelsSectionData))]
    private SerializedProperty textField; // 输入框的属性
    
    
    private void OnEnable()
    {
        // 获取输入框的属性
        textField = serializedObject.FindProperty("AVG"); // 替换成你自己的输入框属性名称
    }
    public override void OnInspectorGUI()
    {
        //这个是绘制原生的GUI
        base.OnInspectorGUI();
        serializedObject.Update();

        // 绘制输入框
        //EditorGUILayout.PropertyField(textField);

        serializedObject.ApplyModifiedProperties();
        
        //通过GUI绘制一个按钮
        if (GUILayout.Button("启动剧情")) 
        {
            Debug.Log(textField.stringValue);
            NovelsManager.Instance.Install(textField.stringValue,0);
           
        }
    }
    
}

