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
    private UserData userData;
    private Transform rolePositions;
    private Dictionary<int,RoleData> roleDataDict=new Dictionary<int, RoleData>();//seat对应RoleData,只存初始的英雄，不包括其他单位

    private GameObject currentRoleGameObject;//当前操作的英雄对象

    //场景中每一个英雄对象都对应一个独一无二Id
    private Dictionary<int, GameObject> localRoleGameObjects =new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> remoteRoleGameObjects =new Dictionary<int, GameObject>();//int对应id

    private List<UserData> userDatas=new List<UserData>();

    private MoveRequest moveRequest;

    //private ShootRequest shootRequest;
    // AttackRequest attackRequest;
    public UserData UserData
    {
        get { return userData; }
        set { userData = value; }
    }

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
        rolePositions = GameObject.Find("RolePositions").transform;
        InitRoleDataDict();
        CreateSyncRequest();
    }
    private void InitRoleDataDict()
    {
        foreach (var ud in userDatas)
        {
            roleDataDict.Add(ud.SeatIndex, new RoleData(rolePositions, ud.SeatIndex));
        }
    }

    public void InitPlayerData(UserData ud,List<UserData> userDatas)
    {
        userData = ud;
        this.userDatas = userDatas;
    }
    
    public void SpawnRoles(CampType campType)
    {
        foreach (UserData ud in userDatas)
        {
            if (ud.CampType != campType) continue;
            RoleData rd = roleDataDict[ud.SeatIndex];
            GameObject go = null;
            go = Object.Instantiate(rd.RolePrefab, rd.SpawnPosition, Quaternion.identity);
            switch (campType)
            {
                case CampType.Fish:
                    go.tag = "Fish";
                    break;
                case CampType.Monkey:
                    go.tag = "Monkey";
                    break;
            }
            
            if (rd.SeatIndex == userData.SeatIndex)
            {
                currentRoleGameObject = go;
                localRoleGameObjects.Add(IdCount,go);
                AddControlScript();
            }
            else
            {
                remoteRoleGameObjects.Add(IdCount, go);
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
    
    public RoleData GetRoleData(int roleIndex)
    {
        return roleDataDict[roleIndex];
    }
    public Transform GetCurrentCamTarget()
    {
        return currentRoleGameObject.transform;
    }

    private UserData GetUserById(int UserId)
    {
        foreach (var ud in userDatas)
        {
            if (ud.Id == UserId)
                return ud;
        }
        Debug.Log("找不到UserId");
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
        if (localRoleGameObjects == null|| localRoleGameObjects.Count == 0)
            return;
        StringBuilder sb = new StringBuilder();
        int count = 0;
        foreach (var go in localRoleGameObjects)
        {
            count++;
            sb.Append(go.Key + "|" + UnityTools.PackVector3(go.Value.transform.position) + "|" +
                      UnityTools.PackVector3(go.Value.transform.eulerAngles));
            if (count < localRoleGameObjects.Count)
                sb.Append(":");
            //moveRequest.SendRequest(go.Value.transform.position.x, go.Value.transform.position.y, go.Value.transform.position.z,
            //    go.Value.transform.eulerAngles.x, go.Value.transform.eulerAngles.y, go.Value.transform.eulerAngles.z);
        }
        moveRequest.SendRequest(sb.ToString());
    }

    public void MoveSync(int goId, Vector3 pos, Vector3 rot)
    {
        remoteRoleGameObjects[goId].transform.position = pos;
        remoteRoleGameObjects[goId].transform.eulerAngles = rot;
        Debug.Log("EndMove");
    }
    
}
