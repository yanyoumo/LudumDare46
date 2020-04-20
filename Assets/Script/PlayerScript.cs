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
        public float delVal = 0.07f;

        public bool IsMoving = false;

        public CharacterController charCtrl;

        public Transform meshRoot;

        public Transform HintArrowPlane;

        public Transform BlurPlane;
        public bool Playing=false;
        public bool GameComplete=false;

        private const float hintMax = 5;
        private float hintCounter = 0;

        public AudioSource pickUpSFX;

        public void Toplay()
        {
            BlurPlane.gameObject.SetActive(false);
            Playing = true;
            hintCounter = 0;
        }

        // Start is called before the first frame update
        void Start()
        {
            MoveForward = Vector3.Normalize(new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z));
            MoveLeft = Vector3.Cross(MoveForward, Vector3.up);

            VisionVal = 0.5f;
            AudioVal = 0.0f;
            FeelingVal = 0.0f;
            CompassVal = 1.0f;

            BlurPlane.gameObject.SetActive(true);

            if (!theArch_LD46.theArch_LD46_Time.firstTimeGame)
            {               
                Toplay();
            }
            GameComplete = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Playing)
            {
                if(hintCounter<= hintMax)
                {
                    hintCounter += theArch_LD46_Time.delTime;
                }
                else
                {
                    HintArrowPlane.gameObject.SetActive(false);
                }

                MoveForward = Vector3.Normalize(new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z));
                MoveLeft = Vector3.Cross(MoveForward, Vector3.up);

                Vector2 inputVec = new Vector2(Input.GetAxis(GlobalHelper.StaticName.INPUT_AXIS_NAME_FORWARD),
                    Input.GetAxis(GlobalHelper.StaticName.INPUT_AXIS_NAME_LEFT));
                Vector3 movingVec = (Input.GetAxis(GlobalHelper.StaticName.INPUT_AXIS_NAME_FORWARD) * MoveForward) + (inputVec.y * MoveLeft);
                movingVec = Vector3.Normalize(movingVec) * speed * theArch_LD46_Time.delTime;

                this.transform.Rotate(0, -Input.GetAxis(GlobalHelper.StaticName.INPUT_AXIS_NAME_LOOK_LEFT), 0);
                charCtrl.Move(movingVec);

                IsMoving = Input.anyKey;

                /*if (IsMoving)
                {*/
                    VisionVal -= delVal * theArch_LD46_Time.delTime;
                    AudioVal -= delVal * theArch_LD46_Time.delTime;
                    FeelingVal -= delVal * theArch_LD46_Time.delTime;
                    CompassVal -= delVal * theArch_LD46_Time.delTime;

                    VisionVal = Mathf.Clamp01(VisionVal);
                    AudioVal = Mathf.Clamp01(AudioVal);
                    FeelingVal = Mathf.Clamp01(FeelingVal);
                    CompassVal = Mathf.Clamp01(CompassVal);

                    //meshRoot.transform.Rotate(Quaternion.Euler(3.0f, 0.0f, 0.0f).eulerAngles);
                    //meshRoot.rotation= meshRoot.rotation.SetFromToRotation(Quaternion.Euler(0.0f, 0.1f, 0.0f));
                //}
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Enemy>())
            {
                SceneManager.LoadScene(StaticName.SCENE_ID_GAMEPLAY, LoadSceneMode.Single);
                BlurPlane.gameObject.SetActive(false);
                Playing = true;
            }
            else if (other.gameObject.GetComponent<Goal>())
            {
                BlurPlane.gameObject.SetActive(true);
                Playing = false;
                GameComplete = true;
            }
            else if (other.gameObject.GetComponent<PickUpScript>())
            {
                pickUpSFX.Play();
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