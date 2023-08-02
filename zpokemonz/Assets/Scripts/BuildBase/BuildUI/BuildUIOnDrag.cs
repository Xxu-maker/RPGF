using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 建造Scroll的Slot
/// </summary>
public class BuildUIOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //拖拽出来然后关闭UI拖拽，然后实例化物体，激活ZInputSystem
    //自己的CanvasGroup
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image image;
    [SerializeField] BuildingBase buildingBase;
    [SerializeField] BuildListCanvas buildListCanvas;
    [SerializeField] BuildInputSystem buildInputSystem;
    private Transform originalParent;//拖拽物原始父节点

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="_base"></param>
    public void SetData(BuildingBase _base)
    {
        buildingBase = _base;
        image.sprite = buildingBase.ObjectSprite;
    }

    public void OnBeginDrag(PointerEventData eventData)//开始
    {
        originalParent = transform.parent;
        transform.SetParent(transform.parent.parent);//脱离父节点,往上一层
        transform.position = eventData.position;//和鼠标拖拽点一致
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)//拖拽中
    {
        transform.position = eventData.position;
        if(System.Object.ReferenceEquals(eventData.pointerCurrentRaycast.gameObject, null))
        {
            OnEndDrag(null);

            //实例化物体
            buildInputSystem.InstantiatePrefab(buildingBase);
            //关掉控制权
            buildListCanvas.gameObject.SetActive(false);
        }

    }

    public void OnEndDrag(PointerEventData eventData)//结束
    {
        //归位
        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        canvasGroup.blocksRaycasts = true;
    }
}
