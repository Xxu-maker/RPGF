using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
public class AnimationName
{
    public Dictionary<string, MappingTable> NameToMapping = new Dictionary<string, MappingTable>();
}
public class SpineData
{
    public Dictionary<string, AnimationName> _SpineData = new Dictionary<string, AnimationName>();
}
public class MappingTable
{
    public Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
}

public class AnalysisSpienExcel
{
    public static SpineData Excute()
    {
        int columnNum = 0, rowNum = 0;
        DataRowCollection collect = ExcelTool.ReadExcel(1, ExcelConfig.excelsFolderPath + "Excel/Spine.xlsx", ref columnNum, ref rowNum);
        SpineData spineData = new SpineData();
        AnimationName animation = new AnimationName();
        MappingTable mappingTable = new MappingTable();
        string currentName = "";
        for (int col = 1; col < columnNum; col++)
        {
            string roleName = collect[0][col].ToString();
            if (roleName != "")
            {
                currentName = roleName;
                spineData._SpineData.Add(currentName, new AnimationName());
            }
            mappingTable = new MappingTable();
            for (int row = 2; row < rowNum; row++)
            {
                mappingTable.keyValuePairs.Add(collect[row][0].ToString(), collect[row][col].ToString());
            }
            spineData._SpineData[currentName].NameToMapping.Add(collect[1][col].ToString(), mappingTable);

        }
        return spineData;
    }
}