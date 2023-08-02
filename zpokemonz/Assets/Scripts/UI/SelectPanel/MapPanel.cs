using UnityEngine;
public class MapPanel : BasePanel
{
    [SerializeField] GameObject scroll;
    [SerializeField] GameObject content;
    private Vector3 position;
    private void Awake()
    {
        scroll.transform.rotation = Quaternion.identity;
        position = content.transform.position;
    }
    public void ExitPanel()
    {
        OnClose();
        UIManager.Instance.BackCtrlPanel();
    }
    public void Reset()
    {
        content.transform.position = position;
    }
}