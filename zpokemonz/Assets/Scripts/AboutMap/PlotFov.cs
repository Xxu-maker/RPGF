using UnityEngine;
/// <summary>
/// 剧情人物视野
/// </summary>
public class PlotFov : MonoBehaviour, PlayerTrigger
{
    [SerializeField] PlotRoleCtrl npc;
    public void OnPlayerTrigger()
    {
        GameManager.Instance.OnEnterCharacterView(npc.TriggerStoryline);
    }
}