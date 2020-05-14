using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public class SubSenseRing : MonoBehaviour
    {
        public BasicSenseType BasicSenseType;
        public float val;

        public MeshRenderer coreRenderer;
        public MeshRenderer ringRenderer;

        public Texture2D senseVisionTexture;
        public Texture2D senseAudioTexture;
        public Texture2D senseFeelingTexture;
        public Texture2D senseCompassTexture;

        private Texture2D getTexBySenseType(BasicSenseType basicSenseType)
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

        public void RefreshIcon()
        {
            coreRenderer.material.SetTexture("_UnlitColorMap", getTexBySenseType(BasicSenseType));
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
            ringRenderer.material.SetColor("_UnlitColor", Color.Lerp(Color.green, Color.red, val));
        }
    }
}