using System.Collections;
using System.Collections.Generic;
using theArch_LD46.GlobalHelper;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //TODO：田子，读取键盘输入，让玩家基于速度移动。留出速度，位置等公开API。需要对当地中带有collider的墙有反应。
    public Vector3 front_vector;//常量 定义前向
    public Vector3 current_vector;
    public float max_speed;
    public float speed;
    public float accelerate;
    public float resistance=0.5f;

    public float max_rot_speed;
    public float rot_speed;
    public float rot_accelerate;
    public float rot_resistance=0.5f;
    
    public Transform player_transform;
    // Start is called before the first frame update
    void Start()
    {
        player_transform = this.transform;
        speed = 0;
        accelerate = 0;

        rot_speed = 0;
        rot_accelerate = 0;
        current_vector = front_vector;
        //resistance = 0.5f;
        //accelerate =new Vector3(0 ,0, 0);
        //crt_velocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        string AxisName=StaticName.INPUT_AXIS_NAME_FORWARD;
        if (Input.GetKey(KeyCode.A))
        {
            rot_accelerate = -1;
        }
        else
        {
            rot_accelerate = 0;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rot_accelerate = 1;
        }
        else
        {
            rot_accelerate = 0;
        }
        if (Input.GetKey(KeyCode.W))
        {
            accelerate = 1;
        }
        else
        {
            accelerate = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            accelerate = -1;
        }
        else
        {
            accelerate = 0;
        }

        if (speed > -max_speed && speed <= -0.001*max_speed)
        {
            speed = Time.deltaTime * (accelerate + resistance);
        }
        else if(speed < max_speed&& speed >=0.001*max_speed)
        {
            speed = Time.deltaTime * (accelerate - resistance);
        }
        else if(speed <= -max_speed)
        {
            speed =-max_speed;
        }
        else if(speed >= max_speed)
        {
            speed = max_speed;
        }
        else
        {
            speed = 0;
        }

        if (rot_speed > -max_rot_speed && rot_speed <= -0.001 * max_rot_speed)
        {
            rot_speed = Time.deltaTime * (rot_accelerate + rot_resistance);
        }
        else if (rot_speed < max_rot_speed && rot_speed >= 0.001 * max_rot_speed)
        {
            rot_speed = Time.deltaTime * (rot_accelerate - rot_resistance);
        }
        else if (rot_speed <= -max_rot_speed)
        {
            rot_speed = -max_rot_speed;
        }
        else if (rot_speed >= max_rot_speed)
        {
            rot_speed = max_rot_speed;
        }
        else
        {
            rot_speed = 0;
        }

        //rot_speed += Time.deltaTime * (rot_accelerate - rot_resistance);
        player_transform.Rotate(player_transform.up, rot_speed * Time.deltaTime);
        player_transform.Translate(front_vector * Time.deltaTime * speed);
    }
}
