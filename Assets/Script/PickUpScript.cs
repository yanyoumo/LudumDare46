using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public class PickUpScript : MonoBehaviour
    {
        public SenseType senseType;// { private set; get; }

        public float val { private set; get; }

        public Texture2D senseVisionTexture;
        public Texture2D senseAudioTexture;
        public Texture2D senseFeelingTexture;
        public Texture2D senseCompassTexture;
        
        public Mesh senseVisionMesh;
        public Mesh senseAudioMesh;
        public Mesh senseFeelingMesh;
        public Mesh senseCompassMesh;

        public bool pendingDead = false;

        public Transform labelTrans;

        public GameMgr gameMgr;

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
                gameMgr.PickUps.Remove(this);
                Destroy(gameObject);
            }
        }
    }
}