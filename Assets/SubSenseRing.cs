using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public class SubSenseRing : MonoBehaviour
    {
        public SenseType senseType;
        public float val;

        public MeshRenderer coreRenderer;
        public MeshRenderer ringRenderer;

        public Texture2D senseVisionTexture;
        public Texture2D senseAudioTexture;
        public Texture2D senseFeelingTexture;
        public Texture2D senseCompassTexture;

        private Texture2D getTexBySenseType(SenseType senseType)
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

        public void RefreshIcon()
        {
            coreRenderer.material.SetTexture("_UnlitColorMap", getTexBySenseType(senseType));
        }

        // Start is called before the first frame update
        void Start()
        {
            RefreshIcon();
        }

        // Update is called once per frame
        void Update()
        {
            val = Mathf.Clamp(1-val, 0.001f, 0.999f);
            ringRenderer.material.SetFloat("_AlphaCutoff", val);
        }
    }
}