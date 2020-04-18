using System.Collections;
using System.Collections.Generic;
using theArch_LD46.GlobalHelper;
using UnityEngine;
using System;

public class PlayerScript : MonoBehaviour
{
    //TODO：田子，读取键盘输入，让玩家基于速度移动。留出速度，位置等公开API。需要对当地中带有collider的墙有反应。
    //不使用旋转的话，那就是八方向行走咯
    public Vector3 front_vector;//常量 定义前向 范围0-7
    //public Vector3 current_vector;
    //public float max_speed;
    //public float speed;
    public float velocity_val = 5;
    private Vector3 velocity;
    //public float resistance = 0.5f;
    private bool a=false, d=false, w=false, s=false;
    public Transform player_transform;
    //private const float unit = 0.70710678f;
    // Start is called before the first frame update
    void Start()
    {
        player_transform = this.transform;
        //speed = 0;
        velocity = new Vector3(0,0,0);
        //current_vector = front_vector;
        //resistance = 0.5f;
        //velocity =new Vector3(0 ,0, 0);
        //crt_velocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //string AxisName = StaticName.INPUT_AXIS_NAME_FORWARD;
        a = Input.GetKey(KeyCode.A);
        s = Input.GetKey(KeyCode.S);
        d = Input.GetKey(KeyCode.D);
        w = Input.GetKey(KeyCode.W);

        if (!(a && s && d) && w) //w
        {
            velocity = velocity_val*(Quaternion.AngleAxis(0.0f, player_transform.up) * front_vector);
        }
        else if(!(a && s) && (d && w)) //wd
        {
            velocity = velocity_val * (Quaternion.AngleAxis(45.0f, player_transform.up) * front_vector);
        }
        else if (!(a && s && w) && d) //d
        {
            velocity = velocity_val * (Quaternion.AngleAxis(90.0f, player_transform.up) * front_vector);
        }
        else if (!(a && w) && (s && d)) //sd
        {
            velocity = velocity_val * (Quaternion.AngleAxis(135.0f, player_transform.up) * front_vector);
        }
        else if (!(a  && d && w) && s) //s
        {
            velocity = velocity_val * (Quaternion.AngleAxis(180.0f, player_transform.up) * front_vector);
        }
        else if ( !(d && w) && (a && s)) //sa
        {
            velocity = velocity_val * (Quaternion.AngleAxis(-135.0f, player_transform.up) * front_vector);
        }
        else if (a && !(s && d && w)) //a
        {
            velocity = velocity_val * (Quaternion.AngleAxis(-90.0f, player_transform.up) * front_vector);
        }
        else if ((a && w) && !(s && d)) //wa
        {
            velocity = velocity_val * (Quaternion.AngleAxis(-45.0f, player_transform.up) * front_vector);
        }
        else
        {
            velocity = new Vector3(0,0,0);
        }
        /*
        if (speed > -max_speed && speed <= -0.001 * max_speed)
        {
            speed = Time.deltaTime * (velocity + resistance);
        }
        else if (speed < max_speed && speed >= 0.001 * max_speed)
        {
            speed = Time.deltaTime * (velocity - resistance);
        }
        else if (speed <= -max_speed)
        {
            speed = -max_speed;
        }
        else if (speed >= max_speed)
        {
            speed = max_speed;
        }
        else
        {
            speed = 0;
        }*/

        //rot_speed += Time.deltaTime * (rot_velocity - rot_resistance);
        player_transform.Translate(Time.deltaTime * velocity);
    }
}
