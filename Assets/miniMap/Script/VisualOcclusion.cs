using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class VisualOcclusion : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.layer = 19;
        gameObject.GetComponent<MeshRenderer>().material = GameObject.FindGameObjectWithTag("visualManager").GetComponent<VisualManager>().visualOcclusionMaterial;
        Destroy(this);
    }

}
