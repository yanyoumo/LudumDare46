using System.Collections;
using System.Collections.Generic;
using theArch_LD46;
using theArch_LD46.GlobalHelper;
using UnityEngine;

namespace theArch_LD46
{
    public class GameMgr : MonoBehaviour
    {
        public GameObject EnemyRoot;
        public GameObject PickUpRoot;
        public GameObject EnemyTemplate;
        public List<EnemyMono> Enemies { private set; get; }
        public List<PickUpMono> PickUps { private set; get; }

        public AudioSource MenuBGM;
        public AudioSource PlayingBGM;

        private float GroundStretch = 10.0f;

        private Vector3 GetRandomGroundPos()
        {
            return new Vector3((Random.value - 0.5f) * 2.0f * GroundStretch, 0.0f,
                (Random.value - 0.5f) * 2.0f * GroundStretch);
        }

        void Awake()
        {
            theArch_LD46.theArch_LD46_Time.Time = Time.timeSinceLevelLoad;
            theArch_LD46.theArch_LD46_Time.delTime = Time.deltaTime;

            Enemies = new List<EnemyMono>();
            PickUps = new List<PickUpMono>();

            PlayingBGM.Play();
        }

        // Start is called before the first frame update
        void Start()
        {
            EnemyMono[] enemiesMono = EnemyRoot.GetComponentsInChildren<EnemyMono>();
            foreach (var enemy in enemiesMono)
            {
                Enemies.Add(enemy);
            }
            PickUpMono[] pickUps = PickUpRoot.GetComponentsInChildren<PickUpMono>();
            foreach (var pickup in pickUps)
            {
                pickup.gameMgr = this;
                PickUps.Add(pickup);
            }
        }

        // Update is called once per frame
        void Update()
        {
            theArch_LD46.theArch_LD46_Time.Time = Time.timeSinceLevelLoad;
            theArch_LD46.theArch_LD46_Time.delTime = Time.deltaTime;
        }

        void OnDestroy()
        {
            //TODO 那两个得清理掉。
            Enemies = null;
            PickUps = null;
        }
    }
}