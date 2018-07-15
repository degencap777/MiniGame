using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tools;
using UnityEngine;

public class MapInfo : MonoBehaviour
{

    public int Hp;
    public int HeightLayer;
    public int CurrentHp { get; private set; } 
    public Vector2 IndexV2 { get; private set; }
    private GamePanel gamePanel;

    IEnumerator Start()
    {
        gameObject.layer = 9;
        transform.position= UnityTools.RoundV3(transform.position);
        CurrentHp = Hp;
        IndexV2= UnityTools.V3ToV2(transform.position);
        while (GameFacade.Instance.GetCurrentPanelType() != UIPanelType.Game)
        {
            yield return null;
        }
        gamePanel = (GamePanel)GameFacade.Instance.GetCurrentPanel();
        gamePanel.MapInfos[(int)IndexV2.x, (int)IndexV2.y] = this;
    }

    public void Damage(int damage)
    {
        Debug.Log("受到伤害:"+damage);
        CurrentHp = CurrentHp - damage < 0 ? 0 : CurrentHp - damage;
        if (CurrentHp <= 0)
        {
            if(gamePanel!=null)
                gamePanel.DestroyCube(IndexV2);
        }
    }

    public void DestroyCube(GameObject effect)
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
