using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Reflection;

public class XmlManager {

    static public bool CheckExist(string name)
    {
        string path = Application.dataPath + "/Resources/SkillXml/" + name + ".xml";
        XmlDocument xml = new XmlDocument();
        xml.Load(path);
        return xml != null;
    }

    public class XmlReader
    {
        static string path = Application.dataPath + "/Resources/SkillXml/Skills.xml";
        public static List<string> FindSkills()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            List<string> res = new List<string>();
            XmlAttributeCollection skills = ((XmlElement)(xml.SelectSingleNode("Root"))).Attributes;
            int count = skills.Count;
            for (int i = 0; i < count; i++)
            {
                res.Add(skills[i].Name);
            }
            return res;
        }
        public static aSkill LoadSkill<aSkill>() where aSkill : Skill, new()
        {
            aSkill skill = new aSkill();
            string skillName = typeof(aSkill).Name;
            XmlDocument xml = new XmlDocument();
#if UNITY_EDITOR
            string path = Application.dataPath + "/Resources/SkillXml/" + skillName + ".xml";
            xml.Load(path);
#else
            string data = Resources.Load("SkillXml/" + skillName).ToString();
            xml.LoadXml(data);
#endif
            XmlElement root = (XmlElement)(xml.SelectSingleNode("Root"));
            skill.SetName(root.GetAttribute("Name"));
            skill.resourcesName_ = root.GetAttribute("Resources");
            int parameterCount = int.Parse(root.GetAttribute("parameterCount"));
            XmlElement parameters = (XmlElement)(root.SelectSingleNode("parameters"));
            skill.parameters = new Dictionary<string, float>();
            for (int i = 0; i < parameterCount; i++)
            {
                skill.parameters[parameters.Attributes[i].Name] = float.Parse(parameters.Attributes[i].Value);
            }
            if (skill == null)
                Debug.Log("ERROR TYPE!");
            if (xml == null)
            {
                Debug.Log("XML FILE NOT FOUND!");
                return null;
            }
            return skill;
        }

        public static Skill LoadSkillInfo(string skillName)
        {
            SkillInfo skill = new SkillInfo();
            XmlDocument xml = new XmlDocument();
            string path = Application.dataPath + "/Resources/SkillXml/" + skillName + ".xml";
            xml.Load(path);
            XmlElement root = (XmlElement)(xml.SelectSingleNode("Root"));
            skill.SetName(root.GetAttribute("Name"));
            skill.resourcesName_ = root.GetAttribute("Resources");
            int parameterCount = int.Parse(root.GetAttribute("parameterCount"));
            XmlElement parameters = (XmlElement)(root.SelectSingleNode("parameters"));
            skill.parameters = new Dictionary<string, float>();
            for (int i = 0; i < parameterCount; i++)
            {
                skill.parameters[parameters.Attributes[i].Name] = float.Parse(parameters.Attributes[i].Value);
            }
            return skill;
        }
    }


    public class XmlWriter
    {
        public static void SaveSkill(string className, string name, string resources, Dictionary<string, float> parameters)
        {
            string skillName = className;
            XmlDocument xml = new XmlDocument();
            string path = Application.dataPath + "/Resources/SkillXml/" + skillName + ".xml";
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement root = (XmlElement)xml.AppendChild(xml.CreateElement("Root"));

            root.SetAttribute("Resources", resources); 
            root.SetAttribute("Name", name);
            root.SetAttribute("parameterCount", parameters.Count.ToString());
            XmlElement parameters_ = (XmlElement)root.AppendChild(xml.CreateElement("parameters"));

            foreach (var keyvaluePair in parameters)
            {
                parameters_.SetAttribute(keyvaluePair.Key, keyvaluePair.Value.ToString());
            }
            xml.Save(path);
        }
    }
}