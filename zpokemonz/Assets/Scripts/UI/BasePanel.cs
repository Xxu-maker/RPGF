using UnityEngine;
public class BasePanel : MonoBehaviour
{
    [SerializeField] CanvasGroup canvas;
    protected CanvasGroup Canvas => canvas;

    public virtual void OnOpen()//打开
    {
        if(canvas.alpha != 1)
        {
            canvas.alpha = 1;
        }
        if(!canvas.blocksRaycasts)
        {
            canvas.blocksRaycasts = true;
        }
        if(!canvas.interactable)
        {
            canvas.interactable = true;
        }
    }

    public virtual void OnClose()//退出
    {
        if(canvas.alpha != 0)
        {
            canvas.alpha = 0;
        }
        if(canvas.blocksRaycasts)
        {
            canvas.blocksRaycasts = false;
        }
        if(canvas.interactable)
        {
            canvas.interactable = false;
        }
    }

    /// <summary>
    /// CanvasGroup开关
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="open"></param>
    public virtual void ShowOrHide(CanvasGroup canvas, bool open)
    {
        canvas.alpha = open? 1 : 0;
        canvas.interactable = open;
        canvas.blocksRaycasts = open;
    }

    public virtual void SetData(Pokemon pokemon) { }
}