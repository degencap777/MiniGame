using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Text;


public class SkillManager : MonoBehaviour {

    protected Dictionary<string, Skill> loadedSkills = new Dictionary<string, Skill>();



    // 该方法通过技能类返回对应技能实例
    public aSkill GetInstanceOfSkill<aSkill>(GameObject requester) where aSkill : Skill, new()
    {
        aSkill skill;
        if (loadedSkills.ContainsKey(typeof(aSkill).ToString()))
        {
            skill = loadedSkills[typeof(aSkill).ToString()] as aSkill;
        }
        else
        {
            skill = XmlManager.XmlReader.LoadSkill<aSkill>();
            loadedSkills[typeof(aSkill).ToString()] = skill;
        }
        aSkill newSkill = new aSkill();
        newSkill.SetName(skill.name);
        newSkill.resourcesName_ = skill.resourcesName_;
        newSkill.parameters = skill.parameters;
        newSkill.owner = requester;
        newSkill.sm = this;
        return newSkill;
    }

    
    // 该方法通过技能类名返回对应技能实例，出于性能原因，如果可能请使用GetInstanceOfSkill<aSkill>(GameObject requester)方法代替。
    public Skill GetInstanceOfSkillWithString(string skillClassName, GameObject requester)
    {
        MethodInfo mi = typeof(SkillManager).GetMethod("GetInstanceOfSkill").MakeGenericMethod(System.Type.GetType(skillClassName));
        return mi.Invoke(this, new object[] { requester }) as Skill;
    }


    public void StartASkill(Skill skill)
    {
        string skillClassName = skill.GetType().ToString();
        GameObject user = skill.owner;

        Debug.Log("SkillManager: " + user.ToString()+" uses skill " + skillClassName);
    }













    public string newSkillName = "新技能";
    public string newSkillClassName = "NewSkill";
    public GameObject resources;
    public uint parameterCount = 1;
    public List<string> parameterName = new List<string>();
    public List<float> parameterValue = new List<float>();
    public Dictionary<string, float> parameters = new Dictionary<string, float>();

    public void GetNewSkillTemplate()
    {
        newSkillName = "新技能";
        newSkillClassName = "NewSkill";
        resources = null;
        parameterCount = 1;
        parameterName = new List<string>();
        parameterName.Add("Param");
        parameterValue = new List<float>();
        parameterValue.Add(0);
    }
    public bool CreateNewSkillClass()
    {
        int parameterCount = Mathf.Min(parameterName.Count, parameterValue.Count);
        parameters = new Dictionary<string, float>();
        for (int i = 0; i < parameterCount; i++)
        {
            parameters[parameterName[i]] = parameterValue[i];
        }
        XmlDocument xml = new XmlDocument();
        string path = Application.dataPath + "/Resources/SkillXml/Skills.xml";
        xml.Load(path);
        string check = ((XmlElement)(xml.SelectSingleNode("Root"))).GetAttribute(newSkillClassName);
        if (check != "") return false;
        ((XmlElement)(xml.SelectSingleNode("Root"))).SetAttribute(newSkillClassName, "1");
        xml.Save(path);
        string newClassTemplate = classTemplate.Replace("_NewSkillClassName_", newSkillClassName);
        path = Application.dataPath + "/SkillSystem/SkillClasses/" + newSkillClassName + ".cs";
        FileStream fileStream = File.OpenWrite(path);
        byte[] map = Encoding.UTF8.GetBytes(newClassTemplate);
        fileStream.Write(map, 0, map.Length);
        fileStream.Close();
        fileStream.Dispose();
        XmlManager.XmlWriter.SaveSkill(newSkillClassName, newSkillName, resources != null ? resources.name : "", parameters);
        System.Diagnostics.Process.Start(path);
        return true;

    }

    public bool DeleteSkillClass(string skillClassName)
    {
        File.Delete(Application.dataPath + "/SkillSystem/SkillClasses/" + skillClassName + ".cs");
        File.Delete(Application.dataPath + "/Resources/SkillXml/" + skillClassName + ".xml");
        XmlDocument xml = new XmlDocument();
        string path = Application.dataPath + "/Resources/SkillXml/Skills.xml";
        xml.Load(path);
        ((XmlElement)(xml.SelectSingleNode("Root"))).RemoveAttribute(skillClassName);
        xml.Save(path);
        return true;
    }

    
    public void LoadSkillClassInfo(string skillClassName)
    {
        Skill skill = XmlManager.XmlReader.LoadSkillInfo(skillClassName);
        newSkillName = skill.name;
        parameterName = new List<string>();
        parameterValue = new List<float>();
        foreach (var keyvaluepair in skill.parameters)
        {
            parameterName.Add(keyvaluepair.Key);
            parameterValue.Add(keyvaluepair.Value);
        }
        resources = Resources.Load<GameObject>("SkillPerferbs/" + skill.resourcesName_);
    }

    public void ModifySkill(string skillClassName)
    {
        int parameterCount = Mathf.Min(parameterName.Count, parameterValue.Count);
        parameters = new Dictionary<string, float>();
        for (int i = 0; i < parameterCount; i++)
        {
            parameters[parameterName[i]] = parameterValue[i];
        }
        XmlManager.XmlWriter.SaveSkill(skillClassName, newSkillName, resources!=null? resources.name:"", parameters);
    }
    

    static string classTemplate = "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class _NewSkillClassName_ : Skill\n{\n    //timeSinceSkillStart     技能从发动以来经过的时间\n    //owner                   技能发动者\n    //name                    技能名(不是类名，是游戏中想要展示的名字)\n    //parameters              可调技能参数的字典\n//resources               资源预置体\n\n\n\n    //这个方法会在技能发动时调用,返回值为false则发动失败，不会调用后续方法\n    protected override void SkillStart()\n    {\n\n    }\n\n    //这个方法会在技能进行过程中不断调用，当返回true表示技能已经完成所有动作\n    protected override bool SkillDidAction()\n    {\n        return false;\n    }\n\n    //这个方法会在技能结束时调用\n    protected override void SkillDidEnd()\n    {\n\n    }\n    //您可以通过该方法提供一个技能的详细描述，您可以通过在文字中嵌入属性字典中的值来避免反复修改代码。\n    public override string GetDescription()\n    {\n        return \"skill \" + name + \" has no specific description.\";\n    }\n}";
}
