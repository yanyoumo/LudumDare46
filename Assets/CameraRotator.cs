using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace theArch_LD46
{
    public class CameraRotator : MonoBehaviour
    {
        public CinemachineVirtualCamera VirtualCamera;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //VirtualCamera.GetCinemachineComponent<CinemachineComposer>().m_ScreenX = Input.GetAxis("Mouse Y");
            //VirtualCamera.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = Input.GetAxis("Mouse X");
        }
    }
}