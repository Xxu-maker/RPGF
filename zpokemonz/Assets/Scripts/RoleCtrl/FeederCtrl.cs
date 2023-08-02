using UnityEngine;
public class FeederCtrl : MonoBehaviour, Interactable
{
    [SerializeField] GameObject trigger;
    public void Interact(Transform initiator)
    {
        trigger.SetActive(true);
        UIManager.Instance.OpenFosterPanel();
    }
}
