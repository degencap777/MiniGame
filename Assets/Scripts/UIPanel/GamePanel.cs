using System;
using System.Collections;
using System.Collections.Generic;
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
    private SkillJoystickItem skillJoystickItem;
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
    
    void Awake()
    {
        Joystick = transform.Find("Joystick").GetComponent<ETCJoystick>();
        showTimerRequest = GetComponent<ShowTimerRequest>();
        startPlayRequest = GetComponent<StartPlayRequest>();
        gameOverRequest = GetComponent<GameOverRequest>();
        quitBattleRequest = GetComponent<QuitBattleRequest>();
        knapsack = transform.Find("KnapsackPanel").GetComponent<Knapsack>();
        skillJoystickItem = transform.Find("SkillJoystickItem").GetComponent<SkillJoystickItem>();
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
	        gameOverTimer += Time.deltaTime;
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
                    default:
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
	}

    internal void ShowTimerSync(int time)
    {
        int min = time==0? 0:time / 60;
        int s = time % 60;
        this.time=min+":"+s;
        isShowTimer = true;
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
        facade.GameOver();
        SceneManager.LoadScene("Main");
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
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
