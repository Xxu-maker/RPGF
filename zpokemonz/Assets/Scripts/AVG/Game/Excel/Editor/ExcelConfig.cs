using System;
using Excel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelData;
using UnityEditor;
using UnityEngine;
using Novels;
using OfficeOpenXml;

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
        
        //DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);
        var collect=PrReadExcel(1,filePath, ref columnNum, ref rowNum);
        
        List<List<Script>> list = new List<List<Script>>();
        List<Script> array = null;

        string ClipsName = collect[0].Name;

        for (int i = 0; i < collect.Count; i++)
        {
            if (collect[i].Name.Length > 0)
            {
                array = new List<Script>();
                list.Add(array);
                ClipsName = collect[i].Name.ToString();
            }

            Script item = new Script();
            item.Sign = ClipsName;
            var key = collect[i].action.ToString();
            item.action = key.Length > 0 ? (NovelsScriptsType)System.Enum.Parse(typeof(NovelsScriptsType), key) : NovelsScriptsType.DialogContent;
            item.Value = collect[i].listDatas;
            
            if (/*item.Value[1].Length > 0 ||*/ collect[i].action.Length > 0)
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
    
    

    public static List<ColumnInfo> PrReadExcel(int tableIndex,String filePath, ref int columnNum, ref int rowNum)
    {
        FileInfo files= new FileInfo(filePath);
        ExcelPackage excelPackage = new ExcelPackage(files);
        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[tableIndex];
        columnNum = worksheet.Dimension.End.Column;
        rowNum = worksheet.Dimension.End.Row;
        List<ColumnInfo> rows = new List<ColumnInfo>();
        System.Object obj;
        string strValue;
           
        for (int i = 1; i <= columnNum; i++)
        {
            for (int j = 1; j <= rowNum; j++)
            {
                obj = worksheet.Cells[j, i].Value;
                strValue = obj == null ? "" : obj.ToString();
                strValue = strValue.Trim();
                switch (i)
                {
                    case 1:
                        rows.Add(new ColumnInfo() { listDatas = new List<string>(), Name =strValue });
                        
                        break;
                    case 2:
                        rows[j - 1].action = strValue;
                        break;
                    default:
                        if (strValue.Length>0)
                        {
                            rows[j - 1].listDatas.Add(strValue);
                        }
                       
                        break;
                }
            }
        }

        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].Name == "" && rows[i].action.Length < 2)
            {
                rows.RemoveAt(i);
                i--;
            }
        }

        return rows;
    
    }


    public static List<Language> PreReadLocalizationExcel(int tableIndex,String filePath, ref int columnNum, ref int rowNum)
    {
        FileInfo files= new FileInfo(filePath);
        ExcelPackage excelPackage = new ExcelPackage(files);
        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[tableIndex];
        columnNum = worksheet.Dimension.End.Column;
        rowNum = worksheet.Dimension.End.Row;
        List<Language> rows = new List<Language>();
        System.Object obj;
        string strValue;
        
        for (int i = 1; i <= columnNum; i++)
        {
            for (int j = 1; j <= rowNum; j++)
            {
                obj = worksheet.Cells[j, i].Value;
                strValue = obj == null ? "" : obj.ToString();
                strValue = strValue.Trim();
                switch (i)
                {
                    case 1:
                        rows.Add(new Language() { key = strValue});
                        break;
                    case 2:
                        rows[j - 1].CN = strValue;
                        break;
                    case 3:
                        rows[j - 1].HK = strValue;
                        break;
                    case 4:
                        rows[j - 1].EN = strValue;
                        break;
                    case 5:
                        rows[j - 1].JP = strValue;
                        break;
                }
            }
        }
        
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].key == "" )
            {
                rows.RemoveAt(i);
                i--;
            }
        }

        return rows;

    }
    //每行的数据
    public class ColumnInfo
    {
        public string Name;
        public string action;
        public List<string> listDatas;

        

        public override string ToString()
        {
            string data = Name;
            data += " , " + action;
            data += " , ";
            for (int i = 0; i < listDatas.Count; i++)
            {
                data += listDatas[i];
            }
            return data;
        }

    }
    
    
    [System.Serializable]
    public class SheetInfo
    {
        public string name;
        public string description;
        public List<ColumnInfo> listInfos;

        public SheetInfo(string name)
        {
            if (name.IndexOf("_") > 0)
            {
                int index = name.IndexOf("_");
                this.name = name.Substring(0, index);
                index++;
                description = name.Substring(index);
            }
            else
            {
                this.name = name;
                description = name;
            }
        }
        
    }
}
