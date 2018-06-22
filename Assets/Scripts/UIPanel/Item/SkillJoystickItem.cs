using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillJoystickItem : MonoBehaviour {

    public float coldTime = 2f;
    private float timer = 0;
    private Image filledImage;
    private bool isStartTimer = false;
    private ETCJoystick JoyStick;
    private Vector2 axis;
    private GamePanel gamePanel;
    public string SkillName = "";

    // Use this for initialization
    void Start()
    {
        filledImage = transform.Find("FilledImage").GetComponent<Image>();
        JoyStick = GetComponent<ETCJoystick>();
        JoyStick.onMove.AddListener(OnMove);
        JoyStick.onMoveEnd.AddListener(OnMoveEnd);
        gamePanel = transform.parent.GetComponent<GamePanel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartTimer)
        {
            JoyStick.activated = false;
            timer += Time.deltaTime;
            filledImage.fillAmount = (coldTime - timer) / coldTime;
            axis=Vector2.zero;
            if (timer >= coldTime)
            {
                filledImage.fillAmount = 0;
                timer = 0;
                isStartTimer = false;
                JoyStick.activated = true;
            }
        }
    }

    public void UseSkillSync(float coldTime)
    {
        if (coldTime == 0) return;
        this.coldTime = coldTime;
        isStartTimer = true;
    }
    private void OnMove(Vector2 move)
    {
        axis = move;
    }
    private void OnMoveEnd()
    {
        if (SkillName == "")
        {
            Debug.Log("没为按钮定义技能");
            return;
        }
        gamePanel.UseSkill(SkillName,axis.x+","+axis.y);
    }
}
