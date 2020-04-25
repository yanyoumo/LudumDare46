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
        public bool IsMoving { private set; get; }
        public bool Playing { private set; get; }
        public bool GameComplete { private set; get; }

        public Dictionary<SenseType,float> PlayerSenseValues{ private set; get; }

        private float _speed = 18.0f;
        private float _delVal = 0.1f;

        public Transform Campos { private set; get; }
        public Transform MeshRoot { private set; get; }
        public Transform BlurPlane { private set; get; }
        private CharacterController _charCtrl;
        private AudioSource _pickUpSfx;
        private VisualEffect _playerMovingEffect;

        private string _comPosName = "CamPos";
        private string _playerMeshName = "PlayerMesh";
        private string _blurPlaneName = "BluringMask";
        private string _playerMovingEffectName = "PlayerMoving";

        private string _playerMovingEffectPropertyName_PlayerPos = "PlayerPos";
        private string _playerMovingEffectPropertyName_SpawnRate = "SpawnRate";
        private readonly Vector3 _playerMovingEffectPropertyVal_PlayerPosOffset = new Vector3(0.0f, 0.5f, 0.0f);
        private readonly float _playerMovingEffectPropertyVal_MovingSpawnRate = 320.0f;

        private Camera MainCam;

        //TODO 意外地相当相当靠谱，可以把材质的颜色在写一下，还有就是这个也到不能解决看到旁边的地形的问题，但是这个表现比UI的好太多了。
        public Transform curtainMesh;

        public void ToPlay()
        {
            BlurPlane.gameObject.SetActive(false);
            Playing = true;
        }

        void Awake()
        {
            _charCtrl = GetComponent<CharacterController>();
            _pickUpSfx = GetComponent<AudioSource>();
            var tmpT = GetComponentsInChildren<Transform>();
            var tmpV = GetComponentsInChildren<VisualEffect>();
            foreach (var trans in tmpT)
            {
                if (trans.gameObject.name==_comPosName)
                {
                    Campos = trans;
                }
                else if (trans.gameObject.name == _playerMeshName)
                {
                    MeshRoot = trans;
                }
                else if (trans.gameObject.name == _blurPlaneName)
                {
                    BlurPlane = trans;
                }
            }

            foreach (var visualEffect in tmpV)
            {
                if (visualEffect.gameObject.name == _playerMovingEffectName)
                {
                    _playerMovingEffect = visualEffect;
                }
            }

            MoveForward = Vector3.Normalize(new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z));
            MoveLeft = Vector3.Cross(MoveForward, Vector3.up);

            PlayerSenseValues = new Dictionary<SenseType, float>();


            foreach (var senseType in StaticData.SenseTypesEnumerable)
            {
                PlayerSenseValues.Add(senseType, DesignerStaticData.GetSenseInitialVal(senseType));
            }

        }

        // Start is called before the first frame update
        void Start()
        {
            MainCam = Camera.main;
            GameComplete = false;
            BlurPlane.gameObject.SetActive(true);
            if (!theArch_LD46_GameData.firstTimeGame)
            {               
                ToPlay();
            }
        }

        void UpdateBasicData()
        {
            MoveForward = Vector3.Normalize(new Vector3(MainCam.transform.forward.x, 0.0f, MainCam.transform.forward.z));
            MoveLeft = Vector3.Cross(MoveForward, Vector3.up);
        }

        void UpdateMovingInput()
        {
            Vector2 inputVec = new Vector2(Input.GetAxis(StaticData.INPUT_AXIS_NAME_FORWARD),Input.GetAxis(StaticData.INPUT_AXIS_NAME_LEFT));
            Vector3 movingVec = (Input.GetAxis(StaticData.INPUT_AXIS_NAME_FORWARD) * MoveForward) + (inputVec.y * MoveLeft);
            movingVec = Vector3.Normalize(movingVec) * _speed * theArch_LD46_Time.delTime;
            _charCtrl.Move(movingVec);
        }

        void UpdateRotatingInput()
        {
            transform.Rotate(0, -Input.GetAxis(StaticData.INPUT_AXIS_NAME_LOOK_LEFT), 0);
        }

        void UpdateGetIsMoving()
        {          
            bool moving = !Utils.MathFloatApproxZero(Input.GetAxis(StaticData.INPUT_AXIS_NAME_FORWARD)) || !Utils.MathFloatApproxZero(Input.GetAxis(StaticData.INPUT_AXIS_NAME_LEFT));
            bool looking = !Utils.MathFloatApproxZero(Input.GetAxis(StaticData.INPUT_AXIS_NAME_LOOK_LEFT));
            IsMoving = moving || looking;
        }

        void UpdateSenseVal()
        {
            foreach (var senseType in StaticData.SenseTypesEnumerable)
            {
                PlayerSenseValues.TryGetValue(senseType, out float val);
                val -= _delVal * theArch_LD46_Time.delTime;
                val = Mathf.Clamp01(val);
                PlayerSenseValues[senseType] = val;
            }
        }

        void UpdateVisualEffect()
        {
            _playerMovingEffect.SetVector3(_playerMovingEffectPropertyName_PlayerPos, transform.position + _playerMovingEffectPropertyVal_PlayerPosOffset);
            _playerMovingEffect.SetFloat(_playerMovingEffectPropertyName_SpawnRate, IsMoving ? _playerMovingEffectPropertyVal_MovingSpawnRate : 0.0f);
        }

        // Update is called once per frame
        void Update()
        {
            if (Playing)
            {
                UpdateBasicData();
                UpdateMovingInput();
                UpdateRotatingInput();
                UpdateGetIsMoving();
                UpdateSenseVal();

                float curtainScale = Mathf.Lerp(1.0f, 7.5f, GetValBySenseType(SenseType.Vision));
                curtainMesh.transform.localScale = new Vector3(curtainScale, 1.0f, curtainScale);
            }
        }

        void LateUpdate()
        {
            UpdateVisualEffect();
        }

        public float GetValBySenseType(SenseType senseType)
        {
            float val = 0.0f;
            PlayerSenseValues.TryGetValue(senseType, out val);
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
                _pickUpSfx.Play();
                PickUpMono pickUpMono = other.gameObject.GetComponent<PickUpMono>();
                Debug.Log("Player got"+pickUpMono.senseType+"PickUp");
                PlayerSenseValues[pickUpMono.senseType] += pickUpMono.val;
                pickUpMono.pendingDead = true;
            }
        }
    }
}