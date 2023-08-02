using UnityEngine;
/// <summary>
/// 用于选取需要移动的物体
/// </summary>
public class MoveObject : MonoBehaviour
{
    [SerializeField] BuildManager buildManager;
    private float startTime;
    private bool wasChoose = false;

    /// <summary>
    /// 记录点击时间
    /// </summary>
    public void RecordTime()
    {
        if(!wasChoose)
        {
            startTime = Time.time;
        }
    }

    /// <summary>
    /// 超过1s选取该物体
    /// </summary>
    public void Select()
    {
        if(Time.time - startTime > 1f && !wasChoose)
        {
            RaycastHit2D hit = buildManager.BuildInputSystem.GetRaycastHit2D();
            if(hit.collider)
            {
                wasChoose = true;
                buildManager.PlacementSystem.SelectPlacedObject(hit.collider.gameObject.GetComponent<Building>());
            }
        }
    }

    /// <summary>
    /// 结束操作
    /// </summary>
    public void EndInput()
    {
        if(wasChoose)
        {
            buildManager.BackObjectMovingHandler();
            wasChoose = false;
        }
    }
}