using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using theArchitectTechPack.GlobalHelper;

namespace theArch_LD46
{
    public enum SenseType
    {
        Vision,
        Audio,
        Feeling,
        Compass,
    }

    public static class theArch_LD46_Time
    {      
        public static float Time;//In second.
        public static float delTime;//In second.
        public static bool firstTimeGame = true;
    }

}

namespace theArch_LD46.GlobalHelper
{
    public static partial class StaticName
    {
        //
        public static readonly string INPUT_AXIS_NAME_FORWARD = "MoveForward";
        public static readonly string INPUT_AXIS_NAME_LEFT = "MoveLeft";
        public static readonly string INPUT_AXIS_NAME_LOOK_LEFT = "LookLeft";

        public static readonly string INPUT_BUTTON_NAME_GAME_START = "GameStart";

        public static readonly int SCENE_ID_GAMEPLAY = 0;
    }
}