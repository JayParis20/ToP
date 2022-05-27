using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThresholdScript : MonoBehaviour
{
    bool triggered = false;
    public Renderer rend;
    public Transform allTMPs;
    Vector3 startScale;

    void Start()
    {
        startScale = rend.transform.localScale;
    }

    void Update()
    {
        if (triggered) {
            rend.material.SetFloat("_Triggered", Mathf.Lerp(rend.material.GetFloat("_Triggered"), 1f, Time.deltaTime * 19f));
            rend.transform.localScale = Vector3.Lerp(rend.transform.localScale, new Vector3(startScale.x, startScale.y, startScale.z * 2.55f), Time.deltaTime * 19);

            foreach (TextMeshPro tmp in allTMPs.GetComponentsInChildren<TextMeshPro>()) {
                tmp.transform.localScale = Vector3.Lerp(tmp.transform.localScale, Vector3.one * 2.001f, Time.deltaTime * 19);
            }

            if(rend.transform.localScale.z > startScale.z * 2.5f) {
                Destroy(gameObject);
            }
        }
        
    }

    public void TriggeredThresh() {
        triggered = true;
    }
}
