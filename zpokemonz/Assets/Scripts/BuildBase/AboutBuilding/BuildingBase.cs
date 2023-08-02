using UnityEngine;
[CreateAssetMenu(menuName = "建筑预制体/创建新建筑")]
public class BuildingBase : ScriptableObject
{
    [Header("图")]
    [SerializeField] Sprite objectSprite;
    [Header("预制体")]
    [SerializeField] Building building;
    [Header("大小 (宽 和 长)")]
    [SerializeField] Vector2 occupancyGrid;

    public Sprite ObjectSprite => objectSprite;
    /// <summary>
    /// 建筑预制体
    /// </summary>
    public Building Building => building;
    /// <summary>
    /// 占用网格大小
    /// </summary>
    public Vector2 OccupancyGrid => occupancyGrid;
}