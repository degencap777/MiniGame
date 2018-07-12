using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tools;
using Common;
using UnityEngine;

public class AttackRequest : BaseRequest
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
        public int Layer;
        public int InstanceId;
        public int TargetId;
        public Vector2 TargetIndexV2;

        public Damage(int layer, int instanceId, int targetId)
        {
            Layer = layer;
            InstanceId = instanceId;
            TargetId = targetId;
        }
        public Damage(int layer, int instanceId,Vector2 targetIndexV2)
        {
            Layer = layer;
            InstanceId = instanceId;
            TargetIndexV2 = targetIndexV2;
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
                if (damage.Layer == 9)
                {
                    PlayerManager.AttackSync(damage.InstanceId, damage.TargetIndexV2);
                }
                else if(damage.Layer==10)
                {
                    PlayerManager.AttackSync(damage.InstanceId, damage.TargetId);
                }
                else
                {
                    PlayerManager.AttackSync(damage.InstanceId,damage.TargetId,false);
                }
            }
        }
    }
    public void SendRequest(int instanceId, string data)
    {
        base.SendRequest(instanceId + "|" + data);
    }
    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        Debug.Log(data);
        string[] strs = data.Split('|');
        int layer= int.Parse(strs[0]);
        int instanceId = int.Parse(strs[1]);
        if (layer == 9)
        {
            damageQueue.Enqueue(new Damage(layer,instanceId,UnityTools.ParseVector2(strs[2])));
        }
        else if (layer == 10)
        {
            damageQueue.Enqueue(new Damage(layer, instanceId, int.Parse(strs[2])));
        }
        else
        {
            damageQueue.Enqueue(new Damage(layer, instanceId, int.Parse(strs[2])));
        }
    }
}
