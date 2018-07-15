using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class SpeedUp : Skill
{
    //timeSinceSkillStart     技能从发动以来经过的时间
    //owner                   技能发动者
    //name                    技能名(不是类名，是游戏中想要展示的名字)
    //parameters              可调技能参数的字典
    //resources               资源预置体

    GameObject go;
    private PlayerInfo pi;
    //这个方法会在技能发动时调用
    protected override bool SkillStart()
    {
        go = resources;
        go.transform.parent = owner.transform;
        go.transform.localPosition=Vector3.zero;
        pi = owner.GetComponent<PlayerInfo>();
        pi.MoveSpeed += 3;
        pi.TurnSpeed += 3;
        pi.anim.speed = 1.5f;

        if (pi.VisualTest != null)
        {
            if (pi.VisualTest.InVisual())
                GameFacade.Instance.PlaySound("SpeedUp");
        }
        else
        {
            GameFacade.Instance.PlaySound("SpeedUp");
        }
        return true;
    }

    //这个方法会在技能进行过程中不断调用，当返回false表示技能已经完成所有动作
    protected override bool SkillAction()
    {
        if (timeSinceSkillStart < parameters.TryGet("During"))
        {
            
            return false;
        }
        return true;
    }

    //这个方法会在技能结束时调用
    protected override void SkillEnd()
    {
        Object.Destroy(go);
        if (pi != null)
        {
            pi.MoveSpeed -= 3;
            pi.TurnSpeed -= 3;
            pi.anim.speed = 1f;
        }
    }

    //您可以通过该方法提供一个技能的详细描述，您可以通过在文字中嵌入属性字典中的值来避免反复修改代码。
    public override string GetDescription()
    {
        return "skill " + name + " has no specific description.";
    }

}