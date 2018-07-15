using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualProvider : MonoBehaviour
{
    [HideInInspector]
    //Never try to modify this value manually, it will be managed by VisualManager.
    public ushort id = 0;

    [Range(0, 500)]
    public float visualRange = 4;

    public bool showRangeInEidtor = true;

    public bool active = true;

    public bool noOcclusion = false;
    public bool IsTrueVision = false;
    private VisualManager visualManager;
    void Start()
    {
        visualManager = GameObject.FindGameObjectWithTag("visualManager").GetComponent<VisualManager>();
        if (visualManager == null)
        {
            Debug.Log("No visualManager in the scene.");
        }
        else
        {
            visualManager.Register(this);
        }
    }
    public void AddVisual()
    {
        visualManager.Register(this);
        active = true;
    }
    public void RemoveVisual()
    {
        visualManager.Logout(this);
        active = false;
    }

    public void OnDestroy()
    {
        active = false;
        visualManager.TryToLogout(this);
    }

    private void OnDisable()
    {
        active = false;
    }

    private void OnEnable()
    {
        active = true;
    }

    private void OnDrawGizmos()
    {   
        if (showRangeInEidtor)
        {
            if (active)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, visualRange);
        }
    }
}
