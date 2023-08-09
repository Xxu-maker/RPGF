using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;//NewInputSystem使用时有一些bug
public class PlayerMovement : ZSavable//, Interactable
{
	[SerializeField] Transform _trans;
	[SerializeField] string playerName;
	[SerializeField] List<Sprite> playerSprite;
    [SerializeField] PlayerAnimator playerAnimator;
	[SerializeField] Follow follow;
	[SerializeField] PokemonTeam team;
	protected float _horizontalMovement;
	protected float _verticalMovement;
	private bool isSurfing = false;
	private bool isRunning = false;
	private bool isRiding = false;
	public string PlayerName => playerName;
    public List<Sprite> PlayerSprite => playerSprite;
#region 跑步单车冲浪按键(没弄 暂时废弃)
    /// <summary>
    /// 跑步键
    /// </summary>
	public void RunButton()
	{
		//if(isSurfing)
		//{
		//	return;
		//}
        //if(!isRunning)
		//{
		//	ChangeSpriteAnim(runUp, runDown, runLeft, runRight, idleSprite);
		//	SetMove(true, 6f);
		//}
		//else
		//{
		//	ResetMove();
		//}
	}

	/// <summary>
	/// 骑车键
	/// </summary>
	public void BicycleButton()
	{
		//if(isSurfing)
		//{
		//	return;
		//}

        //if(!isRiding)
		//{
		//	ChangeSpriteAnim(bUp, bDown, bLeft, bRight, idleBicycleSprite);
		//	SetMove(false, 8.5f);
		//}
		//else
		//{
		//	ResetMove();
		//}
	}

	public void Surf(bool surfing)
	{
		//if(surfing)
		//{
		//	follow.OnClose();
		//	ChangeSpriteAnim(surfUp, surfDown, surfLeft, surfRight, idleSurfSprite);
		//}
		//ResetMove(surfing);
		//isSurfing = surfing;
	}

	private void SetMove(bool ror, float speed)
	{
		//isRunning = ror;
		//isRiding = !ror;
        //moveSpeed = speed;
		//follow.moveSpeed = speed;
	}

	public void ResetMove(bool isSurfing = false)
	{
		//if(!isSurfing)
		//{
		//	ChangeSpriteAnim(walkUp, walkDown, walkLeft, walkRight, idleSprite);
		//}
		//isRiding = false;
		//isRunning = false;
		//moveSpeed = 4.5f;
		//follow.moveSpeed = 4.5f;
	}
	public int testDirection()
	{
		if(MoveY == 1f && MoveX == 0f)
		{
			return 0;
		}
		else if(MoveY == -1f && MoveX == 0f)
		{
			return 1;
		}
		else if(MoveY == 0f && MoveX == -1f)
		{
			return 2;
		}
		else if(MoveY == 0f && MoveX == 1f)
		{
			return 3;
		}
        else
		{
			return (int)playerAnimator.CurrentFacingDirection();
		}
	}
#endregion
#region TouchAxis(十字键)
	public virtual void SetHorizontalAxis(float value)//设置角色的水平移动
	{
		if(_horizontalMovement != value)
	    {
			_horizontalMovement = value;
		}
	}
	public virtual void SetVerticalAxis(float value)//设置角色的垂直移动
	{
		if(_verticalMovement != value)
	    {
			_verticalMovement = value;
		}
	}
#endregion
#region HandleUpdate
	public void HandleUpdate()
	{
		//Todo 大剧情状态下不可触发其它事件
		if (GameManager.Instance.AVGState)
		{
			return;
		}
		
		if(!IsMoving)
		{
			if(_horizontalMovement != _verticalMovement)
			{
				Move();
			}

            if(WasNotPreviouslyMoving)
            {
                playerAnimator.StopMoving(MoveY == 1f? 0 : MoveY == -1f? 1 : MoveX == -1f? 2 : MoveX == 1f? 3 : 0);
            }
		}
        else
        {
            playerAnimator.HandleUpdate(MoveX, MoveY, WasNotPreviouslyMoving);
        }

        wasPreviouslyMoving = IsMoving;
	}
    private bool WasNotPreviouslyMoving => IsMoving != wasPreviouslyMoving;
#endregion
#region 对话前前方块检测
	Interactable currentTalk;
	public void Interact()
	{
		//Todo 大剧情状态下不可触发其它事件
		if (GameManager.Instance.AVGState)
		{
			return;
		}
		
		if(currentTalk != null)
		{
			if(GameManager.Instance.FreedomState)
			{
				currentTalk = null;//重置
			}
			else
			{
			    currentTalk.Interact(_trans);//继续对话
				return;
			}
		}
		//查找collider
		Collider2D collider = Physics2D.OverlapCircle
		(
			_trans.position + new Vector3(MoveX, MoveY, 0f),//FacingDir(interactPos)
			0.2f,
			GameLayers.Instance.TalkLayer
		);

		if(collider != null)
		{
            currentTalk = collider.GetComponent<Interactable>();
			currentTalk.Interact(_trans);
		}
	}

	public void ResetInteractable()
	{
		currentTalk = null;
	}

	public void NewInputSysInteract(InputAction.CallbackContext ctx)
	{
		if(ctx.phase == InputActionPhase.Performed)
		{
			Interact();
		}
	}
#endregion
#region 碰撞检测
	Collider2D[] collider2Ds = new Collider2D[1];
	private Vector3 outSide = new Vector3(0f, 0.3f, 0f);
	PlayerTrigger triggerable;
    private void OnMoveOver()
	{
		//point	圆形的中心 radius圆形的半径 results用于接收结果的数组
		//layerMask 筛选器,用于检查仅在指定层上的对象
		Physics2D.OverlapCircleNonAlloc
		(
			_trans.position - outSide,
		    0.2f,
			collider2Ds,
			GameLayers.Instance.TriggerableLayers
		);

		if(collider2Ds[0] == null)
		{
			triggerable = null;
			return;
		}

		if(triggerable != null)
		{
		    triggerable.OnPlayerTrigger();
		    collider2Ds[0] = null;
		}
		else
		{
			triggerable = collider2Ds[0].GetComponent<PlayerTrigger>();
			if(triggerable != null)
			{
			    triggerable.OnPlayerTrigger();
			    IsMoving = false;
			    collider2Ds[0] = null;
			}
		}
	}
#endregion
#region 换场景位置调整
	public void GoSomeWhere(Vector2 termini, PortalDirection d)//目的地 进口方向
	{
		_trans.position = termini;
		//跟随位置调整(小体型精灵)
		switch(d)
        {
            case PortalDirection.Up:    termini.y -= 1.09f; break;
            case PortalDirection.Down:  termini.y += 0.91f; break;
            case PortalDirection.Left:  termini.x += 1f; termini.y -= 0.09f; break;
            case PortalDirection.Right: termini.x -= 1f; termini.y -= 0.09f; break;
        }
		follow.transform.position = termini;
		triggerable = null;
	}
#endregion
#region 设置跟随宝可梦
	public void SetFollowPokemon(int id)
	{
		if(isSurfing )//|| !character.C_Moving)
		{
			return;
		}
		Pokemon pokemon = team.Pokemons[id];
		follow.ResetPokemon(pokemon.Base.ID, pokemon.Shiny, _trans.position);
	}
#endregion
#region Character
    [SerializeField] float moveSpeed;
    private bool IsMoving;
    private float MoveX;
    private float MoveY;
    private bool wasPreviouslyMoving;

    public async void Move()
    {
        IsMoving = true;
        Vector3 targetPos = _trans.position;
        if(_horizontalMovement != 0)
        {
            targetPos.x += _horizontalMovement;
        }
        else if(_verticalMovement != 0)
        {
            targetPos.y += _verticalMovement;
        }

        //设置MoveX Y控制动画
        MoveX = _horizontalMovement;
        MoveY = _verticalMovement;

        if(!IsPlayerPathClear(targetPos))
        {
            AudioManager.Instance.CantMoveAudio();
            IsMoving = false;
            return;
        }

        //follow移动
        follow.MoveHandler(_horizontalMovement, _verticalMovement);
        while ((targetPos - _trans.position).sqrMagnitude > Mathf.Epsilon)
        {
            _trans.position = Vector3.MoveTowards(_trans.position, targetPos, moveSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
        _trans.position = targetPos;
        IsMoving = false;
        OnMoveOver();
    }

    /// <summary>
    /// 玩家专用检查路径通畅
    /// </summary>
    /// <param name="targetPos">目的地</param>
    /// <returns>是否通畅</returns>
    private bool IsPlayerPathClear(Vector3 targetPos)
    {
        //Physics2D.BoxCast
        //origin   盒体在2D空间中的起点   size      盒体的大小
        //angle    盒体的角度(以度为单位) direction 表示盒体方向的矢量。
        //distance 盒体的最大投射距离     layerMask 过滤器,用于仅在特定层上检测碰撞体

        Vector3 diff = targetPos - _trans.position;
        Vector3 dir = diff.normalized;
        Vector2 origin = _trans.position + dir;
        float distance = diff.magnitude - 1;
        //先检测一遍移动人物，减少人物穿越bug *//可能有用，但不多, 如果都测为空的瞬间还是会穿, 只想到标记网格的方法，但太复杂
        if(Physics2D.BoxCast(origin, MyData.rectangularCast, 0f, dir, distance, GameLayers.Instance.NpcLayer))
        {
            return false;
        }
        return !Physics2D.BoxCast(origin, MyData.boxCast, 0f, dir, distance, GameLayers.Instance.BuildingLayer);
    }

    public void LookTowards(Vector3 targetPos)
    {
        float xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(_trans.position.x);
        float ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(_trans.position.y);
        if(xdiff == 0 || ydiff == 0)
        {
            MoveX = xdiff;
            MoveY = ydiff;
            if(!IsMoving)
            {
		    	playerAnimator.RefreshIdleSprites(MoveY == 1f? 0 : MoveY == -1f? 1 : MoveX == -1f? 2 : MoveX == 1f? 3 : 0);
            }
        }
    }

    /// <summary>
    /// Tap按键转向
    /// </summary>
    /// <param name="d"></param>
    public void SetPlayerDirection(int direction)
    {
        if(!IsMoving)
        {
            playerAnimator.RefreshIdleSprites(direction);
            switch(direction)
            {
                case 0: MoveY =  1f; MoveX = 0f; break;
                case 1: MoveY = -1f; MoveX = 0f; break;
                case 2: MoveX = -1f; MoveY = 0f; break;
                case 3: MoveX =  1f; MoveY = 0f; break;
            }
        }
    }
#endregion
#region 存取数据
    public override object CaptureState()
	{
		Vector3 currentPos = _trans.position;
		PlayerSaveData saveData = new PlayerSaveData()
		{
			scene = GameManager.Instance.CurrentScene,
			x = currentPos.x,
			y = currentPos.y,
			z = currentPos.z,
			isSurfing = isSurfing,
			isRiding = isRiding,
			isRunning = isRunning,
			boxesSaveData = team.GetPlayerTeamData(),
			followSaveData = follow.CaptureState()
		};
		return saveData;
	}

	public override void RestoreState(object state)
	{
		PlayerSaveData saveData = (PlayerSaveData)state;

		//宝可梦
		team.SetTeamData(saveData.boxesSaveData);

	    //follow数据
		follow.RestoreState(saveData.followSaveData);

		if(saveData.isRiding)
		{
			BicycleButton();
		}
		else if(saveData.isSurfing)
		{
			Surf(true);
		}
		else if(saveData.isRunning)
		{
			RunButton();
		}

		//玩家位置
		_trans.position = new Vector3(saveData.x, saveData.y, saveData.z);
	}
#endregion
}

[System.Serializable]
public class PlayerSaveData
{
	public int scene;
	public float x, y, z;
	public bool isSurfing;
	public bool isRunning;
	public bool isRiding;
	public PokemonSaveData[][] boxesSaveData;
	public FollowSaveData followSaveData;
}