using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[ExecuteAlways]
public class SavableEntity : MonoBehaviour
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    [SerializeField] string uniqueId = "";
    /// <summary>
    /// 单个GameObject需要保存的内容
    /// </summary>
    [SerializeField] ZSavable[] savables;

    /// <summary>
    /// 全局查找
    /// </summary>
    static Dictionary<string, SavableEntity> globalLookup = new Dictionary<string, SavableEntity>();
    public string UniqueId => uniqueId;

    /// <summary>
    /// 用于捕获附加SavableEntity的游戏对象的状态
    /// </summary>
    public object CaptureState()
    {
        Dictionary<int, object> state = new Dictionary<int, object>();
        int length = savables.Length;
        for(int i = 0; i < length; i++)
        {
            state[i] = savables[i].CaptureState();
        }
        return state;
    }

    /// <summary>
    /// 用于还原附加了savableEntity的游戏对象的状态
    /// </summary>
    public void RestoreState(object state)
    {
        Dictionary<int, object> stateDict = (Dictionary<int, object>)state;
        int length = savables.Length;
        for(int i = 0; i < length; i++)
        {
            if (stateDict.ContainsKey(i))
            {
                savables[i].RestoreState(stateDict[i]);
            }
        }
    }

#region UUID
#if UNITY_EDITOR
    /// <summary>
    /// 用于生成SavableEntity的UUID的更新方法
    /// </summary>
    private void Update()
    {
        //不在播放模式下执行
        if(Application.IsPlaying(gameObject))
        {
            return;
        }

        //不为预制件生成Id（预制件场景的路径为空）
        if(String.IsNullOrEmpty(gameObject.scene.path))
        {
            return;
        }

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty property = serializedObject.FindProperty("uniqueId");

        if(String.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }

        globalLookup[property.stringValue] = this;
    }
#endif
    private bool IsUnique(string candidate)
    {
        if (!globalLookup.ContainsKey(candidate))
        {
            return true;
        }
        if (globalLookup[candidate] == this)
        {
            return true;
        }
        //处理现场卸货unloading cases
        if (globalLookup[candidate] == null)
        {
            globalLookup.Remove(candidate);
            return true;
        }
        //处理诸如designer手动更改UUID之类的边缘情况
        if (globalLookup[candidate].UniqueId != candidate)
        {
            globalLookup.Remove(candidate);
            return true;
        }
        return false;
    }
#endregion
}