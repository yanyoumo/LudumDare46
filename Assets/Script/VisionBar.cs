using System.Collections;
using System.Collections.Generic;
using theArch_LD46.GlobalHelper;
using UnityEngine;

namespace theArch_LD46
{
    public class VisionBar : MonoBehaviour
    {
        /*public float playerCompassVal;
        public float playerFeelingVal;
        public float playerVisionVal;
        public float playerHearingVal;*/

        public float playerVisionEnemyTh;
        public VisionValBar visionValBar;
        public Transform visionThBar;

        private Queue<SenseType> senseQueue;

        public SubSenseRing subSenseRingA;
        public SubSenseRing subSenseRingB;
        public SubSenseRing subSenseRingC;

        private SubSenseRing[] subRingArray;

        public PlayerMono player;

        // Start is called before the first frame update
        void Start()
        {
            senseQueue = new Queue<SenseType>();
            playerVisionEnemyTh = DesignerStaticData.ENEMY_HITTING_POWER;
            subRingArray = new[] {subSenseRingA, subSenseRingB, subSenseRingC};
        }

        public void HitEffect(float targetVal)
        {
            visionValBar.HitEffect(targetVal);
        }

        private bool ValAboveTH(float val)
        {
            return val >= 0.001f;
        }

        // Update is called once per frame
        void Update()
        {
            if (theArch_LD46_GameData.GameStatus == GameStatus.Playing)
            {
                playerVisionEnemyTh = Mathf.Clamp01(playerVisionEnemyTh);
                visionThBar.transform.localPosition = new Vector3(playerVisionEnemyTh * -3.4f, 0.0f, 0.0f);

                visionValBar.val = player.GetValBySenseType(SenseType.Vision);

                Queue<SenseType> oldQueue = senseQueue;

                if (senseQueue.Count > 0)
                {
                    if (!ValAboveTH(player.GetValBySenseType(senseQueue.Peek())))
                    {
                        //这么些是建立在所有其他Sense衰减速度都一样且不会被直接扣减的前提下。
                        //先这么写，这个数据结构估计得改。
                        //可能还是得弄个List自己维护…………
                        senseQueue.Dequeue();
                    }
                }

                foreach (var senseType in StaticData.SenseTypesEnumerable)
                {
                    if (senseType != SenseType.Vision)
                    {
                        if (ValAboveTH(player.GetValBySenseType(senseType)))
                        {
                            if (!senseQueue.Contains(senseType))
                            {
                                senseQueue.Enqueue(senseType);
                            }
                        }
                    }
                }

                bool hasQueueChange = (oldQueue == senseQueue);

                Debug.Assert(senseQueue.Count <= 3);
                for (var i = 0; i < subRingArray.Length; i++)
                {
                    if (hasQueueChange)
                    {
                        if (i < senseQueue.Count)
                        {
                            subRingArray[i].gameObject.SetActive(true);
                            subRingArray[i].senseType = senseQueue.ToArray()[i];
                            subRingArray[i].val = player.GetValBySenseType(senseQueue.ToArray()[i]);
                            subRingArray[i].RefreshIcon();
                        }
                        else
                        {
                            subRingArray[i].gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (i < senseQueue.Count)
                        {
                            subRingArray[i].val = player.GetValBySenseType(senseQueue.ToArray()[i]);
                        }
                    }
                }
            }
        }
    }
}