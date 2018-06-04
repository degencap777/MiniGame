using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    private Text localPlayerUsername;
    private Text localPlayerTotalCount;
    private Text localPlayerWinCount;

    private Text enemyPlayerUsername;
    private Text enemyPlayerTotalCount;
    private Text enemyPlayerWinCount;

    private Transform MonkeyPanel;
    private Transform FishPanel;
    private Transform startButton;
    private Transform exitButton;
    private Transform sendButton;
    private Text inputText;

    private GameObject OtherPlayerChatMsgItem;
    private GameObject LocalPlayerChatMsgItem;


    private UserData roomOwner = null;
    private UserData localPlayer = null;
    private List<UserData> playerList=new List<UserData>();
    private Room room;

    private QuitRoomRequest quitRoomRequest;
    private StartGameRequest startGameRequest;
    private RoomChatRequest roomChatRequest;

    private bool isPopPanel = false;
    private bool isRoomUpdate = false;
    private bool isAddMsg = false;

    private struct Msg
    {
        public Msg(string Data)
        {
            string[] strs = Data.Split(',');
            this.Message = strs[2];
            this.PlayerName = strs[1];
            this.IsLocalMsg = strs[0] == "1";
        }

        public string PlayerName;
        public string Message;
        public bool IsLocalMsg;
    }
    private Msg msg = new Msg();

    public Transform content;
    // Use this for initialization
    private void Awake()
    {
        //MonkeyPanel = transform.Find("MonkeyPanel");
        //FishPanel = transform.Find("FishPanel");

        //localPlayerUsername = MonkeyPanel.Find("Username").GetComponent<Text>();
        //localPlayerTotalCount = MonkeyPanel.Find("TotalCount").GetComponent<Text>();
        //localPlayerWinCount = MonkeyPanel.Find("WinCount").GetComponent<Text>();
        //enemyPlayerUsername = FishPanel.Find("Username").GetComponent<Text>();
        //enemyPlayerTotalCount = FishPanel.Find("TotalCount").GetComponent<Text>();
        //enemyPlayerWinCount = FishPanel.Find("WinCount").GetComponent<Text>();

        startButton = transform.Find("StartButton");
        exitButton = transform.Find("ExitButton");
        sendButton = transform.Find("ChatDialog/InputPanel/SendButton");
        inputText = transform.Find("ChatDialog/InputPanel/InputField/Text").GetComponent<Text>();

        startButton.GetComponent<Button>().onClick.AddListener(OnStartClick);
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitClick);
        sendButton.GetComponent<Button>().onClick.AddListener(OnSendClick);


        OtherPlayerChatMsgItem = Resources.Load<GameObject>("UIItem/OtherPlayerChatMsgItem");
        LocalPlayerChatMsgItem = Resources.Load<GameObject>("UIItem/LocalPlayerChatMsgItem");

        quitRoomRequest = GetComponent<QuitRoomRequest>();
        startGameRequest = GetComponent<StartGameRequest>();
        roomChatRequest = GetComponent<RoomChatRequest>();
    }

    void Start()
    {
        SetLocalPlayerSync();
    }

    void Update()
    {
        if (isPopPanel)
        {
            uiMng.PopPanel();
            isPopPanel = false;
        }
        if (isRoomUpdate)
        {
            //TODO 更新房间信息
            isRoomUpdate = false;
        }
        if (isAddMsg)
        {
            AddMsgItem();
            isAddMsg = false;
        }
    }
    public void SetLocalPlayerSync()
    {
        localPlayer = facade.GetUserData();
    }
    public void SetAllPlayerSync(List<UserData> udList)
    {
        this.playerList = udList;
        isRoomUpdate = true;

    }
    public void AddMsgItem()
    {
        GameObject MsgItem = Instantiate(msg.IsLocalMsg?LocalPlayerChatMsgItem:OtherPlayerChatMsgItem);
        MsgItem.transform.SetParent(content);
        MsgItem.transform.Find("PlayerNameText").GetComponent<Text>().text = msg.PlayerName;
        MsgItem.transform.Find("ChatMsgBg/ChatMsgText").GetComponent<Text>().text = msg.Message;
    }

    public void AddMsgItemSync(string msgData)
    {
        msg = new Msg(msgData);
        isAddMsg = true;
    }
    //public void SetLocalPlayerRes(string username, string totalCount, string winCount)
    //{
    //    localPlayerUsername.text = username;
    //    localPlayerTotalCount.text = "总场数:" + totalCount;
    //    localPlayerWinCount.text = "胜利:" + winCount;
    //}
    //private void SetEnemyPlayerRes(string username, string totalCount, string winCount)
    //{
    //    enemyPlayerUsername.text = username;
    //    enemyPlayerTotalCount.text = "总场数:" + totalCount;
    //    enemyPlayerWinCount.text = "胜利:" + winCount;
    //}
    //public void ClearEnemyPlayerRes()
    //{
    //    enemyPlayerUsername.text = "";
    //    enemyPlayerTotalCount.text = "等待玩家加入...";
    //    enemyPlayerWinCount.text = "";
    //}
    private void OnStartClick()
    {
        startGameRequest.SendRequest();
    }

    private void OnExitClick()
    {
        quitRoomRequest.SendRequest();
    }

    private void OnSendClick()
    {
        string msgName = localPlayer.Username;
        string msg = inputText.text;
        roomChatRequest.SendRequest(msgName+","+msg);
    }

    public void OnExitResponse()
    {
        isPopPanel = true;
    }

    public void OnStartResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.NotFind)
        {
            uiMng.ShowMessage("您不是房主，无法开始游戏！");
        }
        //else if (returnCode == ReturnCode.Fail)
        //{
        //    uiMng.ShowMessage("人数不够，无法开始游戏! ");
        //}
        else
        {
            uiMng.PushPanelSync(UIPanelType.LoadGame);
        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
        EnterAnim();
    }

    public override void OnResume()
    {
        base.OnResume();
        EnterAnim();
    }

    public override void OnPause()
    {
        base.OnPause();
        ExitAnim();
    }

    public override void OnExit()
    {
        base.OnExit();
        ExitAnim();
    }
    private void EnterAnim()
    {
        
    }

    private void ExitAnim()
    {
        gameObject.SetActive(false);
    }
}
