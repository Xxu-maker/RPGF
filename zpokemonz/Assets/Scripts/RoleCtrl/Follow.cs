using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
/// <summary>
/// 跟随宝可梦
/// </summary>
public class Follow : MonoBehaviour, Interactable
{
    public bool isRes;
    public string path;
    [SerializeField] Transform _trans;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Dialog dialog;
    private List<Sprite> walkUpSprite = new List<Sprite>();
    private List<Sprite> walkDownSprite = new List<Sprite>();
    private List<Sprite> walkLeftSprite = new List<Sprite>();
    private List<Sprite> walkRightSprite = new List<Sprite>();
    public float moveSpeed;
    /// <summary>
    /// 玩家上一次的X位移,follow的下次位移
    /// </summary>
    private float x = 0;
    /// <summary>
    /// 玩家上一次的Y位移,follow的下次位移
    /// </summary>
    private float y = -1;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator currentAnim;
    //能跟上就不用
    //Queue<Vector2> queue = new Queue<Vector2>(); Enqueue加 Dequeue删

    bool isActive;
    private int currentID = 25;

    private void Start()
    {
        if(isRes)
        {
            SetNpcSprite(path);
        }
        walkUpAnim    = new SpriteAnimator(walkUpSprite, spriteRenderer);
        walkDownAnim  = new SpriteAnimator(walkDownSprite, spriteRenderer);
        walkLeftAnim  = new SpriteAnimator(walkLeftSprite, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprite, spriteRenderer);
        currentAnim   = walkDownAnim;
        isActive      = true;
    }

    /// <summary>
    /// 根据玩家Input计算follow移动
    /// </summary>
    /// <param name="inputX"></param>
    /// <param name="inputY"></param>
    /// <returns>Vector2 move</returns>
    public async void MoveHandler(float _x, float _y)
    {
        Vector3 targetPos = _trans.position;
        targetPos.x += x;
        targetPos.y += y;

        if      (_x > 0f && x !=  1f)
        {
            x =  1f; y =  0f; currentAnim = walkRightAnim;
        }
        else if (_x < 0f && x != -1f)
        {
            x = -1f; y =  0f; currentAnim = walkLeftAnim;
        }
        else if (_y > 0f && y !=  1f)
        {
            x =  0f; y =  1f; currentAnim = walkUpAnim;
        }
        else if (_y < 0f && y != -1f)
        {
            x =  0f; y = -1f; currentAnim = walkDownAnim;
        }

        while((targetPos - _trans.position).sqrMagnitude > Mathf.Epsilon)
        {
            _trans.position =
	    	    Vector3.MoveTowards(_trans.position, targetPos, moveSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
        _trans.position = targetPos;
    }

    private void Update()
    {
        currentAnim.HandleUpdate();
    }

    /// <summary>
    /// 关掉follow
    /// </summary>
    public void OnClose()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 重置Pokemon
    /// </summary>
    /// <param name="pokeID"></param>
    /// <param name="isShiny"></param>
    /// <param name="pos"></param>
    public void ResetPokemon(int pokeID, bool isShiny, Vector3 pos)
    {
        if(currentID == pokeID)
        {
            return;
        }

        /*if(x != 0)
        {
            pos.x += x > 0? -1 : 1;
            pos.y -= 0.09f;
        }
        else
        {
            if(y > 0)
            {
                pos.y -= 1.09f;
            }
            else
            {
                pos.y += 0.91f;
            }
        }*/
        //_trans.position = pos;

        walkUpSprite.Clear();
        walkDownSprite.Clear();
        walkLeftSprite.Clear();
        walkRightSprite.Clear();
        currentID = pokeID;
        SetNpcSprite($"Pokemon/Follow/{pokeID}f/");

        if(!isActive)
        {
            gameObject.SetActive(true);
        }
    }

    public void SetNpcSprite(string loadPath)
    {
        Sprite[] sprites = ResM.Instance.LoadAllSprites(loadPath);
        if(sprites == null || sprites.Length == 0)
        {
            currentID = 54;
            sprites = ResM.Instance.LoadAllSprites("Pokemon/Follow/54f/");
        }

        if(sprites.Length < 9)
        {
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

    public void Interact(Transform initiator)
    {
        if(DialogManager.Instance.Free)
        {
            DialogManager.Instance.Info
            (
                dialog, AllPokemon.GetPokemonByID(currentID).Name, null
            );
        }
        else
        {
            DialogManager.Instance.Typing();
        }
    }

#region 存储和读取数据
    public FollowSaveData CaptureState()
	{
        Vector3 currentPos = _trans.position;
        FollowSaveData saveData = new FollowSaveData()
        {
            pokemonID = currentID,
            x = currentPos.x,
            y = currentPos.y,
            z = currentPos.z,
            nextX = x,
            nextY = y,
            isActive = isActive
        };
		return saveData;
	}

	public void RestoreState(FollowSaveData saveData)
	{
		_trans.position = new Vector3(saveData.x, saveData.y, saveData.z);
        x = saveData.nextX; y = saveData.nextY;
        if(isActive)
        {
            currentID = saveData.pokemonID;
            //就开follow，false就关了
        }
	}
    #endregion
}

[System.Serializable]
public struct FollowSaveData
{
    public int pokemonID;
    public float x, y, z;
    public float nextX;//follow的下次位移，玩家的上次位移
    public float nextY;
    public bool isActive;
}