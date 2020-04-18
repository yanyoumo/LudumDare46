using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
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
        }
    }
}