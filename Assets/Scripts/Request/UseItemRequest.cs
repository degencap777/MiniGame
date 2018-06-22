using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class UseItemRequest : BaseRequest {

    public PlayerManager PlayerManager;

    class ItemData
    {
        public int Id;
        public int instanceId;
        public string ItemName;
        public string Point;

        public ItemData(int id,int instanceId, string itemName, string point = null)
        {
            this.Id = id;
            this.instanceId = instanceId;
            this.ItemName = itemName;
            this.Point = point;
        }
    }

    readonly Queue<ItemData> _itemQueue = new Queue<ItemData>();
    enum ItemType
    {
        Trigger,
        UnTrigger
    }
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Item;
        base.Awake();
    }

    void FixedUpdate()
    {
        if (_itemQueue.Count != 0)
        {
            int currentCount = _itemQueue.Count;
            for (int i = 0; i < currentCount; i++)
            {
                var item = _itemQueue.Dequeue();
                PlayerManager.UseItemSync(item.Id,item.instanceId, item.ItemName, item.Point);
            }
        }
    }
    public void SendRequest(int id,int instanceId, string itemName, string point = null)
    {
        if (point == null)
            base.SendRequest(((int)ItemType.Trigger) + "|" +id+"|"+ instanceId + "|" + itemName);
        else
            base.SendRequest(((int)ItemType.UnTrigger) + "|" + id + "|" + instanceId + "|" + itemName + "|" + point);
    }
    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        Debug.Log(data);
        string[] strs = data.Split('|');
        ItemType itemType = (ItemType)int.Parse(strs[0]);
        switch (itemType)
        {
            case ItemType.Trigger:
                _itemQueue.Enqueue(new ItemData(int.Parse(strs[1]), int.Parse(strs[2]),strs[3]));
                break;
            case ItemType.UnTrigger:
                _itemQueue.Enqueue(new ItemData(int.Parse(strs[1]), int.Parse(strs[2]), strs[3],strs[4]));
                break;
        }
    }
}
