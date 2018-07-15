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

    private SkillManager skillManager;
    private ItemManager itemManager;

    public const int FISH_NUM = 8;
    public const int MONKEY_NUM = 3;
    public const int PLAYER_NUM = 11;
    public const int ROLE_NUM = 4;
    public const int MAX_ROLE_NUM_IN_SCENE = 100;
    public const int MAX_ROLE_NUM_OF_PLAYER = 10;
    public int AltarCount = 0;

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
        skillManager = transform.Find("SkillManager").GetComponent<SkillManager>();
        itemManager = transform.Find("ItemManager").GetComponent<ItemManager>();
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

    #region Audio

    public void SetAudioSetting(bool music,bool sound,float musicValue,float soundValue)
    {
        audioMng.musicOn = music;
        audioMng.soundOn = sound;
        audioMng.musicVolume = musicValue;
        audioMng.soundVolume = soundValue;
    }
    public void OnMusicChanged(bool isOn)
    {
        audioMng.musicOn = isOn;
        if (isOn)
        {
            audioMng.ResumeMusic();
        }
        else
        {
            audioMng.PauseMusic();
        }
    }
    public void OnSoundChanged(bool isOn)
    {
        audioMng.soundOn = isOn;
        if (isOn)
        {
            audioMng.ResumeAllSounds();
        }
        else
        {
            audioMng.PauseAllSounds();
        }
    }

    public void OnMusicSliderValueChange(float value)
    {
        audioMng.musicVolume = value;
    }

    public void OnSoundSliderValueChange(float value)
    {
        audioMng.soundVolume = value;
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

    public void PlayMusic(string musicName,bool loop=true)
    {
        audioMng.PlayMusic(audioMng.Clips[musicName],loop);
    }
    public void PlaySound(string soundName, bool loop = false)
    {
        audioMng.PlaySound(audioMng.Clips[soundName],loop);
    }

    public void StopSound(string soundName)
    {
        audioMng.StopSound(audioMng.Clips[soundName]);
    }
    public List<GameObject> GetLocalGameObjects()
    {
        return playerMng.GetLocalGameObjects();
    }

    public bool IsRolesChange()
    {
        return playerMng.IsRolesChange;
    }
    public GameObject GetCurrentOpTarget()
    {
        return playerMng.GetCurrentOpTarget();
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

    public void SetCurrentRole(GameObject go)
    {
        playerMng.SetCurrentRole(go);
    }
    public void SetCurrentRole(int instanceid)
    {
        playerMng.SetCurrentRole(instanceid);
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
    public void UseSkillSync(string skillName,float coldTime)
    {
        if (uiMng.GetCurrentPanel().GetType().Name == "GamePanel")
        {
            ((GamePanel)uiMng.GetCurrentPanel()).UseSkillSync(skillName,coldTime);
        }
    }

    public void Attack(int instanceId,GameObject target)
    {
        playerMng.Attack(instanceId,target);
    }
    public Skill GetSkill(string name)
    {
        return skillManager.GetInstanceOfSkillWithString(name, null);
    }

    public Dictionary<string , GameObject> GetSkillItemDict()
    {
        return resourceManager.SkillItemDict;
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

    public CampType QuitPlayer(int id)
    {
        return playerMng.QuitPlayer(id);
    }

    public void OpenAltar(Vector3 position)
    {
        PlaySound("AltarOpen");
        AltarCount++;
        playerMng.OpenAltar(position,AltarCount);
        string msg = "开启第" + AltarCount + "座祭坛\n";
        if (AltarCount == 1)
        {
            msg += "小鲲获得侦查海鸥";
        }
        else if (AltarCount==2)
        {
            msg += "小鲲可以自己种方块啦";
            if (GetCurrentPanelType() == UIPanelType.Game)
            {
                ((GamePanel) GetCurrentPanel()).OpenAltar = true;
            }
        }
        else if (AltarCount == 3)
        {
            msg += "海鸥获得真实视野";
        }
        ShowMessage(msg);
        
    }
}
