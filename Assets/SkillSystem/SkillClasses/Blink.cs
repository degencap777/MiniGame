using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : Skill
{
    //timeSinceSkillStart     技能从发动以来经过的时间
    //owner                   技能发动者
    //name                    技能名(不是类名，是游戏中想要展示的名字)
    //parameters              可调技能参数的字典
//resources               资源预置体


    private GameObject go;
    //您可以通过该方法提供一个技能的详细描述，您可以通过在文字中嵌入属性字典中的值来避免反复修改代码。
    public override string GetDescription()
    {
        return "skill " + name + " has no specific description.";
    }

    protected override bool SkillAction()
    {
        return false;
    }

    protected override void SkillEnd()
    {
        
    }

    protected override bool SkillStart()
    {
        Vector3 dir = Direction * parameters.TryGet("Distance");
        go = resources;
        go.transform.position = owner.transform.position;
        go.transform.eulerAngles = Quaternion.LookRotation(dir).eulerAngles;
        Rigidbody rb= owner.GetComponent<Rigidbody>();
        Debug.Log(rb.position+" "+dir);
        rb.MovePosition(rb.position+ dir);
        go.AddComponent<DestroyForTime>().time = parameters.TryGet("During");
        return false;
    }
}