using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace theArch_LD46
{
    public class EnemyMono : PlaceableBase
    {
        public bool XAxisPatrolOrZ = true;

        public bool GoForward = false;

        private float speed = 0.1f;

        public CharacterController charCtrl;

        public Transform meshRoot;

        public VisualEffect vf;

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
            Vector3 forward;
            forward = XAxisPatrolOrZ ? new Vector3(1.0f,0.0f,0.0f) : new Vector3(0.0f, 0.0f, 1.0f);
            charCtrl.Move(forward * (GoForward ? 1.0f : -1.0f) * speed);

            meshRoot.transform.Rotate(0, GoForward ? 1.5f : -1.5f, 0);

            vf.SetVector3("EnemyPos",transform.position);
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!hit.collider.gameObject.GetComponent<PlayerMono>())
            {
                if (!hit.collider.gameObject.GetComponent<PickUpMono>())
                {
                    if (!hit.collider.gameObject.GetComponent<EnemyMono>())
                    {
                        GoForward = !GoForward;
                    }
                }
            }          
        }
    }
}