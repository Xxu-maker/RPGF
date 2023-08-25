using System;
using UnityEditor;
using UnityEngine;


namespace ExcelTool
{
    /// <summary>
    /// 窗口工具
    /// </summary>
    public static class ToolEditorWindow
    {
        public delegate bool DelUIElement<S, P>(S name, P options);
        public delegate T DelUIElement<S, T, P>(S name, T value, P options);

        /// <summary>
        /// 获取固定窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static T GetWindow<T>(string name, Action firstCallback = null, float x = 0, float y = 0, float width = 400, float height = 400) where T : EditorWindow
        {
            T win = EditorWindow.GetWindow<T>(true, name);
            int times = PlayerPrefs.GetInt(name);
            if (times < 1)
            {
                win.position = new Rect(x, y, width, height);
                firstCallback?.Invoke();
            }

            PlayerPrefs.SetInt(name, times + 1);
            return win;
        }

        /// <summary>
        /// 基本UI布局:添加UI            
        /// </summary>
        public static void AddUIElement(string name, Action callback, DelUIElement<string, GUILayoutOption[]> addUI, float udSpace = 20, float lrSpace = 200, params GUILayoutOption[] options)
        {
            GUILayout.Space(udSpace);
            GUILayout.BeginHorizontal();
            GUILayout.Space(lrSpace);
            if (addUI.Invoke(name, options))
            {
                callback?.Invoke();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 基本UI布局:添加UI            
        /// </summary>
        public static void AddUIElement<T>(string name, ref T value, Action callback, DelUIElement<string, T, GUILayoutOption[]> addUI, float udSpace = 20, float lrSpace = 200, params GUILayoutOption[] options)
        {
            GUILayout.Space(udSpace);
            GUILayout.BeginHorizontal();
            GUILayout.Space(lrSpace);
            T newValue = addUI.Invoke(name, value, options);
            if (Equals(value, newValue))
            {
                callback?.Invoke();
            }

            value = newValue;
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void GetBtn(string name, Action callback, float width = 100, float height = 30)
        {
            if (GUILayout.Button(name, GUILayout.Width(width), GUILayout.Height(height)))
            {
                callback?.Invoke();
            }
        }

        /// <summary>
        /// 添加水平上单独一个按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void GetSingleBtn(string name, Action callback, float width = 100, float height = 30, float udSpace = 20, float lrSpace = 200)
        {
            AddUIElement(name, callback, GUILayout.Button, udSpace, lrSpace, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// 添加水平上两个按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void GetDoubleBtn(string name1, Action callback1, string name2, Action callback2, float width = 100, float height = 30, float udSpace = 20, float lrSpace = 200, float btnSpace = 50)
        {
            GUILayout.Space(udSpace);
            GUILayout.BeginHorizontal();
            GUILayout.Space(GetDis(width, lrSpace, btnSpace));
            GetBtn(name1, callback1, width, height);
            GUILayout.Space(btnSpace);
            GetBtn(name2, callback2, width, height);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 获取单行输入框            
        /// </summary>
        public static void GetInputField(string name, ref string value, Action callback, float width = 300, float udSpace = 20, float lrSpace = 50)
        {
            AddUIElement(name, ref value, callback, EditorGUILayout.TextField, udSpace, lrSpace, GUILayout.Width(width), GUILayout.ExpandWidth(false));
        }

        /// <summary>
        /// 获取float输入框            
        /// </summary>
        public static void GetFloatField(string name, ref float value, Action callback, float width = 300, float udSpace = 20, float lrSpace = 50)
        {
            AddUIElement(name, ref value, callback, EditorGUILayout.FloatField, udSpace, lrSpace, GUILayout.Width(width), GUILayout.ExpandWidth(false));
        }

        /// <summary>
        /// 获取int输入框            
        /// </summary>
        public static void GetIntField(string name, ref int value, Action callback, float width = 300, float udSpace = 20, float lrSpace = 50)
        {
            AddUIElement(name, ref value, callback, EditorGUILayout.IntField, udSpace, lrSpace, GUILayout.Width(width));
        }

        /// <summary>
        /// 获取Toggle输入框            
        /// </summary>
        public static void GetToggle(string name, ref bool value, Action callback, float width = 300, float udSpace = 20, float lrSpace = 50)
        {
            AddUIElement(name, ref value, callback, EditorGUILayout.Toggle, udSpace, lrSpace, GUILayout.Width(width));
        }

        /// <summary>
        /// 获取合适的距离            
        /// </summary>
        public static float GetDis(float width = 300, float lrSpace = 10, float uiSpace = 100)
        {
            return lrSpace + width / 2 - width - uiSpace / 2;
        }
    }

}
