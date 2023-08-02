using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 建造输入系统
/// </summary>
public class BuildInputSystem : MonoBehaviour
{
    public Action<Vector3> v_OnMouseHold;
    public Action n_OnMouseClick, n_OnMouseHold, n_OnMouseUp;
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask buildingMask;
    [SerializeField] List<GameObject> newGameObjects = new List<GameObject>();
    [SerializeField] List<Building> playerPlaceBuilding = new List<Building>();
    [SerializeField] PlacementSystem placementSystem;
    private int currentObjectNumber;//当前放置的序号
    public BuildingState state;
    public event Action EndInput;

    private void Update()
    {
        CheckClickDownEvent();
        CheckClickHoldEvent();
        CheckClickUpEvent();
    }

    /// <summary>
    /// 检查是否可放置
    /// </summary>
    /// <returns></returns>
    private Vector3? RaycastGround()
    {
        RaycastHit hit;//从光线投射获取信息
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            return hit.point;
        }
        else
        {
            return null;
        }
    }

    public RaycastHit2D GetRaycastHit2D()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.down, buildingMask);
    }

    //private float fixX;
    //private float fixY;
    private Building building;
    /// <summary>
    /// 实例化预制体
    /// </summary>
    /// <param name="buildingBase"></param>
    public void InstantiatePrefab(BuildingBase buildingBase)
    {
        Vector3? hitPoint = RaycastGround();
        if(hitPoint != null)
        {
            placementSystem.PlaceBuilding(buildingBase, hitPoint.Value);
        }
        else
        {
            Debug.LogError("摄像机没扫到Collider");
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 鼠标按下
    /// </summary>
    private void CheckClickDownEvent()
    {
        //if(Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)//确保鼠标位置为空
        if(Input.GetMouseButtonDown(0))
        {
            n_OnMouseClick?.Invoke();
            /*Vector3? hitPoint = RaycastGround();
            if(hitPoint != null)
            {
                OnMouseClick?.Invoke(hitPoint.Value);
            }*/
        }
    }

    /// <summary>
    /// 鼠标按住
    /// </summary>
    private void CheckClickHoldEvent()
    {
        //if(Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)//确保鼠标位置为空
        if(Input.GetMouseButton(0))
        {
            if(state == BuildingState.Moving)
            {
                if(EventSystem.current.IsPointerOverGameObject() == false)
                {
                    Vector3? hitPoint = RaycastGround();
                    if(hitPoint != null)
                    {
                        v_OnMouseHold?.Invoke(hitPoint.Value);
                    }
                }
            }
            else
            {
                n_OnMouseHold.Invoke();
            }
            /*Vector3? hitPoint = RaycastGround();
            if(hitPoint != null)
            {
                v_OnMouseHold?.Invoke(hitPoint.Value);
                //placementSystem.MovingObject(hitPoint.Value);
                //float vx = hitPoint.Value.x;
                //float vy = hitPoint.Value.y;

                ////修正物体位置使其保持在格子内//0.5f随物体大小可调整fixX,fixY
                //newGameObjects[currentObjectNumber].transform.position =
                //    new Vector2
                //    (
                //        vx > 0? (int)vx + 0.5f : (int)vx - 0.5f,
                //        vy > 0? (int)vy + 0.5f : (int)vy - 0.5f
                //    );
            }*/

            /*Vector3? position = RaycastGround();
            if(position != null)
            {
                OnMouseHold?.Invoke(position.Value);
            }*/
        }
    }

    /// <summary>
    /// 鼠标抬起
    /// </summary>
    private void CheckClickUpEvent()
    {
        //if(Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false)
        if(Input.GetMouseButtonUp(0))
        {
            n_OnMouseUp?.Invoke();
            /*Vector3? position = RaycastGround();
            if(position != null)
            {
                n_OnMouseUp?.Invoke();
            }*/
        }
    }

    /// <summary>
    /// 确认放置
    /// </summary>
    public void Determine()
    {
        if(!placementSystem.SaveCurrentObject())
        {
            return;
        }
        /*//检查放置
        if(building != null && !building.IsItInBuildableGridRange())
        {
            return;
        }

        //刷新背包
        //关闭该物体网格检测
        building.CloseBuildableDetection();
        building = null;
        currentObjectNumber = -1;*/

        EndInput.Invoke();
    }

    /// <summary>
    /// 取消放置
    /// </summary>
    public void Cancel()
    {
        if(placementSystem.CancelPlace())
        {
            EndInput.Invoke();
        }
    }
}
public enum BuildingState{ Moving, Select, Camera }