using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tools;
using UnityEngine;

public class PlayerItem : MonoBehaviour {

    private CharactorItems charactorItems;
    private PlayerManager playerManager;
    private Rigidbody rb;
    public float RotateSpeed { get { return playerInfo.TurnSpeed; } }
    public float MoveSpeed { get { return playerInfo.MoveSpeed; } }
    private Vector3 position = Vector3.positiveInfinity;
    private PlayerInfo playerInfo;
    private bool isLocal = false;
    private Vector3 moveDirection;

    private Item currentItem = null;
    // Use this for initialization
    void Start()
    {
        charactorItems = GetComponent<CharactorItems>();
        rb = transform.GetComponent<Rigidbody>();
        playerInfo = GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInfo.CurrentState == PlayerInfo.State.UsingItem)
        {
            if (IsMoveDone())
            {
                UseItem();
                playerInfo.CurrentState = PlayerInfo.State.Idle;
            }
            if (position != Vector3.positiveInfinity)
                MoveToUseItem();
        }
        else
        {
            currentItem = null;
            position = Vector3.positiveInfinity;
        }
    }

    private void UseItem(Item item=null)
    {
        if (item == null)
        {
            charactorItems.Items[currentItem.GetType().Name].Position = position;
            charactorItems.UseItem(currentItem.GetType().Name);
            if(isLocal)
                GameFacade.Instance.UseItemSync(true);
        }
        else
        {
            charactorItems.Items[item.GetType().Name].Position = position;
            charactorItems.UseItem(item.GetType().Name);
        }
    }
    public void StartUseItem(bool islocal,Item item, string point = null)
    {
        this.isLocal = islocal;
        if (point != null)
        {
            position = (UnityTools.ParseVector3(point));
            moveDirection = (position - transform.position).normalized;
            moveDirection.y = 0;
            playerInfo.CurrentState = PlayerInfo.State.UsingItem;
        }
        else
        {
            UseItem(item);
            if (islocal)
            {
                GameFacade.Instance.UseItemSync(true);
            }
        }
        currentItem = item;
    }
    public PlayerItem SetPlayerMng(PlayerManager playerMng)
    {
        this.playerManager = playerMng;
        return this;
    }

    private bool IsMoveDone()
    {
        if (Mathf.Abs(transform.rotation.eulerAngles.y - Quaternion.LookRotation(moveDirection, transform.up).eulerAngles.y) <= 20)
        {
            if (Mathf.Abs(Mathf.Pow(position.x - transform.position.x,2)+ Mathf.Pow(position.z - transform.position.z, 2)) <= 3*3)
            {
                return true;
            }
        }
        return false;
    }
    private void MoveToUseItem()
    {
        
        transform.rotation = Quaternion.Lerp(rb.rotation,Quaternion.LookRotation(moveDirection, transform.up),Time.fixedDeltaTime * RotateSpeed);
        
        //Debug.Log(moveDirection);
        transform.Translate(moveDirection * Time.fixedDeltaTime * MoveSpeed, Space.World);
        //transform.position=Vector3.Lerp(transform.position, new Vector3(position.x, transform.position.y,position.z),Time.deltaTime );
    }
}
