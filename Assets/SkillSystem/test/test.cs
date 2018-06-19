using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    CharactorSkills cs;

	// Use this for initialization
	void Start () {
        cs = GetComponent<CharactorSkills>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
        {
            cs.UseSkill<SpeedUp>();
        }	
	}
}
