using UnityEngine;
/// <summary>
/// 用于宝可梦电脑对话物体
/// </summary>
public class PCCtrl : MonoBehaviour, Interactable
{
    [SerializeField] Sprite[] pcActiveSprites;
    [SerializeField] SpriteRenderer sprite;
    public void Interact(Transform initiator)
    {
        sprite.sprite = pcActiveSprites[1];
        UIManager.Instance.PCPanel.PCSelection();
    }
}