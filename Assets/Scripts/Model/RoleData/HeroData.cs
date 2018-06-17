using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class HeroData:RoleData
{
    /// <summary>
    /// 房间内的位置对应固定的英雄
    /// </summary>
    public List<int> seatIndex { get; set; }
    public int attackDamage { get; set; }

    public HeroData(int id, CampType campType, string name, RoleType roleType, string description, string path, int hp, int mp, int moveSpeed, 
        int turnSpeed, bool isSkyVision, List<int> seatIndex, int attackDamage) :base( id,  campType,  name,  roleType,  description, path,
            hp, mp, moveSpeed, turnSpeed, isSkyVision)
    {
        this.seatIndex = seatIndex;
        this.attackDamage = attackDamage;
    }
}
