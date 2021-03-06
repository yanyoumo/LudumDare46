﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using theArchitectTechPack.GlobalHelper;

namespace theArch_LD46
{
    public enum BasicSenseType
    {
        //操，在这儿咬我一口……
        Vision,
        Audio,
        Feeling,
        Compass,
    }

    public enum SuperSenseType
    {
        SuperAudio,
        SuperFeeling,
        SuperCompass,
    }

    public enum GameStatus
    {
        Starting,
        Playing,
        Ended
    }

    public static class theArch_LD46_GameData
    {
#if UNITY_EDITOR
        public static GameStatus GameStatus = GameStatus.Starting;
#else
        public static GameStatus GameStatus = GameStatus.Starting;
#endif
        //public static bool firstTimeGame = true;
    }

    public static class theArch_LD46_Time
    {      
        public static float Time;//In second.
        public static float delTime;//In second.
        public static float UnscaleTime;//In second.
    }

}

namespace theArch_LD46.GlobalHelper
{
    public static partial class StaticData
    {
        public static IEnumerable<BasicSenseType> SenseTypesEnumerable;
        public static IEnumerable<SuperSenseType> SuperSenseTypesEnumerable;
        //
        public static readonly string INPUT_AXIS_NAME_FORWARD = "MoveForward";
        public static readonly string INPUT_AXIS_NAME_LEFT = "MoveLeft";
        public static readonly string INPUT_AXIS_NAME_LOOK_LEFT = "LookLeft";

        public static readonly string INPUT_BUTTON_NAME_GAME_START = "GameStart";

        /*public static readonly int SCENE_ID_GAMEPLAY = 0;
        public static readonly int SCENE_ID_ADDITIVE_CORE = 1;
        public static readonly int SCENE_ID_ADDITIVE_LV1 = 2;*/

        public static readonly int SCENE_ID_GAMEPLAY_ADDITIVE_CORE = 0;
        public static readonly int SCENE_ID_GAMEPLAY_ADDITIVE_LV0 = 1;
        public static readonly int SCENE_ID_GAMEPLAY_ADDITIVE_LV1 = 2;

        static StaticData()
        {
            SenseTypesEnumerable = Utils.GetEnumValuesAsArray<BasicSenseType>();
            SuperSenseTypesEnumerable = Utils.GetEnumValuesAsArray<SuperSenseType>();
        }
    }
}