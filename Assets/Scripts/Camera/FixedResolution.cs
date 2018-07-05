using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedResolution : MonoBehaviour {

    private Camera mainCamera;
    private int scaleWidth = 0;
    private int scaleHeight = 0;

    void Awake()
    {
        SetDesignContentScale();
    }
    void Start()
    {
        //Screen.SetResolution(1280, 800, true, 60);
        mainCamera = Camera.main;
        //  float screenAspect = 1920 / 1080;  现在android手机的主流分辨。
        //  mainCamera.aspect --->  摄像机的长宽比（宽度除以高度）
        mainCamera.aspect = 1.7777778f;
    }

    public void SetDesignContentScale()
    {
#if UNITY_ANDROID
        if (scaleWidth == 0 && scaleHeight == 0)
        {
            int width = Screen.currentResolution.width;
            int height = Screen.currentResolution.height;
            int designWidth = 1920;
            int designHeight = 1080;
            float s1 = (float) designWidth / (float) designHeight;
            float s2 = (float) width / (float) height;
            if (s1 < s2)
            {
                designWidth = (int) Mathf.FloorToInt(designHeight * s2);
            }
            else if(s1>s2)
            {
                designHeight = (int) Mathf.FloorToInt(designWidth / s2);
            }
            float contentScale = (float) designWidth / (float) width;
            if (contentScale < 1.0f)
            {
                scaleWidth = designWidth;
                scaleHeight = designHeight;
            }
        }
        if (scaleWidth > 0 && scaleHeight > 0)
        {
            if (scaleWidth % 2 == 0)
            {
                scaleWidth += 1;
            }
            else
            {
                scaleWidth -= 1;
            }
            Screen.SetResolution(scaleWidth,scaleHeight,true);
        }
#endif
    }
    void OnApplicationPause(bool paused)
    {
        if (!paused)
        {
            SetDesignContentScale();
        }
    }
}
