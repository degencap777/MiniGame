using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public static class UnityTools
    {

        public static Vector3 ParseVector3(string str)
        {
            string[] strs = str.Split(',');
            float x = float.Parse(strs[0]);
            float y = float.Parse(strs[1]);
            float z = float.Parse(strs[2]);
            return new Vector3(x, y, z);
        }

        public static string PackVector3(Vector3 vector3)
        {
            return vector3.x + "," + vector3.y + "," + vector3.z;
        }
    }
}
