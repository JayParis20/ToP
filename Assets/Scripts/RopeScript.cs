using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RopeScript : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform ropeGraphic;

    public BoxCollider trigger;
    public BoxCollider col;
    public SphereCollider A_SC;
    public SphereCollider B_SC;

    public bool ropeEnabled = true;

    public bool speedRope = false;

    public PlayerLocomotion pl;

    float resetDelay = 0.25f;

    void Start()
    {
        pl = GameObject.Find("_MyPlayer").GetComponent<PlayerLocomotion>();
    }

    void Update()
    {
        //if(resetDelay > 0)
        //    resetDelay -= Time.deltaTime;
        //enabled = resetDelay <= 0;

        trigger.enabled = ropeEnabled;
        col.enabled = ropeEnabled;
        A_SC.enabled = ropeEnabled;
        B_SC.enabled = ropeEnabled;

        ropeGraphic.position = (A.position + B.position) / 2f;
        ropeGraphic.transform.LookAt(B.position);
        ropeGraphic.transform.localScale = new Vector3(0.5f,0.5f,Vector3.Distance(A.position,B.position));

        if(pl != null) {
            if (pl.gameStarted) {
                if (pl.currentRope == null) {
                    if(resetDelay > 0)
                        resetDelay -= Time.deltaTime;

                    if (!ropeEnabled && resetDelay <= 0) {
                        ropeEnabled = true;
                    }
                }

                if (Vector3.Distance(pl.transform.position, B.position) < 1.5f && pl.currentRope != this) {
                    //Debug.Log("Near");
                    //pl.SetUpRopeMovement(B_SC);
                }
            }
        }

        
    }

    public void DisableRope() {
        ropeEnabled = false;
        resetDelay = 0.25f;
    }

    public void EnableRope() {
        //ropeEnabled = true;
    }

    public void StartRefresh() {
        
    }
}
