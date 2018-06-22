using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<Item> itemList;

    private Canvas canvas;
    protected Slot[] slotlList;

    private float targetAlpha = 1;

    private float smoothing = 4;

    private CanvasGroup canvasGroup;
    
    #region ToolTip
    private ToolTip toolTip;
    private bool isToolTipShow = false;
    private Vector2 toolTipPositionOffset = new Vector2(10, -10);
    #endregion
    #region PickedItem

    private bool _isPickedItem = false;

    public bool IsPickedItem
    {
        get { return _isPickedItem; }
        set { _isPickedItem = value; }
    }

    protected Item pickedItem { get { return pickedSlot.Item; } }
    protected Slot pickedSlot;

    #endregion
    // Use this for initialization
    public virtual void Start ()
	{
	    canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        slotlList = GetComponentsInChildren<Slot>();
	    canvasGroup = GetComponent<CanvasGroup>();
	    toolTip = transform.Find("ToolTip").GetComponent<ToolTip>();
	    //_pickedItem.Hide();
    }

    public void ShowToolTip(string content)
    {
        if (_isPickedItem) return;
        toolTip.Show(content);
        isToolTipShow = true;

    }
    public void HideToolTip()
    {
        toolTip.Hide();
        isToolTipShow = false;
    }
}
