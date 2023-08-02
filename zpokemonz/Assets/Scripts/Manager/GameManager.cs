using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : SingletonMono<GameManager>
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    private enum GameState { Freedom, Battle, Dialog, CutScene, Pause }

    [Header("Player")]
    [SerializeField] PlayerMovement playerCtrl;
    [SerializeField] PokemonTeam playerTeam;
    [SerializeField] Inventory playerInventory;

    [Header("Other")]
    [SerializeField] BattleSystem battlesys;
    [SerializeField] GameObject FreedomLight;
    [SerializeField] LoadingScene loading;

    public SceneDetails CurrentSceneD{ get; private set; }
    public SceneDetails PrevSceneD{ get; private set; }

    GameState state;
    GameState stateBeforePause;
    private int currentScene = 1;
    /// <summary>
    /// 现在是否是自由状态
    /// </summary>
    public bool FreedomState => state == GameState.Freedom;
    /// <summary>
    /// 现在是否是战斗状态
    /// </summary>
    public bool BattleState => state == GameState.Battle;
    public PlayerMovement Player  => playerCtrl;
    public PokemonTeam PlayerTeam => playerTeam;
    public Inventory Inventory => playerInventory;
    public int CurrentScene => currentScene;

    protected override void Awake()
    {
        base.Awake();
        AllPokemon.Init();
        AllSkill.Init();
        AllConditionData.Init();
    }

    private void Start()
    {
        battlesys.OnBattleOver += EndBattle;
        //对话框
        DialogManager.Instance.OnShowDialog  += OnShowDialog;
        DialogManager.Instance.OnCloseDialog += OnCloseDialog;
    }

    private void OnShowDialog()
    {
        ChangeGameState(GameState.Dialog);
        UIManager.Instance.DialogCover();
    }

    private void OnCloseDialog()
    {
        if(state == GameState.Dialog)
        {
            ChangeGameState(GameState.Freedom);
            UIManager.Instance.DialogResume();
        }
    }

    private void Update()
    {
        if(state == GameState.Freedom)
        {
            playerCtrl.HandleUpdate();
        }
    }

    /// <summary>
    /// 切换游戏状态
    /// </summary>
    /// <param name="_state"></param>//
    private void ChangeGameState(GameState _state)
    {
        //Debug.Log($"{state.ToString()}改为{_state.ToString()}");
        if(state != _state)
        {
            state = _state;
            //if(battlesys.isActiveAndEnabled && state != GameState.Battle)
            //{
            //    Debug.LogError("error");
            //}
        }
    }

    public void PauseGame(bool pause)
    {
        if(pause)
        {
            stateBeforePause = state;
            ChangeGameState(GameState.Pause);
        }
        else
        {
            if(state != GameState.Battle)
            {
                ChangeGameState(stateBeforePause);
            }
        }
    }
#region 对战管理
    public void StartBattle(PokemonBase pBase, int level)//从草里传 不find
    {
        //进入野外战斗
        ReadyToBattle();
        battlesys.StartWildBattle(playerCtrl, playerTeam, new Pokemon(pBase, level));
    }

    public void StartTrainerBattle(TrainerCtrller trainer)
    {
        //进入训练家对战
        ReadyToBattle();
        battlesys.StartTrainerBattle(playerCtrl, playerTeam, trainer);
        UIManager.Instance.DialogResume();
    }

    void ReadyToBattle()
    {
        loading.BattleFade();
        WeatherManager.Instance.CloseWeather();
        ChangeGameState(GameState.Battle);
        UIManager.Instance.OnClose();
        AudioManager.Instance.BattleBGM();
        //FreedomLight.SetActive(false);
    }

    /// <summary>
    /// 进入事件视野
    /// </summary>
    /// <param name="_action"></param>
    public void OnEnterCharacterView(Action<PlayerMovement> _action)
    {
        ChangeGameState(GameState.CutScene);
        playerCtrl.ResetInteractable();
        _action?.Invoke(playerCtrl);
    }

    void EndBattle(bool won)
    {
        ChangeGameState(GameState.Freedom);

        AudioManager.Instance.NormalBgm();
        //FreedomLight.SetActive(true);
        UIManager.Instance.ExitBattle();

        battlesys.gameObject.SetActive(false);

        AudioManager.Instance.ReStartBAD();
    }
#endregion
#region 场景管理
    public async void LoadScene(Vector2 go, PortalDirection d, int sceneNum)
    {
        UIManager.Instance.OnClose();
        ChangeGameState(GameState.Pause);
        loading.NormalBlackPanel();
        await UniTask.Delay(1000);

        if(sceneNum != 0)//场景切换判断
        {
            //yield return SceneManager.LoadSceneAsync(sceneNum);
            SceneManager.LoadScene(sceneNum);
            currentScene = sceneNum;
        }
        //GC.Collect();
        playerCtrl.GoSomeWhere(go, d);
        loading.ExitNormalBlackPanel();
        ChangeGameState(GameState.Freedom);
        UIManager.Instance.OnOpen();
    }

    public async void LoadScene(int n)
    {
        ChangeGameState(GameState.Pause);

        SceneManager.LoadScene(n);
        currentScene = n;

        //平滑过渡
        loading.NormalBlackPanelQuickFade();
        //GC.Collect();
        await UniTask.Delay(1500);
        loading.ExitNormalBlackPanel();

        ChangeGameState(GameState.Freedom);
    }

    /// <summary>
    /// 设置当前场景
    /// </summary>
    /// <param name="currScene"></param>
    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevSceneD = CurrentSceneD;
        CurrentSceneD = currScene;
    }
#endregion
}