using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    private Transform MonkeyPanel;
    private Transform FishPanel;
    private Transform startButton;
    private Transform exitButton;
    private Transform sendButton;
    private InputField inputField;

    private GameObject OtherPlayerChatMsgItem;
    private GameObject LocalPlayerChatMsgItem;
    private GameObject ChangeSeatItem;

    private UserData roomOwner = null;
    private UserData localPlayer = null;
    public List<UserData> playerList=new List<UserData>();
    private Room room;
    private SeatItem[] FishSeats;
    private SeatItem[] MonkeySeats;

    private QuitRoomRequest quitRoomRequest;
    private StartGameRequest startGameRequest;
    private RoomChatRequest roomChatRequest;
    private ChangeSeatRequest changeSeatRequest;

    //异步控制
    private bool isPopPanel = false;
    private bool isRoomUpdate = false;
    private bool isAddMsg = false;
    private int confirmId = -1;

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

        FishSeats=new SeatItem[GameFacade.FISH_NUM];
        MonkeySeats=new SeatItem[GameFacade.MONKEY_NUM];

        startButton = transform.Find("StartButton");
        exitButton = transform.Find("ExitButton");
        sendButton = transform.Find("InputPanel/SendButton");
        inputField= transform.Find("InputPanel/InputField").GetComponent<InputField>();

        startButton.GetComponent<Button>().onClick.AddListener(OnStartClick);
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitClick);
        sendButton.GetComponent<Button>().onClick.AddListener(OnSendClick);


        OtherPlayerChatMsgItem = Resources.Load<GameObject>("UIItem/OtherPlayerChatMsgItem");
        LocalPlayerChatMsgItem = Resources.Load<GameObject>("UIItem/LocalPlayerChatMsgItem");
        ChangeSeatItem = Resources.Load<GameObject>("UIItem/ChangeSeatItem");
        

        quitRoomRequest = GetComponent<QuitRoomRequest>();
        startGameRequest = GetComponent<StartGameRequest>();
        roomChatRequest = GetComponent<RoomChatRequest>();
        changeSeatRequest = GetComponent<ChangeSeatRequest>();

        FishSeats = transform.Find("FishPlayerPanel").GetComponentsInChildren<SeatItem>();
        MonkeySeats = transform.Find("MonkeyPlayerPanel").GetComponentsInChildren<SeatItem>();
        for (int i = 0; i < GameFacade.FISH_NUM ; i++)
        {
            FishSeats[i].index = i;
        }
        for (int i = 0; i < GameFacade.MONKEY_NUM; i++)
        {
            MonkeySeats[i].index = i+ GameFacade.FISH_NUM;
        }
    }

    void Start()
    {
        
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
            SetAllPlayer();
            isRoomUpdate = false;
        }
        if (isAddMsg)
        {
            AddMsgItem();
            isAddMsg = false;
        }
        if (confirmId != -1)
        {
            AskForConfirm(confirmId);
            confirmId = -1;
        }
    }

    #region Find_Function
    public int FindIdByName(string name)
    {
        foreach (var userData in playerList)
        {
            if (userData.Username == name)
                return userData.Id;
        }
        return -1;
    }

    public SeatItem FindSeatById(int id)
    {
        foreach (var seat in FishSeats)
        {
            if (seat.id == id)
            {
                return seat;
            }
        }
        foreach (var seat in MonkeySeats)
        {
            if (seat.id == id)
                return seat;
        }
        Debug.Log("找不到id:" + id + "对应的seat");
        return null;
    }
    #endregion


    public void SetRoomOwnerSync(UserData ud)
    {
        localPlayer = ud;
        FishSeats[ud.SeatIndex].SetSeatItem(localPlayer.Id);
    }
    public void SetAllPlayerSync(List<UserData> udList)
    {
        this.playerList = udList;
        isRoomUpdate = true;
    }

    public void SetAllPlayer()
    {
        foreach (var seat in FishSeats)
        {
                seat.Clear();
        }
        foreach (var seat in MonkeySeats)
        {
                seat.Clear();
        }
        foreach (var ud in playerList)
        {
            switch (ud.CampType)
            {
                case CampType.Fish:
                    FishSeats[ud.SeatIndex].SetSeatItem(ud.Id);
                    break;
                case CampType.Monkey:
                    MonkeySeats[ud.SeatIndex - GameFacade.FISH_NUM].SetSeatItem(ud.Id);
                    break;
            }
        }
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
        string msg = inputField.text;
        roomChatRequest.SendRequest(msgName+","+msg);
        inputField.text = "";
    }

    public void OnExitResponse()
    {
        foreach (var seat in FishSeats)
        {
            seat.ClearAsync();
        }
        foreach (var seat in MonkeySeats)
        {
            seat.ClearAsync();
        }
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

    public void OnChangeSeatConfirmClick(string data)
    {
        changeSeatRequest.SendRequest(data);
    }
    public void OnChangeSeatButtonClick(int id,int index)
    {
        changeSeatRequest.SendRequest(id+","+index);
    }
    public void OnChangeSeatResponse(int id1,int id2)
    {
        SwapSeat(FindSeatById(id1),FindSeatById(id2));
    }


    public void JoinSeat(int id,int index)
    {
        SeatItem seat;
        SeatItem currentSeat = FindSeatById(id);
        if (index < GameFacade.FISH_NUM)
        {
            seat = FishSeats[index];
        }
        else
        {
            seat = MonkeySeats[index];
        }
        SwapSeat(currentSeat,seat);
        seat.isChangeText = true;
        currentSeat.ClearAsync();
    }

    private void SwapSeat(SeatItem seat1,SeatItem seat2)
    {
        int id = seat1.id;
        Color sprite = seat1.color;
        seat1.id = seat2.id;
        seat1.color = seat2.color;
        seat2.id = id;
        seat2.color = sprite;
    }

    public void AskForConfirmAsync(int id)
    {
        confirmId = id;
    }
    public void AskForConfirm(int id)
    {
        string name="";
        foreach (var userData in playerList)
        {
            if (userData.Id == id)
                name = userData.Username;
        }
        if (name == "")
            return;
        GameObject ConfirmButton = Instantiate(ChangeSeatItem);
        
        ConfirmButton.transform.SetParent(content);
        ConfirmButton.transform.Find("PlayerNameText").GetComponent<Text>().text = name;
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
        gameObject.SetActive(false);
    }

    public override void OnExit()
    {
        base.OnExit();
        uiMng.DestroyPanel(UIPanelType.Room);
    }
    private void EnterAnim()
    {
        
    }
    
    
}
