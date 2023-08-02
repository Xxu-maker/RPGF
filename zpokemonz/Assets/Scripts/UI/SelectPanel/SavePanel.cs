using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
public class SavePanel : BasePanel
{
    [SerializeField] SavingSystem savingSystem;
    [SerializeField] List<SaveFileSlot> saveFileSlots;
    [SerializeField] Text createOrCover;//加载/创建字符提示
    [SerializeField] InputField inputField;//创建输入框
    [SerializeField] CanvasGroup coverTipCG;//覆盖提示面板
    [SerializeField] CanvasGroup createTipCG;//创建提示面板
    [SerializeField] CanvasGroup deleteCG;//删除提示面板
    [SerializeField] InputField deleteInputField;//删除面板输入框
    public override void OnOpen()
    {
        base.OnOpen();
        //设置saveFieldSlots
        for(int i = 0; i < 3; ++i)
        {
            string path = Path.Combine(Application.persistentDataPath, "TestData" + i.ToString());
            if (!File.Exists(path))
            {
                saveFileSlots[i].SetData(null, false);
            }
            else
            {
                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    //反序列化对象
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    Dictionary<string, object> tess = (Dictionary<string, object>)binaryFormatter.Deserialize(fs);
                    saveFileSlots[i].SetData((GameMessage)tess["SaveFileNameAndTime"], true);
                }
            }
        }
    }
    public void ExitPanel()
    {
        OnClose();
        UIManager.Instance.BackCtrlPanel();
    }

    private int saveNumber;
    public void SaveFile()
    {
        //*Item和Ability不能存ScriptableObject得和pokemon一样做个总读取的存储

        return;//****************没弄好

        //Debug.Log("save");
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
        saveNumber = x;

        //确认是否有文件
        if(saveFileSlots[x].Exist)
        {
            //覆盖提示
            ShowOrHide(coverTipCG, true);
        }
        else
        {
            //创建提示
            ShowOrHide(createTipCG, true);
        }
    }



    private string savePathPrefixes = "TestData";
    private int currentSelectSlotID;
    //saveSlot按键
    public void CheckIfItsSaved(int n)
    {
        if(currentSelectSlotID != n)
        {
            currentSelectSlotID = n;
        }
        //有文件切换按钮字符
        if(saveFileSlots[n].Exist)
        {
            createOrCover.text = "覆盖";
        }
        else
        {
            createOrCover.text = "创建";
        }
    }

    /// <summary>
    /// 检查打开序号和进入序号是否相同
    /// </summary>
    /// <returns></returns>
    public bool SelectNumCheck()
    {
        //检查选择的slot
        for(int i = 0; i < 3; ++i)
        {
            if(saveFileSlots[i].IsOn)
            {
                if(currentSelectSlotID == i)
                {
                    return true;
                }
            }
        }
        //Debug.LogError("存档面板序号不同");
        return false;
    }
#region 覆盖面板提示
    public void CoverPanelYes()
    {
        //覆盖存档
        string fileName = savePathPrefixes + currentSelectSlotID.ToString();
        savingSystem.Save(fileName, saveFileSlots[currentSelectSlotID].GetSaveFileName);
        CoverPanelNo();

        if(!SelectNumCheck())
        {
            return;
        }

        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName)))
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                //反序列化对象
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                saveFileSlots[currentSelectSlotID].SetData
                (
                    (GameMessage)((Dictionary<string, object>)binaryFormatter.Deserialize(fs))["SaveFileNameAndTime"],
                    true
                );
            }
        }
    }

    public void CoverPanelNo()
    {
        //关掉覆盖面板
        ShowOrHide(coverTipCG, false);
    }
#endregion
#region 创建提示面板
    public void CreatePanelYes()
    {
        string fileName = savePathPrefixes + currentSelectSlotID.ToString();
        savingSystem.Save(fileName, inputField.text);

        CreatePanelNo();

        if(!SelectNumCheck())
        {
            return;
        }

        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName)))
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                //反序列化对象
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                Dictionary<string, object> tess = (Dictionary<string, object>)binaryFormatter.Deserialize(fs);
                saveFileSlots[currentSelectSlotID].SetData((GameMessage)tess["SaveFileNameAndTime"], true);
            }
        }
    }

    public void CreatePanelNo()
    {
        //关掉创建面板
        ShowOrHide(createTipCG, false);
    }
#endregion
#region 删除提示面板
    private int deleteNumber;
    /// <summary>
    /// 打开删除面板
    /// </summary>
    public void DeleteFile()
    {
        //删除提示面板(创建时不提示)
        if(createOrCover.text == "覆盖")
        {
            ShowOrHide(deleteCG, true);
        }
    }

    public void DeletePanelYes()
    {
        if(deleteInputField.text == "确认")
        {
            deleteInputField.text = null;
            if(!SelectNumCheck())
            {
                return;
            }
            savingSystem.Delete(savePathPrefixes + currentSelectSlotID.ToString());
            saveFileSlots[currentSelectSlotID].SetData(null, false);
            ShowOrHide(deleteCG, false);
        }
    }
    public void DeletePanelNo()
    {
        //关掉删除面板
        ShowOrHide(deleteCG, false);
    }
#endregion
}