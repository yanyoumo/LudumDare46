using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace theArch_LD46
{
    /*
    //TODO：田子，读取键盘输入，让玩家基于速度移动。留出速度，位置等公开API。需要对当地中带有collider的墙有反应。
    //不使用旋转的话，那就是八方向行走咯
    public Vector3 front_vector;//常量 定义前向 范围0-7
    //public Vector3 current_vector;
    //public float max_speed;
    //public float speed;
    public float speed = 5;
    private Vector3 velocity;
    //public float resistance = 0.5f;
    
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
            velocity = speed*(Quaternion.AngleAxis(0.0f, player_transform.up) * front_vector);
        }
        else if(!(a && s) && (d && w)) //wd
        {
            velocity = speed * (Quaternion.AngleAxis(45.0f, player_transform.up) * front_vector);
        }
        else if (!(a && s && w) && d) //d
        {
            velocity = speed * (Quaternion.AngleAxis(90.0f, player_transform.up) * front_vector);
        }
        else if (!(a && w) && (s && d)) //sd
        {
            velocity = speed * (Quaternion.AngleAxis(135.0f, player_transform.up) * front_vector);
        }
        else if (!(a  && d && w) && s) //s
        {
            velocity = speed * (Quaternion.AngleAxis(180.0f, player_transform.up) * front_vector);
        }
        else if ( !(d && w) && (a && s)) //sa
        {
            velocity = speed * (Quaternion.AngleAxis(-135.0f, player_transform.up) * front_vector);
        }
        else if (a && !(s && d && w)) //a
        {
            velocity = speed * (Quaternion.AngleAxis(-90.0f, player_transform.up) * front_vector);
        }
        else if ((a && w) && !(s && d)) //wa
        {
            velocity = speed * (Quaternion.AngleAxis(-45.0f, player_transform.up) * front_vector);
        }
        else
        {
            if (velocity.magnitude > 0.001f)
            {
                velocity = velocity * 0.5f;
            }
            else
            {
                velocity =new Vector3(0,0,0);
            }
        }
        
        //if (speed > -max_speed && speed <= -0.001 * max_speed)
        //{
        //    speed = Time.deltaTime * (velocity + resistance);
        //}
        //else if (speed < max_speed && speed >= 0.001 * max_speed)
        //{
        //    speed = Time.deltaTime * (velocity - resistance);
        //}
        //else if (speed <= -max_speed)
        //{
        //    speed = -max_speed;
        //}
        //else if (speed >= max_speed)
        //{
        //    speed = max_speed;
        //}
        //else
        //{
        //    speed = 0;
        //}

        //rot_speed += Time.deltaTime * (rot_velocity - rot_resistance);
        player_transform.Translate(Time.deltaTime * velocity);
      */
    public class PlayerScript : MonoBehaviour
    {

        private Vector3 MoveForward;
        private Vector3 MoveLeft;

        public Transform Campos;

        public float VisionVal { private set; get; }
        public float AudioVal { private set; get; }
        public float FeelingVal { private set; get; }
        public float CompassVal { private set; get; }

        public float speed = 0.75f;
        public float delVal = 0.1f;

        public CharacterController charCtrl;

        //move key control
        private bool a = false, d = false, w = false, s = false;
        private Vector3 velocity;
        // Start is called before the first frame update
        void Start()
        {
            MoveForward = -Vector3.Normalize(new Vector3(Campos.position.x, 0.0f, Campos.position.z));
            MoveLeft = Vector3.Cross(MoveForward, Vector3.up);

            VisionVal = 0.75f;
            AudioVal = 0.75f;
            FeelingVal = 0.75f;
            CompassVal = 0.75f;
        }
        void player_move(Vector3 MoveForward)
        {
            a = Input.GetKey(KeyCode.A);
            s = Input.GetKey(KeyCode.S);
            d = Input.GetKey(KeyCode.D);
            w = Input.GetKey(KeyCode.W);

            if (!(a || s || d) && w) //w
            {
                velocity = speed * (Quaternion.AngleAxis(0.0f, this.transform.up) * MoveForward);
            }
            else if (!(a || s) && (d && w)) //wd
            {
                velocity = speed * (Quaternion.AngleAxis(45.0f, this.transform.up) * MoveForward);
            }
            else if (!(a || s || w) && d) //d
            {
                velocity = speed * (Quaternion.AngleAxis(90.0f, this.transform.up) * MoveForward);
            }
            else if (!(a || w) && (s && d)) //sd
            {
                velocity = speed * (Quaternion.AngleAxis(135.0f, this.transform.up) * MoveForward);
            }
            else if (!(a || d || w) && s) //s
            {
                velocity = speed * (Quaternion.AngleAxis(180.0f, this.transform.up) * MoveForward);
            }
            else if (!(d || w) && (a && s)) //sa
            {
                velocity = speed * (Quaternion.AngleAxis(-135.0f, this.transform.up) * MoveForward);
            }
            else if (a && !(s || d || w)) //a
            {
                velocity = speed * (Quaternion.AngleAxis(-90.0f, this.transform.up) * MoveForward);
            }
            else if ((a && w) && !(s || d)) //wa
            {
                velocity = speed * (Quaternion.AngleAxis(-45.0f, this.transform.up) * MoveForward);
            }
            else
            {
                if (velocity.magnitude > 0.001f)
                {
                    velocity = velocity * 0.5f;
                }
                else
                {
                    velocity = new Vector3(0, 0, 0);
                }
            }
            this.transform.Translate(Time.deltaTime * velocity);
            return;
        }
        // Update is called once per frame
        void Update()
        {
            Vector2 inputVec=new Vector2(Input.GetAxis(GlobalHelper.StaticName.INPUT_AXIS_NAME_FORWARD), Input.GetAxis(GlobalHelper.StaticName.INPUT_AXIS_NAME_LEFR));
            Vector3 movingVec = (inputVec.x * MoveForward) + (inputVec.y * MoveLeft);
            movingVec = Vector3.Normalize(movingVec) * speed;
            //gameObject.transform.position += movingVec;

            charCtrl.Move(movingVec);

            VisionVal -= delVal * theArch_LD46_Time.delTime;
            AudioVal -= delVal * theArch_LD46_Time.delTime;
            FeelingVal -= delVal * theArch_LD46_Time.delTime;
            CompassVal -= delVal * theArch_LD46_Time.delTime;

            VisionVal = Mathf.Clamp01(VisionVal);
            AudioVal = Mathf.Clamp01(AudioVal);
            FeelingVal = Mathf.Clamp01(FeelingVal);
            CompassVal = Mathf.Clamp01(CompassVal);

            player_move(MoveForward);

        }
    }
}