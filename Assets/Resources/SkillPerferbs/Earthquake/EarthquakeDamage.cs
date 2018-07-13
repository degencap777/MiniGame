using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeDamage : MonoBehaviour
{
    
    void Update()
    {
        transform.Translate(Time.deltaTime*new Vector3(0,0,6.5f));
    }
    void OnTriggerEnter(Collider collider)
    {
        GameObject role = collider.gameObject;
        if (role.layer == 10&&role.tag=="Fish")
        {
            PlayerInfo pi = role.GetComponent<PlayerInfo>();
            pi.Damage(200);
        }
    }
}
