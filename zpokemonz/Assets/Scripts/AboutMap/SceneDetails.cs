using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 用于场景拆卸和场景内数据保存
/// </summary>
public class SceneDetails : MonoBehaviour
{
    [SerializeField] string loadMapName;
    [SerializeField] List<SceneDetails> connectedScenes;
    /// <summary>
    /// 场景数据管理
    /// </summary>
    /// <value></value>
    [SerializeField] List<SceneFileHandler> sceneFile;
    public bool IsLoaded{ get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            LoadScene();
            GameManager.Instance.SetCurrentScene(this);
            UIManager.Instance.MapTip.DisPlayMapTip(loadMapName);
            //加载所有连接的场景
            if(connectedScenes != null)
            {
                foreach(SceneDetails scene in connectedScenes)
                {
                    scene.LoadScene();
                }
            }
            //卸载不再连接的场景
            if(GameManager.Instance.PrevSceneD != null)
            {
                List<SceneDetails> previouslyLoadedScenes = GameManager.Instance.PrevSceneD.connectedScenes;
                foreach(SceneDetails scene in previouslyLoadedScenes)
                {
                    if(!connectedScenes.Contains(scene) && scene != null)
                    {
                        scene.UnLoadScene();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    public void LoadScene()
    {
        if(!IsLoaded)
        {
            SceneManager.LoadSceneAsync(loadMapName, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    /// <summary>
    /// 卸载场景
    /// </summary>
    public void UnLoadScene()
    {
        if(IsLoaded)
        {
            SceneManager.UnloadSceneAsync(loadMapName);
            IsLoaded = false;
        }
    }

#region 卸载场景时储存, 加载场景时读取
    /// <summary>
    /// 存储当前场景数据
    /// </summary>
    public void SaveSceneFile()
    {
        if(sceneFile == null) { return; }

        foreach(SceneFileHandler fileHandler in sceneFile)
        {
            fileHandler.Save();
        }
    }

    /// <summary>
    /// 读取场景数据
    /// </summary>
    public void LoadSceneField()
    {
        if(sceneFile == null) { return; }

        foreach(SceneFileHandler fileHandler in sceneFile)
        {
            fileHandler.Load();
        }
    }
#endregion
}