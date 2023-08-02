using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject[] backGround;
    [SerializeField] RectTransform rectTransform;//存档面板
    [SerializeField] Text loadText;//读取或者创建
    [SerializeField] RectTransform start;//面板动画起始位置
    [SerializeField] RectTransform end;//面板动画结束
    [SerializeField] List<SaveFileSlot> saveFileSlots;//存档位slots
    [SerializeField] CanvasGroup deleteCG;//删除确认面板
    [SerializeField] InputField deleteInputField;//删除确认输入框
    private bool on;

    private void Start()
    {
        Application.targetFrameRate = 60;
        for(int i = 0; i < 3; ++i)
        {
            string path = Path.Combine(Application.persistentDataPath, "TestData" + i.ToString());
            if (!File.Exists(path))
            {
                saveFileSlots[i].SetData(null, false);
            }
            else
            {
                GameMessage gameMessage;
                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    //反序列化对象
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    Dictionary<string, object> tess = (Dictionary<string, object>)binaryFormatter.Deserialize(fs);
                    gameMessage = (GameMessage)tess["SaveFileNameAndTime"];
                }
                saveFileSlots[i].SetData(gameMessage, true);
                //print("have");
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// 自定义UI
    /// </summary>
    public void CustomUI()
    {
        SceneManager.LoadScene(2);
    }

    public void TestBuild()
    {
        SceneManager.LoadScene(5);
    }

    public void EffectsTest()
    {
        SceneManager.LoadScene(6);
    }

    /// <summary>
    /// 存档面板动画
    /// </summary>
    public void SaveFieldSelect()
    {
        if(!on)
        {
            rectTransform.DOLocalMoveX(end.localPosition.x, 0.5f);
        }
        else
        {
            rectTransform.DOLocalMoveX(start.localPosition.x, 0.5f);
        }
        on = !on;
    }

    public void LoadGame()
    {
        //
    }

    /// <summary>
    /// 加载最近一次游玩的存档
    /// </summary>
    public void LoadRecentlyFile()
    {
        //根据存档内时间判断
    }

    public void ResetName()
    {
        //
    }
#region 删除存档
    private int deleteNumber;
    public void OnOpenDeletePanel()
    {
        if(loadText.text == "创建游戏")
        {
            ShowOrHide(deleteCG, true);
        }
        //检查选择的slot
        int x = -1;
        for(int i = 0; i < 3; ++i)
        {
            if(saveFileSlots[i].IsOn)
            {
                x = i;
                break;
            }
        }

        if(x == -1)
        {
            return;
        }
        deleteNumber = x;
    }

    public void Delete()
    {
        if(deleteInputField.text == "确认")
        {
            //SavingSystem.Instance.Delete("TestData" + deleteNumber.ToString());
            //saveFileSlots[deleteNumber].SetData(null, false);
        }
    }

    public void DeleteClose()
    {
        ShowOrHide(deleteCG, false);
    }
#endregion

    /// <summary>
    /// 检查是否有存档
    /// </summary>
    /// <param name="n"></param>
    public void CheckIfItsSaved(int n)
    {
        //有文件切换按钮字符
        if(saveFileSlots[n].Exist)
        {
            loadText.text = "载入游戏";
        }
        else
        {
            loadText.text = "创建游戏";
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
}