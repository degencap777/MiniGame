using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PetData : RoleData
{
    public PetData(int id, CampType campType, string name, RoleType roleType, string description, string path, int hp, int mp, int moveSpeed, int turnSpeed, bool isSkyVision,string imagePath) : base(id, campType, name, roleType, description, path, hp, mp, moveSpeed, turnSpeed, isSkyVision,imagePath)
    {
    }
}
