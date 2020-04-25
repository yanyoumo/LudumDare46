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
        public GameObject EnemyRoot;
        public GameObject PickUpRoot;
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

        public AudioSource MenuBGM;
        public AudioSource PlayingBGM;

        public bool dataReady = false;

        public GoalMono GoalTransform;

        private float GroundStretch = 10.0f;

        public MainGamePlayUI gamePlayUI;

        private Vector3 GetRandomGroundPos()
        {
            return new Vector3((Random.value - 0.5f) * 2.0f * GroundStretch, 0.0f,
                (Random.value - 0.5f) * 2.0f * GroundStretch);
        }

        void Awake()
        {
            //这里目前是游戏最早点。但是可能会被反复触发。
            theArch_LD46_Time.Time = Time.timeSinceLevelLoad;
            theArch_LD46_Time.delTime = Time.deltaTime;

            SortedEnemies = new List<EnemyMono>();
            SortedPickUps = new List<PickUpMono>();

            PlayingBGM.Play();
        }

        // Start is called before the first frame update
        void Start()
        {
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

        // Update is called once per frame
        void Update()
        {
            theArch_LD46_Time.Time = Time.timeSinceLevelLoad;
            theArch_LD46_Time.delTime = Time.deltaTime;
            if (theArch_LD46_GameData.GameStatus == GameStatus.Starting)
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
            }
        }

        void LateUpdate()
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
                if (!dataReady)
                {
                    dataReady = true;
                }
            }
        }

        void OnDestroy()
        {
            //TODO 那两个得清理掉。
            SortedEnemies = null;
            SortedPickUps = null;
        }
    }
}