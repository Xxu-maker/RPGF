using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
/// <summary>
/// 过渡画面
/// </summary>
public class LoadingScene : MonoBehaviour
{
    [SerializeField] CanvasGroup blackFadePanel;
    [SerializeField] CanvasGroup circleFadeCG;
    [SerializeField] Image circleFadeImage;

    private void Awake()
    {
        blackFadePanel.alpha = 1;
        ExitNormalBlackPanel();
    }

    /// <summary>
    /// 黑屏
    /// </summary>
    public void NormalBlackPanel()
    {
        blackFadePanel.DOFade(1f, 1f);
    }

    /// <summary>
    /// 结束过渡
    /// </summary>
    public void ExitNormalBlackPanel()
    {
        blackFadePanel.DOFade(0f, 1f);
    }

    public void NormalBlackPanelQuickFade()
    {
        blackFadePanel.DOFade(1f, 0.5f);
    }

    /// <summary>
    /// 战斗过渡
    /// </summary>
    public async void BattleFade()
    {
        circleFadeCG.alpha = 1;
        circleFadeImage.DOFillAmount(1f, 0.5f);

        await UniTask.Delay(1000);

        circleFadeCG.DOFade(0f, 1f);
        circleFadeImage.fillAmount = 0f;
    }
}