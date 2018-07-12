using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Common;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public ETCJoystick Joystick { get; private set; }

    #region Chat

    private Transform sendButton;
    public Transform content;
    private Transform chatDialog;
    private Button chatButton;
    private InputField inputField;

    private GameObject OtherPlayerChatMsgItem;
    private GameObject LocalPlayerChatMsgItem;
    private GameObject ChangeSeatItem;

    private GameChatRequest gameChatRequest;
    #endregion

    private ShowTimerRequest showTimerRequest;
    private GameOverRequest gameOverRequest;
    private StartPlayRequest startPlayRequest;
    private QuitBattleRequest quitBattleRequest;
    private QuitGameRequest quitGameRequest;
    private DestroyRequest destroyRequest;

    private List<Transform> skillPos=new List<Transform>();

    private Dictionary<string,SkillJoystickItem> JoystickItemDict=new Dictionary<string, SkillJoystickItem>();
    private Dictionary<string, SkillItem> buttonItemDict = new Dictionary<string, SkillItem>();

    public Dictionary<string ,GameObject> SkillItemDict { get; private set; }
    public Dictionary<string, GameObject> EffectDict { get; private set; }
    
    private Text timer;
    private bool isShowTimer = false;
    private string time = null;

    #region GameOver

    private Color gameOver = new Color(43 / 255, 43 / 255, 43 / 255);
    private Color win = new Color(1, 216 / 255, 0);
    private Color lose = new Color(43 / 255, 43 / 255, 43 / 255);
    private Text gameOverText;
    private ReturnCode returnCode = ReturnCode.NotFind;
    private Result result;
    private float gameOverTimer = 0;
    private Button closeButton;

    #endregion

    private Knapsack knapsack;

    private Transform roleSelectPanel;

    public CampType CampType { get; private set; }
    private Player player;
    private GameObject _role;
    public GameObject Role
    {
        get
        {
            if (_role == null)
            {
                _role = facade.GetCurrentOpTarget();
                if(_role!=null)
                    SetPlayer(_role);
            }
            else if(_role != facade.GetCurrentOpTarget())
            {
                Debug.Log("GamePanel改变Role");
                _role = facade.GetCurrentOpTarget();
                SetPlayer(_role);
            }
            return _role;
        }
    }
    public List<GameObject> LocalRoles { get { return facade.GetLocalGameObjects(); } }
    public bool IsRolesChange { get { return facade.IsRolesChange(); } }

    private int deadCount = 0;
    public MapInfo[,] MapInfos=new MapInfo[120,120];

    private Dictionary<int,GameObject> headDict=new Dictionary<int, GameObject>();
    private bool skillHasSet = false;

    public GamePanel()
    {
        CampType = CampType.Middle;
    }


    void Awake()
    {
        Joystick = transform.Find("Joystick").GetComponent<ETCJoystick>();
        showTimerRequest = GetComponent<ShowTimerRequest>();
        startPlayRequest = GetComponent<StartPlayRequest>();
        gameOverRequest = GetComponent<GameOverRequest>();
        quitBattleRequest = GetComponent<QuitBattleRequest>();
        quitGameRequest = GetComponent<QuitGameRequest>();
        destroyRequest = GetComponent<DestroyRequest>();
        gameChatRequest = GetComponent<GameChatRequest>();
        knapsack = transform.Find("KnapsackPanel").GetComponent<Knapsack>();
        for (int i = 0; i < 3; i++)
        {
            skillPos.Add(transform.Find("Skill"+i));
        }
        
        roleSelectPanel = transform.Find("RoleSelectPanel");

        timer = transform.Find("TimerPanel/Time").GetComponent<Text>();
        gameOverText = transform.Find("GameOverPanel/GameOver").GetComponent<Text>();
        closeButton=transform.Find("GameOverPanel/CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseClick);

        EffectDict = facade.GetEffectDict();
        SkillItemDict = facade.GetSkillItemDict();

        transform.Find("SettingPanel/QuitGameButton").GetComponent<Button>().onClick.AddListener(OnQuitGameClick);

        chatDialog = transform.Find("ChatDialog");
        sendButton = transform.Find("ChatDialog/InputPanel/SendButton");
        chatButton = transform.Find("ChatDialog/ChatButton").GetComponent<Button>();
        inputField = transform.Find("ChatDialog/InputPanel/InputField").GetComponent<InputField>();
        sendButton.GetComponent<Button>().onClick.AddListener(OnSendClick);
        chatButton.onClick.AddListener(OnChatClick);
        inputField.onEndEdit.AddListener(x => OnSendClick());

        OtherPlayerChatMsgItem = Resources.Load<GameObject>("UIItem/Chat/OtherPlayerChatMsgItem");
        LocalPlayerChatMsgItem = Resources.Load<GameObject>("UIItem/Chat/LocalPlayerChatMsgItem");
        ChangeSeatItem = Resources.Load<GameObject>("UIItem/ChangeSeatItem");
        
    }


    void OnEnable()
    {
        EasyTouch.On_TouchDown += On_TouchDown;
        EasyTouch.SetEnableAutoSelect(true);
        EasyTouch.SetAutoUpdatePickedObject(false);
        LayerMask mask = EasyTouch.Get3DPickableLayer();
        mask = mask | (1 << 9) | (1 << 10);
        EasyTouch.Set3DPickableLayer(mask);
    }
    void Start()
    {
        showTimerRequest.SendRequest();
        timer.text = "15:00";
        gameOverText.text = null;
        closeButton.gameObject.SetActive(false);
    }
	// Update is called once per frame
	void Update ()
	{
	    Gesture gesture = EasyTouch.current;
	    if (isShowTimer)
	    {
	        timer.text = time;
	        isShowTimer = false;
	    }
	    if (returnCode!=ReturnCode.NotFind)
	    {
	        GameOver();
	    }
        if (deadCount == 1)
        {
            gameOverRequest.SendRequest();
            deadCount = 0;
        }
	    if (headDict.Count > 0)
	    {
	        roleSelectPanel.gameObject.SetActive(true);
	    }
	    else
	    {
	        roleSelectPanel.gameObject.SetActive(false);
        }
	    if (IsRolesChange)
	    {
	        foreach (GameObject role in LocalRoles)
	        {
	            SetPlayer(role);
	        }
	    }

    }

    private void OnChatClick()
    {
        float width = chatDialog.rectTransform().sizeDelta.x;
        if (chatDialog.rectTransform().rect.height == 0)
        {
#if UNITY_ANDROID
            chatDialog.rectTransform().sizeDelta = new Vector2(width, (float)500 / 1080 * Screen.currentResolution.height);
#elif UNITY_EDITOR
            chatDialog.rectTransform().sizeDelta = new Vector2(width, 500);
#endif
        }
        else
        {
            chatDialog.rectTransform().sizeDelta = new Vector2(width, 0);
        }
    }
    private void OnSendClick()
    {
        string msgName = player.UserData.Username;
        string msg = inputField.text;
        if (string.IsNullOrEmpty(msg)) return;
        gameChatRequest.SendRequest(msgName + "," + msg);
        inputField.text = "";
    }
    
    public void AddMsgItem(string username,string msg)
    {
        GameObject MsgItem = Instantiate(player.UserData.Username.Equals(username) ? LocalPlayerChatMsgItem : OtherPlayerChatMsgItem);
        MsgItem.transform.SetParent(content);
        MsgItem.transform.Find("PlayerNameText").GetComponent<Text>().text = username;
        MsgItem.transform.Find("ChatMsgBg/ChatMsgText").GetComponent<Text>().text = msg;
    }
    internal void ShowTimerSync(int time)
    {
        int min = time==0? 0:time / 60;
        int s = time % 60;
        this.time=min+":"+s;
        isShowTimer = true;
    }

    private void GameOver()
    {
        facade.GameOver();
        gameOverTimer += Time.deltaTime;
        gameOverText.transform.parent.GetComponent<Image>().raycastTarget = true;
        if (gameOverTimer < 2)
        {
            gameOverText.text = "GameOver";
            gameOverText.color = gameOver;
        }
        else if (gameOverTimer < 4)
        {
            switch (returnCode)
            {
                case ReturnCode.Win:
                    gameOverText.color = win;
                    break;
                case ReturnCode.Lose:
                    gameOverText.color = lose;
                    break;
            }

            gameOverText.text = returnCode.ToString();
        }
        else
        {
            closeButton.gameObject.SetActive(true);
        }
    }

    public void QuitGame(int id=-1)
    {
        if (id == -1)
        {
            facade.GameOver();
            gameOverTimer += Time.deltaTime;
            gameOverText.transform.parent.GetComponent<Image>().raycastTarget = true;
            if (gameOverTimer < 2)
            {
                gameOverText.text = "主机退出游戏\n游戏结束";
                gameOverText.color = gameOver;
            }
            else
            {
                closeButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (facade.QuitPlayer(id)==CampType.Fish)
            {
                deadCount++;
            }
        }
    }
    public void GameOverAsync(ReturnCode returnCode,Result result=null)
    {
        this.returnCode = returnCode;
        this.result = result;
    }

    private void OnQuitGameClick()
    {
        facade.GameOver();
        quitGameRequest.SendRequest();
        OnCloseClick();
    }
    private void OnCloseClick()
    {
        quitBattleRequest.SendRequest();
        uiMng.PopPanel();
        uiMng.PopPanel();
        uiMng.DestroyPanel(UIPanelType.Game);
        uiMng.DestroyPanel(UIPanelType.LoadGame);
        SceneManager.LoadScene("Main");
        facade.GameOver();
    }

    public void UseSkill(string skillName,string axis=null)
    {
        facade.UseSkill(skillName,axis);
    }

    public void UseSkillSync(string skillName,float coldTime)
    {
        JoystickItemDict[skillName].UseSkillSync(coldTime);
    }

    public void UseItem(string itemName, string point = null)
    {
        facade.UseItem(itemName, point);
    }

    public void UseItemSync(bool isUse)
    {
        knapsack.UseItemSync(isUse);
    }

    private void SetInteractive(bool active)
    {
        int alpha = active ? 1 : 0;
        knapsack.gameObject.GetComponent<CanvasGroup>().alpha = alpha;
        knapsack.gameObject.GetComponent<CanvasGroup>().interactable = active;
        foreach (SkillItem skillItem in buttonItemDict.Values)
        {
            skillItem.gameObject.GetComponent<CanvasGroup>().alpha = alpha;
            skillItem.gameObject.GetComponent<CanvasGroup>().interactable = active;
        }
        foreach (SkillJoystickItem skillJoystickItem in JoystickItemDict.Values)
        {
            skillJoystickItem.gameObject.GetComponent<CanvasGroup>().alpha = alpha;
            skillJoystickItem.GetComponent<ETCJoystick>().activated = active;
        }
    }
    public void Die(GameObject go)
    {
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        if (pi.Player.IsLocal)
        {
            if (pi.RoleType == RoleType.Hero)
            {
                knapsack.Clear();
                SetInteractive(false);
                foreach (GameObject head in headDict.Values)
                {
                    Destroy(head);
                }
            }
            else
            {
                Destroy(headDict[pi.InstanceId]);
            }
        }
        if (pi.Player.CampType == CampType.Fish )
        {
            deadCount++;
        }
    }
    /// <summary>
    /// 只有英雄能复活
    /// </summary>
    /// <param name="player"></param>
    /// <param name="go"></param>
    public void Revive(GameObject go)
    {
        PlayerInfo pi = go.GetComponent<PlayerInfo>();
        Player player = pi.Player;
        SetPlayer(go);
        if (player.CampType == CampType.Fish&& pi.RoleType == RoleType.Hero)
        {
            deadCount--;
        }
    }

    public void SetCamp(CampType campType)
    {
        this.CampType = campType;
    }

    /// <summary>
    /// Player不能为空
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(GameObject role)
    {
        if (role == null|| role.GetComponent<PlayerInfo>().Player.IsLocal == false) return;
        PlayerInfo pi = role.GetComponent<PlayerInfo>();
        if (headDict.TryGet(pi.InstanceId) == null)
        {
            AddHeadImage(role);
        }
        if (pi.RoleType != RoleType.Hero)
        {
            SetInteractive(false);
            return;
        }
        SetInteractive(true);
        if(skillHasSet)return;
        if (this.player == null)
        {
            this.player = pi.Player;
        }
        switch (CampType)
        {
            case CampType.Monkey:
                switch (pi.Name)
                {
                    case "Monkey0":
                        AddSkillItem("Summon",0);
                        AddSkillItem("Transfer",1);
                        break;
                    case "Monkey1":
                        AddSkillItem("Eye", 0);
                        AddSkillItem("Trap", 1);
                        break;
                    case "Monkey2":
                        AddSkillItem("Ruin", 0);
                        AddSkillItem("Earthquake", 1);
                        break;
                }
                break;
            case CampType.Fish:
                AddSkillItem("SpeedUp",0);
                AddSkillItem("Blink",1);
                break;
            case CampType.Middle:
                break;
        }
        skillHasSet = true;
    }

    private void AddSkillItem(string skill,int skillIndex)
    {
        GameObject skillItem = Instantiate(SkillItemDict[skill], skillPos[skillIndex]);
        ((RectTransform)skillItem.transform).localPosition = Vector3.zero;
        bool isStick = facade.GetSkill(skill).parameters.ContainsKey("Stick");
        if (!isStick)
        {
            buttonItemDict.Add(skill, skillItem.GetComponent<SkillItem>());
        }
        else
        {
            JoystickItemDict.Add(skill, skillItem.GetComponent<SkillJoystickItem>());
        }
    }
    private void AddHeadImage(GameObject player)
    {
        if (headDict.ContainsKey(player.GetComponent<PlayerInfo>().InstanceId)) return;
        GameObject go = Instantiate(player.GetComponent<PlayerInfo>().RoleData.HeadPrefab, roleSelectPanel);
        headDict.Add(player.GetComponent<PlayerInfo>().InstanceId, go);
        go.AddComponent<HeadItem>().SetGamePanel(this,player);
    }

    public void RemoveHeadItem(int instanceId)
    {
        headDict.Remove(instanceId);
    }


    public void DestroyCube(Vector2 pos)
    {
        MapInfo cube = MapInfos[(int)pos.x, (int)pos.y];
        if (cube == null) return;
        cube.DestroyCube(EffectDict["Bomb"]);
    }
    void On_TouchDown(Gesture gesture)
    {
        Debug.Log(gesture.type);
        if (Role == null) return;
        //TODO 先寻路，到达后开始攻击
        if (gesture.pickedObject != null&&Role!=null)
        {
            if (gesture.pickedObject.tag == CampType.ToString()) return;
            PlayerInfo pi = Role.GetComponent<PlayerInfo>();
            if (pi.RoleType == RoleType.Hero)
            {
                switch (CampType)
                {
                    case CampType.Monkey:
                        facade.Attack(Role.GetComponent<PlayerInfo>().InstanceId, gesture.pickedObject);
                        break;
                    case CampType.Fish:
                        facade.Attack(Role.GetComponent<PlayerInfo>().InstanceId,gesture.pickedObject);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    void OnDestroy()
    {
        EasyTouch.On_TouchDown -= On_TouchDown;
    }

    public List<GameObject> OnSkillJoyMoveStart()
    {
        List<GameObject> effectList = new List<GameObject> {EffectDict["Ring"], EffectDict["Target"]};
        return effectList;
    }
}
