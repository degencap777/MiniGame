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


    private UserData roomOwner = null;
    private UserData localPlayer = null;
    private List<UserData> playerList=new List<UserData>();
    private Room room;

    private QuitRoomRequest quitRoomRequest;
    private StartGameRequest startGameRequest;
    private bool isPopPanel = false;
    private bool isRoomUpdate = false;

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

        startButton.GetComponent<Button>().onClick.AddListener(OnStartClick);
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitClick);


        quitRoomRequest = GetComponent<QuitRoomRequest>();
        startGameRequest = GetComponent<StartGameRequest>();
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

    public void OnExitResponse()
    {
        isPopPanel = true;
    }

    public void OnStartResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Fail)
        {
            uiMng.ShowMessage("您不是房主，无法开始游戏！");
        }
        else
        {
            uiMng.PushPanelSync(UIPanelType.Game);
            facade.EnterPlayingSync();
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
        //MonkeyPanel.localPosition = new Vector3(-1000, 51, 0);
        //MonkeyPanel.DOLocalMoveX(-148.7f, 0.4f);
        //FishPanel.localPosition = new Vector3(1000, 51, 0);
        //FishPanel.DOLocalMoveX(142.2f, 0.4f);
        startButton.localScale = Vector3.zero;
        startButton.DOScale(1, 0.4f);
        exitButton.localScale = Vector3.zero;
        exitButton.DOScale(1, 0.4f);
    }

    private void ExitAnim()
    {
        //MonkeyPanel.DOLocalMoveX(-1000, 0.4f);
        //FishPanel.DOLocalMoveX(1000, 0.4f);
        startButton.DOScale(0, 0.4f);
        exitButton.DOScale(0, 0.4f).OnComplete(() => gameObject.SetActive(false));
    }
}
