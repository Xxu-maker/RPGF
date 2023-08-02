using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用于单个Scene文件的储存和读取(可以由Scene拆卸时保存，不用find)
/// </summary>
public class SceneFileHandler : MonoBehaviour
{
    public bool isLoad;

    [Tooltip("一张图可以只用一个SavableEntity存(Entity可以存多个物体存档)")]
    [SerializeField] List<SavableEntity> savableEntities;

    /// <summary>
    /// 存储在临时存档中
    /// </summary>
    public void Save()
    {
        SavingSystem.Instance.CaptureEntityStatesToCurrentState(savableEntities);
    }

    /// <summary>
    /// 从临时存档读取
    /// </summary>
    public void Load()
    {
        SavingSystem.Instance.RestoreEntityStatesFromCurrentState(savableEntities);
    }
}