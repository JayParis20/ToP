using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvalancheSystem : MonoBehaviour
{
    public GameObject warning_1;
    public GameObject warning_2;
    public GameObject warning_3;

    bool animating_1 = false;
    bool animating_2 = false;
    bool animating_3 = false;

    float t1 = 0f;
    float t2 = 0f;
    float t3 = 0f;

    public AnimationCurve pulse;
    float animSpeed = 1.6f;

    public GameObject avalanchePrefab;
    public Transform pivot;
    public Transform spawns;

    public Transform blizzard;
    public Transform edges;

    public PlayerLocomotion pl;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        edges.position = new Vector3(edges.position.x, pl.transform.position.y, edges.position.z);
        spawns.position = pivot.position;

        if (animating_1) {
            if (t1 < 3f) {
                t1 += Time.deltaTime * animSpeed;
                foreach (RawImage RI in warning_1.GetComponentsInChildren<RawImage>()) {
                    RI.color = new Color(RI.color.r, RI.color.g, RI.color.b,Mathf.Clamp(pulse.Evaluate(t1) * 5f,0f,1f));
                }
            } else {
                t1 = 0;
                animating_1 = false;
            }
        }
        if (animating_2) {
            if (t2 < 3f) {
                t2 += Time.deltaTime * animSpeed;
                foreach (RawImage RI in warning_2.GetComponentsInChildren<RawImage>()) {
                    RI.color = new Color(RI.color.r, RI.color.g, RI.color.b, Mathf.Clamp(pulse.Evaluate(t2) * 5f, 0f, 1f));
                }
            } else {
                t2 = 0;
                animating_2 = false;
            }
        }
        if (animating_3) {
            if (t3 < 3f) {
                t3 += Time.deltaTime * animSpeed;
                foreach (RawImage RI in warning_3.GetComponentsInChildren<RawImage>()) {
                    RI.color = new Color(RI.color.r, RI.color.g, RI.color.b, Mathf.Clamp(pulse.Evaluate(t3) * 5f, 0f, 1f));
                }
            } else {
                t3 = 0;
                animating_3 = false;
            }
        }
        if (pl.gameStarted && !pl.hasSummited) {
            //blizzard.Translate(Vector3.up * 10f * Time.deltaTime);
        }
    }

    public void StartAvalanche(int id, float lifetime, float speed) {
        if(id == 1) {
            t1 = 0;
            animating_1 = true;
            GameObject newAva = Instantiate(avalanchePrefab,spawns.GetChild(id-1).position, spawns.GetChild(id-1).rotation);
            //newAva.GetComponent<SingleAvalanche>().lifetime = 
        }
        if (id == 2) {
            t2 = 0;
            animating_2 = true;
            GameObject newAva = Instantiate(avalanchePrefab, spawns.GetChild(id - 1).position, spawns.GetChild(id - 1).rotation);
        }
        if (id == 3) {
            t3 = 0;
            animating_3 = true;
            GameObject newAva = Instantiate(avalanchePrefab, spawns.GetChild(id - 1).position, spawns.GetChild(id - 1).rotation);
        }
    }

    public void StopAll() {
        t1 = 3.1f;
        //animating_1 = false;
        t2 = 3.1f;
        //animating_2 = false;
        t3 = 3.1f;

        warning_1.SetActive(false);
        warning_2.SetActive(false);
        warning_3.SetActive(false);
    }
}
