using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Tools;
using Common;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerManager : BaseManager
{
    public PlayerManager(GameFacade facade) : base(facade) { }

    //UIPanel使用
    public UserData UserData;

    private int _idCount = 0;
    private int IdCount { get { return  _idCount++; } }

    //资源
    private List<RoleData> _roleDataList;
    public List<RoleData> RoleDataList { get
        {
            return _roleDataList ?? (_roleDataList = facade.GetRoleDataList());
        }
    }
    private Dictionary<string, GameObject> _effectPrefabs;
    private Dictionary<string, GameObject> EffectPrefabs
    {
        get
        {
            if (_effectPrefabs == null) _effectPrefabs = facade.GetEffectDict();
            return _effectPrefabs;
        }
    }

    //private GameObject _currentCamGameObject;
    //private GameObject currentCamGameObject { get { return _currentCamGameObject; } set
    //    {
    //        _currentCamGameObject = value;
    //        facade.CamFollowTarget(_currentCamGameObject.transform);
    //    } }

    public Player LocalPlayer { get { return GetLocalPlayer(); } }

    //游戏结束后需要清空
    //场景中每一个英雄对象都对应一个独一无二Id
    private Dictionary<int, GameObject> roleGameObjects =new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> itemGameObjects=new Dictionary<int, GameObject>();
    private readonly List<Player> playerList = new List<Player>();
    
    private MoveRequest moveRequest;
    private UseSkillRequest useSkillRequest;
    private UseItemRequest useItemRequest;
    private AttackRequest attackRequest;

    private SkillManager skillManager;
    private ItemManager itemManager;

    public bool IsRolesChange = false;

    //private ShootRequest shootRequest;

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
        if(LocalPlayer!=null)
        if (LocalPlayer!=null&&LocalPlayer.RoleInstanceIdList.Count!=0)
        {
            if(roleGameObjects.TryGet(LocalPlayer.CurrentRoleInstanceId) == null)
                LocalPlayer.CurrentRoleInstanceId = LocalPlayer.RoleInstanceIdList[0];
            Debug.Log("当前英雄ID"+ LocalPlayer.CurrentRoleInstanceId);
        }
    }
    
    public void EnterPlaying()
    {
        //TODO 进入游戏场景后
        SetPlayerSpawnPosition();
        CreateSyncRequest();

        if (facade.GetCurrentPanel().GetType().Name == "GamePanel")
        {
            ((GamePanel) facade.GetCurrentPanel()).SetCamp(LocalPlayer.CampType); 
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
        facade.PlaySound("Swapn"+campType);
        foreach (Player player in playerList)
        {
            if (player.CampType != campType) continue;
            HeroData rd = GetHeroDataBySeatIndex(player.SeatIndex);
            GameObject go = Object.Instantiate(rd.RolePrefab, player.SpawnPosition, Quaternion.identity);
            player.Reference=new GameObject("Player"+player.UserData.Id);
            player.Reference.transform.SetPositionAndRotation(new Vector3(0,0,0),Quaternion.identity );
            
            int instanceId = IdCount;
            SetRoleList(instanceId,go);
            InitPlayerInfo(rd, go, player, instanceId);
            SetCurrentRole(go);

            if (player.CampType == LocalPlayer.CampType)
            {
                go.AddComponent<VisualProvider>();
            }
            else
            {
                go.AddComponent<VisualTest>();
            }

            if (player.IsLocal)
            {
                go.AddComponent<PlayerMove>().SetPlayerMng(this);
            }
        }
    }
    private void InitPlayerInfo(RoleData roleData, GameObject go, Player player,int instanceId)
    {
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        pi.Init(this, roleData, player, instanceId);
    }
    #region Get_Function
    private HeroData GetHeroDataBySeatIndex(int seatIndex)
    {
        foreach (var roleData in RoleDataList)
        {
            if (roleData.RoleType == RoleType.Hero)
            {
                if(roleData.CampType==CampType.Fish&&roleData.Name!="Fish0")continue;;
                if (((HeroData)roleData).seatIndex.Contains(seatIndex))
                    return (HeroData)roleData;

            }
        }
        return null;
    }

    private Player GetLocalPlayer()
    {
        if (playerList == null || playerList.Count == 0) return null;
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

    public GameObject GetCurrentOpTarget()
    {
        if (LocalPlayer == null) return null;
        return roleGameObjects.TryGet(LocalPlayer.CurrentRoleInstanceId);
    }

    public List<GameObject> GetLocalGameObjects()
    {
        List<GameObject> gos=new List<GameObject>();
        for (int i = 0; i < LocalPlayer.RoleInstanceIdList.Count; i++)
        {
            int id = LocalPlayer.RoleInstanceIdList[i];
            if (roleGameObjects.TryGet(id) == null)
            {
                LocalPlayer.RoleInstanceIdList.Remove(id);
            }
            else
            {
                gos.Add(roleGameObjects[id]);
            }
        }
        return gos;
    }
    #endregion

    #region Set_Function

    private void SetRoleList(int instanceId,GameObject go)
    {
        roleGameObjects.Add(instanceId, go);
    }

    public void SetItemList(int id, GameObject go)
    {
        itemGameObjects.Add(id,go);
    }
    /// <summary>
    /// 本地和远程的都可以
    /// </summary>
    /// <param name="go"></param>
    public void SetCurrentRole(GameObject go)
    {
        if (go == null) return;
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        go.GetComponent<PlayerInfo>().Player.CurrentRoleInstanceId = pi.InstanceId;
    }
    public void SetCurrentRole(int instanceId)
    {
        GameObject go = roleGameObjects.TryGet(instanceId);
        if (go == null) return;
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        go.GetComponent<PlayerInfo>().Player.CurrentRoleInstanceId = pi.InstanceId;
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
        attackRequest = playerSyncRequest.AddComponent<AttackRequest>();
        attackRequest.PlayerManager = this;
    }

    public void Move()
    {
        if (LocalPlayer == null|| LocalPlayer.RoleInstanceIdList.Count == 0)
            return;
        StringBuilder sb = new StringBuilder();
        int count = 0;
        foreach (var id in LocalPlayer.RoleInstanceIdList)
        {
            GameObject go = roleGameObjects.TryGet(id);
            if (go==null||!go.GetComponent<PlayerInfo>().anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded")||go.GetComponent<PlayerInfo>().IsMove==false)continue;
            
            count++;
            sb.Append(id + "|" + UnityTools.PackVector3(go.transform.position) + "|" +
                      UnityTools.PackVector3(go.transform.eulerAngles)+ "|"+go.GetComponent<PlayerInfo>().anim.GetFloat("Forward") + ":");
        }
        if (count == 0) return;
        sb.Remove(sb.Length - 1, 1);
        moveRequest.SendRequest(sb.ToString());
    }

    public void MoveSync(int goId, Vector3 pos, Vector3 rot,float forward)
    {
        GameObject go = roleGameObjects.TryGet(goId);
        if (go == null) return;
        PlayerInfo playerInfo = go.GetComponent<PlayerInfo>();
        if (playerInfo != null)
        {
            playerInfo.IsMove = true;
        }
        roleGameObjects[goId].transform.position = pos;
        roleGameObjects[goId].transform.eulerAngles = rot;
        roleGameObjects[goId].GetComponent<PlayerInfo>().anim.SetFloat("Forward",forward);
    }

    public void Attack(int instanceId,GameObject target)
    {
        int layer = target.layer;
        if (layer == 9)
        {
            attackRequest.SendRequest(layer + "|" + instanceId + "|" + UnityTools.PackVector2(target.GetComponent<MapInfo>().IndexV2));
        }
        else if (layer == 10)
        {
            attackRequest.SendRequest(layer + "|" + instanceId + "|" + target.GetComponent<PlayerInfo>().InstanceId);
        }
        else if(layer==11)
        {
            attackRequest.SendRequest(layer+"|"+instanceId+"|"+target.GetInstanceID());
        }
    }

    public void AttackSync(int InstanceId,Vector2 pos)
    {
        GameObject role = roleGameObjects.TryGet(InstanceId);
        GameObject target = null;
        if (GameFacade.Instance.GetCurrentPanelType() == UIPanelType.Game)
        {
            Debug.Log(pos);
            GamePanel gamePanel = (GamePanel) facade.GetCurrentPanel();
            target = gamePanel.MapInfos[(int) pos.x, (int) pos.y].gameObject;
        }
        if (role != null && target != null)
        {
            role.GetComponent<PlayerAttack>().Attack(target);
        }
    }
    public void AttackSync(int InstanceId, int targetId,bool isHero=true)
    {
        GameObject role = roleGameObjects.TryGet(InstanceId);
        GameObject target;
        if (isHero)
            target = roleGameObjects.TryGet(targetId);
        else
        {
            target = itemGameObjects.TryGet(targetId);
        }
        if (role != null&&target!=null)
        {
            role.GetComponent<PlayerAttack>().Attack(target);
        }
    }
    public void UseSkill(string skillName,string axis=null)
    {
        roleGameObjects[GetCurrentGoId()].GetComponent<PlayerInfo>().ToUseSkill = true;
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

    //public void Damage(int instanceId, string data)
    //{
    //    damageRequest.SendRequest(instanceId,data);
    //}

    //public void DamageSync(int instanceId, DamageType damageType, string data)
    //{
    //    GameObject go = roleGameObjects.TryGet(instanceId);
    //    if (go == null)
    //    {
    //        Debug.Log("ROLE不存在");
    //        return;
    //    }
    //    PlayerInfo playerInfo = go.GetComponent<PlayerInfo>();
    //    switch (damageType)
    //    {
    //        case DamageType.Damage:
    //            playerInfo.Damage(int.Parse(data));
    //            break;
    //        case DamageType.SpeedDown:
    //            break;
    //        case DamageType.Destroy:
    //            GameObject.FindWithTag("RemovableCube").AddComponent<DestroyForTime>().time = 0.5f;
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException("damageType", damageType, null);
    //    }
    //}
    /// <summary>
    /// 只处理英雄死亡
    /// </summary>
    /// <param name="instanceId"></param>
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
                            SetRoleList(newInstanceId, dead);
                            InitPlayerInfo(rd, dead, pi.Player, newInstanceId);
                            SetCurrentRole(dead);

                            if (pi.Player.IsLocal)
                            {
                                dead.AddComponent<PlayerMove>().AddLimit(pi.Player.DeadPostion, 3).SetPlayerMng(this);
                            }
                            if (facade.GetCurrentPanel().GetType().Name == "GamePanel")
                            {
                                ((GamePanel)facade.GetCurrentPanel()).Die(go);
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
        Debug.Log(go==null);
        if (go == null) return;
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        pi.Revive();
        switch (pi.CampType)
        {
            case CampType.Monkey:
                go.transform.position = pi.Player.SpawnPosition;
                SetCurrentRole(go);
                break;
            case CampType.Fish:
                go.SetActive(true);
                go.transform.position = pi.Player.DeadPostion;
                SetCurrentRole(go);
                if (pi.Player.IsLocal)
                {
                    if (facade.GetCurrentPanel().GetType().Name == "GamePanel")
                    {
                        ((GamePanel)facade.GetCurrentPanel()).Revive(go);
                    }
                }
                
                break;
            case CampType.Middle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OpenAltar(Vector3 position,int altarCount)
    {
        foreach (RoleData rd in RoleDataList)
        {
            if (rd.Name == "Bird0"&&altarCount==1)
            {
                foreach (Player player in playerList)
                {
                    if(player.CampType==CampType.Fish)
                        AddPet((PetData) rd, roleGameObjects.TryGet(player.CurrentRoleInstanceId));
                }
            }
            if (rd.Name == "Fish" + altarCount)
            {
                Mesh mesh = rd.RolePrefab.GetComponent<MeshFilter>().sharedMesh;
                foreach (Player player in playerList)
                {
                    if (player.CampType == CampType.Fish)
                    {
                        GameObject role = roleGameObjects.TryGet(player.RoleInstanceIdList[0]);
                        if (role != null)
                        {
                            role.GetComponent<MeshFilter>().mesh = mesh;
                        }
                    }

                }
            }
        }
    }
    public GameObject AddPet(PetData pd,GameObject go)
    {
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        Player player = pi.Player;
        GameObject pet = Object.Instantiate(pd.RolePrefab, go.transform.position, Quaternion.identity);

        int instanceId = IdCount;
        SetRoleList(instanceId, pet);
        InitPlayerInfo(pd, pet, player, instanceId);
        SetCurrentRole(pet);

        if (player.CampType == LocalPlayer.CampType)
        {
            pet.AddComponent<VisualProvider>();
        }
        else
        {
            pet.AddComponent<VisualTest>();
        }
        if (player.IsLocal)
        {
            pet.AddComponent<PlayerMove>().SetPlayerMng(this);
        }
        return pet;
    }

    public void GameOver()
    {
        _idCount = 0;
        playerList.Clear();
        roleGameObjects.Clear();
        itemGameObjects.Clear();
    }

    public CampType QuitPlayer(int id)
    {
        Player player = null;
        foreach (Player p in playerList)
        {
            if (p.UserData.Id == id)
            {
                player = p;
                break;
            }
        }
        Object.Destroy(player.Reference);
        foreach (int instanceId in player.RoleInstanceIdList)
        {
            GameObject go = roleGameObjects.TryGet(instanceId);
            if(go==null)continue;
            roleGameObjects.Remove(instanceId);
            Object.Destroy(go);
        }
        playerList.Remove(player);
        return player.CampType;
    }
}
