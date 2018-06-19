using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class UseSkillRequest : BaseRequest
{

    public PlayerManager PlayerManager;

    class skillData
    {
        public int instanceId;
        public string skillName;
        public string axis;

        public skillData(int instanceId, string skillName, string axis=null)
        {
            this.instanceId = instanceId;
            this.skillName = skillName;
            this.axis = axis;
        }
    }

    readonly Queue<skillData> _skillQueue=new Queue<skillData>();
    enum SkillType
    {
        Button,
        Joystick,
    }
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Skill;
        base.Awake();
    }

    void FixedUpdate()
    {
        if (_skillQueue.Count != 0)
        {
            int currentCount = _skillQueue.Count;
            for (int i = 0; i < currentCount; i++)
            {
                var skill = _skillQueue.Dequeue();
                PlayerManager.UseSkillSync(skill.instanceId,skill.skillName,skill.axis);
            }
        }
    }
    public void SendRequest(int instanceId,string skillName, string axis = null)
    {
        if (axis == null)
            base.SendRequest(((int)SkillType.Button) + "|" +instanceId+ "|" + skillName);
        else
            base.SendRequest(((int)SkillType.Joystick) + "|" + instanceId + "|" + skillName + "|" + axis);
    }
    public override void OnResponse(string data)
    {
        base.OnResponse(data);
        Debug.Log(data);
        string[] strs = data.Split('|');
        SkillType skillType = (SkillType) int.Parse(strs[0]);
        switch (skillType)
        {
            case SkillType.Button:
                _skillQueue.Enqueue(new skillData(int.Parse(strs[1]), strs[2]));
                break;
            case SkillType.Joystick:
                _skillQueue.Enqueue(new skillData(int.Parse(strs[1]), strs[2], strs[3]));
                break;
        }
    }
}
