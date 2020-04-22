using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace theArchitectTechPack.GlobalHelper
{
    public static partial class StaticName
    {

    }

    public static partial class Utils
    {

        public static bool  MathFloatApproxZero(float a)
        {
            return Mathf.Approximately(a,0.0f);
        }

        [CanBeNull]
        public static IEnumerable<T> GetEnumValuesAsArray<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        [CanBeNull]
        public static T GenerateWeightedRandom<T>(T[] lib)
        {
            Dictionary<T, float> _lib = new Dictionary<T, float>();
            Debug.Assert(lib.Length > 0);
            foreach (var type in lib)
            {
                _lib.Add(type, 1.00f / lib.Length);
            }

            return GenerateWeightedRandom(_lib);
        }

        [CanBeNull]
        public static T GenerateWeightedRandom<T>(Dictionary<T, float> lib)
        {
            //有这个东西啊，不要小看他，这个很牛逼的。
            //各种分布都可以有的。
            float totalWeight = 0;
            foreach (var weight in lib.Values)
            {
                totalWeight += weight;
            }

            Debug.Assert((Mathf.Abs(totalWeight - 1) < 1e-3) && (lib.Count > 0),
                "totalWeight=" + totalWeight + "||lib.Count=" + lib.Count);
            float val = Random.value;
            totalWeight = 0;
            foreach (var keyValuePair in lib)
            {
                totalWeight += keyValuePair.Value;
                if (val <= totalWeight)
                {
                    return keyValuePair.Key;
                }
            }
            return default;
        }

        public static T[] Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = (int)(Random.value * (n--));
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
            return array;
        }

        public static string TMPColorXml(string content, Color col)
        {
            var hexCol = ColorUtility.ToHtmlStringRGB(col);
            return "<color=#" + hexCol + "> " + content + " </color> ";
        }

        public static string TMPColorBold(string content)
        {
            return "<b> " + content + " </b> ";
        }

        public static string PaddingFloat4Digit(float input)
        {
            int inputInt = Mathf.FloorToInt(input);
            if (inputInt >= 10000)
            {
                return "????";
            }
            else if (inputInt >= 1000)
            {
                return inputInt.ToString();
            }
            else if (inputInt >= 100)
            {
                return "0" + inputInt;
            }
            else if (inputInt >= 10)
            {
                return "00" + inputInt;
            }
            else
            {
                return "000" + inputInt;
            }
        }

        public static string PaddingFloat3Digit(float input)
        {
            int inputInt = Mathf.FloorToInt(input);
            if (inputInt >= 1000)
            {
                return "???";
            }
            else if (inputInt >= 100)
            {
                return inputInt.ToString();
            }
            else if (inputInt >= 10)
            {
                return "0" + inputInt;
            }
            else
            {
                return "00" + inputInt;
            }
        }
    }
}