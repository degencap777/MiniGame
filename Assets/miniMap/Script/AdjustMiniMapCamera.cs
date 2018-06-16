using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustMiniMapCamera : MonoBehaviour {

    [Range(0,31)]
    public int maskLayer = 20;

    public Camera occlussionCamera;
	void Start () {
        occlussionCamera.GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize;
        GetComponent<Camera>().cullingMask = (1 << maskLayer);
        Destroy(this);
	}
}
