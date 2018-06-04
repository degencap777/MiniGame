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
    public override void OnInit()
    {
        base.OnInit();
        cameraGO = Camera.main.gameObject;
        //cameraAnim = cameraGO.GetComponent<Animator>();
        //followTarget = cameraGO.GetComponent<FollowTarget>();
    }


    public void FollowRole()
    {
        followTarget.target = facade.GetRolePosition();
        cameraAnim.enabled = false;
        Quaternion targetQuaternion = Quaternion.LookRotation(followTarget.target.position - cameraGO.transform.position);
        cameraGO.transform.DORotateQuaternion(targetQuaternion, 1f).OnComplete(delegate ()
        {
            followTarget.enabled = true;
        });
        originelPosition = cameraGO.transform.position;
        originelRotation = cameraGO.transform.eulerAngles;
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

    public void UpdateCamera()
    {
        cameraGO = Camera.main.gameObject;
    }
}
