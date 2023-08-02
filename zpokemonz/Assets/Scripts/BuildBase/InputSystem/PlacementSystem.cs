using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 放置时处理，保管临时及存档物体
/// </summary>
public class PlacementSystem : MonoBehaviour
{
    //没弄好*************************************因为路是2*2有点难处理
    /// <summary>
    /// 临时道路
    /// </summary>
    private Dictionary<Vector3, Building> temporaryRoadObjects = new Dictionary<Vector3, Building>();

    /// <summary>
    /// 建筑
    /// </summary>
    private Dictionary<Vector3, BuildingBase> buildingDictionary = new Dictionary<Vector3, BuildingBase>();

    private List<Building> temporaryObjects = new List<Building>();

    /// <summary>
    /// 当前输入控制的Object
    /// </summary>
    private GameObject currentObject;

    /// <summary>
    /// 当前控制建筑的base
    /// </summary>
    private BuildingBase currentObjectBase;

    /// <summary>
    /// 实例化放置建筑
    /// </summary>
    /// <param name="buildingBase"></param>
    /// <param name="hitPoint"></param>
    public void PlaceBuilding(BuildingBase buildingBase, Vector2 hitPoint)
    {
        //new Vector2(hitPoint.x, hitPoint.y)
        GameObject clone = Instantiate(buildingBase.Building.gameObject, hitPoint, Quaternion.identity);
        temporaryObjects.Add(clone.GetComponent<Building>()) ;
        temporaryObjects[temporaryObjects.Count - 1].OpenBuildableDetection();
        currentObjectBase = buildingBase;
        currentObject = clone;
    }

    /// <summary>
    /// 移动当前放置物体
    /// </summary>
    /// <param name="hitPoint"></param>
    public void MovingObject(Vector3 hitPoint)
    {
        //修正物体位置使其保持在格子内//0.5f随物体大小可调整fixX,fixY
        currentObject.transform.position =
            new Vector2
            (
                hitPoint.x > 0? (int)hitPoint.x + 0.5f : (int)hitPoint.x - 0.5f,
                hitPoint.y > 0? (int)hitPoint.y + 0.5f : (int)hitPoint.y - 0.5f
            );
    }

    /// <summary>
    /// 确认放置（bug: 会透过UI移动原位置）
    /// </summary>
    /// <returns></returns>
    public bool SaveCurrentObject()
    {
        //检查是否占用其它物品网格
        if(!temporaryObjects[temporaryObjects.Count - 1].IsItInBuildableGridRange())
        {
            return false;
        }

        buildingDictionary.Add(currentObject.transform.position, currentObjectBase);
        currentObject = null;
        return true;
    }

    /// <summary>
    /// 取消放置
    /// </summary>
    public bool CancelPlace()
    {
        if(currentObject == null)
        {
            return false;
        }

        temporaryObjects.RemoveAt(temporaryObjects.Count - 1);
        Destroy(currentObject);
        currentObject = null;
        return true;
    }

    /// <summary>
    /// 选择已放置的物体进行操作
    /// </summary>
    /// <param name="building"></param>
    public void SelectPlacedObject(Building building)
    {
        //print("已选择");
        //加入临时列表
        temporaryObjects.Add(building);
        //打开grid放置检查
        building.OpenBuildableDetection();
        //设为当前操作物
        currentObject = building.gameObject;
        //从Dictionary里拿到这个
        buildingDictionary.Remove(currentObject.transform.position);
    }

    public async void TestLoad()
    {
        foreach(Building building in temporaryObjects)
        {
            Destroy(building.gameObject);
            await Task.Delay(50);
        }
        await Task.Delay(2000);
        foreach(KeyValuePair<Vector3, BuildingBase> kvp in buildingDictionary)
        {
            Instantiate(kvp.Value.Building.gameObject, kvp.Key, Quaternion.identity);
            await Task.Delay(50);
        }
    }
}