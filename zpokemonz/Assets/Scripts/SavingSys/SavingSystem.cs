using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
//*******
/*
  偷懒可以弄三个档 playerData1 playerData2 playerData3
  名字也存在存档里，进界面读取名字， 但文件名是playerData

  进地图存取相应的数据
*/
public class SavingSystem : SingletonMono<SavingSystem>
{
    [SerializeField] Text pathText;

    /// <summary>
    /// 复制原存档, 用于实时保存场景中数据, 游玩时仅对此Dictionary进行读写
    /// 在玩家需要保存时，只要再对玩家属性文件和最后一个场景文件保存即可
    /// </summary>
    Dictionary<string, object> currentGameState = new Dictionary<string, object>();

    private const string saveFileMenu = "PlayerSaveFileMenu";

    /// <summary>
    /// 游玩时先复制源存档
    /// </summary>
    /// <param name="saveFile"></param>
    public void CopySaveFileToCurrentGameState(string saveFile)//Dictionary<string, object>
    {
        currentGameState = LoadFile(saveFile);
    }

    /// <summary>
    /// 保存至CurrentState
    /// </summary>
    /// <param name="savableEntities"></param>
    public void CaptureEntityStatesToCurrentState(List<SavableEntity> savableEntities)
    {
        foreach (SavableEntity savable in savableEntities)
        {
            currentGameState[savable.UniqueId] = savable.CaptureState();
        }
    }

    /// <summary>
    /// 加载CurrentState中数据
    /// </summary>
    /// <param name="savableEntities"></param>
    public void RestoreEntityStatesFromCurrentState(List<SavableEntity> savableEntities)
    {
        foreach (SavableEntity savable in savableEntities)
        {
            string id = savable.UniqueId;
            if (currentGameState.ContainsKey(id))
            {
                savable.RestoreState(currentGameState[id]);
            }
        }
    }

    public void Save(string saveFileName, string messageName)
    {
        //System.DateTime.Now(2022/3/3 0:40:03)
        //object timeAndFileName = ;
        currentGameState["SaveFileNameAndTime"] = new GameMessage(messageName, System.DateTime.Now.ToString());

        //其它场景数据已保存
        //保存当前场景数据和玩家数据
        CaptureState(currentGameState);
        //GameControl.Instance.CurrentSceneD.SaveSceneFile();
        ////保存玩家数据
        //SavableEntity savable = GameControl.Instance.PlayerSavableEntity;
        //currentGameState[savable.UniqueId] = savable.CaptureState();
        //保存
        SaveFile(saveFileName, currentGameState);
    }

    public void Load(string saveFile)
    {
        currentGameState = LoadFile(saveFile);
        RestoreState(currentGameState);
    }

    public void Delete(string saveFile)
    {
        File.Delete(GetPath(saveFile));
    }

    /// <summary>
    /// 保存当前游戏中所有可保存对象的状态
    /// </summary>
    /// <param name="state"></param>
    private void CaptureState(Dictionary<string, object> state)
    {
        SavableEntity[] savables = FindObjectsOfType<SavableEntity>();
        int length = savables.Length;
        for(int i = 0; i < length; ++i)
        {
            state[savables[i].UniqueId] = savables[i].CaptureState();
        }
    }

    /// <summary>
    /// 恢复玩家数据及当前场景数据
    /// </summary>
    /// <param name="state"></param>
    private void RestoreState(Dictionary<string, object> state)
    {
        SavableEntity[] savables = FindObjectsOfType<SavableEntity>();
        int length = savables.Length;
        for(int i = 0; i < length; ++i)
        {
            string id = savables[i].UniqueId;
            if (state.ContainsKey(id))
            {
                savables[i].RestoreState(state[id]);
            }
        }
    }

    /// <summary>
    /// 存储文件
    /// </summary>
    /// <param name="saveFile"></param>
    /// <param name="state"></param>
    void SaveFile(string saveFile, Dictionary<string, object> state)
    {
        string path = GetPath(saveFile);

        using (FileStream fs = File.Open(path, FileMode.Create))
        {
            //序列化object
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fs, state);
        }

        //print($"saving to {path}");
        pathText.text = path;
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="saveFile"></param>
    /// <returns></returns>
    Dictionary<string, object> LoadFile(string saveFile)
    {
        string path = GetPath(saveFile);
        //检查文件是否存在
        if (!File.Exists(path)) { return new Dictionary<string, object>(); }

        using (FileStream fs = File.Open(path, FileMode.Open))
        {
            //反序列化对象
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            return (Dictionary<string, object>) binaryFormatter.Deserialize(fs);
        }
    }

    /// <summary>
    /// 获取存档路径
    /// </summary>
    /// <param name="saveFile"></param>
    private string GetPath(string saveFile)
    {
        return Path.Combine(Application.persistentDataPath, saveFile);
    }
}