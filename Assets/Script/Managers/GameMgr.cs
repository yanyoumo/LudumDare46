using System.Collections.Generic;
using System.Linq;
using theArchitectTechPack.GlobalHelper;
using theArch_LD46.GlobalHelper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace theArch_LD46
{
    public class GameMgr : MonoBehaviour
    {
        //TODO play complete 这些东西要换成GameState
        //TODO pickUp的清理要换成GameMgr里面统一管理。
        //TODO UI不能和player直接交互。
        //TODO Scene的加载趁此换成Additive的。
        public const string PlayerSpawnName = "PlayerSpawn";
        public const string EnemyRootName = "EnemyRoot";
        public const string PickUpRootName = "PickUpRoot";
        public const string GoalName = "Goal";

        private Transform PlayerSpawn;
        private GameObject EnemyRoot;
        private GameObject PickUpRoot;
        public GameObject EnemyTemplate;

        private List<EnemyMono> SortedEnemies { set; get; }
        public List<PickUpMono> SortedPickUps { set; get; }

        public SenseDisplayingData senseDisplayingData { private set; get; }
        public float playerGoalAngle { private set; get; }
        private EnemyMono[] FilteredSortedEnemies;
        private PickUpMono[] FilteredSortedPickUps;
        public Vector3[] FilteredSortedEnemiesPos { private set; get; }
        public HearingSenseData[] FilteredSortedPickUpsData { private set; get; }
        public PlayerMono player;
        //public AudioSource MenuBGM;
        //public AudioSource PlayingBGM;
        //public bool dataReady = false;
        private GoalMono GoalTransform;
        private float GroundStretch = 10.0f;
        public MainGamePlayUI gamePlayUI;

        public bool levelSwitching { private set; get; }

        private int CurrentLevelID;
        private int PendingLevelID;

        void Awake()
        {
            theArch_LD46_GameData.GameStatus = GameStatus.Playing; //先强制跳过开始界面。
            theArch_LD46_Time.Time = Time.timeSinceLevelLoad;
            theArch_LD46_Time.delTime = Time.deltaTime;

            SortedEnemies = new List<EnemyMono>();
            SortedPickUps = new List<PickUpMono>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnLoaded;

            levelSwitching = true;
            //PlayingBGM.Play();
        }

        void Start()
        {
            CurrentLevelID = StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_LV0;
            PendingLevelID = StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_LV0;
            loadPendingLevel();
        }

        private void UpdatePlayerSenseData()
        {
            float curtainAlpha = DesignerStaticData.GetVisionCurtainAlpha(player.GetValBySenseType(SenseType.Vision));
            float curtainRadius = DesignerStaticData.GetVisionCurtainRadius(player.GetValBySenseType(SenseType.Vision));
            int hearingCount = DesignerStaticData.GetHearingCount(player.GetValBySenseType(SenseType.Audio));
            float hearingAlpha = DesignerStaticData.GetHearingAlpha(player.GetValBySenseType(SenseType.Audio));
            int feelingCount = DesignerStaticData.GetFeelingCount(player.GetValBySenseType(SenseType.Feeling));
            float feelingAlpha = DesignerStaticData.GetFeelingAlpha(player.GetValBySenseType(SenseType.Feeling));
            float compassAlpha = DesignerStaticData.GetCompassAlpha(player.GetValBySenseType(SenseType.Compass));
            senseDisplayingData = new SenseDisplayingData(curtainAlpha, curtainRadius, hearingAlpha, hearingCount,
                feelingAlpha, feelingCount, compassAlpha);
        }

        void ResetPlayer()
        {
            player.transform.position = PlayerSpawn.transform.position;
            player.transform.rotation = PlayerSpawn.transform.rotation;
            player.ResetResetableData(this);
        }

        int GetNextLevel()
        {
            //TODO 类似FSM的机制切换关卡。
            return StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_LV1;
        }

        void LoadNextLevel()
        {
            PendingLevelID = GetNextLevel();
            UnLoadLevel();
        }

        void ReloadCurrentLevel()
        {
            PendingLevelID = CurrentLevelID;
            UnLoadLevel();
        }

        private void UnLoadLevel()
        {
            levelSwitching = true;
            SceneManager.UnloadSceneAsync(CurrentLevelID);
        }

        void loadPendingLevel()
        {
            SceneManager.LoadScene(PendingLevelID, LoadSceneMode.Additive);
        }

        void UpdateLevelContents()
        {
            ResetPlayer();

            SortedEnemies = new List<EnemyMono>();
            SortedPickUps = new List<PickUpMono>();

            EnemyMono[] enemiesMono = EnemyRoot.GetComponentsInChildren<EnemyMono>();
            foreach (var enemy in enemiesMono)
            {
                SortedEnemies.Add(enemy);
            }

            PickUpMono[] pickUps = PickUpRoot.GetComponentsInChildren<PickUpMono>();
            foreach (var pickup in pickUps)
            {
                pickup.gameMgr = this;
                SortedPickUps.Add(pickup);
            }
        }

        void UpdateLevelReference()
        {
            GameObject[] gos = FindObjectsOfType<GameObject>();
            Transform[] trans = FindObjectsOfType<Transform>();
            GoalTransform = FindObjectOfType<GoalMono>();
            foreach (var go in gos)
            {
                if (go.name == EnemyRootName)
                {
                    EnemyRoot = go;
                }
                else if (go.name == PickUpRootName)
                {
                    PickUpRoot = go;
                }
            }

            foreach (var tran in trans)
            {
                if (tran.name == PlayerSpawnName)
                {
                    PlayerSpawn = tran;
                }
            }

            Debug.Assert(GoalTransform, "Can't Find goal");
            Debug.Assert(EnemyRoot, "Can't Find Enemies");
            Debug.Assert(PickUpRoot, "Can't Find Pickups");
            Debug.Assert(PlayerSpawn, "Can't Find SpwanPt");
        }

        void OnSceneUnLoaded(Scene scene)
        {
            if (scene.name.ToLower().Contains("_level"))
            {
                loadPendingLevel();
            }
        }

        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            Debug.Log(mode);

            if (scene.name.ToLower().Contains("_level"))
            {
                CurrentLevelID = PendingLevelID;
                UpdateLevelReference();
                UpdateLevelContents();
                levelSwitching = false;
            }
        }

        void Update()
        {
            theArch_LD46_Time.Time = Time.timeSinceLevelLoad;
            theArch_LD46_Time.delTime = Time.deltaTime;


            if (!levelSwitching)
            {
                //Loading的逻辑会在loading中再跑了一圈儿，所以放进来。
                if (player.PlayerDead)
                {
                    ReloadCurrentLevel();
                }

                if (player.Won)
                {
                    LoadNextLevel();
                }

                //先不管，Additive的时候这些都要改。
                /*if (theArch_LD46_GameData.GameStatus == GameStatus.Starting)
                {
                    if (dataReady)
                    {
                        dataReady = false;
                    }
                    if (Input.GetButtonDown(StaticData.INPUT_BUTTON_NAME_GAME_START))
                    {
                        theArch_LD46_GameData.GameStatus = GameStatus.Playing;
                        SceneManager.LoadScene(StaticData.SCENE_ID_GAMEPLAY, LoadSceneMode.Single);
                    }
                }
                if (theArch_LD46_GameData.GameStatus == GameStatus.Ended)
                {
                    if (dataReady)
                    {
                        dataReady = false;
                    }
                    if (Input.GetButtonDown(StaticData.INPUT_BUTTON_NAME_GAME_START))
                    {
                        theArch_LD46_GameData.GameStatus = GameStatus.Starting;
                        SceneManager.LoadScene(StaticData.SCENE_ID_GAMEPLAY, LoadSceneMode.Single);
                    }
                }*/
            }
        }

        void LateUpdate()
        {
            if (!levelSwitching)
            {
                if (theArch_LD46_GameData.GameStatus == GameStatus.Playing)
                {
                    //玩家在Update里面更新参数，所以在这里UpdateData比较靠谱；
                    UpdatePlayerSenseData();
                    //Enemy 现在是无敌的，所以Safe。
                    SortedEnemies = SortedEnemies
                        .OrderBy(enemy => (enemy.transform.position - player.transform.position).magnitude).ToList();
                    SortedPickUps = SortedPickUps
                        .OrderBy(pickUp => (pickUp.transform.position - player.transform.position).magnitude).ToList();

                    FilteredSortedEnemies =
                        Utils.SmarterTruncateArray(senseDisplayingData.HearingCount, SortedEnemies).ToArray();
                    FilteredSortedPickUps =
                        Utils.SmarterTruncateArray(senseDisplayingData.FeelingCount, SortedPickUps).ToArray();

                    FilteredSortedEnemiesPos = new Vector3[FilteredSortedEnemies.Length];
                    FilteredSortedPickUpsData = new HearingSenseData[FilteredSortedPickUps.Length];

                    for (var i = 0; i < FilteredSortedEnemies.Length; i++)
                    {
                        FilteredSortedEnemiesPos[i] = FilteredSortedEnemies[i].transform.position;
                    }

                    for (var i = 0; i < FilteredSortedPickUps.Length; i++)
                    {
                        Vector3 pickDir = FilteredSortedPickUps[i].transform.position - player.transform.position;
                        float angle = Utils.GetSignedAngle(player.MoveForward, player.MoveLeft, pickDir);
                        FilteredSortedPickUpsData[i] = new HearingSenseData(FilteredSortedPickUps[i].senseType, angle);
                    }

                    Vector3 dirGoal = GoalTransform.transform.position - player.transform.position;
                    playerGoalAngle = Utils.GetSignedAngle(player.MoveForward, player.MoveLeft, dirGoal);
                }
            }
        }
    }
}