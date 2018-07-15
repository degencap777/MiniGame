using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour {

    CharactorItems cs;

    public Slot Slot1;
    public Slot Slot2;

    public ItemManager ItemManager;
	// Use this for initialization
	void Start () {
        cs = GetComponent<CharactorItems>();
	    ItemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
	    Slot1 = GameFacade.Instance.GetCurrentPanel().transform.Find("KnapsackPanel/SlotPanel/Slot").GetComponent<Slot>();
	    Slot2 = GameFacade.Instance.GetCurrentPanel().transform.Find("KnapsackPanel/SlotPanel/Slot (1)").GetComponent<Slot>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        Item item = cs.Items["SpeedUp2"];
            //cs.UseItem<SpeedUp>();

	        Slot1.AddItem(item);
        }
	    if (Input.GetKeyDown(KeyCode.Q))
	    {
	        Item item = cs.Items["Transparent"];
	        //cs.UseItem<SpeedUp>();

	        Slot2.AddItem(item);
	    }
	    if (Input.GetKeyDown(KeyCode.E))
	    {
	        Item item = cs.Items["TrueVision"];
	        //cs.UseItem<SpeedUp>();

	        Slot2.AddItem(item);
	    }
	    if (Input.GetKeyDown(KeyCode.R))
	    {
	        Item item = cs.Items["Phase"];
	        //cs.UseItem<SpeedUp>();

	        Slot2.AddItem(item);
	    }
    }
}
