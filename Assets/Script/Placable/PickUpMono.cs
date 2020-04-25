using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace theArch_LD46
{
    public class PickUpMono : PlaceableBase
    {
        public SenseType senseType;// { private set; get; }

        public float val { private set; get; }

        public Texture2D senseVisionTexture;
        public Texture2D senseAudioTexture;
        public Texture2D senseFeelingTexture;
        public Texture2D senseCompassTexture;
        
        /*public Mesh senseVisionMesh;
        public Mesh senseAudioMesh;
        public Mesh senseFeelingMesh;
        public Mesh senseCompassMesh;*/

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
            val = 0.4f;
            Texture2D targetTex;
            switch (senseType)
            {
                case SenseType.Vision:
                    targetTex = senseVisionTexture;
                    break;
                case SenseType.Audio:
                    targetTex = senseAudioTexture;
                    break;
                case SenseType.Feeling:
                    targetTex = senseFeelingTexture;
                    break;
                case SenseType.Compass:
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
                gameMgr.SortedPickUps.Remove(this);
                Destroy(gameObject);
            }

            MeshRoot.position = OrgMeshRootPos +
                                new Vector3(0.0f, 0.2f, 0.0f) * Mathf.Sin(theArch_LD46_Time.Time * 7.5f + CorePhase);
            MeshRingA.transform.Rotate(RingAxisA, 8f);
            MeshRingB.transform.Rotate(RingAxisB, 8f);
        }
    }
}