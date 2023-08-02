using UnityEngine;
//暂时没有更好的处理方式，只能用两个碰撞体处理冲浪
public class SurfTrigger : ZSavable
{
    [SerializeField] Collider2D land;
    [SerializeField] Collider2D ocean;
    private bool isSurfing = false;
    private void OnTriggerEnter2D()
    {
        ocean.isTrigger = isSurfing;
        land.isTrigger = !isSurfing;
        isSurfing = !isSurfing;
        /*if(!isSurfing)//other.CompareTag("Player") &&
        {
            //进水
            isSurfing = true;
            ocean.isTrigger = false;
            land.isTrigger = true;
        }
        else
        {
            //出水
            land.isTrigger = false;
            ocean.isTrigger = true;
            isSurfing = false;
        }*/
        GameManager.Instance.Player.Surf(isSurfing);
    }
    private void OnTriggerExit2D()
    {
        if(isSurfing)//other.CompareTag("Player") &&
        {
            //离开;
            ocean.isTrigger = false;
            land.isTrigger = true;
        }
    }

    public override object CaptureState()
    {
        return isSurfing;
    }

    public override void RestoreState(object state)
    {
        if(isSurfing != (bool)state)
        {
            OnTriggerEnter2D();
        }
    }
}
/*using UnityEngine;
public class TriggerCtrlCollider : MonoBehaviour
{
    [SerializeField] Collider2D triggerEnter;
    [SerializeField] Collider2D triggerExit;
    //[SerializeField] Collider2D iCollider;
    [SerializeField] GameObject iCollider;
    [SerializeField] bool enter;
    private void OnTriggerEnter2D(Collider2D other)
    {
        enter = !enter;
        iCollider.SetActive(enter);
        triggerEnter.isTrigger = enter;
        triggerExit.isTrigger = !enter;
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if(enter)
    //    {
    //        triggerEnter.isTrigger = false;
    //        triggerExit.isTrigger = true;
    //    }
    //}
}*/