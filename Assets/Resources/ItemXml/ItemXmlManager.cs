using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Reflection;

public class ItemXmlManager {

    static public bool CheckExist(string name)
    {
        string path = Application.dataPath + "/Resources/ItemXml/" + name + ".xml";
        XmlDocument xml = new XmlDocument();
        xml.Load(path);
        return xml != null;
    }

    public class XmlReader
    {
        static string path = Application.dataPath + "/Resources/ItemXml/Items.xml";
        public static List<string> FindItems()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            List<string> res = new List<string>();
            XmlAttributeCollection items = ((XmlElement)(xml.SelectSingleNode("Root"))).Attributes;
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                res.Add(items[i].Name);
            }
            return res;
        }
        public static TItem LoadItem<TItem>() where TItem : Item, new()
        {
            TItem item = new TItem();
            string itemName = typeof(TItem).Name;
            XmlDocument xml = new XmlDocument();
#if UNITY_EDITOR
            string path = Application.dataPath + "/Resources/ItemXml/" + itemName + ".xml";
            xml.Load(path);
#else
            string data = Resources.Load("ItemXml/" + itemName).ToString();
            xml.LoadXml(data);
#endif
            XmlElement root = (XmlElement)(xml.SelectSingleNode("Root"));
            item.SetName(root.GetAttribute("Name"));
            item.resourcesName_ = root.GetAttribute("Resources");
            int parameterCount = int.Parse(root.GetAttribute("parameterCount"));
            XmlElement parameters = (XmlElement)(root.SelectSingleNode("parameters"));
            item.parameters = new Dictionary<string, float>();
            for (int i = 0; i < parameterCount; i++)
            {
                item.parameters[parameters.Attributes[i].Name] = float.Parse(parameters.Attributes[i].Value);
            }
            if (item == null)
                Debug.Log("ERROR TYPE!");
            if (xml == null)
            {
                Debug.Log("XML FILE NOT FOUND!");
                return null;
            }
            return item;
        }

        public static Item LoadItemInfo(string itemName)
        {
            ItemInfo item = new ItemInfo();
            XmlDocument xml = new XmlDocument();
            string path = Application.dataPath + "/Resources/ItemXml/" + itemName + ".xml";
            xml.Load(path);
            XmlElement root = (XmlElement)(xml.SelectSingleNode("Root"));
            item.SetName(root.GetAttribute("Name"));
            item.resourcesName_ = root.GetAttribute("Resources");
            int parameterCount = int.Parse(root.GetAttribute("parameterCount"));
            XmlElement parameters = (XmlElement)(root.SelectSingleNode("parameters"));
            item.parameters = new Dictionary<string, float>();
            for (int i = 0; i < parameterCount; i++)
            {
                item.parameters[parameters.Attributes[i].Name] = float.Parse(parameters.Attributes[i].Value);
            }
            return item;
        }
    }


    public class XmlWriter
    {
        public static void SaveItem(string className, string name, string resources, Dictionary<string, float> parameters)
        {
            string itemName = className;
            XmlDocument xml = new XmlDocument();
            string path = Application.dataPath + "/Resources/ItemXml/" + itemName + ".xml";
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