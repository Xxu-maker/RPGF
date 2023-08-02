using UnityEngine;
using Cysharp.Threading.Tasks;
/// <summary>
/// 地图上开门动画
/// </summary>
public class DoorAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] doorSprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] int delay = 10;
    [SerializeField] string audioPath;

    public async void PlayAnim()
    {
        //转场音效
        AudioManager.Instance.PlayAudio
        (
            audioPath == null || audioPath.Length == 0?
            "SoundEffect/开门" : audioPath,
            AudioPlayType.ADES
        );


        foreach(Sprite sprite in doorSprites)
        {
            spriteRenderer.sprite = sprite;
            await UniTask.Delay(delay);
        }
        await UniTask.Delay(1000);
        spriteRenderer.sprite = doorSprites[0];
    }
}