using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class RoleData
{
    public CampType CampType { get; private set; }
    public GameObject RolePrefab { get; private set; }
    public Vector3 SpawnPosition { get; private set; }
    private const string PREFIX_PREFAB = "Prefabs/";
    public int RoleIndex { get; private set; }//一个Prefab一个Index
    public int SeatIndex { get; private set; }//一个Seat对应一个RoleIndex
    public RoleData(Transform rolePositions,int seatIndex)
    {
        this.SeatIndex = seatIndex;
        this.RoleIndex = seatIndex < GameFacade.FISH_NUM ? 0 : seatIndex - GameFacade.FISH_NUM + 1;
        this.CampType = SeatIndex < GameFacade.FISH_NUM ? CampType.Fish : CampType.Monkey;
        this.SpawnPosition = rolePositions.Find(CampType+"Position" + (seatIndex < GameFacade.FISH_NUM ?seatIndex:seatIndex- GameFacade.FISH_NUM)).position;
        
        this.RolePrefab = Resources.Load<GameObject>(PREFIX_PREFAB + CampType + RoleIndex);
        //ArrowPrefab.GetComponent<Arrow>().explosionEffect = explosionEffect;
    }
}
