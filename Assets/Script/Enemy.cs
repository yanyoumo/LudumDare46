using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace theArch_LD46
{
    public class Enemy : MonoBehaviour
    {
        public void SetPosition(Vector3 pos)
        {
            gameObject.transform.position = pos;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}