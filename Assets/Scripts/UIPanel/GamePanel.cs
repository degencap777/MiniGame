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
    private ShowTimerRequest showTimerRequest;
    private GameOverRequest gameOverRequest;
    private StartPlayRequest startPlayRequest;
    private QuitBattleRequest quitBattleRequest;

    public Dictionary<string, GameObject> EffectDict { get; private set; }
    private Knapsack knapsack;
    private Text timer;
    private bool isShowTimer = false;
    private string time = null;

    private Color gameOver = new Color(43 / 255, 43 / 255, 43 / 255);
    private Color win=new Color(1,216/255,0);
    private Color lose=new Color(43 / 255, 43 / 255, 43 / 255);

    private Text gameOverText;
    private ReturnCode returnCode = ReturnCode.NotFind;
    private Result result;

    private float gameOverTimer = 0;
    private Button closeButton;

    private SkillJoystickItem skillJoystickItem;
    private SkillItem skillItem;
    private Transform roleSelectPanel;

    public CampType CampType { get; private set; }
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
    
    private int deadCount = 0;

    private Dictionary<int,GameObject> headDict=new Dictionary<int, GameObject>();

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
        knapsack = transform.Find("KnapsackPanel").GetComponent<Knapsack>();

        skillJoystickItem = transform.Find("SkillJoystickItem").GetComponent<SkillJoystickItem>();
        skillItem = transform.Find("SkillItem").GetComponent<SkillItem>();
        roleSelectPanel = transform.Find("RoleSelectPanel");

        timer = transform.Find("TimerPanel/Time").GetComponent<Text>();
        gameOverText = transform.Find("GameOverPanel/GameOver").GetComponent<Text>();
        closeButton=transform.Find("GameOverPanel/CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseClick);

        EffectDict = facade.GetEffectDict();
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
        if (deadCount == 2)
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

    public void GameOverAsync(ReturnCode returnCode,Result result=null)
    {
        this.returnCode = returnCode;
        this.result = result;
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

    public void UseSkillSync(float coldTime)
    {
        skillJoystickItem.UseSkillSync(coldTime);
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
        skillItem.gameObject.GetComponent<CanvasGroup>().alpha = alpha;
        skillItem.gameObject.GetComponent<CanvasGroup>().interactable = active;
        skillJoystickItem.gameObject.GetComponent<CanvasGroup>().alpha = alpha;
        skillJoystickItem.GetComponent<ETCJoystick>().activated = active;
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
    public void SetPlayer(GameObject player)
    {
        if (player==null||player.GetComponent<PlayerInfo>().Player.IsLocal == false) return;
        PlayerInfo pi = player.GetComponent<PlayerInfo>();
        if (headDict.TryGet(pi.InstanceId) == null)
        {
            AddHeadImage(player);
        }
        if (pi.RoleType != RoleType.Hero)
        {
            SetInteractive(false);
            return;
        }
        SetInteractive(true);

        switch (CampType)
        {
            case CampType.Monkey:
                switch (pi.Name)
                {
                    case "Monkey0":
                        skillItem.SkillName = "SpeedUp";
                        skillJoystickItem.SkillName = "Summon";
                        break;
                    case "Monkey1":
                        break;
                    case "Monkey2":
                        break;
                }
                break;
            case CampType.Fish:
                skillItem.SkillName = "SpeedUp";
                skillJoystickItem.SkillName = "Blink";
                break;
            case CampType.Middle:
                break;
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

    public override void OnExit()
    {
        base.OnExit();
    }
}
