using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace theArch_LD46
{
    public class TimeMgr
    {
        private float TimePiovt;
        private float slowTimeDuration;
        public bool slowMotion { private set; get; }

        public void TimeStretch(float realTimeDuration=1.0f)
        {
            slowMotion = true;
            TimePiovt = Time.unscaledTime;
            slowTimeDuration = realTimeDuration;
        }

        public void ResetTime()
        {
            theArch_LD46_Time.delTime = Time.deltaTime;
            theArch_LD46_Time.Time = 0;
            TimePiovt = 0.0f;
            slowMotion = false;
        }

        public void TimeUpdate()
        {
            //TODO 有一个很大的问题，就是VisualEffect的Time似乎不接受输入，就是考虑还得用TimeScale那个系统……
            if (slowMotion)
            {
                float slowMotionVal = (Time.unscaledTime - TimePiovt) / slowTimeDuration;

                if (slowMotionVal>1)
                {
                    slowMotion = false;
                }

                Time.timeScale = Mathf.Abs(Mathf.Lerp(1.0f, -1.0f, slowMotionVal));
            }
            else
            {
                Time.timeScale = 1.0f;
            }

            theArch_LD46_Time.delTime = Time.deltaTime;
            theArch_LD46_Time.Time = Time.time;
            theArch_LD46_Time.UnscaleTime = Time.unscaledTime;
        }
    }
}