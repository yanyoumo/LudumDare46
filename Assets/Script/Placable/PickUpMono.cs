using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public class PickUpMono : PlaceableBase
    {
        public BasicSenseType BasicSenseType; // { private set; get; }

        public float val { private set; get; }

        public Texture2D senseVisionTexture;
        public Texture2D senseAudioTexture;
        public Texture2D senseFeelingTexture;
        public Texture2D senseCompassTexture;

        public bool pendingDead = false;

        public Transform labelTrans;

        public GameMgr gameMgr;

        public Transform MeshRoot;
        public Transform MeshRingA;
        public Transform MeshRingB;

        private Vector3 OrgMeshRootPos;

        private float CorePhase;
        private Vector3 RingAxisA;
        private Vector3 RingAxisB;

        public void InitPickUp()
        {
            pendingDead = false;
            val = DesignerStaticData.SINGLE_PICKUP_VAL;
            Texture2D targetTex;
            switch (BasicSenseType)
            {
                case BasicSenseType.Vision:
                    targetTex = senseVisionTexture;
                    break;
                case BasicSenseType.Audio:
                    targetTex = senseAudioTexture;
                    break;
                case BasicSenseType.Feeling:
                    targetTex = senseFeelingTexture;
                    break;
                case BasicSenseType.Compass:
                    targetTex = senseCompassTexture;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            labelTrans.GetComponentInChildren<MeshRenderer>().material.SetTexture("_UnlitColorMap", targetTex);

            OrgMeshRootPos = MeshRoot.position;

            CorePhase = UnityEngine.Random.value;
            RingAxisA = UnityEngine.Random.insideUnitSphere;
            RingAxisB = Vector3.Cross(RingAxisA, Vector3.up).normalized;
        }

        // Start is called before the first frame update
        void Start()
        {
            InitPickUp();
        }

        // Update is called once per frame
        void Update()
        {
            if (pendingDead)
            {
                Debug.Assert(gameMgr);
                gameMgr.SortedPickUps.Remove(this);//在build里面GameMgr的引用掉了？？？对，参考GameMgr约135行代码
                Destroy(gameObject);
            }

            float timeDel = theArch_LD46_Time.delTime * 100.0f;

            MeshRoot.position = OrgMeshRootPos +
                                new Vector3(0.0f, 0.2f, 0.0f) * Mathf.Sin(theArch_LD46_Time.Time * 7.5f + CorePhase);
            MeshRingA.transform.Rotate(RingAxisA, 8f * timeDel);
            MeshRingB.transform.Rotate(RingAxisB, 8f * timeDel);
        }
    }
}