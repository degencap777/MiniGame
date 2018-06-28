using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    private Vector3 offset = new Vector3(0, 7, -3);
    private Vector3 currentVelocity = Vector3.zero;
    private float smoothing = 4;
    public float smoothingTime = 0.3f;
    [HideInInspector]
    public bool isFollowPlayer = false;
    
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Gesture current = EasyTouch.current;
        //滑动
        if (isFollowPlayer)
        {
            FollowPlayer();
        }
        else
        {
            if (current != null && (current.type == EasyTouch.EvtType.On_Swipe || current.type == EasyTouch.EvtType.On_Drag) && current.touchCount == 1)
            {
                transform.Translate(smoothing * Vector3.left * current.deltaPosition.x / Screen.width, Space.World);
                transform.Translate(smoothing * Vector3.back * current.deltaPosition.y / Screen.height, Space.World);
            }
        }
    }

    private void FollowPlayer()
    {
        if (target == null) return;
        Vector3 targetPosition = target.position + offset;
        //transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothingTime);
    }
}
