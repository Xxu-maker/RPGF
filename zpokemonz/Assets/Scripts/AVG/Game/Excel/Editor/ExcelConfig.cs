using Excel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
using Novels;

public class ExcelConfig
{
    /// <summary>
    /// 存放excel表文件夹的的路径，本例excel表放在了"Assets/Excel/"当中
    /// </summary>
    public static readonly string excelsFolderPath = Application.dataPath + "/";

    /// <summary>
    /// 存放Excel转化CS文件的文件夹路径
    /// </summary>
    public static readonly string assetPath = "Assets/DataAssets/";
}

public class ExcelTool
{

    /// <summary>
    /// 读取表数据，生成对应的数组
    /// </summary>
    /// <param name="filePath">excel文件全路径</param>
    /// <returns>Item数组</returns>
    public static List<List<Script>> CreateItemArrayWithExcel(string filePath)
    {
        //获得表数据
        int columnNum = 0, rowNum = 0;
        DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

        List<List<Script>> list = new List<List<Script>>();
        List<Script> array = new List<Script>();

        string ClipsName = collect[0][0].ToString();

        for (int i = 0; i < rowNum; i++)
        {
            if (collect[i][0].ToString().Length > 0)
            {
                array = new List<Script>();
                list.Add(array);
                ClipsName = collect[i][0].ToString();
            }

            Script item = new Script();
            item.Sign = ClipsName;
            var key = collect[i][1].ToString();
            item.action = key.Length > 0 ? (NovelsScriptsType)System.Enum.Parse(typeof(NovelsScriptsType), key) : NovelsScriptsType.DialogContent;
            item.Value = new List<string>();

            for (int j = 2; j < columnNum; j++)
            {
                item.Value.Add(collect[i][j].ToString());
            }

            if (/*item.Value[1].Length > 0 ||*/ collect[i][1].ToString().Length > 0)
                array.Add(item);

        }
        return list;
    }
    
    /// <summary>
    /// 读取excel文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="columnNum">行数</param>
    /// <param name="rowNum">列数</param>
    /// <returns></returns>
    static DataRowCollection ReadExcel(string filePath, ref int columnNum, ref int rowNum)
    {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        //Tables[0] 下标0表示excel文件中第一张表的数据
        columnNum = result.Tables[0].Columns.Count;
        rowNum = result.Tables[0].Rows.Count;
        return result.Tables[0].Rows;
    }

    public static DataRowCollection ReadExcel(int tableIndex, string filePath, ref int columnNum, ref int rowNum)
    {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        //Tables[0] 下标0表示excel文件中第一张表的数据
        columnNum = result.Tables[tableIndex].Columns.Count;
        rowNum = result.Tables[tableIndex].Rows.Count;
        return result.Tables[tableIndex].Rows;
    }
}
