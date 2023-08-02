using UnityEngine;
/// <summary>
/// 用于放置时网格是否有物体的检查检查
/// </summary>
public class GridObjectCheck : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    private void OnTriggerExit2D(Collider2D other)
    {
        spriteRenderer.color = MyData.girdGreen;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        spriteRenderer.color = MyData.girdRed;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        spriteRenderer.color = MyData.girdRed;
    }
}