using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthbarRoot : MonoBehaviour {
    public List<Transform> healthBars = new List<Transform>(); //List of helthbars;
    private Camera cam;
    private Transform cameraTransform;                          //Main camera transform
	
    void Start ()
    {
        cam = Camera.main;
        cameraTransform = cam.transform;

        for (int i = 0; i < transform.childCount; i++)
            healthBars.Add(transform.GetChild(i));
	}
	
	void Update () 
    {
        healthBars.Sort(DistanceCompare);

        for (int i = 0; i < healthBars.Count; i++)
        {
            if (healthBars[i] == null)
            {
                healthBars.Remove(healthBars[i]);
                continue;
            }
            healthBars[i].SetSiblingIndex(healthBars.Count - (i+1));
        }
        SetUiIndex();
	}
    private void SetUiIndex()
    {
        int count = transform.parent.childCount;
        //参数为物体在当前所在的子物体列表中的顺序
        //count-1指把child物体在当前子物体列表的顺序设置为最后一个，0为第一个
        transform.SetSiblingIndex(count - 3);
    }
    private int DistanceCompare(Transform a, Transform b)
    {
        if (a == null || b == null)
            return 0;
        return Mathf.Abs((WorldPos(a.position) - cameraTransform.position).sqrMagnitude).CompareTo(Mathf.Abs((WorldPos(b.position) - cameraTransform.position).sqrMagnitude));
    }

    private Vector3 WorldPos(Vector3 pos)
    {
        return cam.ScreenToWorldPoint(pos);
    }
}
