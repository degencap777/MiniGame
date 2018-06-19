using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using EGL = UnityEditor.EditorGUILayout;

[CustomEditor(typeof(SkillManager))]
public class SkillManagerEditor : Editor
{
    SkillManager sm;

    SerializedObject so;

    List<string> skills;
    

    SerializedProperty newSkillName;
    SerializedProperty newSkillClassName;
    SerializedProperty resources;
    SerializedProperty parameterCount;
    SerializedProperty parameterName;
    SerializedProperty parameterValue;

    private void OnEnable()
    {
        sm = (SkillManager)target;
        so = new SerializedObject(target);
        newSkillName = so.FindProperty("newSkillName");
        newSkillClassName = so.FindProperty("newSkillClassName");
        resources = so.FindProperty("resources");
        parameterCount = so.FindProperty("parameterCount");
        parameterName = so.FindProperty("parameterName");
        parameterValue = so.FindProperty("parameterValue");
    }

    bool tryToAddNewSkill = false;
    int tryToModify = -1;

    string msg = "";
    public override void OnInspectorGUI()
    {
        EGL.LabelField("已有技能:");
        skills = XmlManager.XmlReader.FindSkills();
        for (int i = 0; i < skills.Count; i++)
        {
            EGL.BeginHorizontal();
            EGL.LabelField(skills[i]);
            if (!tryToAddNewSkill && tryToModify == -1)
            {
                if (GUILayout.Button("修改"))
                {
                    tryToModify = i;
                    sm.LoadSkillClassInfo(skills[tryToModify]);
                    so.Update();
                }
                if (GUILayout.Button("删除"))
                {
                    sm.DeleteSkillClass(skills[i]);
                    so.Update();
                }
            }
            EGL.EndHorizontal();
        }
        if (tryToModify == -1 && tryToAddNewSkill == false)
        {
            EGL.Separator();
            EGL.Separator();
            EGL.Separator();
            EGL.Separator();
            if (GUILayout.Button("添加一个新技能"))
            {
                tryToAddNewSkill = true;
                sm.GetNewSkillTemplate();
                so.Update();
            }
        }
        if (tryToModify != -1)
        {
            tryToAddNewSkill = false;
            EGL.Separator();
            EGL.Separator();
            EGL.LabelField("类名:  " + skills[tryToModify]);
            EGL.PropertyField(newSkillName, new GUIContent("技能名字"));
            EGL.PropertyField(resources, new GUIContent("技能预置体"));
            EGL.LabelField("该预置体必须处于\"Resource/SkillPerferbs\"文件夹下!");
            EGL.BeginHorizontal();
            EGL.PropertyField(parameterName, new GUIContent("参数名字"), true);
            EGL.PropertyField(parameterValue, new GUIContent("参数值"), true);
            EGL.EndHorizontal();
            EGL.BeginHorizontal();
            if (GUILayout.Button("应用"))
            {
                so.ApplyModifiedProperties();
                sm.ModifySkill(skills[tryToModify]);
                so.Update();
                tryToModify = -1;
            }
            if (GUILayout.Button("取消"))
            {
                tryToModify = -1;
            }
            EGL.EndHorizontal();
        }
        if(tryToAddNewSkill)
        {
            EGL.Separator();
            EGL.Separator();
            EGL.PropertyField(newSkillClassName, new GUIContent("类名"));
            EGL.PropertyField(newSkillName, new GUIContent("技能名字"));
            EGL.PropertyField(resources, new GUIContent("技能预置体"));
            EGL.LabelField("该预置体必须处于\"Resource/SkillPerferbs\"文件夹下!");
            EGL.BeginHorizontal();
            EGL.PropertyField(parameterName, new GUIContent("参数名字"), true);
            EGL.PropertyField(parameterValue, new GUIContent("参数值"), true);
            EGL.EndHorizontal();
            EGL.BeginHorizontal();
            if (GUILayout.Button("应用"))
            {
                so.ApplyModifiedProperties();
                if (sm.CreateNewSkillClass())
                {
                    tryToAddNewSkill = false;
                    msg = "";
                }
                else
                {
                    msg = "请先删除重名类！或更改一个新类名";
                }
            }
            if (GUILayout.Button("取消"))
            {
                tryToAddNewSkill = false;
                msg = "";
            }
            EGL.EndHorizontal();
        }
        EGL.LabelField(msg);
    }
}
