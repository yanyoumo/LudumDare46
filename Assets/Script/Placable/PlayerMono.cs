using System;
using System.Collections;
using System.Collections.Generic;
using theArch_LD46.GlobalHelper;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using theArchitectTechPack.GlobalHelper;
using Object = System.Object;

namespace theArch_LD46
{
    public class PlayerMono : PlaceableBase
    {
        public GameMgr gameMgr;

        public Vector3 MoveForward { private set; get; }
        public Vector3 MoveLeft { private set; get; }
        public bool IsMoving { private set; get; }

        public Dictionary<SenseType,float> PlayerSenseValues{ private set; get; }//现在整个数据结构都要改。
        //public float VisionVal{ private set; get; }

        private float _speed = 18.0f;
        //private float _delVal = 0.1f;

        public Transform Campos { private set; get; }
        public Transform MeshRoot { private set; get; }
        public Transform BlurPlane;
        public bool PlayerDead{ set; get; }
        private bool PlayerHit = false;
        public bool PlayerSlowMo{ set; get; }
        public bool Won { get; set; }

        public VisionBar visionBar;

        private CharacterController _charCtrl;
        private AudioSource _pickUpSfx;
        private VisualEffect _playerMovingEffect;
        public VisualEffect hitEffect;

        private string _comPosName = "CamPos";
        private string _playerMeshName = "PlayerMesh";
        private string _blurPlaneName = "BluringMask";
        private string _playerMovingEffectName = "PlayerMoving";

        private string _playerMovingEffectPropertyName_PlayerPos = "PlayerPos";
        private string _playerMovingEffectPropertyName_SpawnRate = "SpawnRate";
        private readonly Vector3 _playerMovingEffectPropertyVal_PlayerPosOffset = new Vector3(0.0f, 0.5f, 0.0f);
        private readonly float _playerMovingEffectPropertyVal_MovingSpawnRate = 320.0f;

        private Vector3 movingVec;
        private Camera MainCam;

        public CameraJerk camJerk;

        //TODO 意外地相当相当靠谱，可以把材质的颜色再写一下，还有就是这个也到不能解决看到旁边的地形的问题，但是这个表现比UI的好太多了。
        public Transform curtainMesh;

        public void ResetResetableData(MonoBehaviour invoker)
        {
            Debug.Assert(invoker==this|| invoker.GetComponent<GameMgr>(),"Only player and GameMgr could reset player.");
            MoveForward = Vector3.Normalize(new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z));
            MoveLeft = Vector3.Cross(MoveForward, Vector3.up);

            PlayerSenseValues = new Dictionary<SenseType, float>();


            foreach (var senseType in StaticData.SenseTypesEnumerable)
            {
                PlayerSenseValues.Add(senseType, DesignerStaticData.GetSenseInitialVal(senseType));
            }

            movingVec = Vector3.zero;
            PlayerHit = false;
            Won = false;
            PlayerDead = false;
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

            BlurPlane?.gameObject.SetActive(true);
            curtainMesh?.gameObject.SetActive(false);

            //camJerk = Camera.main.gameObject.GetComponent<CameraJerk>();

            ResetResetableData(this);
        }

        public void InitPlayingUI()
        {
            Debug.Assert(theArch_LD46_GameData.GameStatus==GameStatus.Playing);
            BlurPlane?.gameObject.SetActive(false);
            curtainMesh?.gameObject.SetActive(true);
        }


        public void InitEndginingUI()
        {
            Debug.Assert(theArch_LD46_GameData.GameStatus == GameStatus.Ended);
            BlurPlane?.gameObject.SetActive(true);
            curtainMesh?.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            MainCam = Camera.main;
        }

        void UpdateBasicData()
        {
            MoveForward = Vector3.Normalize(new Vector3(MainCam.transform.forward.x, 0.0f, MainCam.transform.forward.z));
            MoveLeft = Vector3.Cross(MoveForward, Vector3.up);
        }

        void UpdateMovingInput()
        {
            Vector2 inputVec = new Vector2(Input.GetAxis(StaticData.INPUT_AXIS_NAME_FORWARD),Input.GetAxis(StaticData.INPUT_AXIS_NAME_LEFT));
            movingVec = (Input.GetAxis(StaticData.INPUT_AXIS_NAME_FORWARD) * MoveForward) + (inputVec.y * MoveLeft);
            movingVec = Vector3.Normalize(movingVec);
        }

        void ActualMoving()
        {
            CollisionFlags res = _charCtrl.Move(movingVec * _speed * theArch_LD46_Time.delTime);
            IsMoving = (res == CollisionFlags.None);
            movingVec =Vector3.zero;
        }

        void UpdateRotatingInput()
        {
            //TODO 这个也要优化，但是优先级不高了。
            transform.Rotate(0,-Input.GetAxis(StaticData.INPUT_AXIS_NAME_LOOK_LEFT) * 100.0f * theArch_LD46_Time.delTime, 0);
        }

        [Obsolete]
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
                if (senseType == SenseType.Vision)
                {
                    if (val >= DesignerStaticData.ENEMY_HITTING_POWER)
                    {
                        val -= DesignerStaticData.PLAYER_DIMINISHING_VAL * theArch_LD46_Time.delTime;
                    }
                }
                else
                {                
                    val -= DesignerStaticData.PLAYER_DIMINISHING_VAL * theArch_LD46_Time.delTime;
                }
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
            if (theArch_LD46_GameData.GameStatus == GameStatus.Playing)
            {
                UpdateBasicData();
                UpdateMovingInput();
                UpdateRotatingInput();
                UpdateSenseVal();

                //TODO 一次PlayerHit但是要完成两件事儿，这个时序要注意！
                //TODO 果然有问题。
                if (PlayerHit)
                {
                    float visionval=GetValBySenseType(SenseType.Vision);
                    visionval -= DesignerStaticData.ENEMY_HITTING_POWER;
                    if (visionval<=0)
                    {
                        PlayerDead = true;
                    }
                    else
                    {
                        //TODO 这里是一个关键的设计点，就是玩家本来还有一次hit一上的血，但是被Hit一下后变成一次血线以下了。
                        //TODO 这里怎么设计要再仔细考虑，现在事相当于多给了一发。就是再一次血线以上时，会卡在一次血线上。
                        visionval = Mathf.Clamp(visionval, DesignerStaticData.ENEMY_HITTING_POWER - 0.005f, 1.0f);
                        PlayerSenseValues[SenseType.Vision] = visionval;
                        visionBar.HitEffect(visionval);
                    }

                    PlayerHit = false;
                }
            }
        }

        void LateUpdate()
        {
            if (theArch_LD46_GameData.GameStatus == GameStatus.Playing)
            {
                ActualMoving();

                //TODO 哦哦，变成这里了，还得想想怎么弄。
                float curtainScale = gameMgr.senseDisplayingData.VisionRadius;
                curtainMesh.transform.localScale = new Vector3(curtainScale, 1.0f, curtainScale);

                UpdateVisualEffect();
            }
        }

        public float GetValBySenseType(SenseType senseType)
        {
            float val = 0.0f;
            PlayerSenseValues.TryGetValue(senseType, out val);
            return val;
        }

        private void OnTriggerEnter(Collider other)
        {
            //TODO 这里应该只弄flag，各种异步的事件去GameMgr里面去同步执行。
            if (other.gameObject.GetComponent<EnemyMono>())
            {
                //撞一下后敌人必须死………………
                Vector3 direction = other.transform.position - transform.position;
                direction = direction.normalized * 0.3f;
                EnemyMono enemy = other.gameObject.GetComponent<EnemyMono>();
                if (!enemy.pendingDead)
                {
                    PlayerHit = true;
                    PlayerSlowMo = true;
                    camJerk.DoJerk(-direction);
                    hitEffect.SetFloat("DamageAngle", Utils.GetSignedAngle(MoveForward, MoveLeft, direction));
                    hitEffect.Play();
                }
                enemy?.HintByPlayer();
            }
            else if (other.gameObject.GetComponent<GoalMono>())
            {
                Won = true;
            }
            else if (other.gameObject.GetComponent<PickUpMono>())
            {
                _pickUpSfx.Play();
                PickUpMono pickUpMono = other.gameObject.GetComponent<PickUpMono>();
#if UNITY_EDITOR
                Debug.Log("Player got"+pickUpMono.senseType+"PickUp");
#endif
                PlayerSenseValues[pickUpMono.senseType] += pickUpMono.val;
                pickUpMono.pendingDead = true;
            }
        }
    }
}