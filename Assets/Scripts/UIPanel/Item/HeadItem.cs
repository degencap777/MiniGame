using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadItem : MonoBehaviour
{
    private int instanceId;
    private GameObject role; 

    private GamePanel gamePanel;
	// Use this for initialization
	void Start () {
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

    void Update()
    {
        if (role == null)
        {
            gamePanel.RemoveHeadItem(instanceId);
            Destroy(gameObject);
        }
    }
    void OnClick()
    {
        GameFacade.Instance.SetCurrentRole(role);
        gamePanel.SetPlayer(role);
    }

    public HeadItem SetGamePanel(GamePanel gamePanel,GameObject go)
    {
        this.role = go;
        this.gamePanel = gamePanel;
        this.instanceId = go.GetComponent<PlayerInfo>().InstanceId;
        return this;
    }
}
