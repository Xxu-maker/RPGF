using UnityEngine;
public class BookPanel : BasePanel
{
    [SerializeField] GameObject scroll;
    //[SerializeField] BookInfiniteScrollView scrollView;
    private void Awake()
    {
        scroll.transform.rotation = Quaternion.identity;
    }
    /*public override void OnOpen()
    {
        OnOpen();
        scrollView.ReadyToBook();
    }*/
    public void ExitPanel()
    {
        OnClose();
        UIManager.Instance.BackCtrlPanel();
    }
}