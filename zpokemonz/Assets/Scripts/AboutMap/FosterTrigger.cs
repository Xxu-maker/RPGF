using UnityEngine;
/// <summary>
/// 饲育屋场地生成宝可梦Trigger
/// </summary>
public class FosterTrigger : MonoBehaviour
{
    [SerializeField] Collider2D triggerEnter;
    [SerializeField] Collider2D triggerExit;
    [SerializeField] FosterYard yard;
    [SerializeField] bool enter;
    private void OnTriggerEnter2D()
    {
        enter = !enter;
        triggerEnter.isTrigger = enter;
        triggerExit.isTrigger = !enter;

        yard.SetPokemon(GameManager.Instance.PlayerTeam.Foster);
    }
}