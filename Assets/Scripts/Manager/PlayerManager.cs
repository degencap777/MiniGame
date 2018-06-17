using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Tools;
using Common;
using UnityEngine;

public class PlayerManager : BaseManager
{
    public PlayerManager(GameFacade facade) : base(facade) { }

    private int _idCount = 0;
    private int IdCount { get { return  _idCount++; } }

    private List<RoleData> _roleDataList;
    public List<RoleData> RoleDataList { get
        {
            return _roleDataList ?? (_roleDataList = facade.GetRoleDataList());
        } }

    private GameObject _currentRoleGameObject;
    public GameObject currentRoleGameObject { get { return _currentRoleGameObject; } set
        {
            _currentRoleGameObject = value;
            facade.CamFollowTarget(_currentRoleGameObject.transform);
        } }

    private List<Player> playerList=new List<Player>();
    private Player _localPlayer;
    private Player localPlayer { get { return _localPlayer ?? (_localPlayer = GetLocalPlayer()); } }


    //场景中每一个英雄对象都对应一个独一无二Id
    private Dictionary<int, GameObject> roleGameObjects =new Dictionary<int, GameObject>();
    

    private MoveRequest moveRequest;

    //private ShootRequest shootRequest;
    //private AttackRequest attackRequest;

    public override void OnInit()
    {
        base.OnInit();
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
    }

    public void InitPlayerData(UserData ud,List<UserData> userDatas)
    {
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
        foreach (var player in playerList)
        {
            player.SpawnPosition = spawnPositions.Find("Position" + player.SeatIndex).transform.position;
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
            switch (campType)
            {
                case CampType.Fish:
                    go.tag = "Fish";
                    break;
                case CampType.Monkey:
                    go.tag = "Monkey";
                    break;
            }
            int instanceId = IdCount;
            roleGameObjects.Add(instanceId, go);
            player.RoleInstanceIdList.Add(instanceId);
            player.currentRoleInstanceId = instanceId;
            if (player.IsLocal)
            {
                currentRoleGameObject = go;
                AddControlScript();
            }
        }
    }
    public void AddControlScript()
    {
        currentRoleGameObject.AddComponent<PlayerMove>().SetPlayerMng(this);
        //int roleIndex = currentRoleGameObject.GetComponent<PlayerInfo>().Index;
        //PlayerAttack playerAttack = currentRoleGameObject.AddComponent<PlayerAttack>();
        //RoleData rd = GetRoleData(ct);
        //playerAttack.arrowPrefab = rd.ArrowPrefab;
        // playerAttack.SetPlayerMng(this);
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
        return currentRoleGameObject.transform;
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
        //shootRequest = playerSyncRequest.AddComponent<ShootRequest>();
        //shootRequest.PlayerMng = this;
        //attackRequest = playerSyncRequest.AddComponent<AttackRequest>();
        //attackRequest.PlayerManager = this;
    }

    public void Move()
    {
        if (localPlayer == null|| localPlayer.RoleInstanceIdList.Count == 0)
            return;
        StringBuilder sb = new StringBuilder();
        int count = 0;
        foreach (var id in localPlayer.RoleInstanceIdList)
        {
            GameObject go = roleGameObjects[id];
            count++;
            sb.Append(id + "|" + UnityTools.PackVector3(go.transform.position) + "|" +
                      UnityTools.PackVector3(go.transform.eulerAngles));
            if (count < localPlayer.RoleInstanceIdList.Count)
                sb.Append(":");
        }
        moveRequest.SendRequest(sb.ToString());
    }

    public void MoveSync(int goId, Vector3 pos, Vector3 rot)
    {
        roleGameObjects[goId].transform.position = pos;
        roleGameObjects[goId].transform.eulerAngles = rot;
    }
    
}
