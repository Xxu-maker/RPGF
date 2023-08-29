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

                AssetDatabase.CreateAsset(EntityData, "Assets/HotFixGameRes/Novels/NovelsChapters/" + name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    /// <summary>
    /// 创建一键对话剧本
    /// </summary>
    static NovelsSectionData CreatePreNovel(List<Script> array, string path, ref NovelsSectionData entityData)
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
                        //dialogueContent.CharaName = array[i].Value[0];
                        dialogueContent.Str = array[i].Value[1];
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
                            selectData.SectionName = array[i].Value[j * 2 + 2];
                            Content.Datas.Add(selectData);
                        }
                        NovelsData.Content = Content;
                        entityData.EventNodes[i] = NovelsData;
                        entityData.EventNodes[i] = NovelsData;
                    }
                    break;
               
            }
        }
        return entityData;
    }
  
    /// <summary>
    /// 语言表导入
    /// </summary>
   

}