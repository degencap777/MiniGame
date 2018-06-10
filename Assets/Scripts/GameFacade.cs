using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common;
using UnityEngine;

public class GameFacade : MonoBehaviour
{
    private static GameFacade _instance;

    public static GameFacade Instance
    {
        get
        {
            //if (_instance == null)
            //{
            //    Debug.Log(1);
            //    _instance = GameObject.Find("Facade").GetComponent<GameFacade>();
            //}
            return _instance;
        }
    }


    private UIManager uiMng;

    private AudioManager audioMng;

    private PlayerManager playerMng;

    private CameraManager cameraMng;

    private RequestManager requestMng;

    private ClientManager clientMng;

    private ResourceManager resourceManager;

    public int FISH_NUM = 8;
    public int MONKEY_NUM = 3;
    public int PLAYER_NUM = 11;

    private bool isSceneUpdate = false;
    private int CurrentSceneIndex = 0;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject); 
        }
    }
    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateManager();
        if (isSceneUpdate)
        {
            EnterPlaying();
            isSceneUpdate = false;
        }
    }

    private void Init()
    {
        uiMng = new UIManager(this);
        audioMng = new AudioManager(this);
        playerMng = new PlayerManager(this);
        requestMng = new RequestManager(this);
        cameraMng = new CameraManager(this);
        clientMng = new ClientManager(this);
        resourceManager=new ResourceManager(this);
        clientMng.OnInit();
        uiMng.OnInit();
        audioMng.OnInit();
        playerMng.OnInit();
        requestMng.OnInit();
        cameraMng.OnInit();
        resourceManager.OnInit();
    }

    private void DestroyManager()
    {
        clientMng.OnDestroy();
        uiMng.OnDestroy();
        audioMng.OnDestroy();
        playerMng.OnDestroy();
        requestMng.OnDestroy();
        cameraMng.OnDestroy();
        resourceManager.OnDestroy();
    }

    private void UpdateManager()
    {
        clientMng.Update();
        uiMng.Update();
        audioMng.Update();
        playerMng.Update();
        requestMng.Update();
        cameraMng.Update();
        resourceManager.Update();
    }
    void OnDestroy()
    {
        DestroyManager();
    }
    public void AddRequest(ActionCode actionCode, BaseRequest Request)
    {
        requestMng.AddRequest(actionCode, Request);
    }

    public void RemoveRequest(ActionCode actionCode)
    {
        requestMng.RemoveRequest(actionCode);
    }
    public void HandleResponse(ActionCode actionCode, string data)
    {
        requestMng.HandleResponse(actionCode, data);
    }

    public void ShowMessage(string msg)
    {
        uiMng.ShowMessage(msg);
    }

    public UIPanelType GetCurrentPanelType()
    {
        return uiMng.GetCurrentPanelType();
    }

    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        clientMng.SendRequest(requestCode, actionCode, data);
    }

    public void PlayBgSound(string soundName)
    {
        audioMng.PlayBgSound(soundName);
    }
    public void PlayNormalSound(string soundName)
    {
        audioMng.PlayNormalSound(soundName);
    }

    public void SetUserData(UserData ud)
    {
        playerMng.UserData = ud;
    }

    public UserData GetUserData()
    {
        return playerMng.UserData;
    }
    public void SetCurrentCampType(CampType rt)
    {
        playerMng.SetCurrentCampType(rt);
    }

    public Transform GetRolePosition()
    {
        return playerMng.GetRolePosition();
    }

    public void UpdateSceneSync()
    {
        isSceneUpdate = true;
    }
    public void EnterPlaying()
    {
        if (CurrentSceneIndex != 1) return;
        cameraMng.UpdateCamera();
        //TODO 游戏初始化
    }

    public void StartPlaying()
    {
        throw new NotImplementedException();
    }
}
