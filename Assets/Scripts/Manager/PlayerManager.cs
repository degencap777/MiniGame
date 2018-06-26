using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Tools;
using Common;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerManager : BaseManager
{
    public PlayerManager(GameFacade facade) : base(facade) { }

    //UIPanel使用
    public UserData UserData;

    private int _idCount = 0;
    private int IdCount { get { return  _idCount++; } }

    private List<RoleData> _roleDataList;
    public List<RoleData> RoleDataList { get
        {
            return _roleDataList ?? (_roleDataList = facade.GetRoleDataList());
        } }

    private GameObject _currentCamGameObject;
    private GameObject currentCamGameObject { get { return _currentCamGameObject; } set
        {
            _currentCamGameObject = value;
            facade.CamFollowTarget(_currentCamGameObject.transform);
        } }

    private readonly List<Player> playerList=new List<Player>();
    private Player _localPlayer;
    public Player LocalPlayer { get { return _localPlayer = GetLocalPlayer(); } }


    //场景中每一个英雄对象都对应一个独一无二Id
    private Dictionary<int, GameObject> roleGameObjects =new Dictionary<int, GameObject>();
    

    private MoveRequest moveRequest;
    private UseSkillRequest useSkillRequest;
    private UseItemRequest useItemRequest;
    private DamageRequest damageRequest;

    private SkillManager skillManager;
    private ItemManager itemManager;

    private Vector3 deadPosition;
    private Dictionary<string, GameObject> _effectPrefabs;
    private Dictionary<string ,GameObject> EffectPrefabs { get
    {
        if (_effectPrefabs == null) _effectPrefabs = facade.GetEffectDict();
        return _effectPrefabs;
    } }

    //private ShootRequest shootRequest;
    //private AttackRequest attackRequest;

    public override void OnInit()
    {
        base.OnInit();
        skillManager = GameObject.FindGameObjectWithTag("SkillManager").GetComponent<SkillManager>();
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
        skillManager.PlayerManager = this;
    }

    public override void Update()
    {
        base.Update();
    }
    
    public void EnterPlaying()
    {
        //TODO 进入游戏场景后
        SetPlayerSpawnPosition();
        CreateSyncRequest();
        CreateBattleManager();

        if (facade.GetCurrentPanel().GetType().Name == "GamePanel")
        {
            ((GamePanel) facade.GetCurrentPanel()).CampType = LocalPlayer.CampType;
        }
    }

    public void InitPlayerData(UserData ud,List<UserData> userDatas)
    {
        playerList.Clear();
        foreach (var userData in userDatas)
        {
            Player player = new Player(userData, userData.SeatIndex);
            if (ud.Id == userData.Id)
            {
                player.IsLocal = true;
            }
            playerList.Add(player);
        }
    }

    private void SetPlayerSpawnPosition()
    {
        Transform spawnPositions = GameObject.Find("RolePositions").transform;
        Vector3 deadPos = spawnPositions.Find("DeadPosition").transform.position;
        foreach (var player in playerList)
        {
            player.SpawnPosition = spawnPositions.Find("Position" + player.SeatIndex).transform.position;
            player.DeadPostion = deadPos;
        }
        
    }

    public void SpawnRoles(CampType campType)
    {
        foreach (Player player in playerList)
        {
            if (player.CampType != campType) continue;
            HeroData rd = GetHeroDataBySeatIndex(player.SeatIndex);
            GameObject go = null;
            go = Object.Instantiate(rd.RolePrefab, player.SpawnPosition, Quaternion.identity);
            player.Reference=new GameObject("Player"+player.UserData.Id);
            player .Reference.transform.SetPositionAndRotation(new Vector3(0,0,0),Quaternion.identity );
            
            int instanceId = IdCount;
            SetRoleList(instanceId,go,player);
            if (player.CampType == rd.CampType)
            {
                go.AddComponent<VisualProvider>();
            }
            else
            {
                go.AddComponent<VisualTest>();
            }
            if (player.IsLocal)
            {
                go.AddComponent<PlayerMove>().SetPlayerMng(this).IsLocal = true;
                currentCamGameObject = go;
                if(facade.GetCurrentPanel().GetType().Name=="GamePanel")
                    ((GamePanel)facade.GetCurrentPanel()).SetPlayer(go);
            }
            go.AddComponent<PlayerSkill>().SetPlayerMng(this);
            go.AddComponent<PlayerItem>().SetPlayerMng(this);
            InitPlayerInfo(rd, go,player,instanceId);
        }
    }
    private void InitPlayerInfo(RoleData roleData, GameObject go, Player player,int instanceId)
    {
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        switch (roleData.RoleType)
        {
            case RoleType.Hero:
            {
                HeroData rd = roleData as HeroData;
                pi.Init(this,rd.Id, rd.CampType, rd.Name, rd.RoleType, rd.Description, rd.Hp, rd.Mp, rd.MoveSpeed,
                    rd.TurnSpeed, rd.IsSkyVision, player, instanceId, rd.attackDamage);
            }
                break;
            case RoleType.Pet:
            {
                PetData rd = (PetData)roleData;
                pi.Init(this,rd.Id, rd.CampType, rd.Name, rd.RoleType, rd.Description, rd.Hp, rd.Mp, rd.MoveSpeed,
                    rd.TurnSpeed, rd.IsSkyVision, player, instanceId);
            }
                break;
        }
    }
    #region Get_Function
    private HeroData GetHeroDataBySeatIndex(int seatIndex)
    {
        foreach (var roleData in RoleDataList)
        {
            if(roleData.RoleType==RoleType.Hero)
                if (((HeroData)roleData).seatIndex.Contains(seatIndex))
                    return (HeroData)roleData;
        }
        return null;
    }
    public Transform GetCurrentCamTarget()
    {
        return currentCamGameObject.transform;
    }

    private Player GetLocalPlayer()
    {
        foreach (var player in playerList)
        {
            if (player.IsLocal)
                return player;
        }
        return null;
    }

    private int GetCurrentGoId()
    {
        return LocalPlayer.CurrentRoleInstanceId;
    }
    #endregion

    #region Set_Function

    private void SetRoleList(int instanceId,GameObject go,Player player)
    {
        roleGameObjects.Add(instanceId, go);
        player.RoleInstanceIdList.Add(instanceId);
        player.CurrentRoleInstanceId = instanceId;
    }
    

    #endregion
    public void UpdateResult(int totalCount, int winCount)
    {
        //userData.TotalCount = totalCount;
        //userData.WinCount = winCount;
    }
    
    private void CreateSyncRequest()
    {
        GameObject playerSyncRequest = new GameObject("PlayerSyncRequest");
        moveRequest = playerSyncRequest.AddComponent<MoveRequest>();
        moveRequest.PlayerManager = this;
        useSkillRequest = playerSyncRequest.AddComponent<UseSkillRequest>();
        useSkillRequest.PlayerManager = this;
        useItemRequest = playerSyncRequest.AddComponent<UseItemRequest>();
        useItemRequest.PlayerManager = this;
        damageRequest = playerSyncRequest.AddComponent<DamageRequest>();
        damageRequest.PlayerManager = this;
    }

    private void CreateBattleManager()
    {
        GameObject battleManager=new GameObject("BattleManager");
    }
    public void Move()
    {
        if (LocalPlayer == null|| LocalPlayer.RoleInstanceIdList.Count == 0)
            return;
        StringBuilder sb = new StringBuilder();
        int count = 0;
        foreach (var id in LocalPlayer.RoleInstanceIdList)
        {
            GameObject go = roleGameObjects[id];
            if (go.GetComponent<PlayerInfo>().CurrentState != PlayerInfo.State.Move)continue;
            count++;
            sb.Append(id + "|" + UnityTools.PackVector3(go.transform.position) + "|" +
                      UnityTools.PackVector3(go.transform.eulerAngles)+":");
        }
        if (count == 0) return;
        sb.Remove(sb.Length - 1, 1);
        moveRequest.SendRequest(sb.ToString());
    }

    public void MoveSync(int goId, Vector3 pos, Vector3 rot)
    {
        GameObject go = roleGameObjects.TryGet(goId);
        if (go == null) return;
        PlayerInfo playerInfo = go.GetComponent<PlayerInfo>();
        if (playerInfo != null)
            playerInfo.CurrentState= PlayerInfo.State.Move;
        roleGameObjects[goId].transform.position = pos;
        roleGameObjects[goId].transform.eulerAngles = rot;
    }

    
    public void UseSkill(string skillName,string axis=null)
    {
        useSkillRequest.SendRequest(GetCurrentGoId(),skillName,axis);
    }

    public void UseSkillSync(int instanceId,string skillName,string axis=null)
    {
        GameObject go = roleGameObjects.TryGet(instanceId);
        if (go == null) return;
        Skill skill = skillManager.GetInstanceOfSkillWithString(skillName, go);
        if (skill == null)
        {
            Debug.Log("技能不存在");
            return;
        }
        Debug.Log(go.GetComponent<PlayerInfo>().Name);
        go.GetComponent<PlayerSkill>().StartUseSkill(skill,axis);
    }

    public void UseItem(string itemName, string point = null)
    {
        useItemRequest.SendRequest(LocalPlayer.UserData.Id,GetCurrentGoId(), itemName, point);
    }

    public void UseItemSync(int id,int instanceId, string itemName, string point = null)
    {
        GameObject go = roleGameObjects[instanceId];
        bool isLocal = id == LocalPlayer.UserData.Id;
        Item item = itemManager.GetInstanceOfItemWithString(itemName, go);
        if (item == null)
        {
            Debug.Log("道具不存在");
            return;
        }
        go.GetComponent<PlayerItem>().StartUseItem(isLocal, item, point);
    }

    public void Damage(int instanceId, string data)
    {
        damageRequest.SendRequest(instanceId,data);
    }

    public void DamageSync(int instanceId, DamageType damageType, string data)
    {
        GameObject go = roleGameObjects.TryGet(instanceId);
        if (go == null)
        {
            Debug.Log("ROLE不存在");
            return;
        }
        PlayerInfo playerInfo = go.GetComponent<PlayerInfo>();
        switch (damageType)
        {
            case DamageType.Damage:
                playerInfo.Damage(int.Parse(data));
                break;
            case DamageType.SpeedDown:
                break;
            case DamageType.Destroy:
                GameObject.FindWithTag("RemovableCube").AddComponent<DestroyForTime>().time = 0.5f;
                break;
            default:
                throw new ArgumentOutOfRangeException("damageType", damageType, null);
        }
    }

    public void Die(int instanceId)
    {
        GameObject go = roleGameObjects.TryGet(instanceId);
        if (go != null)
        {
            PlayerInfo pi = go.GetComponent<PlayerInfo>();
            switch (pi.CampType)
            {
                case CampType.Monkey:
                    Revive(instanceId);
                    break;
                case CampType.Fish:
                    go.SetActive(false);
                    GameObject refGo = pi.Player.Reference.gameObject;
                    refGo.name = pi.Player.UserData.Username + "Destroy";
                    pi.Player.Reference = new GameObject("Player" + pi.Player.UserData.Id);
                    foreach (Transform child in refGo.transform)
                    {
                        if (child != null)
                        {
                            Object.Instantiate(EffectPrefabs["Bomb"], child);
                            child.gameObject.AddComponent<DestroyForTime>();
                        }
                    }
                    refGo.gameObject.AddComponent<DestroyForTime>();
                    foreach (var rd in RoleDataList)
                    {
                        if (rd.Id == 3)
                        {
                            GameObject dead = Object.Instantiate(rd.RolePrefab, pi.Player.DeadPostion, Quaternion.identity);
                            pi.Player.Dead = dead;
                            int newInstanceId = IdCount;
                            SetRoleList(newInstanceId, dead, pi.Player);
                            InitPlayerInfo(rd, dead, pi.Player, newInstanceId);

                            if (pi.Player.IsLocal)
                            {
                                dead.AddComponent<PlayerMove>().AddLimit(pi.Player.DeadPostion,3).SetPlayerMng(this).IsLocal = true;
                                currentCamGameObject = dead;
                                if (facade.GetCurrentPanel().GetType().Name == "GamePanel")
                                {
                                    ((GamePanel)facade.GetCurrentPanel()).Die(pi.Player);
                                }
                            }
                            if (pi.Player.CampType == LocalPlayer.CampType)
                            {
                                dead.AddComponent<VisualProvider>();
                            }
                            else
                            {
                                dead.AddComponent<VisualTest>();
                            }
                        }
                    }
                    break;
                case CampType.Middle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }

    public void Revive(int instanceId)
    {
        GameObject go = roleGameObjects.TryGet(instanceId);
        if (go == null) return;
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        switch (pi.CampType)
        {
            case CampType.Monkey:
                go.transform.position = pi.Player.SpawnPosition;
                break;
            case CampType.Fish:
                go.SetActive(true);
                go.transform.position = pi.Player.DeadPostion;
                if (pi.Player.IsLocal)
                {
                    currentCamGameObject = go; ;
                    if (facade.GetCurrentPanel().GetType().Name == "GamePanel")
                    {
                        ((GamePanel)facade.GetCurrentPanel()).Revive(pi.Player, go);
                    }
                }
                pi.Player.CurrentRoleInstanceId = instanceId;
                pi.Player.Dead.SetActive(false);
                pi.Player.Dead.AddComponent<DestroyForTime>();
                break;
            case CampType.Middle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void GameOver()
    {
        _idCount = 0;
        _localPlayer = null;
        playerList.Clear();
        roleGameObjects.Clear();
    }
}
