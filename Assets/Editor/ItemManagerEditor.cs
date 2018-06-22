using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using EGL = UnityEditor.EditorGUILayout;

[CustomEditor(typeof(ItemManager))]
public class ItemManagerEditor : Editor
{
    ItemManager sm;

    SerializedObject so;

    List<string> items;
    

    SerializedProperty newItemName;
    SerializedProperty newItemClassName;
    SerializedProperty resources;
    SerializedProperty parameterCount;
    SerializedProperty parameterName;
    SerializedProperty parameterValue;

    private void OnEnable()
    {
        sm = (ItemManager)target;
        so = new SerializedObject(target);
        newItemName = so.FindProperty("newItemName");
        newItemClassName = so.FindProperty("newItemClassName");
        resources = so.FindProperty("resources");
        parameterCount = so.FindProperty("parameterCount");
        parameterName = so.FindProperty("parameterName");
        parameterValue = so.FindProperty("parameterValue");
    }

    bool tryToAddNewItem = false;
    int tryToModify = -1;

    string msg = "";
    public override void OnInspectorGUI()
    {
        EGL.LabelField("已有道具:");
        items = ItemXmlManager.XmlReader.FindItems();
        for (int i = 0; i < items.Count; i++)
        {
            EGL.BeginHorizontal();
            EGL.LabelField(items[i]);
            if (!tryToAddNewItem && tryToModify == -1)
            {
                if (GUILayout.Button("修改"))
                {
                    tryToModify = i;
                    sm.LoadItemClassInfo(items[tryToModify]);
                    so.Update();
                }
                if (GUILayout.Button("删除"))
                {
                    sm.DeleteItemClass(items[i]);
                    so.Update();
                }
            }
            EGL.EndHorizontal();
        }
        if (tryToModify == -1 && tryToAddNewItem == false)
        {
            EGL.Separator();
            EGL.Separator();
            EGL.Separator();
            EGL.Separator();
            if (GUILayout.Button("添加一个新道具"))
            {
                tryToAddNewItem = true;
                sm.GetNewItemTemplate();
                so.Update();
            }
        }
        if (tryToModify != -1)
        {
            tryToAddNewItem = false;
            EGL.Separator();
            EGL.Separator();
            EGL.LabelField("类名:  " + items[tryToModify]);
            EGL.PropertyField(newItemName, new GUIContent("道具名字"));
            EGL.PropertyField(resources, new GUIContent("道具预置体"));
            EGL.LabelField("该预置体必须处于\"Resource/ItemPrefabs\"文件夹下!");
            EGL.BeginHorizontal();
            EGL.PropertyField(parameterName, new GUIContent("参数名字"), true);
            EGL.PropertyField(parameterValue, new GUIContent("参数值"), true);
            EGL.EndHorizontal();
            EGL.BeginHorizontal();
            if (GUILayout.Button("应用"))
            {
                so.ApplyModifiedProperties();
                sm.ModifyItem(items[tryToModify]);
                so.Update();
                tryToModify = -1;
            }
            if (GUILayout.Button("取消"))
            {
                tryToModify = -1;
            }
            EGL.EndHorizontal();
        }
        if(tryToAddNewItem)
        {
            EGL.Separator();
            EGL.Separator();
            EGL.PropertyField(newItemClassName, new GUIContent("类名"));
            EGL.PropertyField(newItemName, new GUIContent("道具名字"));
            EGL.PropertyField(resources, new GUIContent("道具预置体"));
            EGL.LabelField("该预置体必须处于\"Resource/ItemPrefabs\"文件夹下!");
            EGL.BeginHorizontal();
            EGL.PropertyField(parameterName, new GUIContent("参数名字"), true);
            EGL.PropertyField(parameterValue, new GUIContent("参数值"), true);
            EGL.EndHorizontal();
            EGL.BeginHorizontal();
            if (GUILayout.Button("应用"))
            {
                so.ApplyModifiedProperties();
                if (sm.CreateNewItemClass())
                {
                    tryToAddNewItem = false;
                    msg = "";
                }
                else
                {
                    msg = "请先删除重名类！或更改一个新类名";
                }
            }
            if (GUILayout.Button("取消"))
            {
                tryToAddNewItem = false;
                msg = "";
            }
            EGL.EndHorizontal();
        }
        EGL.LabelField(msg);
    }
}
