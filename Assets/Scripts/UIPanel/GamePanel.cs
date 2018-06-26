using System;
using System.Collections;
using System.Collections.Generic;
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

    public CampType CampType = CampType.Middle;

    private int deadCount = 0;
    
    
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

        timer = transform.Find("TimerPanel/Time").GetComponent<Text>();
        gameOverText = transform.Find("GameOverPanel/GameOver").GetComponent<Text>();
        closeButton=transform.Find("GameOverPanel/CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseClick);
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

    public void SetPlayer(GameObject player)
    {
        knapsack.Player = player;
        if(player!=null)
            knapsack.gameObject.SetActive(true);
    }

    public void Die(Player player)
    {
        if (player.IsLocal)
        {
            skillJoystickItem.gameObject.SetActive(false);
            skillItem.gameObject.SetActive(false);
            SetPlayer(null);
        }
        if (player.CampType == CampType.Fish)
        {
            deadCount++;
        }
    }

    public void Revive(Player player,GameObject go)
    {
        if (player.IsLocal)
        {
            skillJoystickItem.gameObject.SetActive(true);
            skillItem.gameObject.SetActive(true);
            SetPlayer(go);
        }
        if (player.CampType == CampType.Fish)
        {
            deadCount--;
        }
    }
    public override void OnExit()
    {
        base.OnExit();
    }
}
