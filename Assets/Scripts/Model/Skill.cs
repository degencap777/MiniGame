using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {

    public Skill(string data)
    {
        string[] strs = data.Split(',');
        string[] pos = strs[0].Split('-');
        this.Position = new Vector3(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(pos[2]));
        string[] rot = strs[1].Split('-');
        this.Rotation = new Vector3(int.Parse(rot[0]), int.Parse(rot[1]), int.Parse(rot[2]));
    }

	public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public float Damage { get; set; }
    public float Range { get; set; }
    public string Effect { get; set; }
    
}
