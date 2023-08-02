using UnityEngine;
using UnityEngine.SceneManagement;
public class PausePanel : BasePanel
{
    [SerializeField] CoreObject core;
    public void ExitPanel()
    {
        OnClose();
        UIManager.Instance.BackCtrlPanel();
    }

    public void MainMenu()//返回主菜单
    {
        SceneManager.LoadSceneAsync(0);
        core.MainMenu();
    }
}