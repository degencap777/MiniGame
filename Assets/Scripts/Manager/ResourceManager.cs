using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using LitJson;
using UnityEngine;

public class ResourceManager : BaseManager {

    public ResourceManager(GameFacade facade) : base(facade) { }

    private List<RoleData> _roleDataList;
    public List<RoleData> RoleDataList{get { return IsRoleDataLoaded ? _roleDataList : null; }}

    private List<HeroData> _heroDataList;
    public List<HeroData> HeroDataList { get { return isHeroDataLoaded ? _heroDataList : null; } }

    public Dictionary<string, GameObject> EffectDict
    {
        get
        {
            return _effectDict;
        }
    }

    private Dictionary<string ,GameObject> _effectDict;
    private Dictionary<string ,GameObject> _skillItemDict=new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> SkillItemDict { get { return _skillItemDict; } }

    public bool IsRoleDataLoaded = false;
    private bool isHeroDataLoaded = false;
    public override void OnInit()
    {
        base.OnInit();
        ParseRoleDataJson();
        LoadEffect();
        LoadSkillItem();
    }
    
    public override void Update()
    {
        base.Update();
        if (IsRoleDataLoaded)
        {
            if (isHeroDataLoaded)
            {
                foreach (RoleData roleData in RoleDataList)
                {
                    if (roleData.RoleType == RoleType.Hero)
                        _heroDataList.Add((HeroData)roleData);
                }
                isHeroDataLoaded = true;
            }
        }
    }

    void LoadSkillItem()
    {
        GameObject[] skillItems = Resources.LoadAll<GameObject>("UIItem/Skill");
        foreach (GameObject skillItem in skillItems)
        {
            _skillItemDict.Add(skillItem.name,skillItem);
        }
    }
    void ParseRoleDataJson()
    {
        _roleDataList = new List<RoleData>();
        TextAsset itemText = Resources.Load<TextAsset>("Json/RoleData");
        JsonData itemsData = JsonMapper.ToObject(itemText.text);
        RoleData roleData = null;
        foreach (JsonData itemData in itemsData)
        {
            int id = (int)itemData["id"];
            CampType campType = (CampType) Enum.Parse(typeof(CampType), itemData["campType"].ToString());
            string name = itemData["name"].ToString();
            RoleType roleType = (RoleType)Enum.Parse(typeof(RoleType), itemData["roleType"].ToString());
            string description = itemData["description"].ToString();
            string path = itemData["path"].ToString();
            int hp = (int)itemData["hp"];
            int mp = (int)itemData["mp"];
            int moveSpeed = (int)itemData["moveSpeed"];
            int turnSpeed = (int)itemData["turnSpeed"];
            bool isSkyVision = itemData["isSkyVision"].ToString() == "true";
            string imagePath = itemData["imagePath"].ToString();
            switch (campType)
            {
                case CampType.Fish:
                    switch (roleType)
                    {
                        case RoleType.Hero:
                            JsonData seatIndexs = itemData["seatIndex"];
                            List<int> seatIndexList = new List<int>();
                            foreach (JsonData seatIndex in seatIndexs)
                            {
                                seatIndexList.Add((int)seatIndex);
                            }
                            int attackDamage = (int)itemData["attackDamage"];
                            roleData=new HeroData(id,campType,name, roleType,description,path,hp,mp,moveSpeed,turnSpeed,isSkyVision,seatIndexList,attackDamage,imagePath);
                            break;
                        case RoleType.Pet:
                            roleData = new PetData(id, campType, name, roleType, description, path, hp, mp, moveSpeed, turnSpeed, isSkyVision,imagePath);
                            break;
                    }
                    break;
                case CampType.Monkey:
                    switch (roleType)
                    {
                        case RoleType.Hero:
                            JsonData seatIndexs = itemData["seatIndex"];
                            List<int> seatIndexList = new List<int>();
                            foreach (JsonData seatIndex in seatIndexs)
                            {
                                seatIndexList.Add((int)seatIndex);
                            }
                            int attackDamage = (int)itemData["attackDamage"];
                            roleData = new HeroData(id, campType, name, roleType, description, path, hp, mp, moveSpeed, turnSpeed, isSkyVision, seatIndexList, attackDamage,imagePath);
                            break;
                        case RoleType.Pet:
                            roleData = new PetData(id, campType, name, roleType, description, path, hp, mp, moveSpeed, turnSpeed, isSkyVision,imagePath);
                            break;
                    }
                    break;
                case CampType.Middle:
                    break;
            }
            _roleDataList.Add(roleData);
            Debug.Log(roleData.ToString());
        }
        IsRoleDataLoaded = true;
    }

    void LoadEffect()
    {
        GameObject[] effects = Resources.LoadAll<GameObject>("Prefabs/Effect");
        _effectDict=new Dictionary<string, GameObject>();
        foreach (GameObject effcet in effects)
        {
            _effectDict.Add(effcet.name, effcet);
        }
    }
}
