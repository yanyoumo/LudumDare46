using System;
using System.Collections;
using System.Collections.Generic;
using theArch_LD46.GlobalHelper;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using theArchitectTechPack.GlobalHelper;

namespace theArch_LD46
{
    public class PlayerMono : PlaceableBase
    {

        public Vector3 MoveForward { private set; get; }
        public Vector3 MoveLeft { private set; get; }

        public Transform Campos;

        public Dictionary<SenseType,float> playerSenseVals{ private set; get; }

        /*public float VisionVal { private set; get; }
        public float AudioVal { private set; get; }
        public float FeelingVal { private set; get; }
        public float CompassVal { private set; get; }*/

        private float Speed = 18.0f;
        private float DelVal = 0.07f;

        public bool IsMoving = false;

        public CharacterController charCtrl;

        public Transform meshRoot;

        //public Transform HintArrowPlane;

        public Transform BlurPlane;
        public bool Playing=false;
        public bool GameComplete=false;

        //private const float hintMax = 5;
        //private float hintCounter = 0;

        public AudioSource pickUpSFX;

        public VisualEffect vf;

        public void Toplay()
        {
            BlurPlane.gameObject.SetActive(false);
            Playing = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            MoveForward = Vector3.Normalize(new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z));
            MoveLeft = Vector3.Cross(MoveForward, Vector3.up);

            playerSenseVals=new Dictionary<SenseType, float>();


            foreach (var senseType in StaticData.SenseTypesEnumerable)
            {
                playerSenseVals.Add(senseType, DesignerStaticData.GetSenseInitialVal(senseType));
            }

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
                vf.SetVector3("PlayerPos", this.transform.position+new Vector3(0.0f, 0.5f, 0.0f));

                MoveForward = Vector3.Normalize(new Vector3(Camera.main.transform.forward.x, 0.0f,
                    Camera.main.transform.forward.z));
                MoveLeft = Vector3.Cross(MoveForward, Vector3.up);

                Vector2 inputVec = new Vector2(Input.GetAxis(GlobalHelper.StaticData.INPUT_AXIS_NAME_FORWARD),
                    Input.GetAxis(GlobalHelper.StaticData.INPUT_AXIS_NAME_LEFT));
                Vector3 movingVec = (Input.GetAxis(GlobalHelper.StaticData.INPUT_AXIS_NAME_FORWARD) * MoveForward) +
                                    (inputVec.y * MoveLeft);
                movingVec = Vector3.Normalize(movingVec) * Speed * theArch_LD46_Time.delTime;

                this.transform.Rotate(0, -Input.GetAxis(GlobalHelper.StaticData.INPUT_AXIS_NAME_LOOK_LEFT), 0);
                charCtrl.Move(movingVec);

                IsMoving = Input.anyKey;

                vf.SetFloat("SpawnRate", IsMoving ? 320.0f : 0.0f);

                foreach (var senseType in StaticData.SenseTypesEnumerable)
                {
                    playerSenseVals.TryGetValue(senseType, out float val);
                    val-= DelVal * theArch_LD46_Time.delTime;
                    val = Mathf.Clamp01(val);
                    playerSenseVals[senseType] = val;
                }
            }
        }

        public float GetValBySenseType(SenseType senseType)
        {
            Debug.Assert(playerSenseVals.TryGetValue(senseType, out float val));
            return val;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<EnemyMono>())
            {
                SceneManager.LoadScene(StaticData.SCENE_ID_GAMEPLAY, LoadSceneMode.Single);
                BlurPlane.gameObject.SetActive(false);
                Playing = true;
            }
            else if (other.gameObject.GetComponent<GoalMono>())
            {
                BlurPlane.gameObject.SetActive(true);
                Playing = false;
                GameComplete = true;
            }
            else if (other.gameObject.GetComponent<PickUpMono>())
            {
                pickUpSFX.Play();
                PickUpMono pickUpMono = other.gameObject.GetComponent<PickUpMono>();
                Debug.Log("Player got"+pickUpMono.senseType+"PickUp");
                playerSenseVals[pickUpMono.senseType] += pickUpMono.val;
                pickUpMono.pendingDead = true;
            }
        }
    }
}