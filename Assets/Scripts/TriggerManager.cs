using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public List<BoxCollider> allTriggers;
    public float refreshTime = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        if (refreshTime > 0f && refreshTime > -1)
            refreshTime -= Time.deltaTime;
        else if(refreshTime > -1){
            foreach (BoxCollider BC in allTriggers) {
                BC.enabled = true;
            }
            refreshTime = -3f;
        }
    }

    public void RefreshTriggers(bool disable) {
        refreshTime = disable ? -3f : 0.5f;

        foreach (BoxCollider BC in allTriggers) {
            BC.enabled = false;
        }
    }
}
