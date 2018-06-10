using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSeatItem : MonoBehaviour
{

    private RoomPanel roomPanel;
    
	// Use this for initialization
	void Awake ()
	{
	    roomPanel = FindObjectOfType(typeof(RoomPanel)) as RoomPanel;
        transform.Find("ChatMsgBg/ConfirmButton").GetComponent<Button>().onClick.AddListener(delegate ()
	    {
	        int id = roomPanel.FindIdByName(transform.Find("PlayerNameText").GetComponent<Text>().text);
            
	        if (id == -1)
	        {
	            return;
	        }
            roomPanel.OnChangeSeatConfirmClick("Y" + "," + id);
            DestroyImmediate(gameObject);
	    });
	    transform.Find("ChatMsgBg/RefuseButton").GetComponent<Button>().onClick.AddListener(delegate ()
	    {
	        int id = roomPanel.FindIdByName(transform.Find("PlayerNameText").GetComponent<Text>().text);
	        if (id == -1)
	        {
	            return;
	        }
	        roomPanel.OnChangeSeatConfirmClick("Y" + "," + id);
            DestroyImmediate(gameObject);
        });
    }
	
    public void SetPanel(RoomPanel roomPanel)
    {
        this.roomPanel = roomPanel;
    }
}
