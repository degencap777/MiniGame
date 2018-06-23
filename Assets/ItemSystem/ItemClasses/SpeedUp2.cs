using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp2 : Item
{
    //timeSinceItemStart     道具从发动以来经过的时间
    //owner                   道具发动者
    //name                    道具名(不是类名，是游戏中想要展示的名字)
    //parameters              可调道具参数的字典
    //resources               资源预置体

    GameObject go;

    //这个方法会在道具发动时调用
    protected override bool ItemStart()
    {
        go = resources;
        go.AddComponent<DestroyForTime>().time = parameters.TryGet("CD");
        go.transform.parent = owner.GetComponent<PlayerInfo>().Player.Reference.transform;
        go.SetActive(true);
        go.transform.position = Position;
        return true;
    }

    //这个方法会在道具进行过程中不断调用，当返回false表示道具已经完成所有动作
    protected override bool ItemAction()
    {
        if (timeSinceItemStart < parameters.TryGet("During"))
        {

            return false;
        }
        return true;
    }

    //这个方法会在道具结束时调用
    protected override void ItemEnd()
    {
        Object.Destroy(go);
    }

    //您可以通过该方法提供一个道具的详细描述，您可以通过在文字中嵌入属性字典中的值来避免反复修改代码。
    public override string GetDescription()
    {
        return "这个技能使用后就能进行加速";
    }
}