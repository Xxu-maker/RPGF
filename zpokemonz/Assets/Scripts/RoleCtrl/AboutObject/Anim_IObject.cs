using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using Cysharp.Threading.Tasks;
/// <summary>
/// 互动物体之动画播放
/// </summary>
public class Anim_IObject : InteractionObjectBase
{
    [Header("sprites动画")]
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("动态光照")]
    [SerializeField] Light2D light2D;
    [Tooltip("不能大于光照半径最大值")]
    [SerializeField] float max;
    [Tooltip("不能小于0")]
    [SerializeField] float min;
    [Range(0.01f, 0.05f)]
    [SerializeField] float speed;

    public override void _Active()
    {
        if(play)
        {
            play = false;
        }
        else
        {
            Animator();
        }
    }

    private bool play;
    public async void Animator()
    {
        bool sAnim = spriteRenderer != null;
        bool lightAnim = light2D != null;
        if(lightAnim)
        {
            light2D.gameObject.SetActive(true);
        }
        int frame = 0;
        int count = sprites.Length;
        bool front = false;
        Vector3 vec = Vector3.one;

        play = true;
        while(play)
        {
            if(sAnim)
            {
                if(frame >= count)
                {
                    frame = 0;
                }
                spriteRenderer.sprite = sprites[frame];
                frame++;
            }

            if(lightAnim)
            {
                if(front)
                {
                    light2D.pointLightInnerRadius += speed;
                    if(light2D.pointLightInnerRadius > max)
                    {
                        front = false;
                    }
                }
                else
                {
                    light2D.pointLightInnerRadius -= speed;
                    if(light2D.pointLightInnerRadius < min)
                    {
                        front = true;
                    }
                }
            }

            await UniTask.Delay(100);
        }

        //关闭
        if(sAnim)
        {
            spriteRenderer.sprite = null;
        }

        if(lightAnim)
        {
            light2D.gameObject.SetActive(false);
        }
    }
}