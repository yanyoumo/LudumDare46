using System.Collections;
using System.Collections.Generic;
using theArch_LD46.GlobalHelper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace theArch_LD46
{
    public class PlayerScript : MonoBehaviour
    {

        public Vector3 MoveForward { private set; get; }
        public Vector3 MoveLeft { private set; get; }

        public Transform Campos;

        public float VisionVal;// { private set; get; }
        public float AudioVal { private set; get; }
        public float FeelingVal { private set; get; }
        public float CompassVal { private set; get; }

        public float speed = 0.75f;
        public float delVal = 0.1f;

        public bool IsMoving = false;

        public CharacterController charCtrl;

        public Transform meshRoot;

        // Start is called before the first frame update
        void Start()
        {
            MoveForward = Vector3.Normalize(new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z));
            MoveLeft = Vector3.Cross(MoveForward, Vector3.up);

            VisionVal = 0.75f;
            AudioVal = 0.0f;
            FeelingVal = 0.0f;
            CompassVal = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 inputVec=new Vector2(Input.GetAxis(GlobalHelper.StaticName.INPUT_AXIS_NAME_FORWARD), Input.GetAxis(GlobalHelper.StaticName.INPUT_AXIS_NAME_LEFR));
            Vector3 movingVec = (inputVec.x * MoveForward) + (inputVec.y * MoveLeft);
            movingVec = Vector3.Normalize(movingVec) * speed * theArch_LD46_Time.delTime;

            charCtrl.Move(movingVec);

            IsMoving = Input.anyKey;

            if (IsMoving)
            {
                VisionVal -= delVal * theArch_LD46_Time.delTime;
                AudioVal -= delVal * theArch_LD46_Time.delTime;
                FeelingVal -= delVal * theArch_LD46_Time.delTime;
                CompassVal -= delVal * theArch_LD46_Time.delTime;

                VisionVal = Mathf.Clamp01(VisionVal);
                AudioVal = Mathf.Clamp01(AudioVal);
                FeelingVal = Mathf.Clamp01(FeelingVal);
                CompassVal = Mathf.Clamp01(CompassVal);

                meshRoot.transform.Rotate(Quaternion.Euler(10.0f, 0.0f, 0.0f).eulerAngles);
                //meshRoot.rotation= meshRoot.rotation.SetFromToRotation(Quaternion.Euler(0.0f, 0.1f, 0.0f));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Enemy>())
            {
                SceneManager.LoadScene(StaticName.SCENE_ID_GAMEPLAY, LoadSceneMode.Single);
            }
            else if (other.gameObject.GetComponent<PickUpScript>())
            {
                PickUpScript pickUpScript = other.gameObject.GetComponent<PickUpScript>();
                Debug.Log("Player got"+pickUpScript.senseType+"PickUp");
                switch (pickUpScript.senseType)
                {
                    case SenseType.Vision:
                        VisionVal += pickUpScript.val;
                        break;
                    case SenseType.Audio:
                        AudioVal += pickUpScript.val;
                        break;
                    case SenseType.Feeling:
                        FeelingVal += pickUpScript.val;
                        break;
                    case SenseType.Compass:
                        CompassVal += pickUpScript.val;
                        break;
                }
                pickUpScript.pendingDead = true;
            }
        }
    }
}