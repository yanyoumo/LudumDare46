using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace theArch_LD46
{
    [ExecuteInEditMode]
    public class WallPrefabReplacer : MonoBehaviour
    {
        public GameObject Template;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Transform[] tmpT = GetComponentsInChildren<Transform>();
            foreach (Transform o in tmpT)
            {
                /*if (o.name.Contains("Wall_A (1)"))
                {
                    Debug.Log(o.transform.localPosition);
                }*/
                if (o.name.Contains("Wall_"))
                {
                    GameObject go=(PrefabUtility.InstantiatePrefab(Template) as GameObject);
                    go.transform.localPosition = o.transform.localPosition + this.transform.localPosition;
                    go.transform.localRotation = o.transform.localRotation;
                    go.transform.localScale = o.transform.localScale;
                    go.transform.parent = transform;
                    go.name = "WallPrefab_" + o.name.ToLower();
                    o.gameObject.SetActive(false);
                }
            }
        }
    }
}