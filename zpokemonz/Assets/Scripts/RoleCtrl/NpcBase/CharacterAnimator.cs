using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 角色朝向
/// </summary>
public enum FacingDirection{Up, Down, Left, Right}
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    [SerializeField] Sprite[] sprites;
    public FacingDirection DefaultDirection => defaultDirection;
    private List<Sprite> idleSprite = new List<Sprite>();
    private List<Sprite> walkUpSprite = new List<Sprite>();
    private List<Sprite> walkDownSprite = new List<Sprite>();
    private List<Sprite> walkLeftSprite = new List<Sprite>();
    private List<Sprite> walkRightSprite = new List<Sprite>();
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator currentAnim;
    //public string path;
    private float MoveX;
    private float MoveY;
    private bool IsMoving;
    private bool wasPreviouslyMoving;

    private void Start()
    {
        SetNpcSprite();
        walkUpAnim    = new SpriteAnimator(walkUpSprite,    spriteRenderer);
        walkDownAnim  = new SpriteAnimator(walkDownSprite,  spriteRenderer);
        walkLeftAnim  = new SpriteAnimator(walkLeftSprite,  spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprite, spriteRenderer);

        //设置人物朝向
        SetFacingDirection(defaultDirection);
    }

    private SpriteAnimator prevAnim;
    public void HandleUpdate(bool c_isMoving)
    {
        IsMoving = c_isMoving;
        if(IsMoving)
        {
            prevAnim = currentAnim;
            if(MoveX == 1f)
            {
                currentAnim = walkRightAnim;
            }
            else if(MoveX == -1f)
            {
                currentAnim = walkLeftAnim;
            }
            else if(MoveY == 1f)
            {
                currentAnim = walkUpAnim;
            }
            else if(MoveY == -1f)
            {
                currentAnim = walkDownAnim;
            }

            if(currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
            {
                currentAnim.Start();
            }
            currentAnim.HandleUpdate();
        }
        else
        {
            if(IsMoving != wasPreviouslyMoving)
            {
                //停止移动时重置
                currentAnim.Start();
                //设置静止动画
                spriteRenderer.sprite = idleSprite[MoveY == 1? 0 : MoveY == -1? 1: MoveX == -1? 2 : 3];
            }
        }
        wasPreviouslyMoving = IsMoving;
    }

    /// <summary>
    /// 设置Npc朝向
    /// </summary>
    /// <param name="dir"></param>
    public void SetFacingDirection(FacingDirection dir)
    {
        switch(dir)
        {
            case FacingDirection.Up:    MoveY =  1f; spriteRenderer.sprite = idleSprite[0]; currentAnim = walkUpAnim; break;
            case FacingDirection.Down:  MoveY = -1f; spriteRenderer.sprite = idleSprite[1]; currentAnim = walkDownAnim; break;
            case FacingDirection.Left:  MoveX = -1f; spriteRenderer.sprite = idleSprite[2]; currentAnim = walkLeftAnim; break;
            case FacingDirection.Right: MoveX =  1f; spriteRenderer.sprite = idleSprite[3]; currentAnim = walkRightAnim; break;
        }
    }

    public void SetMoveXYValue(float x, float y)
    {
        MoveX = x;
        MoveY = y;
    }

    /// <summary>
    /// 设置代替Update静止更新
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    public void UpdateMoveXY(float _x, float _y)
    {
        MoveX = _x;
        MoveY = _y;
        if(!IsMoving)
        {
            spriteRenderer.sprite = idleSprite[_y == 1f? 0 : _y == -1f? 1: _x == -1f? 2 : 3];
        }
    }
#region 加载分配
    public bool LoadSprites(string path)
    {
        sprites = ResM.Instance.LoadAllSprites(path);
        if(sprites.Length < 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void SetNpcSprite()
    {
        if(sprites.Length < 9)
        {
            idleSprite.Add(sprites[0]);
            idleSprite.Add(sprites[2]);
            idleSprite.Add(sprites[4]);
            idleSprite.Add(sprites[6]);

            walkUpSprite.Add(sprites[1]);
            walkUpSprite.Add(sprites[0]);

            walkDownSprite.Add(sprites[3]);
            walkDownSprite.Add(sprites[2]);

            walkLeftSprite.Add(sprites[5]);
            walkLeftSprite.Add(sprites[4]);

            walkRightSprite.Add(sprites[7]);
            walkRightSprite.Add(sprites[6]);
        }
        else
        {
            idleSprite.Add(sprites[0]);
            idleSprite.Add(sprites[3]);
            idleSprite.Add(sprites[6]);
            idleSprite.Add(sprites[9]);

            walkUpSprite.Add(sprites[1]);
            walkUpSprite.Add(sprites[0]);
            walkUpSprite.Add(sprites[2]);
            walkUpSprite.Add(sprites[0]);

            walkDownSprite.Add(sprites[4]);
            walkDownSprite.Add(sprites[3]);
            walkDownSprite.Add(sprites[5]);
            walkDownSprite.Add(sprites[3]);

            walkLeftSprite.Add(sprites[7]);
            walkLeftSprite.Add(sprites[6]);
            walkLeftSprite.Add(sprites[8]);
            walkLeftSprite.Add(sprites[6]);

            walkRightSprite.Add(sprites[10]);
            walkRightSprite.Add(sprites[9]);
            walkRightSprite.Add(sprites[11]);
            walkRightSprite.Add(sprites[9]);
        }
    }
#endregion
}