using System.Collections.Generic;
using UnityEngine;
public class SymbolEncounterCtrl : MonoBehaviour, Interactable
{
    [Header("基本信息")]
    [SerializeField] string trainerName;
    [SerializeField] Sprite faceSprite;
    [SerializeField] PokemonBase pokemonBase;
    [Header("对话内容")]
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogLost;
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
        if(DialogManager.Instance.Free)
        {
            character.LookTowards(initiator.position);
            GameManager.Instance.StartBattle(pokemonBase, Random.Range(10, 20));
            gameObject.SetActive(false);
            Destroy(this, 2f);
            //DialogManager.Instance.Info(dialogLost, trainername, faceSprite);
        }
    }
}