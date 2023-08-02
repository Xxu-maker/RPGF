using Cysharp.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 对战训练家
/// </summary>
public class TrainerCtrller : ZSavable, Interactable
{
    [Header("基本信息")]
    [SerializeField] string trainerName;
    [SerializeField] Sprite faceSprite;
    [SerializeField] Sprite trainerSprite;
    [SerializeField] Pokemon[] pokemons = new Pokemon[6];
    [Header("对话内容")]
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogLost;
    [Header("对战视野")]
    [SerializeField] GameObject exclamation;
    [SerializeField] BoxCollider2D fov;
    [SerializeField] bool battleLost;
    [SerializeField] Character character;
    private int nowPokemonNum;
    public string TrainerName => trainerName;
    public Sprite TrainerSprite => trainerSprite;

    private void Start()
    {
        if(battleLost)
        {
            fov.isTrigger = false;
        }
        SetFovRotation(character.Animator.DefaultDirection);
        foreach(Pokemon pokemon in pokemons)//初始化team
        {
            if(pokemon.Base != null)
            {
                pokemon.Init();
            }
        }
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public async void TriggerTrainerBattle(PlayerMovement player)
    {
        exclamation.SetActive(true);
        AudioManager.Instance.ViewPlayer();
        await UniTask.Delay(500);
        exclamation.SetActive(false);

        Vector3 diff = player.transform.position - transform.position;
        Vector2 moveVec = diff - diff.normalized;
        moveVec.x = Mathf.Round(moveVec.x);
        moveVec.y = Mathf.Round(moveVec.y);
        if(moveVec.y == -1f)
        {
            moveVec.y = 0f;
        }
        if(moveVec != Vector2.zero)
        {
            await character.IsFinishMoving(moveVec, null);
        }
        character.LookTowards(player.transform.position);

        DialogManager.Instance.Info( dialog, trainerName, faceSprite, Battle );

        player.LookTowards(transform.position);

        fov.isTrigger = false;
    }
    private void Battle() { GameManager.Instance.StartTrainerBattle(this); }

    public void Interact(Transform initiator)
    {
        if(DialogManager.Instance.Free)
        {
            character.LookTowards(initiator.position);
            if(!battleLost)
            {
                fov.isTrigger = false;
                DialogManager.Instance.Info( dialog, trainerName, faceSprite, Battle );
            }
            else
            {
                DialogManager.Instance.Info(dialogLost, trainerName, faceSprite);
            }
        }
        else
        {
            DialogManager.Instance.Typing();
        }
    }
    public void SetFovRotation(FacingDirection dir)
    {
        Vector3 vec = Vector3.zero;
        switch(dir)
        {
            case FacingDirection.Up:    vec.z = 180f; break;
            //case FacingDirection.Down:  vec.z = 0f;   break;
            case FacingDirection.Left:  vec.z = 270f; break;
            case FacingDirection.Right: vec.z = 90f;  break;
        }
        fov.transform.eulerAngles = vec;
    }
    public Pokemon GetHealthyPokemon()
    {
        nowPokemonNum = 0;
        foreach(Pokemon pokemon in pokemons)
        {
            if(pokemon.Base != null && pokemon.isActive)
            {
                ++nowPokemonNum;
                return pokemon;
            }
        }
        return null;
    }
    private void CurePokemon()
    {
        foreach(Pokemon pokemon in pokemons)
        {
            if(pokemon.Base == null)
            {
                break;
            }
            pokemon.OnCureAll();
        }
    }
    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }
#region 存储和读取
    public override object CaptureState()
    {
        return battleLost;
    }

    public override void RestoreState(object state)
    {
        battleLost = (bool)state;
        if(battleLost)
        {
            fov.gameObject.SetActive(false);
        }
    }
#endregion
}