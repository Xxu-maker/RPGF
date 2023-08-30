using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Novels;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class ExcelEditorWindow : OdinEditorWindow
 {
     public static  List<string> filePaths;
     public static  bool[] fileSelections;
     private static List<FileInfo> fileInfo;
     [MenuItem("Assets/导入剧本")]
     public static void ShowWindow()
     {
         DirectoryInfo dir = new DirectoryInfo(ExcelConfig.excelsFolderPath + "Excel/Chapter");
         FileInfo[] fil = dir.GetFiles();
         filePaths = new List<string>();
         fileInfo = new List<FileInfo>();
         foreach (FileInfo f in fil)
         {
             if (f.FullName.Contains("meta")||f.FullName.Contains(".DS_Store"))
                 continue;
             
             filePaths.Add(f.Name);
             fileInfo.Add(f);
             
         }
         fileSelections= new bool[filePaths.Count];
        
         GetWindow<ExcelEditorWindow>("Excel Preview");
        
     }

     private void OnGUI()
     {
         GUILayout.Label("Files:", EditorStyles.boldLabel);
         
         if (filePaths != null && filePaths.Count > 0)
         {
             
             for (int i = 0; i < filePaths.Count; i++)
             {
                 fileSelections[i] = EditorGUILayout.Toggle( filePaths[i],fileSelections[i]);
             }
         }
         else
         {
             GUILayout.Label("No files selected.");
         }

         if (GUILayout.Button("导入剧本"))
         {
             BuildSelectedFilesPlot();
         }
     }

     private void BuildSelectedFilesPlot()
     {
         for (int i = 0; i < fileInfo.Count; i++)
         {
             if (fileSelections[i])
             {
                 var data = ExcelTool.CreateItemArrayWithExcel(fileInfo[i].FullName);

                 for (int j = 0; j < data.Count; j++)
                 {
                     var Data=  data[j];

                     //确保文件夹存在
                     if (!Directory.Exists(ExcelConfig.assetPath))
                     {
                         Directory.CreateDirectory(ExcelConfig.assetPath);
                     }
                     
                     var name = data[j][0].Sign;
                     var EntityData = ScriptableObject.CreateInstance<Novels.NovelsSectionData>();

                     ExcelBuild.CreatePreNovel(Data, name, ref EntityData);
              
                     AssetDatabase.CreateAsset(EntityData, $"Assets/Resources/Config/NovelsChapters/{name}.asset");
                     AssetDatabase.SaveAssets();
                     AssetDatabase.Refresh();
                 }
             }
         }
     }

    

     private void OnSelectionChange()
     {
        
         
         fileSelections= new bool[filePaths.Count];
          
         Repaint();
     }
 }
