using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VisualTest : MonoBehaviour {
    
    public bool isInVisual = false;

    public float testRadius = 20f;

    public bool autoTest = true;

    public bool autoHideWhenOutOfVisual = true;

    public bool showTestRadiusInEditor = true;
    public bool showTestRayInEditor = true;

    private MeshRenderer mr;

    void Start() {
        mr = GetComponent<MeshRenderer>();
    }

    void FixedUpdate()
    {
        if (autoTest) isInVisual = InVisual();
        if (autoHideWhenOutOfVisual) mr.enabled = autoTest? isInVisual : InVisual();
    }

    public bool InVisual()
    {
        Vector3 pos = transform.position;
        var Colliders = Physics.OverlapSphere(transform.position, testRadius);
        for (int i = 0; i < Colliders.Length; ++i)
        {
            var vp = Colliders[i].GetComponent<VisualProvider>();
            if (vp != null && vp.active)
            {
                Vector3 target = vp.transform.position;
                float dis = Vector3.Distance(target, pos);
                if (dis < vp.visualRange)
                {
                    Ray ray = new Ray(pos, target - pos);
                    if (showTestRayInEditor) Debug.DrawLine(pos, target);
                    if (!Physics.Raycast(ray, dis, 1 << 19)){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (showTestRadiusInEditor)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, testRadius);
        }
    }
}
