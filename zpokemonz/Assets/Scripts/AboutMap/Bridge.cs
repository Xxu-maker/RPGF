using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// 模拟桥上和桥下 (需要保存状态)
/// </summary>
public class Bridge : ZSavable
{
    [SerializeField] GameObject edge;//在桥上时, 桥两边的碰撞
    [SerializeField] GameObject underBridge;//桥下过桥碰撞
    [SerializeField] TilemapRenderer bridgeTileMap;//桥的TileMap
    [SerializeField] Collider2D enterTrigger;//进口Trigger
    [SerializeField] Collider2D exitTrigger;//出口Trigger
    private bool enter = false;//标记
    private void OnTriggerEnter2D()//Collider2D other
    {
        if(!enter)
        {
            enter = true;
            edge.SetActive(true);
            underBridge.SetActive(false);
            enterTrigger.isTrigger = false;
            exitTrigger.isTrigger = true;
            bridgeTileMap.sortingLayerName = "ForeGround";//Roof上ForeGround下
        }
        else
        {
            enter = false;
            edge.SetActive(false);
            underBridge.SetActive(true);
            enterTrigger.isTrigger = true;
            exitTrigger.isTrigger = false;
            bridgeTileMap.sortingLayerName = "Roof";
        }
    }

    //桥的状态需要保存*

    public override object CaptureState()
    {
        return enter;
    }

    public override void RestoreState(object state)
    {
        if(enter != (bool) state)
        {
            OnTriggerEnter2D();
        }
    }
}