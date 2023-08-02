using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
/// <summary>
/// 对话文本
/// </summary>
public class DialogManager : SingletonMono<DialogManager>
{
    [SerializeField] Text npcNameText;
    [SerializeField] Image faceImage;
    [SerializeField] Text dialogText;
    [SerializeField] CanvasGroup dialogBoxCG;
    [SerializeField] Dialog pickUpDialog;
    [SerializeField] Image paintings;
    [SerializeField] CanvasGroup paintingsCG;
    [SerializeField] BoolSelectionTip questionSelection;
    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    private Dialog dialog;
    private event Action OnDialogFinished;
    public bool IsShowing { get; private set; }
    public bool Free = true;

    /// <summary>
    /// 首次传入Dialog
    /// </summary>
    public void Info(Dialog dia, string name, Sprite face, Action onFinished = null)
    {
        Free = false;
        dialog = dia;
        npcNameText.text = name;
        faceImage.color = Color.white;
        faceImage.sprite = face;
        OnDialogFinished = onFinished;
        TypingSet();
    }

    /// <summary>
    /// 捡到道具时传入
    /// </summary>
    public void PickUpItemsInfo(string itemName)
    {
        Free = false;
        OnDialogFinished = null;
        pickUpDialog.Lines[0] = string.Concat("捡到了", itemName, "!");
        dialog = pickUpDialog;
        npcNameText.text = "";
        faceImage.color = MyData.hyaline;
        TypingSet();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void TypingSet()
    {
        IsShowing = true;
        dialogBoxCG.alpha = 1;
        lineCount = dialog.Lines.Count;
        OnShowDialog?.Invoke();
        Typing();
    }

    private int lineCount;
    private int currentLine;
    private bool typing;
    /// <summary>
    /// 输出字
    /// </summary>
    public async void Typing()
    {
        if(typing) { return; }
        typing = true;
        if(currentLine == lineCount)
        {
            ClearDialog();
            typing = false;
            return;
        }

        dialogText.text = "";
        char[] currentLetters = dialog.Lines[currentLine].ToCharArray();
        foreach(char letter in currentLetters)
        {
            dialogText.text += letter;
            await UniTask.Delay(50);
        }
        currentLine++;
        typing = false;
    }

    /// <summary>
    /// 墙上的画
    /// </summary>
    /// <param name="sprite"></param>
    public void ShowPaintings(Sprite sprite)
    {
        Free = false;
        paintings.sprite = sprite;
        paintingsCG.alpha = 1;
    }

    /// <summary>
    /// 关闭画
    /// </summary>
    public void ClosePaintings()
    {
        Free = true;
        paintingsCG.alpha = 0;
    }

    /// <summary>
    /// 清空Dialog
    /// </summary>
    public void ClearDialog()
    {
        currentLine = 0;
        dialogBoxCG.alpha = 0;
        IsShowing = false;
        OnDialogFinished?.Invoke();
        OnCloseDialog?.Invoke();
        Free = true;
    }

    /// <summary>
    /// 选择
    /// </summary>
    public void SetSelection(Dialog _dialog, Action<bool> _action)
    {
        Info(_dialog, null, null);
        questionSelection.SetData(null, _action);
    }
}