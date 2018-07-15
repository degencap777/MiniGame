using System.Collections;
using System.Collections.Generic;
using Common;
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
        if (range == null && target == null)
        {
            List<GameObject> effectList = gamePanel.OnSkillJoyMoveStart();
            range = Instantiate(effectList[0], gamePanel.Role.transform);
            target = Instantiate(effectList[1], gamePanel.Role.transform);
        }
        else
        {
            range.gameObject.SetActive(true);
            target.gameObject.SetActive(true);
        }
        range.transform.parent = gamePanel.Role.transform;
        target.transform.parent = gamePanel.Role.transform;
        range.transform.localPosition = Vector3.zero;
        range.GetComponent<Projector>().orthographicSize = skill.parameters["Distance"];
        target.GetComponent<Projector>().orthographicSize =
            skill.parameters.TryGet("Range") == 0 ? 1 : skill.parameters.TryGet("Range");
    }
    private void OnMove(Vector2 move)
    {
        if (gamePanel.Role != null && gamePanel.Role.GetComponent<PlayerInfo>().RoleType != RoleType.Hero)
        {
            range.gameObject.SetActive(false);
            target.gameObject.SetActive(false);
            return;
        }
        axis = move;
        target.transform.position =
            new Vector3(move.x, 0, move.y) * skill.parameters["Distance"] + range.transform.position;
    }
    private void OnMoveEnd()
    {
        range.gameObject.SetActive(false);
        target.gameObject.SetActive(false);
        Debug.Log("本地按下"+SkillName+"技能按钮");
        if (SkillName == "Create")
        {
            Vector3 targetPos = new Vector3(axis.x,0,axis.y)* skill.parameters["Distance"] +
                                new Vector3(GameFacade.Instance.GetCurrentOpTarget().transform.position.x, 0, GameFacade.Instance.GetCurrentOpTarget().transform.position.z);

            Collider[] colliders = Physics.OverlapSphere(targetPos + new Vector3(0, 0.5f, 0), 0.4f);
            foreach (var collider in colliders)
            {
                GameObject g = collider.gameObject;
                if (LayerMask.LayerToName(g.layer) == "Cube")
                {
                    return;
                }
            }
        }
        gamePanel.UseSkill(SkillName,axis.x+","+axis.y);
    }
    
}
