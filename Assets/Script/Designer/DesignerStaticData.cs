using System;
using System.Collections;
using System.Collections.Generic;
using theArch_LD46.GlobalHelper;
using UnityEngine;

namespace theArch_LD46
{
    public struct HearingSenseData
    {
        public BasicSenseType BasicSenseType { get; }
        public float angle { get; }

        public HearingSenseData(BasicSenseType basicSenseType,float _angle)
        {
            BasicSenseType = basicSenseType;
            angle = _angle;
        }
    }

    public struct SenseDisplayingData
    {
        public float VisionAlpha { get; }
        public float VisionRadius { get; }

        public float HearingAlpha { get; }
        public int HearingCount { get; }

        public float FeelingAlpha { get; }
        public int FeelingCount { get; }

        public float CompassAlpha { get; }

        public SenseDisplayingData(
            float visionAlpha = 1,
            float visionRadius = 1,
            float hearingAlpha = 1,
            int hearingCount = 1,
            float feelingAlpha = 1,
            int feelingCount = 1,
            float compassAlpha = 1)
        {
            VisionAlpha = visionAlpha;
            VisionRadius = visionRadius;
            HearingAlpha = hearingAlpha;
            HearingCount = hearingCount;
            FeelingAlpha = feelingAlpha;
            FeelingCount = feelingCount;
            CompassAlpha = compassAlpha;
        }
    }

    public static class DesignerStaticData
    {
        //把HittingPower提高了，相当于需要更加重视敌人了。也就是听觉掉落物的意义被提高了。       
        public static readonly float ENEMY_HITTING_POWER = 0.35f;
        public static readonly float VISION_PICKUP_VAL = 0.3f;
        public static readonly float SINGLE_PICKUP_VAL = 1.0f;
        public static readonly float SLOWMOTION_DURATION = 0.25f;
        public static readonly float PLAYER_VISION_DIMINISHING_VAL = 0.075f;
        public static readonly float PLAYER_BASIC_DIMINISHING_VAL = 0.175f;
        public static readonly float PLAYER_SUPER_DIMINISHING_VAL = 0.3f;
        //这块儿每个不同的Sense都要被不同地解释，这个就没有特别好的辙。
        private static readonly Dictionary<int, int> LevelFSM; 

        public static float GetCompassAlpha(float val)
        {
            return Mathf.Clamp01(val);
        }

        public static int GetFeelingCount(float val)
        {
            return Mathf.FloorToInt(val * 5);
        }

        public static float GetFeelingAlpha(float val)
        {
            return Mathf.Clamp01(val);
        }

        public static int GetHearingCount(float val)
        {
            return -1;
        }

        public static float GetHearingAlpha(float val)
        {
            return Mathf.Clamp01(val);
        }

        public static float GetVisionCurtainRadius(float val)
        {
            val = Mathf.Clamp01((val - ENEMY_HITTING_POWER) / (1 - ENEMY_HITTING_POWER));
            return Mathf.Sqrt(Mathf.Lerp(1.0f, 5.0f * 5.0f, val));
            //return Mathf.Lerp(1.0f, 5.0f, val);
        }

        public static float GetVisionCurtainAlpha(float visionVal)
        {
            visionVal = Mathf.Clamp01(visionVal);
            return 1 - visionVal;
        }

        static DesignerStaticData()
        {
            LevelFSM = new Dictionary<int, int>()
            {
                {StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_LV0, StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_LV1},
                {StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_LV1, -1}
            };
        }

        public static SuperSenseType GetSuperVersionOfSense(BasicSenseType basicSenseType)
        {
            switch (basicSenseType)
            {
                case BasicSenseType.Audio:
                    return SuperSenseType.SuperAudio;
                case BasicSenseType.Feeling:
                    return SuperSenseType.SuperFeeling;
                case BasicSenseType.Compass:
                    return SuperSenseType.SuperCompass;
                default:
                    throw new ArgumentOutOfRangeException(nameof(basicSenseType), basicSenseType, null);
            }
        }

        public static float GetSenseInitialVal(BasicSenseType basicSenseType)
        {
            switch (basicSenseType)
            {
                case BasicSenseType.Vision:
                    return 1.0f;
                case BasicSenseType.Audio:
                    return 0.0f;
                case BasicSenseType.Feeling:
                    return 0.0f;
                case BasicSenseType.Compass:
                    return 0.0f;
                default:
                    return 0.0f;
            }
        }

        public static int GetNextLevel(int currentLevelID)
        {
            if (LevelFSM.TryGetValue(currentLevelID, out int res))
            {
                return res;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static int GetStartingBG()
        {
            return StaticData.SCENE_ID_GAMEPLAY_ADDITIVE_LV1;
        }
    }
}