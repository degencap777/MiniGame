using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillJoystickItem : MonoBehaviour {

    private float coldTime = 2f;
    private float timer = 0;
    private Image filledImage;
    private bool isStartTimer = false;
    private ETCJoystick JoyStick;
    private Vector2 axis;
    private GamePanel gamePanel;
    public string SkillName = "";
    private Skill skill;
    private GameObject range;
    private GameObject target;

    // Use this for initialization
    void Start()
    {
        filledImage = transform.Find("FilledImage").GetComponent<Image>();
        JoyStick = GetComponent<ETCJoystick>();
        JoyStick.onMove.AddListener(OnMove);
        JoyStick.onTouchUp.AddListener(OnMoveEnd);
        JoyStick.onMoveStart.AddListener(OnMoveStart);
        gamePanel = transform.parent.parent.GetComponent<GamePanel>();
        SkillName = name.Split('(')[0];
        skill = GameFacade.Instance.GetSkill(SkillName);
        coldTime = skill.parameters["CD"];
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
    private void OnMoveStart()
    {
        List<GameObject> effectList = gamePanel.OnSkillJoyMoveStart();
        range = effectList[0];
        range.GetComponent<Projector>().orthographicSize *= skill.parameters["Distance"];
        target = effectList[1];
        target.GetComponent<Projector>().orthographicSize *=
            skill.parameters.TryGet("Range") == 0 ? 1 : skill.parameters.TryGet("Range");
    }
    private void OnMove(Vector2 move)
    {
        axis = move;
        target.transform.position =
            new Vector3(move.x, 0, move.y) * skill.parameters["Distance"] + range.transform.position;
    }
    private void OnMoveEnd()
    {
        Debug.Log("本地按下"+SkillName+"技能按钮");
        if (SkillName == "")
        {
            Debug.Log("没为按钮定义技能");
            return;
        }
        Destroy(range);
        Destroy(target);
        gamePanel.UseSkill(SkillName,axis.x+","+axis.y);
    }
    
}
