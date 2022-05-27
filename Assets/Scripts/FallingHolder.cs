using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingHolder : MonoBehaviour
{
    public GameObject fallingObj;

    bool triggered = false;
    public float delay = 0.5f;
    public float fallSpeed = 30f;
    public BoxCollider trigger;

    void Start()
    {
        
    }

    void Update()
    {
        if (triggered) {
            if (delay > 0)
                delay -= Time.deltaTime;
            else {
                fallingObj.transform.Translate(new Vector3(0, -1, 0) * (fallSpeed * 0.7f) * Time.deltaTime);
            }
        }
    }

    public void TriggerFall() {
        triggered = true;
        trigger.enabled = false;
    }
}
