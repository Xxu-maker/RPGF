using System.Collections.Generic;
//using Cysharp.Threading.Tasks;
using UnityEngine;
/// <summary>
/// NPC状态
/// </summary>
public enum NpcState{Idle, Walking, Talking}
/// <summary>
/// NPC职业
/// </summary>
public enum NpcVocation{None, Nurse}
public class NPCCtrller : ZSavable, Interactable
{
    [Header("基本信息")]
    [SerializeField] string trainerName;
    [SerializeField] Sprite faceSprite;
    //[SerializeField] NpcVocation vocation;
    [Header("对话内容")]
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog otherDialog;
    [Header("移动顺序和间隔")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    [Header("持有道具")]
    [SerializeField] List<ItemSlot> items;
    [SerializeField] bool isGiveItems;
    [SerializeField] Character character;
    NpcState state;
    float idleTimer = 0f;
    int currentPattern = 0;

    private void Update()
    {
        if(state == NpcState.Idle)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                state = NpcState.Walking;
                #pragma warning disable 4014//不需要等待
                character.IsFinishMoving(movementPattern[currentPattern], FinishMoving);
            }
        }
        character.HandleUpdate();
    }

    public void FinishMoving(bool positionMoved)
    {
        if(positionMoved)
        {
            currentPattern = (currentPattern +1) % movementPattern.Count;
        }
        state = NpcState.Idle;
    }

    public void Interact(Transform initiator)
    {
        if(state == NpcState.Idle || state == NpcState.Talking)
        {
            if(DialogManager.Instance.Free)
            {
                state = NpcState.Talking;
                character.LookTowards(initiator.position);
                if(!isGiveItems)
                {
                    DialogManager.Instance.Info
                    (
                        otherDialog, trainerName, faceSprite,
                        () => {idleTimer = 0f; state = NpcState.Idle;}
                    );
                    GameManager.Instance.Inventory.LayInItemList(items);
                    AudioManager.Instance.GetItemsAudio();
                    isGiveItems = true;
                    items = null;
                }
                else
                {
                    DialogManager.Instance.Info
                    (
                        dialog, trainerName, faceSprite,
                        () => {idleTimer = 0f; state = NpcState.Idle;}
                    );
                }
            }
            else
            {
                DialogManager.Instance.Typing();
            }
        }
    }

    public void SetBaseData(string name, string id, RuleMove ruleMove, string s)
    {
        if(character.Animator.LoadSprites("Pokemon/Follow/" + id + "f" + s))
        {
            trainerName = name;
            faceSprite = ResM.Instance.Load<Sprite>("Pokemon/Expression/" + id);
            dialog = ruleMove.dialog;
            movementPattern = ruleMove.move;
            timeBetweenPattern = ruleMove.intervalTime;
        }
        else
        {
            character.Animator.LoadSprites("Pokemon/Follow/25f");
            trainerName = "还没有做这只宝可梦";
            faceSprite = ResM.Instance.Load<Sprite>("Pokemon/Expression/25");
            dialog = ruleMove.dialog;
            movementPattern = ruleMove.move;
            timeBetweenPattern = ruleMove.intervalTime;
        }
    }


    public override object CaptureState()
    {
        return isGiveItems;
    }
    public override void RestoreState(object state)
    {
        isGiveItems = (bool)state;
    }
}
//public struct NormalNpcSaveData