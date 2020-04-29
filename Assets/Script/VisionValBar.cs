using System.Collections;
using System.Collections.Generic;
using theArch_LD46;
using UnityEngine;

public class VisionValBar : MonoBehaviour
{
    //TODO 技术上好了，但是几个条的颜色还得好好调。
    public float val;
    public Transform PosVal;
    public Transform NegVal;
    public Transform TransitionVal;

    private bool HittingEffectUsing = false;
    private float lastVal;
    private float HittingEffectTimePivot = 0.0f;
    private float HittingEffectDuration = 0.8f;

    void Awake()
    {
        HittingEffectUsing = false;
    }

    public void HitEffect(float targetVal)
    {
        lastVal = val;
        HittingEffectTimePivot = theArch_LD46_Time.UnscaleTime;
        TransitionVal.gameObject.SetActive(true);
        HittingEffectUsing = true;
    }

    void Update()
    {
        //TODO 秒打那个效果必须做。
        val = Mathf.Clamp01(val);
        NegVal.transform.localPosition = new Vector3(Mathf.Lerp(5.0f, 0.0f, 1-val), -0.005f, 0.0f);
        NegVal.transform.localScale = new Vector3(1-val, 1.0f, 1.0f);

        PosVal.transform.localPosition = new Vector3(Mathf.Lerp(-5.0f, 0.0f, val), 0.0f, 0.0f);
        PosVal.transform.localScale = new Vector3(val, 1.0f, 1.0f);
        if (val<=DesignerStaticData.ENEMY_HITTING_POWER)
        {
            PosVal.transform.GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", Color.red);
        }
        else
        {
            PosVal.transform.GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", Color.green);
        }
        if (HittingEffectUsing)
        {
            float lerper = (theArch_LD46_Time.UnscaleTime - HittingEffectTimePivot) / HittingEffectDuration;
            if (lerper>=1.0f)
            {
                HittingEffectUsing = false;
            }

            float transVal = lastVal * (1 - lerper);
            TransitionVal.transform.localPosition = new Vector3(Mathf.Lerp(-5.0f, 0.0f, transVal), -0.0025f, 0.0f);
            TransitionVal.transform.localScale = new Vector3(transVal, 1.0f, 1.0f);
        }
        else
        {
            TransitionVal.gameObject.SetActive(false);
        }
    }
}
