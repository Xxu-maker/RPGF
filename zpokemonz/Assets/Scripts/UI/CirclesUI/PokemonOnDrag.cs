using UnityEngine;
using UnityEngine.EventSystems;
public class PokemonOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] CanvasGroup canvasGroup;
    public int ID;
    private Transform originalParent;//拖拽物原始父节点
    private SingleParamDelegate odOnClickDelegate;
    private SingleParamDelegate odSetFollow;
    private TwoParamsDelegate odSwap;
    public void SetDelegate(SingleParamDelegate _setFollow, TwoParamsDelegate _swap)
    {
        odSetFollow = _setFollow;
        odSwap = _swap;
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
    }
    public void OnEndDrag(PointerEventData eventData)//结束
    {
        transform.SetParent(originalParent);//不换位置,只刷新宝可梦
        transform.position = originalParent.position;
        GameObject obj = eventData.pointerCurrentRaycast.gameObject;
        if(!System.Object.ReferenceEquals(obj, null) && obj.CompareTag("CirclePokemon"))
        {
            odSwap(ID, obj.GetComponent<PokemonOnDrag>().ID);
        }
        else
        {
            odSetFollow(ID);
        }
        canvasGroup.blocksRaycasts = true;
    }

    public void DesPanel()//按钮 弹出可以详细面板
    {
        //odOnClickDelegate(ID);
    }
}