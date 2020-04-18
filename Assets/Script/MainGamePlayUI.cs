using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace theArch_LD46
{
    public class MainGamePlayUI : MonoBehaviour
    {
        public GameObject EnemyIndTemplate;
        public List<GameObject> EnemyInds;

        public GameMgr gameMgr;
        //public Transform playerTransform;
        public PlayerScript player;

        public RectTransform VisionBlocker;

        public float VisionStrength = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Assert(gameMgr);
            EnemyInds = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {
            if (gameMgr.Enemies != null)
            {
                if (gameMgr.Enemies.Count> EnemyInds.Count)
                {
                    do
                    {
                        EnemyInds.Add(Instantiate(EnemyIndTemplate, transform));
                    } while (gameMgr.Enemies.Count > EnemyInds.Count);
                }
                else
                {
                    for (int i = 0; i < EnemyInds.Count; i++)
                    {
                        EnemyInds[i].gameObject.SetActive(i < gameMgr.Enemies.Count);
                    }
                }

                for (int i = 0; i < EnemyInds.Count; i++)
                {
                    EnemyInds[i].GetComponent<RectTransform>().position=RectTransformUtility.WorldToScreenPoint(Camera.main, gameMgr.Enemies[i].transform.position);
                }
            }
            VisionBlocker.position =RectTransformUtility.WorldToScreenPoint(Camera.main, player.transform.position);
            VisionStrength = player.VisionVal;
            VisionBlocker.localScale = new Vector3(Mathf.Lerp(1.0f, 4.0f, VisionStrength),
                Mathf.Lerp(1.0f, 4.0f, VisionStrength), 1.0f);
            VisionBlocker.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f - VisionStrength);
        }
    }
}