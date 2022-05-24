using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public bool dead = false;
    public Renderer pinholeRend;
    public Transform player;
    float deathDelay = 1.5f;

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
    }
    
    public void PlayerDied() {
        pinholeRend.transform.parent.position = player.position;
        dead = true;
    }
}
