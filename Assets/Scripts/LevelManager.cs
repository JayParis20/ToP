using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public bool dead = false;
    public Renderer pinholeRend;
    public Transform player;
    float deathDelay = 1.5f;

    int currentSegment = 1;

    //Vector3 currentSegmentHeight;
    void Start()
    {
        
    }

    void Update()
    {
        if (dead) {
            pinholeRend.transform.localScale = Vector3.one * 0.963f;
            pinholeRend.material.SetFloat("_Scan", Mathf.MoveTowards(pinholeRend.material.GetFloat("_Scan"), 1f, Time.deltaTime * 3f));

            deathDelay -= Time.deltaTime;
            if(deathDelay < 0f) {
                SceneManager.LoadScene(0);
            }
        }

        if (Keyboard.current.gKey.wasPressedThisFrame) {
            GameObject loadedSegment = GameObject.Find("Seg_1");
            GameObject foundSegment = GameObject.Find("Seg_" + currentSegment.ToString());
            GameObject newSegment = Instantiate(loadedSegment, new Vector3(foundSegment.transform.position.x, foundSegment.transform.position.y + 188.4f,foundSegment.transform.position.z), loadedSegment.transform.rotation);
            newSegment.name = "Seg_" + (currentSegment+1).ToString();
            currentSegment++;
            Debug.Log(currentSegment);
        }
    }
    
    public void PlayerDied() {
        pinholeRend.transform.parent.position = player.position;
        GetComponent<AvalancheSystem>().StopAll();
        dead = true;
    }
}
