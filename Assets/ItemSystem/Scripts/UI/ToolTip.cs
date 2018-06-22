using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{

    private Text toolTipText;
    private Text contenText;
    private CanvasGroup canvasGroup;
    private float targetAlpha = 0;
    public float smoothing = 4;
    private Queue<int> showQueue=new Queue<int>();

    void Start()
    {
        toolTipText = GetComponent<Text>();
        contenText = GameObject.Find("Content").GetComponent<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01)
            {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }

    public void Show(string text)
    {
        canvasGroup.alpha = 0;
        toolTipText.text = text;
        contenText.text = text;
        targetAlpha = 1;
        Invoke("Hide", Mathf.Max(text.Length / 4,2));
        showQueue.Enqueue(1);
    }

    public void Hide()
    {
        showQueue.Dequeue();
        if (showQueue.Count>0) return;
        targetAlpha = 0;
    }

    public void SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
    }
}