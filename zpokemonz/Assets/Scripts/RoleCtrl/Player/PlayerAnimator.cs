using System.Collections.Generic;
using UnityEngine;
//Player专用动画器
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    private DressableAnimator currentAnim;
#region StartSet
    void Start()
	{
        //跑0.2f的帧率
		//runUp     = new SpriteAnimator(runUpSprite,        spriteRenderer, 0.2f);
		//runDown   = new SpriteAnimator(runDownSprite,      spriteRenderer, 0.2f);
        //runLeft   = new SpriteAnimator(runLeftSprite,      spriteRenderer, 0.2f);
        //runRight  = new SpriteAnimator(runRightSprite,     spriteRenderer, 0.2f);

		SetSprites("PlayerSpriteBase/man_base");
        SetSprites("PlayerSpriteBase/douye_hair2");
        SetSprites("PlayerSpriteBase/douye_pant");
        spriteRenderers.Add(_baseSpriteRenderer);
        SetSprite(CostumesType.Hair, false);
        SetSprite(CostumesType.Pant, false);

        SetDressableAnimator();

        //设置人物朝向
        SetFacingDirection(defaultDirection);
	}

    /// <summary>
	/// 初始化设置面朝方向
	/// </summary>
	/// <param name="dir"></param>
    public void SetFacingDirection(FacingDirection dir)
    {
        switch(dir)
        {
            case FacingDirection.Up:     currentAnim = walkUpAnim;    break;//MoveY =  1f;
            case FacingDirection.Down:   currentAnim = walkDownAnim;  break;//MoveY = -1f;
            case FacingDirection.Left:   currentAnim = walkLeftAnim;  break;//MoveX = -1f;
            case FacingDirection.Right:  currentAnim = walkRightAnim; break;//MoveX =  1f;
        }
        RefreshIdleSprites((int)dir);
    }
#endregion
#region Test
    private bool test = false;
    /// <summary>
    /// 换装测试
    /// </summary>
    public void ChangeSpritesTest()
    {
        string path;
        if(!test)
        {
            path = "PlayerSpriteBase/douye_hair1";
            test = true;
        }
        else
        {
            path = "PlayerSpriteBase/douye_hair2";
            test = false;
        }
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        List<Sprite> idleSprite      = new List<Sprite>();
        List<Sprite> walkUpSprite    = new List<Sprite>();
        List<Sprite> walkDownSprite  = new List<Sprite>();
        List<Sprite> walkLeftSprite  = new List<Sprite>();
        List<Sprite> walkRightSprite = new List<Sprite>();
        idleSprite.Add(sprites[0]);
        idleSprite.Add(sprites[3]);
        idleSprite.Add(sprites[6]);
        idleSprite.Add(sprites[9]);
        allIdleSprite[1] = idleSprite;

        walkUpSprite.Add(sprites[1]);
        walkUpSprite.Add(sprites[0]);
        walkUpSprite.Add(sprites[2]);
        walkUpSprite.Add(sprites[0]);
        allWalkUpSprites[1] = walkUpSprite;

        walkDownSprite.Add(sprites[4]);
        walkDownSprite.Add(sprites[3]);
        walkDownSprite.Add(sprites[5]);
        walkDownSprite.Add(sprites[3]);
        allWalkDownSprites[1] = walkDownSprite;

        walkLeftSprite.Add(sprites[7]);
        walkLeftSprite.Add(sprites[6]);
        walkLeftSprite.Add(sprites[8]);
        walkLeftSprite.Add(sprites[6]);
        allWalkLeftSprites[1] = walkLeftSprite;

        walkRightSprite.Add(sprites[10]);
        walkRightSprite.Add(sprites[9]);
        walkRightSprite.Add(sprites[11]);
        walkRightSprite.Add(sprites[9]);
        allWalkRightSprites[1] = walkRightSprite;

        if(player == null)
        {
            player = GetComponent<PlayerMovement>();
        }
        spriteRenderers[1].sprite = allIdleSprite[1][player.testDirection()];
    }
    PlayerMovement player;
#endregion
#region HandleUpdate
    private DressableAnimator prevAnim;
    //更新
	public void HandleUpdate(float MoveX, float MoveY, bool WasNotPreviouslyMoving)
	{
        prevAnim = currentAnim;
        if(MoveX == 1f)
        {
            if(currentAnim != walkRightAnim)
            {
                currentAnim = walkRightAnim;
            }
        }
        else if(MoveX == -1f)
        {
            if(currentAnim != walkLeftAnim)
            {
                currentAnim = walkLeftAnim;
            }
        }
        else if(MoveY == 1f)
        {
            if(currentAnim != walkUpAnim)
            {
                currentAnim = walkUpAnim;
            }
        }
        else if(MoveY == -1f)
        {
            if(currentAnim != walkDownAnim)
            {
                currentAnim = walkDownAnim;
            }
        }

        if(currentAnim != prevAnim || WasNotPreviouslyMoving)
        {
            //转向
            currentAnim.Start();
        }

        currentAnim.HandleUpdate();
	}
    public void StopMoving(int lookTowards)
    {
        //停止移动时重置
        currentAnim.Reset();

        RefreshIdleSprites(lookTowards);
    }
#endregion
#region Sprites管理
    //根据服装数量实例化相应显示
    [SerializeField] GameObject customBodyPrefab;//通用部件Prefab
	[SerializeField] SpriteRenderer _baseSpriteRenderer;//本物体SR
	private DressableAnimator walkUpAnim, walkDownAnim, walkLeftAnim, walkRightAnim;

    //存服装类型和服装道具
    private Dictionary<CostumesType, CostumesItem> playerCostumes = new Dictionary<CostumesType, CostumesItem>
    {
        {CostumesType.Cap    , null},
        {CostumesType.Hair   , null},
        {CostumesType.Blouse , null},
        {CostumesType.Pant   , null},
        {CostumesType.Shoes  , null},
        {CostumesType.Jewelry, null}
    };

    List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();//需要更新的所有SR

    //所有分类图
    List<List<Sprite>> allIdleSprite       = new List<List<Sprite>>();
    List<List<Sprite>> allWalkUpSprites    = new List<List<Sprite>>();
    List<List<Sprite>> allWalkDownSprites  = new List<List<Sprite>>();
    List<List<Sprite>> allWalkLeftSprites  = new List<List<Sprite>>();
    List<List<Sprite>> allWalkRightSprites = new List<List<Sprite>>();

    /// <summary>
    /// 实例化当前持有服饰数量SpriteRenderer Obj
    /// </summary>
    public void InstantiatePlayerSpriteRenderer()
    {
        spriteRenderers = new List<SpriteRenderer>();
        spriteRenderers.Add(_baseSpriteRenderer? _baseSpriteRenderer : GetComponent<SpriteRenderer>());
        foreach(KeyValuePair<CostumesType, CostumesItem> kvp in playerCostumes)
        {
            if(kvp.Value != null)
            {
                GameObject obj = Instantiate(customBodyPrefab, transform.position, Quaternion.identity);
                obj.transform.SetParent(transform);
                spriteRenderers.Add(obj.GetComponent<SpriteRenderer>());
            }
        }
    }

    /// <summary>
    /// 装备服饰
    /// </summary>
    /// <param name="item"></param>
    public void SetPlayerCostumes(CostumesItem item)
    {
        CostumesType type = item.CostumesType;
        bool instantiated = playerCostumes[type] != null;
        playerCostumes[type] = item;
        SetSprite(type, instantiated);
    }

    /// <summary>
    /// 重置玩家人物sprite
    /// </summary>
    /// <param name="type">服饰类型</param>
    /// <param name="instantiate">是否已经实例化</param>
    private void SetSprite(CostumesType type, bool instantiated)
    {
        if(!instantiated)
        {
            GameObject obj = Instantiate(customBodyPrefab, transform.position, Quaternion.identity);
            obj.transform.SetParent(transform);
            spriteRenderers.Add(obj.GetComponent<SpriteRenderer>());
        }
    }

    private void SetSprites(string path)
    {
        Sprite[] sprites = ResM.Instance.LoadAllSprites(path);

        List<Sprite> idleSprite      = new List<Sprite>();
        List<Sprite> walkUpSprite    = new List<Sprite>();
        List<Sprite> walkDownSprite  = new List<Sprite>();
        List<Sprite> walkLeftSprite  = new List<Sprite>();
        List<Sprite> walkRightSprite = new List<Sprite>();
        idleSprite.Add(sprites[0]);
        idleSprite.Add(sprites[3]);
        idleSprite.Add(sprites[6]);
        idleSprite.Add(sprites[9]);
        allIdleSprite.Add(idleSprite);

        walkUpSprite.Add(sprites[1]);
        walkUpSprite.Add(sprites[0]);
        walkUpSprite.Add(sprites[2]);
        walkUpSprite.Add(sprites[0]);
        allWalkUpSprites.Add(walkUpSprite);

        walkDownSprite.Add(sprites[4]);
        walkDownSprite.Add(sprites[3]);
        walkDownSprite.Add(sprites[5]);
        walkDownSprite.Add(sprites[3]);
        allWalkDownSprites.Add(walkDownSprite);

        walkLeftSprite.Add(sprites[7]);
        walkLeftSprite.Add(sprites[6]);
        walkLeftSprite.Add(sprites[8]);
        walkLeftSprite.Add(sprites[6]);
        allWalkLeftSprites.Add(walkLeftSprite);

        walkRightSprite.Add(sprites[10]);
        walkRightSprite.Add(sprites[9]);
        walkRightSprite.Add(sprites[11]);
        walkRightSprite.Add(sprites[9]);
        allWalkRightSprites.Add(walkRightSprite);
    }

    /// <summary>
    /// 设置Animator 在分配Sprite和SpriteRenderer之后
    /// </summary>
    private void SetDressableAnimator()
    {
        walkUpAnim    = new DressableAnimator(ref allWalkUpSprites    , ref spriteRenderers);
        walkDownAnim  = new DressableAnimator(ref allWalkDownSprites  , ref spriteRenderers);
        walkLeftAnim  = new DressableAnimator(ref allWalkLeftSprites  , ref spriteRenderers);
        walkRightAnim = new DressableAnimator(ref allWalkRightSprites , ref spriteRenderers);
    }

    /// <summary>
	/// 刷新不动时Sprites， 面向某个方向
	/// </summary>
	/// <param name="d">方向 enum要转int</param>
	public void RefreshIdleSprites(int d)
	{
		int i = 0;
        foreach(SpriteRenderer sr in spriteRenderers)
        {
            sr.sprite = allIdleSprite[i][d];
            i++;
        }
	}

    /// <summary>
    /// 获得人物当前面向方向
    /// </summary>
    public FacingDirection CurrentFacingDirection()
    {
        if(currentAnim == walkUpAnim)
        {
            return FacingDirection.Up;
        }
        else if(currentAnim == walkDownAnim)
        {
            return FacingDirection.Down;
        }
        else if(currentAnim == walkLeftAnim)
        {
            return FacingDirection.Left;
        }
        else
        {
            return FacingDirection.Right;
        }
    }
#endregion
}