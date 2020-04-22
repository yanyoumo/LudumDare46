using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public class ShakingText : MonoBehaviour
    {
        private Vector3 ZeroingPos = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            ZeroingPos = gameObject.GetComponent<RectTransform>().position;
        }

        // Update is called once per frame
        void Update()
        {
            float delY = Mathf.Sin(theArch_LD46_Time.Time * 10f) * 10f;
            gameObject.GetComponent<RectTransform>().position = ZeroingPos + new Vector3(0.0f, delY, 0.0f);
        }
    }
}