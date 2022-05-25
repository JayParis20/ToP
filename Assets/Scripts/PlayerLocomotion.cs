using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PlayerLocomotion : MonoBehaviour
{
    public int worldRotationID = 0;

    public Transform pivot;
    public Transform rot_0;
    public Transform rot_1;
    public Transform rot_2;
    public Transform rot_3;


    Vector3 playerVel = Vector3.zero;
    public float playerSpeed = 122f;
    public float playerJump = 112f;
    public float playerFall = 1f;
    //float currentJump = 0f;

    Vector3 jumpVelocity;

    public LineRenderer lr;
    public LineRenderer lr_2;
    public LineRenderer lr_3;

    public Rigidbody rigi;
    public CharacterController cc;

    public bool hasSummited = false;
    public bool gameStarted = false;

    public Transform summitCam;
    public Transform normalCam;
    public Transform camHolder;

    public Transform titleUI;

    public Transform aim;
    public Transform moveAim;
    public Light light;

    float startLightIntensity;
    public Transform targetPos;

    bool nextLandIsRotate = false;
    Collider nextRotateCol;

    public Transform surfaceNormal;

    public Transform mA;
    public Transform mB;
    public Transform grv;

    bool hasConnected = true;
    Vector3 toConnectVel;
    Vector3 surfaceMove;

    public Transform graphicsHolder;
    public Transform graphicsLook;
    public Renderer graphics;
    bool sideWall = false;

    public Vector3 lastGroundedPos;

    public TriggerManager tm;
    bool stillInTrigger = false;

    float rotDelay = 0f;
    Vector3 rotDelayPos;

    Vector3 lastPlayerPos;
    Vector3 graphicsStartScale;

    public AvalancheSystem avSystem;
    public LevelManager lm;

    bool onRope = false;

    Vector3 currentGravity;
    bool isSlip = false;

    void Start() {
        startLightIntensity = light.GetComponent<HDAdditionalLightData>().intensity;
        worldRotationID = 3;
        targetPos.position = transform.position;
        lastGroundedPos = transform.position;
        graphicsStartScale = graphics.transform.localScale;
    }

    Vector3 CorVect(Vector3 originalVector) {
        Vector3 finalVector = originalVector;
        switch (worldRotationID) {
            case 0:
                finalVector = new Vector3(originalVector.x, originalVector.y, originalVector.z);
                break;
            case 1:
                finalVector = new Vector3(-originalVector.x, originalVector.y, originalVector.z);
                break;
            case 2:
                finalVector = new Vector3(-originalVector.x, originalVector.y, originalVector.z);
                break;
            case 3:
                finalVector = new Vector3(originalVector.x, originalVector.y, originalVector.z);
                break;
        }
        return finalVector;
    }

    void Update() {
        if(Vector3.Distance(transform.position, lastPlayerPos) > 0.15f)
            lastPlayerPos = transform.position;

        if (rotDelay > 0)
            rotDelay -= Time.deltaTime;

        if (cc != null) {
            if (rotDelay <= 0 && cc.enabled == false) {

                cc.enabled = true;
            } else {
                if (cc.enabled == false) {
                    transform.position = rotDelayPos;
                }
            }
        }

        Quaternion targetRot = Quaternion.identity;
        switch (worldRotationID) {
            case 0:
                targetRot = rot_0.rotation;
                light.transform.parent.rotation = rot_0.rotation;
                graphicsHolder.rotation = rot_0.rotation;
                avSystem.spawns.rotation = rot_0.rotation;
                break;
            case 1:
                targetRot = rot_1.rotation;
                light.transform.parent.rotation = rot_1.rotation;
                graphicsHolder.rotation = rot_1.rotation;
                avSystem.spawns.rotation = rot_1.rotation;
                break;
            case 2:
                targetRot = rot_2.rotation;
                light.transform.parent.rotation = rot_2.rotation;
                graphicsHolder.rotation = rot_2.rotation;
                avSystem.spawns.rotation = rot_2.rotation;
                break;
            case 3:
                targetRot = rot_3.rotation;
                light.transform.parent.rotation = rot_3.rotation;
                graphicsHolder.rotation = rot_3.rotation;
                avSystem.spawns.rotation = rot_3.rotation;
                break;
        }

        pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRot, Time.deltaTime * 7f);

        if (Gamepad.current != null) {
            if (Gamepad.current.buttonEast.wasPressedThisFrame) {
                //hasSummited = true;

                //Debug.LogError(Vector3.Distance(Camera.main.transform.position, transform.position));
            }

            //-----
            float deadzone = 0.2f;
            if (Gamepad.current.rightStick.ReadValue().x > deadzone || Gamepad.current.rightStick.ReadValue().x < -deadzone 
                || Gamepad.current.rightStick.ReadValue().y > deadzone || Gamepad.current.rightStick.ReadValue().y < -deadzone) {
                Vector2 input = new Vector2(Gamepad.current.rightStick.ReadValue().x, Gamepad.current.rightStick.ReadValue().y);
                Vector2 inputDir = input.normalized;

                switch (worldRotationID) {
                    case 0:
                        aim.transform.eulerAngles = Vector3.forward * Mathf.Atan2(-inputDir.x, inputDir.y) * Mathf.Rad2Deg;
                        break;
                    case 1:
                        aim.transform.eulerAngles = Vector3.right * Mathf.Atan2(-inputDir.x, inputDir.y) * Mathf.Rad2Deg;
                        break;
                    case 2:
                        aim.transform.eulerAngles = Vector3.back * Mathf.Atan2(-inputDir.x, inputDir.y) * Mathf.Rad2Deg;
                        break;
                    case 3:
                        aim.transform.eulerAngles = Vector3.left * Mathf.Atan2(-inputDir.x, inputDir.y) * Mathf.Rad2Deg;
                        break;
                }

                float reach = 50f;//35f
                lr.widthMultiplier = 0.75f * (hasConnected ? 1f : 0f);
                lr_2.widthMultiplier = 0.5f * (hasConnected ? 1f : 0f);
                lr_3.widthMultiplier = 0.25f * (hasConnected ? 1f : 0f);
                RaycastHit reachHit;
                RaycastHit reachHit_2;
                RaycastHit reachHit_3;
                if (Physics.Raycast(transform.position, (aim.GetChild(0).position - transform.position), out reachHit, reach)) {
                    /*
                    if (Physics.Raycast(reachHit.point, Vector3.Reflect((reachHit.point - transform.position).normalized,reachHit.normal), out reachHit_2, 199f)) {
                            //--3
                        if (Physics.Raycast(reachHit_2.point, Vector3.Reflect((reachHit_2.point - reachHit.point).normalized, reachHit_2.normal), out reachHit_3, 199f)) {
                            if (Vector3.Distance(reachHit_2.point, reachHit_3.point) < 199f) {

                            } else {
                                        
                            }
                            lr.positionCount = 2;
                            lr.SetPosition(0, transform.position);
                            lr.SetPosition(1, reachHit.point);
                            lr_2.positionCount = 2;
                            lr_2.SetPosition(0, reachHit.point);
                            lr_2.SetPosition(1, reachHit_2.point);
                            lr_3.positionCount = 2;
                            lr_3.SetPosition(0, reachHit_2.point);
                            lr_3.SetPosition(1, reachHit_3.point);
                            lr.enabled = true;
                            lr_2.enabled = true;
                            lr_3.enabled = true;
                        } else {
                            lr.positionCount = 2;
                            lr.SetPosition(0, transform.position);
                            lr.SetPosition(1, reachHit.point);
                            lr_2.positionCount = 2;
                            lr_2.SetPosition(0, reachHit.point);
                            lr_2.SetPosition(1, reachHit_2.point);
                            lr.enabled = true;
                            lr_2.enabled = true;
                            lr_3.enabled = false;
                        }
                    } else {
                        lr.positionCount = 2;
                        lr.SetPosition(0, transform.position);
                        lr.SetPosition(1, reachHit.point);
                        lr.enabled = true;
                        lr_2.enabled = false;
                        lr_3.enabled = false;
                    }
                    */
                    lr.positionCount = 2;
                    lr.SetPosition(0, transform.position);
                    lr.SetPosition(1, reachHit.point);
                    lr.enabled = true;
                    lr_2.enabled = false;
                    lr_3.enabled = false;
                } else {
                    lr.enabled = true;
                    lr.SetPosition(0, transform.position);
                    lr.SetPosition(1, aim.GetChild(0).position);

                    lr_2.enabled = false;
                    lr_3.enabled = false;
                }

                float remainingDist = reach; //rem dist / dist = percent
                float dist1 = 0.01f;
                float dist2 = 0.01f;
                float dist3 = 0.01f;
                float scn1 = 0.01f;
                float scn2 = 0.01f;
                float scn3 = 0.01f;
                if (lr.enabled) {
                    dist1 = Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1));
                    scn1 = (reach - dist1) / dist1;
                    remainingDist -= Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1));
                    if(Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1)) > remainingDist) {
                        //cut short
                        //lr.material.SetFloat("_Scan", remainingDist / Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1)));
                        lr.material.SetFloat("_Scan", scn1);
                    } else {
                        lr.material.SetFloat("_Scan", 1f);
                    }
                    remainingDist = Mathf.Clamp(remainingDist, 0, 999f);
                } else {
                    lr.material.SetFloat("_Scan", 1f);
                }
                if (lr_2.enabled) {
                    dist2 = Vector3.Distance(lr_2.GetPosition(0), lr_2.GetPosition(1));
                    scn2 = (reach - dist1 - dist2) / dist2;
                    remainingDist -= Vector3.Distance(lr_2.GetPosition(0), lr_2.GetPosition(1));
                    if (Vector3.Distance(lr_2.GetPosition(0), lr_2.GetPosition(1)) > remainingDist) {
                        //cut short
                        //lr_2.material.SetFloat("_Scan", remainingDist / Vector3.Distance(lr_2.GetPosition(0), lr_2.GetPosition(1)));
                        lr_2.material.SetFloat("_Scan", scn2);

                    } else {
                        lr_2.material.SetFloat("_Scan", 1f);
                    }
                    remainingDist = Mathf.Clamp(remainingDist, 0, 999f);
                } else {
                    lr_2.material.SetFloat("_Scan", 1f);
                }
                if (lr_3.enabled) {
                    dist3 = Vector3.Distance(lr_3.GetPosition(0), lr_3.GetPosition(1));
                    scn3 = (reach - dist1 - dist2 - dist3) / dist3;
                    remainingDist -= Vector3.Distance(lr_3.GetPosition(0), lr_3.GetPosition(1));
                    if (Vector3.Distance(lr_3.GetPosition(0), lr_3.GetPosition(1)) > remainingDist) {
                        //cut short
                        //lr_3.material.SetFloat("_Scan", remainingDist / Vector3.Distance(lr_3.GetPosition(0), lr_3.GetPosition(1)));
                        lr_3.material.SetFloat("_Scan", scn3);
                    } else {
                        lr_3.material.SetFloat("_Scan", 1f);
                    }
                    remainingDist = Mathf.Clamp(remainingDist, 0, 999f);
                } else {
                    lr_3.material.SetFloat("_Scan", 1f);
                }
                
                lr.material.SetFloat("_Scan", 1f);
                lr_2.material.SetFloat("_Scan", 1f);
                lr_3.material.SetFloat("_Scan", 1f);

                lr.material.SetVector("_PlayerPos", transform.position);
                lr_2.material.SetVector("_PlayerPos", transform.position);
                lr_3.material.SetVector("_PlayerPos", transform.position);


                //Debug.Log(new Vector3(dist1, dist2, dist3));
                //Debug.Log(new Vector3(scn1, scn2, scn3));
                //Debug.Log(remainingDist);

                if (Gamepad.current.rightShoulder.wasReleasedThisFrame) {
                    //Debug.Log("MV");

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, (aim.GetChild(0).position - transform.position), out hit, reach)) { //0.15f new Vector3(inputDir.x, inputDir.y, 0)
                        
                        targetPos.position = hit.point + (transform.position - hit.point).normalized;
                        targetPos.LookAt(transform.position);
                        toConnectVel = (hit.point - transform.position).normalized;
                        hasConnected = false;
                        lastGroundedPos = targetPos.position;
                        //rigi.isKinematic = true;
                        light.GetComponent<HDAdditionalLightData>().intensity = startLightIntensity * 55f;

                        sideWall = (hit.point + hit.normal).y == hit.point.y;
                        switch (worldRotationID) {
                            case 0:
                                surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, sideWall  ? 0 : -90);
                                break;
                            case 1:
                                surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                                break;
                            case 2:
                                surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, sideWall ? 0 : -90);
                                break;
                            case 3:
                                surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                                break;
                        }

                        surfaceNormal.rotation = Quaternion.LookRotation(hit.normal, Camera.main.transform.up);

                        onRope = false;

                        if (hit.collider.tag == "TurnTrigger") {
                            //Debug.LogError("TRIG");
                            //nextLandIsRotate = true;
                            //nextRotateCol = hit.collider;
                        }
                    }
                    if (stillInTrigger) {
                        tm.RefreshTriggers(false);
                        stillInTrigger = false;
                    }
                } else {

                }

                
            } else {
                lr.enabled = false;
                lr_2.enabled = false;
                lr_3.enabled = false;
            }

            if(Gamepad.current.leftStick.ReadValue().x > deadzone || Gamepad.current.leftStick.ReadValue().x < -deadzone) {
                if (stillInTrigger && rotDelay <= 0) {
                    tm.RefreshTriggers(false);
                    stillInTrigger = false;
                }
            }

            if (Gamepad.current.leftStick.ReadValue().x > deadzone || Gamepad.current.leftStick.ReadValue().x < -deadzone
                || Gamepad.current.leftStick.ReadValue().y > deadzone || Gamepad.current.leftStick.ReadValue().y < -deadzone) {

                Vector2 input = new Vector2(Gamepad.current.leftStick.ReadValue().x, Gamepad.current.leftStick.ReadValue().y);
                Vector2 inputDir = input.normalized;

                switch (worldRotationID) {
                    case 0:
                        moveAim.transform.eulerAngles = Vector3.forward * Mathf.Atan2(-inputDir.x, inputDir.y) * Mathf.Rad2Deg;
                        if(sideWall)
                            surfaceMove = new Vector3(0, Gamepad.current.leftStick.ReadValue().y, 0);
                        else
                            surfaceMove = new Vector3(Gamepad.current.leftStick.ReadValue().x, 0, 0);
                        break;
                    case 1:
                        moveAim.transform.eulerAngles = Vector3.right * Mathf.Atan2(-inputDir.x, inputDir.y) * Mathf.Rad2Deg;
                        if (sideWall)
                            surfaceMove = new Vector3(0, Gamepad.current.leftStick.ReadValue().y, 0);
                        else
                            surfaceMove = new Vector3(0, 0, -Gamepad.current.leftStick.ReadValue().x);
                        break;
                    case 2:
                        moveAim.transform.eulerAngles = Vector3.back * Mathf.Atan2(-inputDir.x, inputDir.y) * Mathf.Rad2Deg;
                        if (sideWall)
                            surfaceMove = new Vector3(0, Gamepad.current.leftStick.ReadValue().y, 0);
                        else
                            surfaceMove = new Vector3(-Gamepad.current.leftStick.ReadValue().x, 0, 0);
                        break;
                    case 3:
                        moveAim.transform.eulerAngles = Vector3.left * Mathf.Atan2(-inputDir.x, inputDir.y) * Mathf.Rad2Deg;
                        if (sideWall)
                            surfaceMove = new Vector3(0, Gamepad.current.leftStick.ReadValue().y, 0);
                        else
                            surfaceMove = new Vector3(0, 0, Gamepad.current.leftStick.ReadValue().x);
                        break;
                }
                if (onRope) {
                    surfaceMove = Vector3.zero;
                } else {
                    if (hasConnected) {
                        targetPos.position = transform.position;
                        if (Vector3.Distance(moveAim.GetChild(0).position, mA.position) < Vector3.Distance(moveAim.GetChild(0).position, mB.position)) {
                            //surfaceMove = (mA.position - targetPos.position) * Time.deltaTime * 15f;
                        } else {
                            //surfaceMove = (mB.position - targetPos.position) * Time.deltaTime * 15f;

                        }


                    }
                }


                //Move
            } else {
                surfaceMove = Vector3.zero;
            }

        }

        light.GetComponent<HDAdditionalLightData>().intensity = Mathf.Lerp(light.GetComponent<HDAdditionalLightData>().intensity, startLightIntensity, Time.deltaTime * 17f);

        if (worldRotationID > 3)
            worldRotationID = 0;
        if (worldRotationID < 0)
            worldRotationID = 3;

        //Debug.Log(hasSummited);
        if (gameStarted) {
            if (hasSummited) {
                summitCam.Rotate(new Vector3(0, 20, 0) * Time.smoothDeltaTime, Space.World);
                camHolder.localRotation = Quaternion.Lerp(camHolder.localRotation, summitCam.localRotation, Time.deltaTime * 5f);
                Destroy(cc);
                targetPos.position = GameObject.Find("_SummitPoint").transform.position;
                transform.position = GameObject.Find("_SummitPoint").transform.position;
            } else {
                camHolder.localRotation = Quaternion.Lerp(camHolder.localRotation, normalCam.localRotation, Time.deltaTime * 5f);
            }

            camHolder.localPosition = Vector3.Lerp(camHolder.localPosition, Vector3.zero,Time.deltaTime * 5f);
            titleUI.localPosition = Vector3.Lerp(titleUI.localPosition, new Vector3(450f, 0, 0), Time.deltaTime * 5f);
        } else {
            camHolder.localPosition = Vector3.Lerp(camHolder.localPosition, new Vector3(21.8f,0,0), Time.deltaTime * 5f);
            if (Gamepad.current.startButton.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame) {
                worldRotationID = 0;
                gameStarted = true;
            }
            titleUI.localPosition = Vector3.Lerp(titleUI.localPosition, new Vector3(0, 0, 0), Time.deltaTime * 5f);
        }

        //transform.position = Vector3.MoveTowards(transform.position, targetPos.position, Time.deltaTime * 185f);
        //rigi.velocity = Vector3.zero;
        //rigi.MovePosition(Vector3.MoveTowards(transform.position, targetPos.position, Time.deltaTime * 185f));

        bool overshoot = Vector3.Distance(transform.position, targetPos.GetChild(0).position) < Vector3.Distance(targetPos.position, targetPos.GetChild(0).position);

        if (Vector3.Distance(transform.position, targetPos.position) < 0.5f || (overshoot && !hasConnected)) {
            if (!hasConnected) {
                transform.position = targetPos.position;
                hasConnected = true;
            }
        }
        if (!hasSummited) {
            RaycastHit groundHit;
            if (Physics.Raycast(transform.position, (grv.position - transform.position).normalized, out groundHit, 2.5f)) {
                lastGroundedPos = transform.position;
                //Debug.Log("Grounded");
            } else {
                if (hasConnected) {
                    cc.enabled = false;
                    transform.position = lastGroundedPos;
                    cc.enabled = true;
                    //Debug.Log(lastGroundedPos);
                }
            }
            if (onRope) {
                transform.position = targetPos.position;
            } else {
                if (!hasConnected) {
                    //cc.Move((targetPos.position - transform.position).normalized * 137f * Time.deltaTime);
                    cc.Move(toConnectVel * 137f * Time.deltaTime);
                    currentGravity = Vector3.zero;
                } else {
                    if(sideWall && isSlip)
                        currentGravity += Vector3.down * Time.deltaTime * 5f;
                    cc.Move((surfaceMove * 50f * Time.deltaTime) + (currentGravity * Time.deltaTime));
                }
            }

            if (hasConnected) {
                graphics.transform.localEulerAngles = new Vector3(0, -90, 0);
                graphicsLook.localEulerAngles = new Vector3(0, 90, 0);
                graphics.transform.localScale = graphicsStartScale;
                graphics.transform.localPosition = new Vector3(0, 0.8599997f, 0);
            } else {
                graphics.transform.localEulerAngles = new Vector3(0, -90, -90);
                graphicsLook.transform.LookAt(lastPlayerPos);
                graphics.transform.localScale = new Vector3(graphicsStartScale.x, graphicsStartScale.y * 6f, graphicsStartScale.z);
                //graphics.transform.localPosition = new Vector3(0, 0.8599997f * -3f, 0);

            }
        } else {
            targetPos.position = GameObject.Find("_SummitPoint").transform.position;
            transform.position = GameObject.Find("_SummitPoint").transform.position;
            //graphics.transform.LookAt(Camera.main.transform.position);
            graphics.transform.rotation = pivot.rotation;
            Debug.Log("Summited");
        }

        graphics.material.SetFloat("_Offset", Mathf.Lerp(graphics.material.GetFloat("_Offset"), hasConnected ? 1f : -5f, Time.deltaTime * (hasConnected ? 5f : 29f)));
        


        if (nextLandIsRotate && Vector3.Distance(transform.position, targetPos.position) < 0.1f) {
            RotateLevel(nextRotateCol);
            nextLandIsRotate = false;
        }

        

        
    }

    private void FixedUpdate() {
        //rigi.AddForce(Vector3.down * 1300f * Time.fixedDeltaTime);
    }

    private void LateUpdate() {
        pivot.position = Vector3.Lerp(pivot.position, new Vector3(pivot.position.x, transform.position.y, pivot.position.z), Time.smoothDeltaTime * 5f);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "TurnTrigger") {
            //Debug.Log("TRIGGGG");
            RotateLevel(other);
        }
        if (other.tag == "SummitTrig") {
            hasSummited = true;
        }
        if (other.tag == "TreshTrig") {
            other.transform.parent.GetComponent<ThresholdScript>().TriggeredThresh();
        }
        if (other.tag == "DeathTrig") {
            lm.PlayerDied();
            lr.enabled = false;
            lr_2.enabled = false;
            lr_3.enabled = false;
            this.enabled = false;
            //SceneManager.LoadScene(0);
        }
        if (other.tag == "Rope") {
            Debug.Log("RP");
            if (!onRope) {
                //hasConnected = true;
                Vector3 point = other.transform.parent.GetComponent<RopeScript>().B.position;
                targetPos.position = point + (transform.position - point).normalized * 1.02f;
                targetPos.LookAt(transform.position);
                toConnectVel = (point - transform.position).normalized;
                hasConnected = false;
                lastGroundedPos = targetPos.position;
                //rigi.isKinematic = true;
                light.GetComponent<HDAdditionalLightData>().intensity = startLightIntensity * 55f;
                cc.enabled = false;
                onRope = true;
            }
            /*
            sideWall = (hit.point + hit.normal).y == hit.point.y;
            switch (worldRotationID) {
                case 0:
                    surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, sideWall ? 0 : -90);
                    break;
                case 1:
                    surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case 2:
                    surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, sideWall ? 0 : -90);
                    break;
                case 3:
                    surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                    break;
            }
            */

            //surfaceNormal.rotation = Quaternion.LookRotation(hit.normal, Camera.main.transform.up);

        }
        if (other.GetComponent<AvalancheTrigger>()) {
            AvalancheTrigger avTrig = other.GetComponent<AvalancheTrigger>();
            if (avTrig.isRandom) {
                avSystem.StartAvalanche(Random.Range(1,4),5f,40f);
            } else {
                if(avTrig.left)
                    avSystem.StartAvalanche(1, avTrig.lifetime, avTrig.speed);
                if (avTrig.middle)
                    avSystem.StartAvalanche(2,avTrig.lifetime, avTrig.speed);
                if (avTrig.right)
                    avSystem.StartAvalanche(3,avTrig.lifetime,avTrig.speed);
            }
        }
    }

    void RotateLevel(Collider other) {
        if (Vector3.Distance(Camera.main.transform.position, transform.position) > 173.8879f) {
            //Left trigger
            worldRotationID++;
            surfaceNormal.Rotate(Vector3.up * -90f, Space.World);
        } else {
            worldRotationID--;
            surfaceNormal.Rotate(Vector3.up * 90f, Space.World);

        }
        /*
        switch (worldRotationID) {
            case 0:
                surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, -90);
                break;
            case 1:
                surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                break;
            case 2:
                surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, -90);
                break;
            case 3:
                surfaceNormal.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                break;
        }
        */
        transform.position = new Vector3(other.transform.GetChild(0).position.x, transform.position.y, other.transform.GetChild(0).position.z);
        targetPos.position = new Vector3(other.transform.GetChild(0).position.x, transform.position.y, other.transform.GetChild(0).position.z);
        rotDelayPos = new Vector3(other.transform.GetChild(0).position.x, transform.position.y, other.transform.GetChild(0).position.z);
        cc.enabled = false;
        //Debug.LogError(transform.position.x.ToString() + "*" + transform.position.z.ToString());
        //Debug.LogError(other.transform.GetChild(0).position.x.ToString() + "*" + other.transform.GetChild(0).position.z.ToString());

        tm.RefreshTriggers(true);
        stillInTrigger = true;
        rotDelay = 0.2f;
    }
}
