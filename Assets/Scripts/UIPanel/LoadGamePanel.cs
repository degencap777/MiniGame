using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGamePanel : BasePanel
{

    private Slider LoadingProgress;
    private Text percentageText;
    private LoadGameRequest loadGameRequest;
    private int dataLoadPercentage = 0;//最大值为10
    void Awake()
    {
        LoadingProgress = transform.Find("LoadingProgress").GetComponent<Slider>();
        loadGameRequest = transform.GetComponent<LoadGameRequest>();
        percentageText = transform.Find("PercentageText").GetComponent<Text>();
    }
    // Use this for initialization
    void Start ()
    {
        percentageText.text = "";
        LoadGame();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void LoadGame()
    {
        StartCoroutine(StartLoading("Game"));
        loadGameRequest.SendRequest();
    }

    private void SetLoadingPercentage(int percentage)
    {
        LoadingProgress.value = percentage;
        percentageText.text = "载入中："+percentage + "%";
    }
    private IEnumerator StartLoading(string scene)
    {
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(scene);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }
        toProgress = 90;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            SetLoadingPercentage(displayProgress);
            yield return new WaitForEndOfFrame();
        }
        while (true)
        {
            toProgress = 90 + dataLoadPercentage;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
            if (displayProgress == 100) break;
        }

        op.allowSceneActivation = true;
        StartGame();
    }

    //此回调表示加载完毕
    public void OnLoadGameResponse()
    {
        dataLoadPercentage = 10;
    }

    private void StartGame()
    {
        uiMng.PushPanelSync(UIPanelType.Game);
        facade.UpdateSceneAsync();
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
    }
}
