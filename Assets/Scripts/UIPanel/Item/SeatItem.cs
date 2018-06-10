using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeatItem : MonoBehaviour
{


    private RoomPanel roomPanel;

    public int id=-1;
    public int index = 0;
    public bool isChangeText = false;
    public bool isClear = false;
    private Image image;
    public Color color;
    private Color defaultColor;

    private Text text;
	// Use this for initialization
	void Awake()
	{
	    roomPanel = transform.parent.parent.GetComponent<RoomPanel>();
        transform.Find("ChangeSeatButton").GetComponent<Button>().onClick.AddListener(OnChangeSeatButtonClick);
	    text = transform.Find("ChangeSeatButton/Text").GetComponent<Text>();

        image = transform.Find("Image").GetComponent<Image>();
	    defaultColor = image.color;
        color= image.color;
    }

    void Update()
    {
        if (isChangeText)
        {
            text.text = "交换";
            image.color=Color.red;
            isChangeText = false;
        }
        if (isClear)
        {
            Clear();
            isClear = false;
        }
    }
    public void SetSeatItem(int id)
    {
        this.id = id;
        isChangeText = true;
    }

    private void OnChangeSeatButtonClick()
    {
        roomPanel.OnChangeSeatButtonClick(id,index);
    }

    public void ClearAsync()
    {
        id = -1;
        isClear = true;
    }
    public void Clear()
    {
        id = -1;
        image.color = defaultColor;
        text.text = "加入";
    }
}
