using UnityEngine;
/// <summary>
/// 不移动的交互物体
/// </summary>
public class StaticInteractionObject : MonoBehaviour, Interactable
{
    private enum ObjectType{ TwoPatterns, GuideBoard, Painting }
    [Header("基本信息")]
    [SerializeField] string trainerName;
    [SerializeField] Sprite faceSprite;
    [SerializeField] ObjectType type;

    [Header("互动脚本Base")]
    [SerializeField] InteractionObjectBase _base;

    [Header("对话内容")]
    [SerializeField] Dialog dialog;

    [Header("画作")]
    [SerializeField] Sprite paintings;

    public void Interact(Transform initiator)
    {
        switch(type)
        {
            case ObjectType.TwoPatterns:

                if(DialogManager.Instance.Free)
                {
                    DialogManager.Instance.SetSelection(dialog, Selection);
                    //(bool value) => { if(value) {_base._Active();} DialogManager.Instance.ClearDialog(); }
                }

            break;

            case ObjectType.Painting:

                if(DialogManager.Instance.Free)
                {
                    DialogManager.Instance.ShowPaintings(paintings);
                }
                else
                {
                    DialogManager.Instance.ClosePaintings();
                }

            break;

            case ObjectType.GuideBoard:

                if(DialogManager.Instance.Free)
                {
                    DialogManager.Instance.Info(dialog, trainerName, faceSprite);
                }
                else
                {
                    DialogManager.Instance.Typing();
                }

            break;
        }
    }

    private void Selection(bool yes)
    {
        if(yes)
        {
            _base._Active();
        }
        DialogManager.Instance.ClearDialog();
        GameManager.Instance.Player.ResetInteractable();
    }
}