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

    private Transform _target;

    public Transform Target
    {
        get
        {
            if (_target == null)
            {
                if (facade.GetCurrentOpTarget() != null)
                {
                    _target = facade.GetCurrentOpTarget().transform;
                    FollowCurrentTarget();
                }
            }
            else if (_target != facade.GetCurrentOpTarget())
            {
                _target = facade.GetCurrentOpTarget() == null ? null : facade.GetCurrentOpTarget().transform;
                FollowCurrentTarget();
            }
            return _target;
        }
    }


    public override void OnInit()
    {
        base.OnInit();
        cameraGO = Camera.main.gameObject;
        cameraAnim = cameraGO.GetComponent<Animator>();
        followTarget = cameraGO.GetComponent<FollowTarget>();
    }

    public override void Update()
    {
        base.Update();
        if (Target == null)
        {
            StopFollowTarget();
        }
    }
    public void FollowCurrentTarget()
    {
        followTarget.target = _target;
        if (_target == null)
            StopFollowTarget();
        else
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
    /// <summary>
    /// 应该用不到
    /// </summary>
    public void StopFollowTarget()
    {
        followTarget.isFollowPlayer = false;
    }
    public void GameOver()
    {
        followTarget.isFollowPlayer = false;
    }
}
