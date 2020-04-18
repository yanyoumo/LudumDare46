using System.Collections;
using System.Collections.Generic;
using theArch_LD46;
using UnityEngine;

namespace theArch_LD46
{
    public class GameMgr : MonoBehaviour
    {
        public GameObject EnemyTemplate;
        public List<Enemy> Enemies { private set; get; }
        public List<PickUpScript> PickUps { private set; get; }

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

            Enemies = new List<Enemy>();
            PickUps = new List<PickUpScript>();
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject go = Instantiate(EnemyTemplate);
                Enemy enemy = go.GetComponent<Enemy>();
                enemy.SetPosition(GetRandomGroundPos());
                Enemies.Add(enemy);
            }
        }

        // Update is called once per frame
        void Update()
        {
            theArch_LD46.theArch_LD46_Time.Time = Time.timeSinceLevelLoad;
            theArch_LD46.theArch_LD46_Time.delTime = Time.deltaTime;

            //Camera.current.WorldToScreenPoint(Vector3.zero);
        }

        void OnDestroy()
        {
            //TODO 那两个得清理掉。
            Enemies = null;
            PickUps = null;
        }
    }
}