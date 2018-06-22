using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Text;


public class ItemManager : MonoBehaviour {

    protected Dictionary<string, Item> loadedItems = new Dictionary<string, Item>();



    // 该方法通过道具类返回对应道具实例
    public TItem GetInstanceOfItem<TItem>(GameObject requester) where TItem : Item, new()
    {
        TItem item;
        if (loadedItems.ContainsKey(typeof(TItem).ToString()))
        {
            item = loadedItems[typeof(TItem).ToString()] as TItem;
        }
        else
        {
            item = ItemXmlManager.XmlReader.LoadItem<TItem>();
            loadedItems[typeof(TItem).ToString()] = item;
        }
        TItem newItem = new TItem();
        newItem.SetName(item.name);
        newItem.resourcesName_ = item.resourcesName_;
        newItem.parameters = item.parameters;
        newItem.owner = requester;
        newItem.sm = this;
        return newItem;
    }

    
    // 该方法通过道具类名返回对应道具实例，出于性能原因，如果可能请使用GetInstanceOfItem<TItem>(GameObject requester)方法代替。
    public Item GetInstanceOfItemWithString(string itemClassName, GameObject requester)
    {
        MethodInfo mi = typeof(ItemManager).GetMethod("GetInstanceOfItem").MakeGenericMethod(System.Type.GetType(itemClassName));
        return mi.Invoke(this, new object[] { requester }) as Item;
    }


    public void StartAItem(Item item)
    {
        string itemClassName = item.GetType().ToString();
        GameObject user = item.owner;

        Debug.Log("ItemManager: " + user.ToString()+" uses item " + itemClassName);
    }













    public string newItemName = "新道具";
    public string newItemClassName = "NewItem";
    public GameObject resources;
    public uint parameterCount = 1;
    public List<string> parameterName = new List<string>();
    public List<float> parameterValue = new List<float>();
    public Dictionary<string, float> parameters = new Dictionary<string, float>();

    public void GetNewItemTemplate()
    {
        newItemName = "新道具";
        newItemClassName = "NewItem";
        resources = null;
        parameterCount = 1;
        parameterName = new List<string>();
        parameterName.Add("Param");
        parameterValue = new List<float>();
        parameterValue.Add(0);
    }
    public bool CreateNewItemClass()
    {
        int parameterCount = Mathf.Min(parameterName.Count, parameterValue.Count);
        parameters = new Dictionary<string, float>();
        for (int i = 0; i < parameterCount; i++)
        {
            parameters[parameterName[i]] = parameterValue[i];
        }
        XmlDocument xml = new XmlDocument();
        string path = Application.dataPath + "/Resources/ItemXml/Items.xml";
        xml.Load(path);
        string check = ((XmlElement)(xml.SelectSingleNode("Root"))).GetAttribute(newItemClassName);
        if (check != "") return false;
        ((XmlElement)(xml.SelectSingleNode("Root"))).SetAttribute(newItemClassName, "1");
        xml.Save(path);
        string newClassTemplate = classTemplate.Replace("_NewItemClassName_", newItemClassName);
        path = Application.dataPath + "/ItemSystem/ItemClasses/" + newItemClassName + ".cs";
        FileStream fileStream = File.OpenWrite(path);
        byte[] map = Encoding.UTF8.GetBytes(newClassTemplate);
        fileStream.Write(map, 0, map.Length);
        fileStream.Close();
        fileStream.Dispose();
        ItemXmlManager.XmlWriter.SaveItem(newItemClassName, newItemName, resources != null ? resources.name : "", parameters);
        System.Diagnostics.Process.Start(path);
        return true;

    }

    public bool DeleteItemClass(string itemClassName)
    {
        File.Delete(Application.dataPath + "/ItemSystem/ItemClasses/" + itemClassName + ".cs");
        File.Delete(Application.dataPath + "/Resources/ItemXml/" + itemClassName + ".xml");
        XmlDocument xml = new XmlDocument();
        string path = Application.dataPath + "/Resources/ItemXml/Items.xml";
        xml.Load(path);
        ((XmlElement)(xml.SelectSingleNode("Root"))).RemoveAttribute(itemClassName);
        xml.Save(path);
        return true;
    }

    
    public void LoadItemClassInfo(string itemClassName)
    {
        Item item = ItemXmlManager.XmlReader.LoadItemInfo(itemClassName);
        newItemName = item.name;
        parameterName = new List<string>();
        parameterValue = new List<float>();
        foreach (var keyvaluepair in item.parameters)
        {
            parameterName.Add(keyvaluepair.Key);
            parameterValue.Add(keyvaluepair.Value);
        }
        resources = Resources.Load<GameObject>("ItemPrefabs/" + item.resourcesName_);
    }

    public void ModifyItem(string itemClassName)
    {
        int parameterCount = Mathf.Min(parameterName.Count, parameterValue.Count);
        parameters = new Dictionary<string, float>();
        for (int i = 0; i < parameterCount; i++)
        {
            parameters[parameterName[i]] = parameterValue[i];
        }
        ItemXmlManager.XmlWriter.SaveItem(itemClassName, newItemName, resources!=null? resources.name:"", parameters);
    }
    

    static string classTemplate = "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\npublic class _NewItemClassName_ : Item\n{\n    //timeSinceItemStart     道具从发动以来经过的时间\n    //owner                   道具发动者\n    //name                    道具名(不是类名，是游戏中想要展示的名字)\n    //parameters              可调道具参数的字典\n//resources               资源预置体\n\n\n\n    //这个方法会在道具发动时调用,返回值为false则发动失败，不会调用后续方法\n    protected override void ItemStart()\n    {\n\n    }\n\n    //这个方法会在道具进行过程中不断调用，当返回true表示道具已经完成所有动作\n    protected override bool ItemDidAction()\n    {\n        return false;\n    }\n\n    //这个方法会在道具结束时调用\n    protected override void ItemDidEnd()\n    {\n\n    }\n    //您可以通过该方法提供一个道具的详细描述，您可以通过在文字中嵌入属性字典中的值来避免反复修改代码。\n    public override string GetDescription()\n    {\n        return \"item \" + name + \" has no specific description.\";\n    }\n}";
}
