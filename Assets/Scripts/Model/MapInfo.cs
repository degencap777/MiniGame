using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tools;
using UnityEngine;

public class MapInfo : MonoBehaviour
{

    public int Hp;
    public int HeightLayer;
    public int CurrentHp { get; private set; } 
    public GameObject Effect;
    public float EffectTime;
    public Vector2 IndexV2 { get { return UnityTools.V3ToV2(UnityTools.RoundV3(transform.position)); } }
    private GamePanel gamePanel;

    void Awake()
    {
        transform.position= UnityTools.RoundV3(transform.position);
        if (GameFacade.Instance.GetCurrentPanelType().GetType().Name == "GamePanel")
            gamePanel = (GamePanel)GameFacade.Instance.GetCurrentPanel();
        if(gamePanel!=null)
            gamePanel.MapInfos[(int)IndexV2.x, (int)IndexV2.y] = this;
    }

    public void Damage(int damage)
    {
        CurrentHp = Hp - damage < 0 ? 0 : Hp - damage;
        if (CurrentHp <= 0)
        {
            if(gamePanel!=null)
                gamePanel.DestroyCube(IndexV2);
        }
    }

    public void Destroy(GameObject effect)
    {
        if (gamePanel!=null)
            gamePanel.MapInfos[(int)IndexV2.x, (int)IndexV2.y] = null;
        Destroy(gameObject);
        GameObject bomb = Instantiate(effect, transform.position, Quaternion.identity);
        bomb.transform.position -= new Vector3(0, 0.5f, 0);
        float duration = bomb.GetComponent<ParticleSystem>().main.duration;
        Destroy(bomb, duration);
    }
}
