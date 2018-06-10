using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{

    public Transform target;
    private Vector3 offset = new Vector3(0, 5f, -5f);

    private float smoothing = 4;
    public bool isFollowPlayer = false;
    
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Gesture current = EasyTouch.current;
        //滑动
        if (current != null && current.type == EasyTouch.EvtType.On_Swipe && current.touchCount == 1)
        {
            transform.Translate(smoothing*Vector3.left * current.deltaPosition.x / Screen.width,Space.World);
            transform.Translate(smoothing*Vector3.back * current.deltaPosition.y / Screen.height, Space.World);
        }
        if (isFollowPlayer)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
    }
}
