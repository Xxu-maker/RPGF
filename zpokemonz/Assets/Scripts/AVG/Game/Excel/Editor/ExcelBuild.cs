using Excel;
using Novels;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExcelBuild : UnityEditor.Editor
{
    //[MenuItem("Tools/Excel/创建数据资产")]
    public static void CreateItemAsset()
    {
        DirectoryInfo dir = new DirectoryInfo(ExcelConfig.excelsFolderPath + "Excel/Chapter");
        FileInfo[] fil = dir.GetFiles();
        foreach (FileInfo f in fil)
        {
            //int size = Convert.ToInt32(f.Length);
            //long size = f.Length;
            if (f.FullName.Contains("meta"))
                continue;

            var data = ExcelTool.CreateItemArrayWithExcel(f.FullName);

            for (int i = 0; i < data.Count; i++)
            {
                ItemManager manager = ScriptableObject.CreateInstance<ItemManager>();
                manager.dataArray = data[i];

                //确保文件夹存在
                if (!Directory.Exists(ExcelConfig.assetPath))
                {
                    Directory.CreateDirectory(ExcelConfig.assetPath);
                }

                var name = data[i][0].Sign;
                string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, name);
                AssetDatabase.CreateAsset(manager, assetPath);

                var EntityData = ScriptableObject.CreateInstance<Novels.NovelsSectionData>();

                CreatePreNovel(manager.dataArray, name, ref EntityData);
              
                AssetDatabase.CreateAsset(EntityData, $"Assets/Resources/Config/NovelsChapters/{name}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    /// <summary>
    /// 创建一键对话剧本
    /// </summary>
     public static NovelsSectionData CreatePreNovel(List<Script> array, string path, ref NovelsSectionData entityData)
    {
        entityData.EventNodes = new INovelsNode[array.Count];

        for (int i = 0; i < array.Count; ++i)
        {
            switch (array[i].action)
            {
                case NovelsScriptsType.DialogContent:
                    {
                        var NovelsData = new NovelsNodeData();
                        var dialogueContent = new DialogueContent();
                        dialogueContent.Str = array[i].Value[0];
                        NovelsData.Content = dialogueContent;
                        entityData.EventNodes[i] = NovelsData;
                       
                    }
                    break;
                case NovelsScriptsType.BlackScreenContent:
                    {
                        var NovelsData = new NovelsNodeData();
                        var Content = new BlackScreenContent();
                        Content.Str = array[i].Value[0];
                        NovelsData.Content = Content;
                        entityData.EventNodes[i] = NovelsData;
                    }
                    break;
                case NovelsScriptsType.SelectContent:
                    {
                        var NovelsData = new NovelsNodeData();
                        var Content = new SelectContent();
                        var length = int.Parse(array[i].Value[0]);
                        Content.Datas = new List<SelectContent.ButtonSelectData>();
                        for (int j = 0; j < length; j++)
                        {
                            var selectData = new SelectContent.ButtonSelectData();
                            selectData.Content = array[i].Value[j * 2 + 1];
                            selectData.SectionName = "Config/NovelsChapters/"+array[i].Value[j * 2 + 2];
                            Content.Datas.Add(selectData);
                        }
                        NovelsData.Content = Content;
                        entityData.EventNodes[i] = NovelsData;
                    }
                    break;
                case NovelsScriptsType.Delay:
                {
                    var setNodeData = new SetNodeData();
                    var contnet = new Delay();
                    contnet.DelayTime = float.Parse(array[i].Value[0]);
                    setNodeData.SetData = contnet;
                    entityData.EventNodes[i] = setNodeData;
                    break;
                }
                case NovelsScriptsType.JumpPlot:
                {
                    var setNodeData = new SetNodeData();
                    var action= new JumpNextAVGPlot();
                    action.StartNovelName = new GlobalConfig.NovelsChapterData();
                    action.StartNovelName.SectionNodes = "Config/NovelsChapters/"+array[i].Value[0];
                    setNodeData.SetData = action;
                    entityData.EventNodes[i] = setNodeData;
                    break;
                }
                case NovelsScriptsType.ShowCharaSet:
                {
                    var setNodeData = new SetNodeData();
                    var action= new ShowCharaSet();
                    action.name = array[i].Value[0];
                    var res = AssetDatabase.LoadAssetAtPath<Sprite>(array[i].Value[1]);
                    action.image = res;
                    //立绘效果 0关闭 1显示 2压暗
                    action.State =  (ShowCharaSet.CharaShowData.EEffType)int.Parse(array[i].Value[2]);
                    //立绘位置 0左 1右 2中间
                    action.PosType= (ShowCharaSet.CharaShowData.EPosType)int.Parse(array[i].Value[3]);
                    //立绘是否水平翻转
                    action.IsFlipX= array[i].Value[4] == "TRUE";
                    action.Size = new Vector2(640, 720);
                    setNodeData.SetData = action;
                    entityData.EventNodes[i] = setNodeData;
                    break;
                }
                case NovelsScriptsType.SetBackground:
                {
                    var setNodeData = new SetNodeData();
                    var action= new SetBackground();
                    action.IsShow = array[i].Value[0] == "TRUE";
                    var res = AssetDatabase.LoadAssetAtPath<Sprite>(array[i].Value[1]);
                    action.SpriteData = res;
                    setNodeData.SetData = action;
                    entityData.EventNodes[i] = setNodeData;
                    break;
                }
                case NovelsScriptsType.BackGameScene:
                {
                    var setNodeData = new SetNodeData();
                    var action = new BackGameScene();
                    setNodeData.SetData = action;
                    entityData.EventNodes[i] = setNodeData;
                    break;
                }
               
            }
        }
        return entityData;
    }
  
    /// <summary>
    /// 语言表导入
    /// </summary>
   

}

