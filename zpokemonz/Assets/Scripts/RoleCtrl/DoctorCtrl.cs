using System.Collections.Generic;
using UnityEngine;
public class DoctorCtrl : MonoBehaviour, Interactable
{
    [Header("基本信息")]
    [SerializeField] string doctorName;
    [SerializeField] Sprite faceSprite;
    [Header("对话内容")]
    [SerializeField] Dialog dialog;
    [Header("移动顺序和间隔")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
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
                if(movementPattern.Count > 0)
                {
                    #pragma warning disable 4014//不需要等待
                    character.IsFinishMoving(movementPattern[currentPattern], FinishMoving);
                }
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
        if(DialogManager.Instance.Free)
        {
            state = NpcState.Talking;
            character.LookTowards(initiator.position);
            DialogManager.Instance.Info(dialog, doctorName, faceSprite,
                () => {idleTimer = 0f; state = NpcState.Idle;});

            GameManager.Instance.PlayerTeam.CurePokemon();
            AudioManager.Instance.CurePokemon();
            UIManager.Instance.UpdateCirclePanel();
        }
        else
        {
            DialogManager.Instance.Typing();
        }
    }
}