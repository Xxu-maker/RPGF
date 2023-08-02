using UnityEngine;
using UnityEngine.EventSystems;
public class PCOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int ID;
    public CanvasGroup Canvas;
    private SingleParamDelegate onClickedDelegate;
    private TwoParamsDelegate onDragDelegate;
    public void SetDelegate(SingleParamDelegate onClick, TwoParamsDelegate onDrag)
    {
        onClickedDelegate = onClick;
        onDragDelegate = onDrag;
    }

    public void PokemonTip()
    {
        onClickedDelegate(ID);
    }
#region 拖拽
    private Transform originalParent;//拖拽物原始父节点
    public void OnBeginDrag(PointerEventData eventData)//开始
    {
        originalParent = transform.parent;
        transform.SetParent(transform.parent.parent.parent);//脱离父节点,往上两层
        transform.position = eventData.position;//和鼠标拖拽点一致
        Canvas.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)//拖拽中
    {
        transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)//结束
    {
        transform.SetParent(originalParent);//不换位置, 只刷新宝可梦
        transform.position = originalParent.position;
        GameObject obj = eventData.pointerCurrentRaycast.gameObject;
        if(System.Object.ReferenceEquals(obj, null))
        {
            Canvas.blocksRaycasts = true;
            return;
        }
        else if(obj.CompareTag("PcPokemon"))
        {
            onDragDelegate(ID, obj.GetComponent<PCOnDrag>().ID);
        }
        Canvas.blocksRaycasts = true;
    }
#endregion
}