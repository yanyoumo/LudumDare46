using System.Collections;
using System.Collections.Generic;
using theArch_LD46.GlobalHelper;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //TODO：田子，读取键盘输入，让玩家基于速度移动。留出速度，位置等公开API。需要对当地中带有collider的墙有反应。

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string AxisName=StaticName.INPUT_AXIS_NAME_FORWARD;
    }
}
