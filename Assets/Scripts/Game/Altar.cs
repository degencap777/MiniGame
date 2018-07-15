using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    private float timer = 5;
    private bool IsOpened = false;
    void OnTriggerStay(Collider collider)
    {
        if (IsOpened) return;
        GameObject go = collider.gameObject;
        if (go.tag=="Fish")
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                GameFacade.Instance.OpenAltar(transform.position);
                IsOpened = true;
            }
        }
    }
}
