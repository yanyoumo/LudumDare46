using System;
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
        public const string WallRootName = "WallRoot";
        public const string GoalName = "Goal";

        private Transform PlayerSpawn;
        private GameObject EnemyRoot;
        private GameObject PickUpRoot;
        private GameObject WallRoot;
        public GameObject EnemyTemplate;

        public List<EnemyMono> SortedEnemies { set; get; }
        public List<PickUpMono> SortedPickUps { set; get; }
        public Wallmono[] Walls { set; get; }

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

        private TimeMgr timeMgr;

        void Awake()
        {
            if (timeMgr == null)
            {
                timeMgr=new TimeMgr();
                timeMgr.ResetTime();
            }

            SortedEnemies = new List<EnemyMono>();
            SortedPickUps = new List<PickUpMono>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnLoaded;

            levelSwitching = true;
            //PlayingBGM.Play();
        }

        void Start()
        {
            StartGame();
        }

        private void UpdatePlayerSenseData()
        {
            float curtainAlpha = DesignerStaticData.GetVisionCurtainAlpha(player.GetValBySenseType(BasicSenseType.Vision));
            float curtainRadius = DesignerStaticData.GetVisionCurtainRadius(player.GetValBySenseType(BasicSenseType.Vision));
            int hearingCount = DesignerStaticData.GetHearingCount(player.GetValBySenseType(BasicSenseType.Audio));
            float hearingAlpha = DesignerStaticData.GetHearingAlpha(player.GetValBySenseType(BasicSenseType.Audio));
            int feelingCount = DesignerStaticData.GetFeelingCount(player.GetValBySenseType(BasicSenseType.Feeling));
            float feelingAlpha = DesignerStaticData.GetFeelingAlpha(player.GetValBySenseType(BasicSenseType.Feeling));
            float compassAlpha = DesignerStaticData.GetCompassAlpha(player.GetValBySenseType(BasicSenseType.Compass));
            senseDisplayingData = new SenseDisplayingData(curtainAlpha, curtainRadius, hearingAlpha, hearingCount,
                feelingAlpha, feelingCount, compassAlpha);
        }

        #region SceneMgrRelated

        void ResetPlayer()
        {
            player.transform.position = PlayerSpawn.transform.position;
            player.transform.rotation = PlayerSpawn.transform.rotation;
            player.ResetResetableData(this);
        }

        void UnloadLevelWAsyncLoadPending(int next)
        {
            PendingLevelID = next;
            UnLoadLevel();
        }

        void ReloadLevel()
        {
            UnloadLevelWAsyncLoadPending(CurrentLevelID);
        }

        private void UnLoadLevel()
        {
            levelSwitching = true;
            SceneManager.UnloadSceneAsync(CurrentLevelID);
        }

        private void loadPendingLevel()
        {
            SceneManager.LoadScene(PendingLevelID, LoadSceneMode.Additive);
        }

        void UpdateLevelContents()
        {
            ResetPlayer();

            SortedEnemies = new List<EnemyMono>();
            SortedPickUps = new List<PickUpMono>();

            EnemyMono[] enemiesMono = FindObjectsOfType<EnemyMono>();
            foreach (var enemy in enemiesMono)
            {
                enemy.gameMgr = this;
                SortedEnemies.Add(enemy);
            }

            // 这里使用GetComponentsInChildren和sceneLoaded一起用时序似乎有问题。
            // 在这种loading时，还是都用FindObjectsOfType吧。
            PickUpMono[] pickUps = FindObjectsOfType<PickUpMono>();
            foreach (var pickup in pickUps)
            {
                pickup.gameMgr = this;
                SortedPickUps.Add(pickup);
            }

            Walls = FindObjectsOfType<Wallmono>().ToArray();
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
                else if (go.name == WallRootName)
                {
                    WallRoot = go;
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
            Debug.Assert(WallRoot, "Can't Find WallRoot");
        }

        void OnSceneUnLoaded(Scene scene)
        {
            if (scene.name.ToLower().Contains("_level"))
            {
                loadPendingLevel();
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
#if UNITY_EDITOR
            Debug.Log("OnSceneLoaded: " + scene.name);
            Debug.Log(mode);
#endif

            if (scene.buildIndex == PendingLevelID)
            {
                if (scene.buildIndex != StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_CORE)
                {
                    CurrentLevelID = PendingLevelID;
                    UpdateLevelReference();
                    UpdateLevelContents();
                    levelSwitching = false;
                }
            }
        }
        
        void StartPlay()
        {
            theArch_LD46_GameData.GameStatus = GameStatus.Playing;
            gamePlayUI.InitUIForPlay();
            player.InitPlayingUI();
            UnloadLevelWAsyncLoadPending(StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_LV0);
        }

        void StartGame()
        {
            levelSwitching = true;
            theArch_LD46_GameData.GameStatus = GameStatus.Starting;
            CurrentLevelID = DesignerStaticData.GetStartingBG();
            PendingLevelID = DesignerStaticData.GetStartingBG();
            timeMgr.ResetTime();
            //gamePlayUI.InitUIForStarting();
            loadPendingLevel();
        }

        void RestartGame()
        {
            levelSwitching = true;
            theArch_LD46_GameData.GameStatus = GameStatus.Starting;
            CurrentLevelID = DesignerStaticData.GetStartingBG();
            PendingLevelID = DesignerStaticData.GetStartingBG();
            timeMgr.ResetTime();
            //gamePlayUI.InitUIForEnding();
            UnloadLevelWAsyncLoadPending(CurrentLevelID);
        }

        #endregion

        void Update()
        {
            timeMgr.TimeUpdate();

            if (!levelSwitching)
            {
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    //timeMgr.TimeStretch();
                }
#endif
                switch (theArch_LD46_GameData.GameStatus)
                {
                    case GameStatus.Starting:
                        //Waiting For Enter
                        if (Input.GetButtonDown(StaticData.INPUT_BUTTON_NAME_GAME_START))
                        {
                            StartPlay();
                        }
                        break;
                    case GameStatus.Playing:
                        //Loading的逻辑会在loading中再跑了一圈儿，所以放进来。
                        if (player.PlayerSlowMo)
                        {
                            if (!timeMgr.slowMotion)
                            {
                                //timeMgr.TimeStretch(DesignerStaticData.SLOWMOTION_DURATION);
                            }

                            player.PlayerSlowMo = false;
                        }

                        if (player.PlayerDead)
                        {
                            ReloadLevel();
                        }

                        if (player.Won)
                        {
                            //theArch_LD46_GameData.GameStatus = GameStatus.Ended;
                            int nextLevel = DesignerStaticData.GetNextLevel(CurrentLevelID);
                            if (nextLevel!=-1)
                            {                               
                                UnloadLevelWAsyncLoadPending(nextLevel);
                            }
                            else
                            {
                                theArch_LD46_GameData.GameStatus = GameStatus.Ended;
                            }
                        }
                        break;
                    case GameStatus.Ended:
                        if (Input.GetButtonDown(StaticData.INPUT_BUTTON_NAME_GAME_START))
                        {
                            RestartGame();
                            gamePlayUI.InitUIForRestart();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
                        FilteredSortedPickUpsData[i] = new HearingSenseData(FilteredSortedPickUps[i].BasicSenseType, angle);
                    }

                    Vector3 dirGoal = GoalTransform.transform.position - player.transform.position;
                    playerGoalAngle = Utils.GetSignedAngle(player.MoveForward, player.MoveLeft, dirGoal);
                }
            }
        }
    }
}