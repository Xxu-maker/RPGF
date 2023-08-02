using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 用于战斗时背景面板切换
/// </summary>
public class BattleBackGround : MonoBehaviour
{
    [SerializeField] CanvasGroup canvas;
    [SerializeField] Image backGround;
    [SerializeField] Image playerStage;
    [SerializeField] Image enemyStage;
    public void ChangeBackGround()
    {
        int hh = System.DateTime.Now.Hour;
    }
}