using UnityEngine;
public enum PortalDirection{ Up, Down, Left, Right }
public enum ChangeSceneType{ Number, String }
/// <summary>
/// 用于玩家场景传送的Trigger
/// </summary>
public class Portal : MonoBehaviour, PlayerTrigger
{
    [Header("传送门方向(玩家进入方向)")]
    [SerializeField] PortalDirection direction;

    [Header("传送坐标(要基于玩家精准坐标计算)")]
    [SerializeField] Vector2 whereGo;

    [Header("门动画")]
    [SerializeField] DoorAnimator doorAnimator;

    [Header("场景切换(默认0不换场景)")]
    [SerializeField] ChangeSceneType type;
    [SerializeField] int sceneNum;
    [SerializeField] string SceneName;

    public void OnPlayerTrigger()
    {
        if(doorAnimator == null)
        {
            AudioManager.Instance.ChangeTheSceneAudio();
        }
        else
        {
            doorAnimator.PlayAnim();
        }
        GameManager.Instance.LoadScene(whereGo, direction, sceneNum);
    }
}