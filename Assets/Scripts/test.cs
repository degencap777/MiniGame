using UnityEngine;
using System.Collections;

public class test : MonoBehaviour
{

    private GameObject clone;
    private LineRenderer line;
    int i;
    //带有LineRender物体
    public GameObject target;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //实例化对象
            clone = (GameObject)Instantiate(target, target.transform.position, Quaternion.identity);
            clone.AddComponent<Rigidbody>();
            //获得该物体上的LineRender组件
            line = clone.GetComponent<LineRenderer>();
            //设置起始和结束的颜色
            line.startColor=Color.red;
            line.endColor=Color.blue;
            //设置起始和结束的宽度
            line.startWidth = 0.2f;
            line.endWidth = 0.1f;
            //计数
            i = 0;
        }
        if (Input.GetMouseButton(0))
        {
            //每一帧检测，按下鼠标的时间越长，计数越多
            i++;
            //设置顶点数
            line.positionCount = i;
            //设置顶点位置(顶点的索引，将鼠标点击的屏幕坐标转换为世界坐标)
            line.SetPosition(i - 1, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15)));


        }

    }
}