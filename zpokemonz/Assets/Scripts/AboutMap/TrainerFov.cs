using UnityEngine;
/// <summary>
/// 训练家战斗视野
/// </summary>
public class TrainerFov : MonoBehaviour, PlayerTrigger
{
    [SerializeField] TrainerCtrller trainer;
    public void OnPlayerTrigger()
    {
        GameManager.Instance.OnEnterCharacterView(trainer.TriggerTrainerBattle);
    }
}