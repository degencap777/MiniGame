using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private Camera cam;
	// Use this for initialization
	void Start () {
		cam=Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(cam.transform);
        transform.rotation=Quaternion.Euler(transform.rotation.eulerAngles.x,-90, transform.rotation.eulerAngles.z);
	}
}
