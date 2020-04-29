using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public class DestoryAfterDelay : MonoBehaviour
    {
        private float delayTimer = 0.0f;
        private float delayMax = 0.75f;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            delayTimer += theArch_LD46_Time.delTime;
            if (delayTimer>=delayMax)
            {
                Destroy(gameObject);
            }
        }
    }
}