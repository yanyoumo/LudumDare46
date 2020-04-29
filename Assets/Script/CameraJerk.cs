using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public class CameraJerk : MonoBehaviour
    {
        private int JerkTimer = 0;
        private readonly int JerkDurationFrameCount = 3;

        private bool Jerking = false;
        private Vector3 dir;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void DoJerk(Vector3 _dir)
        {
            if (!Jerking)
            {
                dir = _dir;
                Jerking = true;
                JerkTimer = 0;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Jerking)
            {
                transform.localPosition= dir;
                JerkTimer += 1;
                if (JerkTimer> JerkDurationFrameCount)
                {
                    Jerking = false;
                }
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }
    }
}