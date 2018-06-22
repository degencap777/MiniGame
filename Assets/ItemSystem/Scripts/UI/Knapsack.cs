using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class Knapsack : Inventory
{

    public GameObject Player;
    private Transform copy;
    private float rotSpeed = 30;
    private GameObject _menu;
    private Button _tipButton;
    private Button _useButton;
    private GameObject PickedImage { get { return pickedSlot.transform.Find("PickedImage").gameObject; } }
    private LayerMask mask = 1 << 18;
    private ItemManager itemManager;
    private GamePanel gamePanel;
    
    public override void Start()
    {
        base.Start();
        _menu = transform.Find("Menu").gameObject;
        _tipButton = _menu.transform.Find("TipButton").GetComponent<Button>();
        _useButton = _menu.transform.Find("UseButton").GetComponent<Button>();
        _useButton.onClick.AddListener(OnUseClick);
        _tipButton = _menu.transform.Find("TipButton").GetComponent<Button>();
        _tipButton.onClick.AddListener(OnTipClick);
        itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
        gamePanel = transform.parent.GetComponent<GamePanel>();
        HideMenu();
    }

    void Update()
    {
        Gesture current = EasyTouch.current;
        if (copy != null)
        {
            copy.Rotate(Vector3.up, Time.deltaTime * rotSpeed);
            if (current!=null&&EasyTouch.current.type== EasyTouch.EvtType.On_TouchStart)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 200, mask.value))
                {
                    Vector3 hitPoint = hitInfo.point;
                    SendUseItem(pickedItem.GetType().Name,RoundV3(hitPoint));
                    Debug.Log(RoundV3(hitPoint));
                    //Destroy(copy.gameObject);
                    //UseItem();
                }
            }
        }
    }
    //Slot来调用
    public void OnSlotClick(Slot slot)
    {
        pickedSlot = slot;
        ShowMenu();
    }
    private void OnTipClick()
    {
        HideMenu();
        ShowToolTip(pickedItem.GetDescription());
        TipItem();
    }
    private void OnUseClick()
    {
        CloseAllSlot();
        HideMenu();
        float trigger;
        pickedItem.parameters.TryGetValue("Trigger", out trigger);
        if (trigger == 0)
        {
            Debug.Log("use Skill");
            SendUseItem(pickedItem.GetType().Name);
            UseItem();
        }
        else
        {
            PickedImage.SetActive(true);
            if (Player != null)
            {
                GameObject resource = Instantiate(pickedItem.Prefab);
                copy = resource.transform.Find("PickedItem");
                copy.parent = Player.transform;
                copy.position = Player.transform.position + new Vector3(0, 1, 0);
                copy.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                copy.gameObject.SetActive(true);
                Destroy(resource);
            }
            //TODO 设置item虚影的位置
        }
    }

    private void TipItem()
    {
        pickedSlot.OnTipClick();
        pickedSlot = null;
    }
    private void UseItem()
    {
        if(copy!=null)
            Destroy(copy.gameObject);
        OpenOtherSlots();
        pickedSlot.UseItem();
        pickedSlot = null;

        Debug.Log("Use Skill After Touch");
    }

    private void StopUseItem()
    {
        if (copy != null)
            Destroy(copy.gameObject);
        OpenOtherSlots();
        pickedSlot.StopUseItem();
        pickedSlot = null;
    }

    private void CloseAllSlot()
    {
        foreach (var slot in slotlList)
        {
            slot.Button.interactable = false;
        }
    }
    private void OpenOtherSlots()
    {
        foreach (var slot in slotlList)
        {
            if(slot!=pickedSlot)
                slot.SetButtonActive();
        }
    }

    private void ShowMenu()
    {
        _menu.SetActive(true);
        _menu.transform.position = new Vector3(pickedSlot.transform.position.x, _menu.transform.position.y, _menu.transform.position.z);
    }
    private void HideMenu()
    {
        _menu.SetActive(false);
    }

    public bool AddItem(Item item)
    {
        foreach (Slot slot in slotlList)
        {
            if (slot.Amount > 0)
            {
                if (slot.Item == item)
                {
                    float capacity = item.parameters.TryGet("Capacity");
                    if (capacity > slot.Amount)
                    {
                        slot.AddItem(item);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void SendUseItem(string name, string point=null)
    {
        gamePanel.UseItem(name,point);
    }
    public void UseItemSync(bool isUse)
    {
        Debug.Log("UIUSEITEM");
        if(isUse)
            UseItem();
        else
        {
            StopUseItem();
        }
    }
    private string RoundV3(Vector3 v3)
    {
        return Mathf.Round(v3.x) + "," + Mathf.Round(v3.y) + "," + Mathf.Round(v3.z);
    }
}
