using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraManager : BaseManager
{

    public CameraManager(GameFacade facade) : base(facade) { }
    private GameObject cameraGO;
    private Animator cameraAnim;
    private FollowTarget followTarget;
    private Vector3 originelPosition;
    private Vector3 originelRotation;
    private Transform target;
    
    public override void OnInit()
    {
        base.OnInit();
        cameraGO = Camera.main.gameObject;
        cameraAnim = cameraGO.GetComponent<Animator>();
        followTarget = cameraGO.GetComponent<FollowTarget>();
    }

    public void FollowTartget(Transform target)
    {
        this.target = target;
        FollowCurrentTarget();
    }
    public void FollowCurrentTarget()
    {
        followTarget.target = target;
        followTarget.isFollowPlayer = true;
        cameraAnim.enabled = false;
    }

    public void WalkThroughScene()
    {
        followTarget.enabled = false;
        cameraGO.transform.DOMove(originelPosition, 1f);
        cameraGO.transform.DORotate(originelRotation, 1f).OnComplete(delegate ()
        {
            cameraAnim.enabled = true;
        });
    }

    //public void EnterPlaying()
    //{
    //    cameraGO= Camera.main.gameObject;
    //}

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
