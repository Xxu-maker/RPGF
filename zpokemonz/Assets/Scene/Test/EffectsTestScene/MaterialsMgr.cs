using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "2DxFX材质数据库",menuName = "2DxFX材质数据库/创建新数据库")]
public class MaterialsMgr : ScriptableObject
{
    [SerializeField] List<MaterialData_2DxFX> materialData;
    public List<MaterialData_2DxFX> MaterialData => materialData;
}

[System.Serializable]
/// <summary>
/// 设置材质数据,不用脚本调用不记录数据
/// </summary>
public class MaterialData_2DxFX
{
    public string matName;
    public bool isScriptingUsed;
    public string variable;
    public AnimationCurve anm;
    public float mul = 1;
    public float speed = 1;
}