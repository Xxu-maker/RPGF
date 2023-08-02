using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 不需要移动的NPC
/// </summary>
public class StaticNpc : ZSavable, Interactable
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    [Header("基本信息")]
    [SerializeField] string trainerName;
    [SerializeField] Sprite faceSprite;
    [Header("对话内容")]
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog otherDialog;
    [Header("持有道具")]
    [SerializeField] List<ItemSlot> items;
    [SerializeField] bool isGiveItems;
    private void Start()
    {
        spriteRenderer.sprite = sprites[(int)defaultDirection];
    }
    public void Interact(Transform initiator)
    {
        if(DialogManager.Instance.Free)
        {
            LookTowards(initiator.position);
            if(!isGiveItems)
            {
                DialogManager.Instance.Info(otherDialog, trainerName, faceSprite);
                GameManager.Instance.Inventory.LayInItemList(items);
                AudioManager.Instance.GetItemsAudio();
                isGiveItems = true;
                items = null;
            }
            else
            {
                DialogManager.Instance.Info(dialog, trainerName, faceSprite);
            }
        }
        else
        {
            DialogManager.Instance.Typing();
        }
    }

    public void LookTowards(Vector3 targetPos)
    {
        float xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        float ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);
        if(xdiff == 0 || ydiff == 0)
        {
            //float x = Mathf.Clamp(xdiff, -1f, 1f);
            //float y = Mathf.Clamp(ydiff, -1f, 1f);
            spriteRenderer.sprite = sprites[ydiff == 1? 0 : ydiff == -1? 1: xdiff == -1? 2 : 3];
        }
    }

    public override object CaptureState()
    {
        return isGiveItems;
    }
    public override void RestoreState(object state)
    {
        isGiveItems = (bool)state;
    }
}