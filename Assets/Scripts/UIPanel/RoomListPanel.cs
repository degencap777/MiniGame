using System.Collections;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    //private Transform battleRes;
    private Transform roomListTrans;
    private VerticalLayoutGroup roomLayout;
    private GameObject roomItemPrefab;
    private List<Room> roomList=null;
    private List<UserData> udList;
    private ListRoomRequest listRoomRequest;
    private CreateRoomRequest createRoomRequest;
    private JoinRoomRequest joinRoomRequest;
    void Awake()
    {
        joinRoomRequest = GetComponent<JoinRoomRequest>();
        listRoomRequest = GetComponent<ListRoomRequest>();
        createRoomRequest = GetComponent<CreateRoomRequest>();
        //battleRes = transform.Find("BattleRes").GetComponent<Transform>();
        roomListTrans = transform.Find("RoomList").GetComponent<Transform>();
        roomLayout = roomListTrans.Find("ScrollPanel/Layout").GetComponent<VerticalLayoutGroup>();
        roomItemPrefab = Resources.Load<GameObject>("UIItem/Room/RoomItem");
        transform.Find("RoomList/CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
        transform.Find("RoomList/CreateRoomButton").GetComponent<Button>().onClick.AddListener(OnCreateRoomClick);
        transform.Find("RoomList/RefreshRoomButton").GetComponent<Button>().onClick.AddListener(OnRefreshRoomClick);
        //Username = transform.Find("BattleRes/Username").GetComponent<Text>();
    }

    void Update()
    {
        if (roomList != null)
        {
            LoadRoomItem(roomList);
            roomList = null;
        }
        if (udList != null)
        {
            RoomPanel panel = uiMng.PushPanel(UIPanelType.Room) as RoomPanel;
            panel.SetAllPlayerSync(udList);
            panel.SetLocalPlayerSync(facade.GetUserData());
            udList = null;
        }
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        EnterAnim();
    }

    private void OnCloseClick()
    {
        PlayClickSound();
        uiMng.PopPanel();
    }

    private void OnCreateRoomClick()
    {
        PlayClickSound();
        BasePanel panel = uiMng.PushPanel(UIPanelType.Room);
        createRoomRequest.SetPanel(panel);
        createRoomRequest.SendRequest();
    }

    private void OnRefreshRoomClick()
    {
        listRoomRequest.SendRequest();
    }

    public override void OnResume()
    {
        base.OnResume();
        listRoomRequest.SendRequest();
        EnterAnim();
    }

    public override void OnPause()
    {
        base.OnPause();
        HideAnim();
    }

    public override void OnExit()
    {
        base.OnExit();
        HideAnim();
    }
    private void EnterAnim()
    {
        
    }

    private void HideAnim()
    {
        gameObject.SetActive(false);
        //roomListTrans.DOLocalMove(new Vector3(1000, 0, 0), 0.2f).OnComplete(() => gameObject.SetActive(false));
    }

    //private void SetBattleRes()
    //{
    //    UserData ud = facade.GetUserData();
    //    battleRes.Find("Username").GetComponent<Text>().text = ud.Username;
    //    battleRes.Find("TotalCount").GetComponent<Text>().text = "总场数：" + ud.TotalCount;
    //    battleRes.Find("WinCount").GetComponent<Text>().text = "胜利：" + ud.WinCount;
    //}

    public void LoadRoomItemSync(List<Room> roomList)
    {
        this.roomList = roomList;
    }
    private void LoadRoomItem(List<Room> roomList)
    {
        RoomItem[] riArray = roomLayout.GetComponentsInChildren<RoomItem>();
        foreach (RoomItem ri in riArray)
        {
            ri.DestroySelf();
        }
        int count = roomList.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject roomItem = Instantiate(roomItemPrefab);
            roomItem.transform.SetParent(roomLayout.transform);
            Room room = roomList[i];
            roomItem.GetComponent<RoomItem>().SetRoomInfo(room, this);
        }
        //int roomCount = GetComponentsInChildren<RoomItem>().Length;
        //Vector2 size = roomLayout.GetComponent<RectTransform>().sizeDelta;
        //roomLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, roomCount * roomItemPrefab.GetComponent<RectTransform>().sizeDelta.y + roomLayout.spacing * Mathf.Max(0, roomCount - 1));
    }

    public void OnJoinClick(int id)
    {
        joinRoomRequest.SendRequest(id);
    }

    public void OnJoinResponse(ReturnCode returnCode, List<UserData> udList)
    {
        switch (returnCode)
        {
            case ReturnCode.NotFind:
                uiMng.ShowMessageSync("房间被销毁无法加入");
                break;
            case ReturnCode.Fail:
                uiMng.ShowMessageSync("房间已满无法加入");
                break;
            case ReturnCode.Success:

                this.udList = udList;
                //udList不为空，异步加载房间
                break;
        }
    }
    
}
