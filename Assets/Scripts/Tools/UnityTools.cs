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
        public static Vector2 ParseVector2(string str)
        {
            string[] strs = str.Split(',');
            float x = float.Parse(strs[0]);
            float y = float.Parse(strs[1]);
            return new Vector2(x, y);
        }
        public static string PackVector2(Vector2 vector2)
        {
            return vector2.x + "," + vector2.y;
        }
        public static Vector3 RoundV3(Vector3 v3)
        {
            return new Vector3(Mathf.Round(v3.x), Mathf.Round(v3.y), Mathf.Round(v3.z));
        }
        public static Vector2 V3ToV2(Vector3 v3)
        {
            return new Vector2(Mathf.Round(v3.x), Mathf.Round(v3.z));
        }
    }
}
