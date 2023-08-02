using UnityEngine;
public class ObjectCtrl : MonoBehaviour, Interactable
{
    private enum GameObjectType{ GuideBoard, Painting }
    [SerializeField] GameObjectType type;
    [Header("基本信息")]
    [SerializeField] string trainerName;
    [SerializeField] Sprite faceSprite;
    [Header("对话内容")]
    [SerializeField] Dialog dialog;
    [Header("画作")]
    [SerializeField] Sprite paintings;

    public void Interact(Transform initiator)
    {
        switch(type)
        {
            case GameObjectType.Painting:

                if(DialogManager.Instance.Free)
                {
                    DialogManager.Instance.ShowPaintings(paintings);
                }
                else
                {
                    DialogManager.Instance.ClosePaintings();
                }

            break;

            case GameObjectType.GuideBoard:

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
}