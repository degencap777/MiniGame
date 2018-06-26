using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public const int FISH_NUM = 8;
    public const int MONKEY_NUM = 3;
    public const int PLAYER_NUM = 11;
    public const int ROLE_NUM = 4;
    public const int MAX_ROLE_NUM_IN_SCENE = 100;
    public const int MAX_ROLE_NUM_OF_PLAYER = 10;

    private bool isSceneUpdate = false;


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
        //gameObject.AddComponent<Test>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateManager();
        if (isSceneUpdate)
        {
            isSceneUpdate = false;
            EnterPlaying();
        }
        if (EasyTouch.current != null)
        {
            Debug.Log(EasyTouch.current.type);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DebugConsole.Clear();
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

    #region ResourceGet
    public List<RoleData> GetRoleDataList()
    {
        return resourceManager.RoleDataList;
    }
    public List<HeroData> GetHeroDataList()
    {
        return resourceManager.HeroDataList;
    }

    public Dictionary<string, GameObject> GetEffectDict()
    {
        return resourceManager.EffectDict;
    }

    #endregion
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

    public BasePanel GetCurrentPanel()
    {
        return uiMng.GetCurrentPanel();
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
    

    public Transform GetCurrentCamTarget()
    {
        return playerMng.GetCurrentCamTarget();
    }

    public void SetUserData(UserData ud)
    {
        playerMng.UserData = ud;
    }

    public UserData GetUserData()
    {
        return playerMng.UserData;
    }
    public void UpdateSceneAsync()
    {
        isSceneUpdate = true;
    }
    public void EnterPlaying()
    {
        playerMng.EnterPlaying();
        //TODO 进入游戏场景后初始化资源
    }

    /// <summary>
    /// 计时器结束，玩家可以开始操作
    /// </summary>
    public void StartPlaying(CampType campType)
    {
        playerMng.SpawnRoles(campType);
    }

    public void FollowCurrentTarget()
    {
        cameraMng.FollowCurrentTarget();
    }
    public void CamFollowTarget(Transform target)
    {
        cameraMng.FollowTartget(target);
    }
    public void SetCamTarget(Transform target)
    {
        cameraMng.SetTarget(target);
    }
    public void InitPlayerData(UserData ud, List<UserData> userDatas)
    {
        playerMng.InitPlayerData(ud,userDatas);
    }

    public void UseSkill(string skillName,string axis=null)
    {
        playerMng.UseSkill(skillName,axis);
    }
    public void UseItem(string itemName, string point = null)
    {
        playerMng.UseItem(itemName, point);
    }

    public void UseItemSync(bool isUse)
    {
        Debug.Log(uiMng.GetCurrentPanel().GetType().Name);
        if (uiMng.GetCurrentPanel().GetType().Name == "GamePanel")
        {
            ((GamePanel)uiMng.GetCurrentPanel()).UseItemSync(isUse);
        }
    }
    public void UseSkillSync(float coldTime)
    {
        if (uiMng.GetCurrentPanel().GetType().Name == "GamePanel")
        {
            ((GamePanel)uiMng.GetCurrentPanel()).UseSkillSync(coldTime);
        }
    }

    public Player GetLocalPlayer()
    {
        return playerMng.LocalPlayer;
    }
    public void GameOver()
    {
        playerMng.GameOver();
        cameraMng.GameOver();
        
    }
}
