using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using OfficeOpenXml;
using System.Text;
using UnityEditor.Callbacks;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem;


namespace ExcelTool
{

    /// <summary>
    /// excel���񵼳����ߴ���
    /// </summary>
    public class ExcelEditorWindow : EditorWindow
    {

        private static ExcelEditorWindow window;
       
        private static bool isOpen = true;
        private static ExcelSettings clsSetting;
        private static string strSettingPath = "";
        private const string strSettingName = "ExcelSetting.txt";
        private const string strCacheName = "Cache.txt";
        private const float fltInputWidth = 700.0f;
        private static DataInfo clsInfo;

        //[MenuItem("Tools/Excel配置表工具")]
        public static void FnInit()
        {
            ToolEditorWindow.GetWindow<ExcelEditorWindow>("Excel配置表工具", null, -100, -150, 1000, 500);
            clsInfo = new DataInfo() { infos = new List<SheetInfo>() };
            PrInit();
            isOpen = true;
        }

        private void OnGUI()
        {
            try
            {
                if (!isOpen) return;

                ToolEditorWindow.GetInputField("Excel文件路径: ", ref clsSetting.strExcelPath, null, width: fltInputWidth);
                ToolEditorWindow.GetInputField("脚本生成路径 : ", ref clsSetting.strCsPath, null, width: fltInputWidth);
                ToolEditorWindow.GetInputField("数据生成路径 : ", ref clsSetting.strDataPath, null, width: fltInputWidth);
                ToolEditorWindow.GetInputField("ScriptObject类的名字 : ", ref clsSetting.strMgrClassName, null, width: fltInputWidth);
                ToolEditorWindow.GetInputField("命名空间 : ", ref clsSetting.strNameSpace, null, width: fltInputWidth);
                //ToolEditorWindow.GetInputField("Config��̳� : ", ref clsSetting.strInherit, null, width: fltInputWidth);
                ToolEditorWindow.GetInputField("缓存数据路径 : ", ref clsSetting.strCachePath, null, width: fltInputWidth);
                ToolEditorWindow.GetDoubleBtn("保存数据", OnSaveConfig, "重置数据", OnResetConfig);
                ToolEditorWindow.GetSingleBtn("导入配置表", OnImportExcelData);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(IOException) && e.ToString().Contains("~$"))
                {
                    Debug.LogWarning("表格未关闭，数据无法读取");
                }
                else
                {
                    Debug.LogWarning("数据无法读取：" + e);
                }

                isOpen = false;
            }
        }

        #region �¼�����
        private void OnImportExcelData()
        {
            PrReadExcel();
            EditorUtility.RequestScriptReload();
        }

        private void OnResetConfig()
        {
            clsSetting = new ExcelSettings();
        }

        private void OnSaveConfig()
        {
            clsSetting.PuBeforeSave();
            PuSaveByBin(clsSetting, strSettingName, strSettingPath);
        }

        #endregion

        #region UI����

        /// <summary>
        /// UI����            
        /// </summary>
        /*        private static void PrUISetting(float height = fltUIHeight)
                {
                    GUI.skin.textField.fixedHeight = height;
                }*/

        #endregion

        #region ScriptsObject


        /// <summary>
        /// ����ScriptObject            
        /// </summary>
        public static void PrCreateNewSObject()
        {
            /*            TestObjects config = ScriptableObject.CreateInstance(typeof(TestObjects)) as TestObjects;
                        config.enemies = new PropertyEnemy[] { new PropertyEnemy() { id = "1", name = "Enemy1"}, new PropertyEnemy() { id = "2", name = "Enemy2" } };
                        AssetDatabase.CreateAsset(config, "Assets/Resources/Config/Test.asset");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();*/

        }

        #endregion

        #region Excel

        private static string GetSObjName => string.Concat(clsSetting.strMgrClassName, "SObj");

       
        [DidReloadScripts]
        public static void PrOnReloadToCreate()
        {
            int id = EditorPrefs.GetInt("ExcelEditorWindow");
            if (id < 1) return;

            string name = EditorPrefs.GetString("ExcelEditorWindow_Mgr");
            if (IsNull(name)) return;

            PrInit();
            if (!Directory.Exists(clsSetting.strDataPath)) Directory.CreateDirectory(clsSetting.strDataPath);
            ScriptableObject sObj = CreateInstance(PuGetTypeName(name, clsSetting.strNameSpace));
            var dicValues = PrGetNameField(sObj.GetType());
            clsInfo = PuLoadByBin<DataInfo>(strCacheName, clsSetting.strCachePath);
            Dictionary<string, Array> dic = new Dictionary<string, Array>();
            foreach (var info in clsInfo.infos)
            {
                if (info.listInfos.Count > 1)
                {
                    dic[string.Concat(PuGetFirstLowerString(info.name), "s")] = PrCreateTypeArray(info);
                }
            }

            foreach (var key in dicValues.Keys)
            {
                dicValues[key].SetValue(sObj, dic[key]);
            }

            EditorPrefs.SetInt("ExcelEditorWindow", 0);
            AssetDatabase.CreateAsset(sObj, Path.Combine(clsSetting.strDataPath, string.Concat(name, ".asset")));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// ��Excel            
        /// </summary>
        private static void PrReadExcel()
        {
            ExcelPackage excelPackage;
            foreach (var info in GetFileInfos(clsSetting.strExcelPath))
            {
                excelPackage = new ExcelPackage(info);
                PrGetDicValues(info.Name.Substring(0, info.Name.LastIndexOf('.')), excelPackage.Workbook.Worksheets[1]);
            }
            //Enum�����ɽű�
            foreach (var info in clsInfo.infos)
            {
                if (info.listInfos.Count < 2)
                {
                    PrCreateClass(info, clsSetting.strCsPath);
                }
            }

            foreach (var info in clsInfo.infos)
            {
                if (info.listInfos.Count > 1)
                {
                    PrCreateClass(info, clsSetting.strCsPath);
                }
            }


            if (clsSetting.strCachePath == null || clsSetting.strCachePath.Length < 3)
            {
                clsSetting.strCachePath = FnGetDefaultPath();
            }

            PuSaveByBin(clsInfo, strCacheName, clsSetting.strCachePath);
            PrCreateScriptObjectClass();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorPrefs.SetInt("ExcelEditorWindow", 1);
        }

        /// <summary>
        /// ��ֵ��ȡ���ֵ�            
        /// </summary>
        private static void PrGetDicValues(string fileName, ExcelWorksheet worksheet)
        {
            //Debug.Log("fileName : " + worksheet.Dimension.End.Column);
            clsInfo.infos.Add(new SheetInfo(fileName));
            List<ColumnInfo> rows = new List<ColumnInfo>();
            System.Object obj;
            int columnCount = worksheet.Dimension.End.Column;
            int rowCount = worksheet.Dimension.End.Row;
            string strValue;
            
            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= columnCount; j++)
                {
                    obj = worksheet.Cells[i, j].Value;
                    strValue = obj == null ? "" : obj.ToString();
                    strValue = strValue.Trim();
                    switch (i)
                    {
                        case 1:
                            rows.Add(new ColumnInfo() { listDatas = new List<string>(), description = strValue });
                            break;
                        case 2:
                            rows[j - 1].name = strValue;
                            break;
                        case 3:
                            rows[j - 1].type = strValue;
                            break;
                        default:
                            rows[j - 1].listDatas.Add(strValue);
                            break;
                    }
                }
            }

            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i].name == null || rows[i].name.Length < 2)
                {
                    rows.RemoveAt(i);
                    i--;
                }
            }

            clsInfo.infos.Last().listInfos = rows;
            clsInfo.infos.Last().PuUpdateWrongData();
        }

        #endregion

        #region I/O

        public static string GetAppPath => UnityEngine.Application.dataPath;

       
        public static string FnGetDefaultPath()
        {
            string defaultPath = UnityEngine.Application.dataPath;
            string[] guidArr = AssetDatabase.FindAssets("ExcelEditorWindow");
            if (guidArr.Length > 0)
            {
                defaultPath = AssetDatabase.GUIDToAssetPath(guidArr[0]);
                defaultPath = Path.Combine(defaultPath.Substring(0, defaultPath.LastIndexOf("/Editor")), "Data");
            }
            else
            {
                Debug.LogError("路径异常");
            }

            return defaultPath;
        }

        public static string PuReadStrFromFile(string path)
        {
            string content = null;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                content = sr.ReadToEnd();
                sr.Close();
                fs.Close();
            }
            catch (IOException)
            {
                Debug.Log("·���ļ������� : " + path);
            }
            return content;
        }

        public static bool PuWriteFileFromStr(string path, string str)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

                sw.Write(str);
                sw.Flush();

                sw.Close();
                fs.Close();

                if (File.Exists(path)) return true;

            }
            catch (IOException e)
            {
                Debug.LogError("����ʧ�� : " + e.Message);
            }

            return false;
        }

        /// <summary>
        /// [Serializable]Class�����Ʒ������浵
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clsT"></param>
        /// <param name="path"></param>
        public static bool PuSaveByBin<T>(T clsT, string file, string path)
        {
            try
            {
                //Debug.Log("����·�� : " + path);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                file = Path.Combine(path, file);
                //if (File.Exists(file)) File.Delete(file);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fileStream = File.Create(file);
                bf.Serialize(fileStream, clsT);
                fileStream.Close();
                /*            if (File.Exists(file))
                            {
                                //Debug.Log("����ɹ���");
                            }*/
            }
            catch (Exception e)
            {
                Debug.LogError("����ʧ�� : " + e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// [Serializable]Class�����Ʒ���������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T PuLoadByBin<T>(string fileName, string path)
        {
            path = Path.Combine(path, fileName);
            T save = default;
            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fileStream = File.Open(path, FileMode.Open);
                save = (T)bf.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                Debug.Log("�浵�ļ�������");
            }
            return save;
        }

        /// <summary>
        /// ��ȡ�ļ���������·��
        /// </summary>
        public static List<FileInfo> GetFileInfos(string path, string filter = ".xlsx")
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
            {
                FileInfo[] infors = dirInfo.GetFiles();
                for (int i = 0; i < infors.Length; i++)
                {
                    if (infors[i].FullName.EndsWith(filter, false, null))
                    {
                        files.Add(infors[i]);
                    }
                }
            }

            return files;
        }

        #endregion

        #region private

        /// <summary>
        /// �������ͻ�ȡnew            
        /// </summary>
        public static Array PrCreateTypeArray(SheetInfo info)
        {
            Type mType = PrGetType(info.name, clsSetting.strNameSpace);
            //Type mType = Type.ReflectionOnlyGetType($"Assembly-CSharp.{PuGetTypeName(info.name, clsSetting.strNameSpace)}", true, false);
            //Debug.Log($"��ȡ������ : {PuGetTypeName(info.name, clsSetting.strNameSpace)}|{mType}");
            Array arrClones = Array.CreateInstance(mType, info.listInfos[0].listDatas.Count);
            for (int i = 0; i < arrClones.Length; i++)
            {
                arrClones.SetValue(Activator.CreateInstance(mType), i);
            }

            var dicValue = PrGetNameField(mType);
            string name, value, type;
            for (int i = 0; i < info.listInfos.Count; i++)
            {
                name = info.listInfos[i].name;
                type = PrGetRealType(info.listInfos[i].type);
                if (dicValue.ContainsKey(name) || dicValue.ContainsKey(PuGetFirstLowerString(name)))
                {
                    for (int j = 0; j < arrClones.Length; j++)
                    {
                        value = info.listInfos[i].listDatas[j];
                        //Debug.Log($"{info.name}|{type}|{value}");
                        if (IsNull(value, 1)) continue;
                        try
                        {
                            switch (type)
                            {
                                case "int":
                                    dicValue[name].SetValue(arrClones.GetValue(j), int.Parse(value));
                                    break;
                                case "float":
                                    dicValue[name].SetValue(arrClones.GetValue(j), float.Parse(value));
                                    break;
                                case "UnityEngine.Sprite":
                                    dicValue[name].SetValue(arrClones.GetValue(j), AssetDatabase.LoadAssetAtPath<Sprite>(value));
                                    break;
                                case "UnityEngine.GameObject":
                                    dicValue[name].SetValue(arrClones.GetValue(j), AssetDatabase.LoadAssetAtPath<GameObject>(value));
                                    break;
                                case "UnityEngine.AudioClip":
                                    dicValue[name].SetValue(arrClones.GetValue(j), AssetDatabase.LoadAssetAtPath<AudioClip>(value));
                                    break;
                                case "enum":
                                    int current = int.Parse(value) - 1;
                                    dicValue[PuGetFirstLowerString(name)].SetValue(arrClones.GetValue(j), Enum.Parse(PrGetType(info.listInfos[i].PuGetEnumName, clsSetting.strNameSpace), current.ToString()));
                                    //dicValue[PuGetFirstLowerString(name)].SetValue(arrClones.GetValue(j), PuGetEnumValues(info.listInfos[i].PuGetEnumName, value));
                                    break;
                                default:
                                    dicValue[name].SetValue(arrClones.GetValue(j), value);
                                    break;
                            }
                            //Debug.Log($"{info.name}|{name}|{dicValue[name].GetValue(arrClones.GetValue(j))}");
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"{info.name}|{name}|{info.listInfos[i].listDatas[j]} ��ʧ�� : {e}");
                        }
                    }
                }
            }

            return arrClones;
        }

        /// <summary>
        /// ��ʼ��            
        /// </summary>
        public static void PrInit()
        {
            strSettingPath = FnGetDefaultPath();
           
            clsSetting = PuLoadByBin<ExcelSettings>(strSettingName, strSettingPath);
            if (clsSetting == null)
            {
                clsSetting = new ExcelSettings();
            }
        }

        /// <summary>
        /// �������ݹ�����ScriptObject��            
        /// </summary>
        public static void PrCreateScriptObjectClass()
        {
            string sObjeFileName = GetSObjName;
            EditorPrefs.SetString("ExcelEditorWindow_Mgr", sObjeFileName);
            string filePath = Path.Combine(clsSetting.strCsPath, string.Concat(sObjeFileName, ".cs"));
            if (File.Exists(filePath)) { File.Delete(filePath); }
           
            List<ColumnInfo> list = new List<ColumnInfo>();
            foreach (var info in clsInfo.infos)
            {
                if (info.listInfos.Count > 1)
                {   
                    list.Add(new ColumnInfo()
                    {
                        name = string.Concat(PuGetFirstLowerString(info.name), "s"),
                        description = info.description,
                        type = string.Concat($"{info.name}[] ")
                    });
                   
                }
            }

            PuWriteFileFromStr(filePath,
                PrCombineClass(sObjeFileName,
                    "",
                    PrCombineParams(list), "", "Sirenix.OdinInspector.SerializedScriptableObject", nameSpace: clsSetting.strNameSpace));
           
            filePath = Path.Combine(clsSetting.strCsPath, string.Concat(clsSetting.strMgrClassName, ".cs"));
            if (File.Exists(filePath)) return;
            
            PuWriteFileFromStr(filePath,
                PrCombineClass(clsSetting.strMgrClassName,
                "",
                "", "partial", sObjeFileName, nameSpace: clsSetting.strNameSpace));
        }

        /// <summary>
        /// ƴ�Ӳ����ɽű�
        /// </summary>
        /// <param name="name">�ļ���</param>
        /// <param name="dic">�ֶ���������</param>
        private static void PrCreateClass(SheetInfo sheetInfo, string path)
        {
            string filePath = Path.Combine(path, string.Concat(sheetInfo.name, ".cs"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (sheetInfo.listInfos.Count < 2)
            {
                string[] param = new string[sheetInfo.listInfos[0].listDatas.Count];
                for (int i = 0; i < param.Length; i++)
                {
                    param[i] = sheetInfo.listInfos[0].listDatas[i];
                }

                PuWriteFileFromStr(filePath, PrCombineEnum(sheetInfo.name, sheetInfo.description, clsSetting.strNameSpace, param));
            }
            else
            {
                PuWriteFileFromStr(filePath, PrCombineClass(sheetInfo.name, sheetInfo.description, PrCombineParams(sheetInfo.listInfos), nameSpace: clsSetting.strNameSpace));
            }
            
        }

        /// <summary>
        /// ö�����ƴ��
        /// </summary>
        /// <param name="name"></param>
        /// <param name="summary">˵��</param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static string PrCombineEnum(string name, string summary, string nameSpace, string[] param)
        {
            bool isNameSpace = nameSpace != null;
            string prefix = isNameSpace ? "\t" : "";
            StringBuilder content = new StringBuilder("");
            if (isNameSpace)
            {
                content.AppendLine($"namespace {nameSpace}");
                content.AppendLine("{");
            }

            content.AppendLine($"{prefix}/// <summary>");
            content.AppendLine($"{prefix}/// {summary}");
            content.AppendLine($"{prefix}/// <summary>");
            //content.AppendLine("[System.Serializable]");
            content.AppendLine($"{prefix}public enum {PuGetEnumName(name)}");
            content.Append($"\n{prefix}");
            content.Append("{\n");
            for (int i = 0; i < param.Length; i++)
            {
                //Debug.Log($"{name} | ö���ֶ� : {param[i]}");
                content.AppendLine($"{prefix}\t{PuGetFirstUperString(param[i])},");
            }

            content.Append($"{prefix}");
            content.Append("}\n");
            if (isNameSpace) content.AppendLine("}");
            return content.ToString();
        }

        /// <summary>
        /// ���ƴ��
        /// </summary>
        /// <param name="name"></param>
        /// <param name="summary">˵��</param>
        /// <param name="inherit">�̳�</param>
        /// <param name="param"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string PrCombineClass(string name, string summary, string param, string extra = null, string inherit = null, string method = null, string nameSpace = null)
        {
            bool isNameSpace = nameSpace != null;
            string prefix = isNameSpace ? "\t" : "";
            StringBuilder content = new StringBuilder("");
            if (isNameSpace)
            {
                content.AppendLine($"namespace {nameSpace}");
                content.AppendLine("{");
            }

            content.AppendLine($"{prefix}/// <summary>");
            content.AppendLine($"{prefix}/// {summary}");
            content.AppendLine($"{prefix}/// <summary>");
            content.AppendLine($"{prefix}[System.Serializable]");
            content.Append($"{prefix}public {(extra == null ? "" : extra)} class {name}");
            if (inherit != null)
            {
                content.Append($" : {inherit}");
            }

            content.Append($"\n{prefix}");
            content.Append("{\n");
            content.AppendLine($"{prefix}\t#region ����\n");
            content.AppendLine(param);
            content.AppendLine($"\n{prefix}\t#endregion\n");
            if (method != null)
            {
                content.AppendLine($"{prefix}\t#region ����\n");
                content.AppendLine(method);
                content.AppendLine($"\n{prefix}\t#endregion\n");
            }

            content.Append($"{prefix}");
            content.Append("}\n");
            if (isNameSpace) content.AppendLine("}");
            return content.ToString();
        }

        /// <summary>
        /// ������ƴ��            
        /// </summary>
        private static string PrCombineParams(List<ColumnInfo> listInfos)
        {
            StringBuilder content = new StringBuilder();
            foreach (var info in listInfos)
            {
                content.AppendLine("\t/// <summary>");
                content.AppendLine($"\t/// {info.description}");
                content.AppendLine("\t/// <summary>");
                content.AppendLine($"\t[UnityEngine.SerializeField, UnityEngine.Header(\"{info.description}\")]");
                if (info.type == "enum")
                {
                    content.AppendLine($"\tpublic {PuGetEnumParamName(info.name)} {info.name};");
                }
                else
                {
                    content.AppendLine($"\tpublic {PrGetRealType(info.type)} {info.name};");
                }
            }
            return content.ToString();
        }

        /// <summary>
        /// ͨ���������ƻ�ȡ����
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static Type PrGetType(string typeName, string nameSpace)
        {
            //Debug.Log($"��ȡ���� {nameSpace}:{typeName}");
            //Debug.Log(typeName);
            typeName = PuGetTypeName(typeName, nameSpace);
            Type type = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly defaultAssembly = assemblies.First(a => a.GetName().Name.StartsWith("Assembly-CSharp"));
            type = defaultAssembly.GetType(typeName);
            //Debug.Log($"�ҵ� {defaultAssembly == null}|{type}");
            return type;
            //return Assembly.Load(nameSpace).GetType(string.Concat(nameSpace, ".", typeName));
        }

        /// <summary>
        /// �������û�ȡʵ������            
        /// </summary>
        public static string PrGetRealType(string type)
        {
            string t = type;
            switch (type)
            {
                case "png":
                case "sprite":
                    t = "UnityEngine.Sprite";
                    break;
                case "gameObject":
                    t = "UnityEngine.GameObject";
                    break;
                case "audio":
                    t = "UnityEngine.AudioClip";
                    break;
                //case "dic":
                    //t = $"Dictionary<string,{type}>";
                    //break;
            }

            return t;
        }

        /// <summary>
        /// ���ҽű���������Ҫ�󶨳�Ա�����֡�����
        /// </summary>
        /// <param name="type">�ű�����</param>
        /// <returns></returns>
        private static Dictionary<string, FieldInfo> PrGetNameField(Type type)
        {

            Dictionary<string, FieldInfo> dicNameField = new Dictionary<string, FieldInfo>();
            try
            {
                foreach (FieldInfo field in type.GetRuntimeFields())
                {
                    //Debug.Log(field.Name);
                    if (!field.IsNotSerialized)
                    {
                        dicNameField[field.Name] = field;
                    }
                }
            }
            catch (Exception)
            {

            }

            return dicNameField;
        }

        /// <summary>
        /// �ж�string 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="minLength"></param>
        /// <returns>true�ǿ�</returns>
        public static bool IsNull(string str, int minLength = 2)
        {
            return str == null || str.Length < minLength;
        }

        /// <summary>
        /// ��ȡ����ĸСд            
        /// </summary>
        public static string PuGetFirstLowerString(string value)
        {
            return string.Concat(value.Substring(0, 1).ToLower(), value.Substring(1));
        }

        /// <summary>
        /// ��ȡ����ĸ��д            
        /// </summary>
        public static string PuGetFirstUperString(string value)
        {
            return string.Concat(value.Substring(0, 1).ToUpper(), value.Substring(1));
        }

        /// <summary>
        /// ��ȡö��������            
        /// </summary>
        public static string PuGetEnumParamName(string name)
        {
            string paramName = name.Substring(0, name.IndexOf('_'));
            //Debug.Log($"    ## {paramName}");
            paramName = PuGetEnumName(PuGetFirstUperString(paramName));
            return paramName;
        }

        /// <summary>
        /// ��ȡ������            
        /// </summary>
        public static string PuGetTypeName(string name, string nameSpace)
        {
            if (!IsNull(nameSpace)) return string.Concat(nameSpace, ".", name);
            return name;
        }
        
        public static object PuGetEnumValues(string type, string value)
        {
            Array array = Enum.GetValues(PrGetType(type, clsSetting.strNameSpace));
            //Debug.Log($"{type}|{array.Length} ");
            Dictionary<string, int> dic = new Dictionary<string, int>();
            for (int i = 0; i < array.Length; i++)
            {
                dic[array.GetValue(i).ToString()] = i;
                //Debug.Log($"{type}|{array.GetValue(i)} ");
            }

            if (dic.ContainsKey(value))
            {
                return array.GetValue(dic[value]);
            }

            return default;
        }
        
        public static string PuGetEnumName(string name) => string.Concat("Enum", name);

        #endregion

        [Serializable]
        public class ExcelSettings
        {
            /// <summary>
            /// Excel�����ļ�·��
            /// </summary>
            public string strExcelPath;
            /// <summary>
            /// ��������·��
            /// </summary>
            public string strDataPath;
            /// <summary>
            /// ���ɽű�·��
            /// </summary>
            public string strCsPath;
            /// <summary>
            /// ��������·��
            /// </summary>
            public string strCachePath;
            /// <summary>
            /// ScriptObject�������
            /// </summary>
            public string strMgrClassName = "ConfigMgr";
            /// <summary>
            /// ����������ļ���
            /// </summary>
            public string strClassName = "ClassTemplate.txt";
            /// <summary>
            /// �����ռ�
            /// </summary>
            public string strNameSpace = "ExcelData";
            /// <summary>
            /// Ĭ�ϼ̳�
            /// </summary>
            //public string strInherit = "BaseConfig";

            public ExcelSettings()
            {
                string basePath = Path.Combine(GetAppPath, "Plugins", "ExcelTool", "Example");
                strExcelPath = Path.Combine(basePath, "Config");
                strCsPath = Path.Combine(basePath, "Scripts", "Config");
                strDataPath = Path.Combine("Assets", "Plugins", "ExcelTool", "Example", "Resources", "Config");
                strCachePath = FnGetDefaultPath();
            }


            /// <summary>
            /// ��ȡExcelPath            
            /// </summary>
            public string PuGetExcelPath()
            {
                return "";
            }

            /// <summary>
            /// ��������ǰ���            
            /// </summary>
            public void PuBeforeSave()
            {
                if (strCachePath == null || strCachePath.Length < 2)
                {
                    strCachePath = FnGetDefaultPath();
                }

                if (strMgrClassName == null || strMgrClassName.Length < 2)
                {
                    strMgrClassName = "ConfigMgr";
                }
            }
        }

        [System.Serializable]
        public class DataInfo
        {
            public List<SheetInfo> infos;
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

            /// <summary>
            /// �Զ�������������,�б�Ҫ��           
            /// </summary>
            public void PuUpdateWrongData()
            {
                if (listInfos.Count <= 2)
                {
                    if (listInfos.Count == 2)
                    {
                        listInfos.RemoveAt(0);
                    }

                    for (int i = 0; i < listInfos[0].listDatas.Count; i++)
                    {
                        if (IsNull(listInfos[0].listDatas[i]))
                        {
                            listInfos[0].listDatas.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        [System.Serializable]
        public class ColumnInfo
        {
            public string name;
            public string description;
            public string type;
            public List<string> listDatas;
            
            public string PuGetEnumName => PuGetEnumParamName(name);

            public override string ToString()
            {
                string data = description;
                data += " , " + name;
                data += " , " + type;
                data += " ,  ";
                for (int i = 0; i < listDatas.Count; i++)
                {
                    data += listDatas[i] + "";
                }
                return data;
            }

        }
    }
}
