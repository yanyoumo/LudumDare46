using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public static class DesignerStaticData
    {
        public static float GetSenseInitialVal(SenseType senseType)
        {
            switch (senseType)
            {
                case SenseType.Vision:
                    return 0.5f;
                case SenseType.Audio:
                    return 0.0f;
                case SenseType.Feeling:
                    return 0.0f;
                case SenseType.Compass:
                    return 0.0f;
                default:
                    return 0.0f;
            }
        }
    }
}