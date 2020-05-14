using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using theArchitectTechPack.GlobalHelper;
using theArch_LD46.GlobalHelper;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace theArch_LD46
{
    public class MainGamePlayUI : MonoBehaviour
    {
        public GameObject EnemyIndTemplate;
        public GameObject PickUpIndTemplate;
        public List<GameObject> EnemyInds;
        public List<GameObject> PickUpInds;

        public Sprite senseVisionTexture;
        public Sprite senseAudioTexture;
        public Sprite senseFeelingTexture;
        public Sprite senseCompassTexture;

        public GameMgr gameMgr;
        //public Transform playerTransform;
        public PlayerMono player;//TODO，要去掉，但是和additive不冲突。

        private Vector3 EnemyOffset=new Vector3(0.0f,2.0f,0.0f);

        public RectTransform GoalInd;
        public GameObject GameOverPanel;

        public Transform GoalTrans;
        public Transform DataTransform;
        //public Transform VisionTransform;

        public Transform GameStartTransform;

        Color[] GenColorsByVal(float val)
        {
            int barCount = Mathf.RoundToInt(val * 10f);
            Color[] res = new Color[10];
            for (int i = 0; i < 10; i++)
            {
                if (i>= barCount)
                {
                    res[i]=Color.gray;
                }
                else
                {
                    if (barCount > 4)
                    {
                        res[i] = Color.green;
                    }
                    else
                    {
                        res[i] = Color.red;
                    }
                }
            }
            return res;
        }

        void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("UI invoked");
        }

        public void InitUIForRestart()
        {
            GameOverPanel.gameObject.SetActive(false);
            GoalTrans.gameObject.SetActive(false);
            GameStartTransform.gameObject.SetActive(true);
        }

        public void InitUIForPlay()
        {
            Debug.Assert(gameMgr);
            EnemyInds = new List<GameObject>();
            PickUpInds = new List<GameObject>();

            GoalTrans.gameObject.SetActive(true);
            GameStartTransform.gameObject.SetActive(false);
        }

        public void InitUIForEnding()
        {
            //TODO 这个等UI整理完了再弄。
            EnemyInds = null;
            PickUpInds = null;
        }

        private Sprite getTexBySenseType(BasicSenseType basicSenseType)
        {
            switch (basicSenseType)
            {
                case BasicSenseType.Vision:
                    return senseVisionTexture;
                case BasicSenseType.Audio:
                    return senseAudioTexture;
                case BasicSenseType.Feeling:
                    return senseFeelingTexture;
                case BasicSenseType.Compass:
                    return senseCompassTexture;
                default:
                    throw new ArgumentOutOfRangeException(nameof(basicSenseType), basicSenseType, null);
            }
        }

        void ResizeAndEnableInds(int targetCount,ref List<GameObject> array, GameObject template, Transform parent)
        {
            Debug.Assert(targetCount >= 0);
            while (targetCount > array.Count)
            {
                array.Add(Instantiate(template, parent));
            } 

            for (int i = 0; i < array.Count; i++)
            {
                array[i].gameObject.SetActive(i < targetCount);
            }
        }

        // Update is called once per frame
        void OnGUI()
        {
            if (theArch_LD46_GameData.GameStatus == GameStatus.Playing)
            {
                if (!gameMgr.levelSwitching)
                {
                    //TODO 这里的Count外面应该处理掉。
                    int hearingCount = 0;
                    int feelingCount = 0;
                    if (gameMgr.FilteredSortedEnemiesPos != null)
                    {                     
                        hearingCount = gameMgr.FilteredSortedEnemiesPos.Length;
                    }
                    if (gameMgr.FilteredSortedPickUpsData != null)
                    {
                        feelingCount = gameMgr.FilteredSortedPickUpsData.Length;
                    }

                    float feelingAlpha = gameMgr.senseDisplayingData.FeelingAlpha;
                    float hearingAlpha = gameMgr.senseDisplayingData.HearingAlpha;
                    float compassAlpha = gameMgr.senseDisplayingData.CompassAlpha;

                    ResizeAndEnableInds(hearingCount, ref EnemyInds, EnemyIndTemplate, transform);
                    ResizeAndEnableInds(feelingCount, ref PickUpInds, PickUpIndTemplate, transform);

                    for (var i = 0; i < hearingCount; i++)
                    {
                        EnemyInds[i].GetComponent<RectTransform>().position =
                            RectTransformUtility.WorldToScreenPoint(Camera.main,
                                gameMgr.FilteredSortedEnemiesPos[i] + EnemyOffset);
                        EnemyInds[i].GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f, hearingAlpha);
                    }

                    for (var i = 0; i < feelingCount; i++)
                    {
                        PickUpInds[i].GetComponent<RectTransform>().rotation =
                            Quaternion.Euler(0, 0, gameMgr.FilteredSortedPickUpsData[i].angle);
                        PickUpInds[i].GetComponentsInChildren<Image>()[1].sprite =
                            getTexBySenseType(gameMgr.FilteredSortedPickUpsData[i].BasicSenseType);
                    }

                    //Vector3 dirGoal = GoalTransform.position - player.transform.position;
                    GoalInd.rotation = Quaternion.Euler(0, 0, gameMgr.playerGoalAngle);
                    GoalInd.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, compassAlpha);
                }
            }else if (theArch_LD46_GameData.GameStatus == GameStatus.Ended)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                GameOverPanel.gameObject.SetActive(true);
            }
        }
    }
}