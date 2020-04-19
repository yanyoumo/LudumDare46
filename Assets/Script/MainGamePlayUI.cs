﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
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
        public PlayerScript player;

        public RectTransform VisionBlocker;

        public float VisionStrength = 0.0f;

        private Vector3 EnemyOffset=new Vector3(0.0f,2.0f,0.0f);
        private Vector3 PlayerOffset=new Vector3(0.0f,0.98f,0.0f);

        public EnergyBar visionBar;
        public EnergyBar hearingBar;
        public EnergyBar feelingBar;
        public EnergyBar compassBar;

        public Transform GoalTransform;
        public RectTransform GoalInd;

        private List<PickUpScript> nearestPickUp;
        private int nearestPickUpCount;

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
                    res[i]=Color.white;
                }
            }
            return res;
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Assert(gameMgr);
            EnemyInds = new List<GameObject>();
            PickUpInds = new List<GameObject>();

            visionBar.SetBlockFrameColor(new []{Color.gray});
            hearingBar.SetBlockFrameColor(new[] { Color.gray });
            feelingBar.SetBlockFrameColor(new[] { Color.gray });
            compassBar.SetBlockFrameColor(new[] { Color.gray });
        }

        private float GetSignedAngle(Vector3 from, Vector3 left ,Vector3 to)
        {
            return Mathf.Sign(Vector3.Dot(from, left)) * Vector3.Angle(player.MoveForward, to);
        }

        private Sprite getTexBySenseType(SenseType senseType)
        {
            switch (senseType)
            {
                case SenseType.Vision:
                    return senseVisionTexture;
                case SenseType.Audio:
                    return senseAudioTexture;
                case SenseType.Feeling:
                    return senseFeelingTexture;
                case SenseType.Compass:
                    return senseCompassTexture;
                default:
                    throw new ArgumentOutOfRangeException(nameof(senseType), senseType, null);
            }
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
                    EnemyInds[i].GetComponent<RectTransform>().position =
                        RectTransformUtility.WorldToScreenPoint(Camera.main,
                            gameMgr.Enemies[i].transform.position + EnemyOffset);
                    EnemyInds[i].GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f, player.AudioVal);
                }
            }

            if (gameMgr.PickUps != null)
            {
                nearestPickUp=new List<PickUpScript>();
                if (gameMgr.PickUps.Count > PickUpInds.Count)
                {
                    do
                    {
                        PickUpInds.Add(Instantiate(PickUpIndTemplate, transform));
                    } while (gameMgr.PickUps.Count > PickUpInds.Count);
                }
                else
                {
                    for (int i = 0; i < PickUpInds.Count; i++)
                    {
                        PickUpInds[i].gameObject.SetActive(i < gameMgr.PickUps.Count);
                    }
                }

                foreach (var gameMgrPickUp in gameMgr.PickUps)
                {
                    nearestPickUp.Add(gameMgrPickUp);
                }

                PickUpScript[] sortedPickups = nearestPickUp
                    .OrderBy(v => (v.transform.position - player.transform.position).magnitude).ToArray<PickUpScript>();

                nearestPickUpCount = Mathf.Min(Mathf.RoundToInt(6 * player.FeelingVal), sortedPickups.Length);

                for (int i = 0; i < PickUpInds.Count; i++)
                {
                    if (i < nearestPickUpCount)
                    {
                        PickUpInds[i].gameObject.SetActive(true);
                        Vector3 dirPick = sortedPickups[i].transform.position - player.transform.position;
                        PickUpInds[i].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0,
                            GetSignedAngle(player.MoveForward, player.MoveLeft, dirPick));
                        PickUpInds[i].GetComponentsInChildren<Image>()[1].sprite =
                            getTexBySenseType(sortedPickups[i].senseType);
                    }
                    else
                    {
                        PickUpInds[i].gameObject.SetActive(false);
                    }
                }
            }

            float visionRag = Mathf.Lerp(1.3f, 4.0f, VisionStrength);

            VisionBlocker.position =RectTransformUtility.WorldToScreenPoint(Camera.main, player.transform.position+ PlayerOffset);
            VisionStrength = player.VisionVal;
            VisionBlocker.localScale = new Vector3(visionRag, visionRag, 1.0f);
            VisionBlocker.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f - VisionStrength);

            visionBar.SetBlockFrameColor(GenColorsByVal(player.VisionVal));
            hearingBar.SetBlockFrameColor(GenColorsByVal(player.AudioVal));
            feelingBar.SetBlockFrameColor(GenColorsByVal(player.FeelingVal));
            compassBar.SetBlockFrameColor(GenColorsByVal(player.CompassVal));

            Vector3 dirGoal = GoalTransform.position - player.transform.position;
            GoalInd.rotation = Quaternion.Euler(0, 0, GetSignedAngle(player.MoveForward, player.MoveLeft, dirGoal));
            GoalInd.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, player.CompassVal);
        }
    }
}