using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
/// <summary>
/// 战斗时训练家出场动画
/// </summary>
public class TrainerAnimator : MonoBehaviour
{
    [SerializeField] Transform _trans;
    [SerializeField] PlayerMovement player;
    [SerializeField] SpriteRenderer sprite;
    public List<Sprite> spriteSheet;
    private Color color = Color.white;

    void Start()
    {
        if(player != null)
        {
            spriteSheet = player.PlayerSprite;
            sprite.sprite = spriteSheet[0];
        }
    }

    public async UniTask PlayPlayerThrowingBallAnim()
    {
        sprite.sprite = spriteSheet[1];
        await UniTask.Delay(60);

        sprite.sprite = spriteSheet[2];
        await UniTask.Delay(10);
        sprite.sprite = spriteSheet[3];
        await UniTask.Delay(10);
        sprite.sprite = spriteSheet[4];
        await UniTask.Delay(10);
    }

    /// <summary>
    /// 播放角色出场动画
    /// </summary>
    public async UniTask PlayNormalAnimator()
    {
        foreach(Sprite s in spriteSheet)
        {
            sprite.sprite = s;
            await UniTask.Delay(10);
        }
    }

    /// <summary>
    /// 激活
    /// </summary>
    public void _Active(Vector3 pos)
    {
        _trans.position = pos;

        if(sprite.color != Color.white)
        {
            sprite.color = Color.white;
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 玩家退场
    /// </summary>
    public void PlayerOut()
    {
        _trans.DOMoveX(_trans.position.x - 10, 2f);
        SpriteFade();
    }

    /// <summary>
    /// Npc退场
    /// </summary>
    public void NpcOut()
    {
        _trans.DOMoveX(_trans.position.x + 3, 0.7f);
        SpriteFade();
    }

    public async void SpriteFade()
    {
        for(float a = 0.9f; a > 0f; a -= 0.1f)
        {
            color.a = a;
            sprite.color = color;
            await UniTask.Delay(60);
        }
        gameObject.SetActive(false);
        sprite.color = Color.white;
    }
}