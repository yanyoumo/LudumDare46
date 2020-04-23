using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSlideInvadeWall : MonoBehaviour
{
    //private Collider camCollider;
    // Start is called before the first frame update
    private Vector3 pendingLocalMovement;

    void Start()
    {
        
    }

    bool Vec3Approx(Vector3 a, Vector3 b)
    {
        bool x = Mathf.Approximately(a.x, b.x);
        bool y = Mathf.Approximately(a.y, b.y);
        bool z = Mathf.Approximately(a.z, b.z);
        return x && y && z;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = transform.position;
        if (pendingLocalMovement==Vector3.zero)
        {
            if (!Vec3Approx(transform.localPosition,Vector3.zero))
            {
                pendingLocalMovement = transform.localPosition.normalized * -0.1f;
            }
            else
            {
                pendingLocalMovement = Vector3.zero;
            }
        }
        transform.localPosition += pendingLocalMovement;
        pendingLocalMovement=Vector3.zero;
    }

    void LateUpdate()
    {
        Vector3 currentLocalPos= transform.localPosition;
        currentLocalPos.x = 0.0f;
        transform.localPosition = currentLocalPos;
    }

    void OnCollisionEnter(Collision collision)
    {
        /*if (other.gameObject.layer == LayerMask.GetMask(LayerMask.LayerToName(9)))
        {*/
        Debug.Log("Cam Struk");
        pendingLocalMovement = new Vector3(0.0f, 0.0f, 0.1f);
        //}
    }
}
