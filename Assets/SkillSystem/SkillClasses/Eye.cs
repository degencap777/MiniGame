using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Skill
{
    //timeSinceSkillStart     技能从发动以来经过的时间
    //owner                   技能发动者
    //name                    技能名(不是类名，是游戏中想要展示的名字)
    //parameters              可调技能参数的字典
//resources               资源预置体


    private GameObject go;

    //这个方法会在技能发动时调用,返回值为false则发动失败，不会调用后续方法
    protected override bool SkillStart()
    {
        Vector3 dir = new Vector3(Direction.x, 0, Direction.z);
        go = resources;
        go.transform.parent = owner.GetComponent<PlayerInfo>().Player.Reference.transform;
        go.transform.position = dir * parameters["Distance"] + new Vector3(owner.transform.position.x, 0, owner.transform.position.z);
        go.AddComponent<DestroyForTime>().time = parameters.TryGet("During");
        if (owner.GetComponent<PlayerInfo>().CampType == GameFacade.Instance.GetLocalPlayer().CampType)
        {
            go.AddComponent<VisualProvider>().noOcclusion = false;
            go.GetComponent<VisualProvider>().visualRange = parameters.TryGet("Range");
        }

        else
        {
            go.AddComponent<VisualTest>();
            //go.SetActive(false);
        }
        PlayerManager.SetItemList(go.GetInstanceID(), go);
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