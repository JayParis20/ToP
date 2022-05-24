using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SingleAvalanche : MonoBehaviour
{
    public VisualEffect vfx;

    public float lifetime = 5f;
    float speed = 40f;
    bool finished = false;

    void Start()
    {
        
    }

    void Update()
    {
        

        if(lifetime > 0) {
            lifetime -= Time.deltaTime;
        } else {
            finished = true;
        }

        if (finished) {
            vfx.SetFloat("Size", Mathf.Lerp(vfx.GetFloat("Size"), 0, Time.deltaTime * 5f));
            vfx.SetFloat("GlobalAlpha", Mathf.Lerp(vfx.GetFloat("GlobalAlpha"), 0, Time.deltaTime * 5f));
            if(vfx.GetFloat("Size") < 0.01f) {
                Destroy(gameObject);
            }
        } else {
            vfx.SetFloat("Size", Mathf.Lerp(vfx.GetFloat("Size"), 3, Time.deltaTime * 5f));
            vfx.SetFloat("GlobalAlpha", Mathf.Lerp(vfx.GetFloat("GlobalAlpha"), 1, Time.deltaTime * 5f));
        }

        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
}