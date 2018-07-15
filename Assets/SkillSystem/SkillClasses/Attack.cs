using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Skill
{
    //timeSinceSkillStart     技能从发动以来经过的时间
    //owner                   技能发动者
    //name                    技能名(不是类名，是游戏中想要展示的名字)
    //parameters              可调技能参数的字典
//resources               资源预置体


    private PlayerInfo pi;
    //这个方法会在技能发动时调用,返回值为false则发动失败，不会调用后续方法
    protected override bool SkillStart()
    {
        pi = owner.GetComponent<PlayerInfo>();
        Collider[] colliders = Physics.OverlapSphere(owner.transform.position, 3);
        GameObject target = null;
        float minDis = 10;
        foreach (var collider in colliders)
        {
            if (collider.tag == "Fish")
            {
                target = collider.gameObject;
                break;
            }
            if (collider.gameObject.layer == 9)
            {
                float dis = Vector3.Distance(collider.transform.position, owner.transform.position);
                if (minDis > dis)
                {
                    target = collider.gameObject;
                    minDis = dis;
                }
            }
        }
        if (target == null) return true;
        owner.GetComponent<PlayerAttack>().Attack(target);
        return true;    
    }

    //这个方法会在技能进行过程中不断调用，当返回true表示技能已经完成所有动作
    protected override bool SkillAction()
    {
        return false;
    }

    //这个方法会在技能结束时调用
    protected override void SkillEnd()
    {

    }
    //您可以通过该方法提供一个技能的详细描述，您可以通过在文字中嵌入属性字典中的值来避免反复修改代码。
    public override string GetDescription()
    {
        return "skill " + name + " has no specific description.";
    }
}