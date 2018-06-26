using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class DamageRequest : BaseRequest
{
    public PlayerManager PlayerManager;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Damage;
        base.Awake();
    }

    class Damage
    {
        public int InstanceId;
        public DamageType DamageType;
        public string Data;

        public Damage(int instanceId, DamageType damageType,string data)
        {
            InstanceId = instanceId;
            Data = data;
            DamageType = damageType;
        }
    }
    readonly Queue<Damage> damageQueue = new Queue<Damage>();
    // Update is called once per frame
    void Update()
    {
        if (damageQueue.Count != 0)
        {
            int currentCount = damageQueue.Count;
            for (int i = 0; i < currentCount; i++)
            {
                var damage = damageQueue.Dequeue();
                PlayerManager.DamageSync(damage.InstanceId,damage.DamageType,damage.Data);
            }
        }
    }
    public void SendRequest(int instanceId, string data)
    {
        base.SendRequest(instanceId+"|"+data);
    }
    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        string[] strs = data.Split('|');
        int instanceId = int.Parse(strs[0]);
        DamageType damageType = (DamageType) int.Parse(strs[1]);
        string s = strs[2];
        Damage damage = new Damage(instanceId, damageType, s);
        damageQueue.Enqueue(damage);
    }
}
