using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Assets.Scripts.Tools;
using Common;
using DG.Tweening;
using UnityEngine;

public class MoveRequest : BaseRequest
{

    private Transform LocalPlayerTransform;
    private PlayerMove LocalPlayerMove;
    private int syncRate = 50;
    private Transform remotePlayerTransform;
    private Animator remotePlayerAnim;
    private bool isSyncRemotePlayer = false;
    private int syncId;
    class syncData
    {
        public int goId;
        public Vector3 pos;
        public Vector3 rot;

        public syncData(int goId, Vector3 pos, Vector3 rot)
        {
            this.goId = goId;
            this.pos = pos;
            this.rot = rot;
        }
    }

    public PlayerManager PlayerManager;
    private syncData[] syncDatas = new syncData[GameFacade.MAX_ROLE_NUM_IN_SCENE];
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Move;

        base.Awake();
    }

    void Start()
    {
        InvokeRepeating("SyncLocalPlayer", 1f, 1f / syncRate);
    }

    void FixedUpdate()
    {
        if (syncDatas.Length != 0)
        {
            for (int i = 0; i < syncDatas.Length; i++)
            {
                if(syncDatas[i]==null)continue;
                syncData sd = syncDatas[i];
                PlayerManager.MoveSync(sd.goId, sd.pos, sd.rot);
                syncDatas[i] = null;
            }
        }
    }
    private void SyncLocalPlayer()
    {
        PlayerManager.Move();//ID+ROLEID+位置
    }

    public void SendRequest(float x, float y, float z, float rotationX, float rotationY, float rotationZ)
    {
        string data = string.Format("{0},{1},{2}|{3},{4},{5}", x, y, z, rotationX, rotationY, rotationZ);
        base.SendRequest(data);
    }
    public override void OnResponse(string data)
    {
        string[] str = data.Split(':');
        for (int i = 0; i < str.Length; i++)
        {
            string[] strs = str[i].Split('|');
            syncDatas[int.Parse(strs[0])] = (new syncData(int.Parse(strs[0]), UnityTools.ParseVector3(strs[1]), UnityTools.ParseVector3(strs[2])));
        }
    }
}
