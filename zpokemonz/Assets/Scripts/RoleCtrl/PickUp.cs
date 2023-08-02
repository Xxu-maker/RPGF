using UnityEngine;
public class PickUp : MonoBehaviour, Interactable
{
    [SerializeField] ItemSlot itemSlot;
    private int i = 0;
    public void Interact(Transform initiator)
    {
        ++i;
        if(DialogManager.Instance.Free)
        {
            DialogManager.Instance.PickUpItemsInfo(itemSlot.Base.ItemName);
            GameManager.Instance.Inventory.LayInItem(itemSlot);
            AudioManager.Instance.GetItemsAudio();
        }
        else
        {
            DialogManager.Instance.Typing();
            if(i == 2)
            {
                Destroy(gameObject);
            }
        }
    }
}