using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public bool dead = false;
    public Renderer pinholeRend;
    public Transform player;
    float deathDelay = 1.5f;

    int currentSegment = 0;

    public int DebuglevelID = 0;
    public int levelID = 0;
    public int levelHeight = 3;


    public Renderer background;
    public Renderer floorRend;
    public TextMeshProUGUI titleFG;
    public TextMeshProUGUI titleBG;

    public List<string> levelTitles;
    public List<Color> blizzardColours;
    public List<Material> mountainMaterials;
    public List<Material> backgroundMaterials;
    public List<int> levelLengths;

    public List<GameObject> allLevelSegments;
    public List<GameObject> allSummitSegments;
    public Transform allSegmentsStart;

    public List<int> meterHeights;

    GameObject lastEndRot;
    public Transform finishTrigger;
    bool hasFinishedGeneration = false;

    public Transform blizzardHolder;
    public GameObject heightThresh;

    public List<RawImage> tutorialUI;
    bool firstTrigger = false;
    int currentM = 250;

    //Vector3 currentSegmentHeight;
    void Start()
    {
        levelID = RuntimeVariables.currentLevel;
        //levelID = DebuglevelID;

        if (RuntimeVariables.endlessMode) {
            levelHeight = 9999;
            titleFG.text = "ENDLESS";
            titleBG.text = "ENDLESS";
        } else {
            levelHeight = levelLengths[levelID];
            titleFG.text = levelTitles[levelID] + meterHeights[levelID].ToString() + "m";
            titleBG.text = levelTitles[levelID] + meterHeights[levelID].ToString() + "m";
        }

        currentM = meterHeights[levelID];
        foreach (TextMeshPro tmp in heightThresh.GetComponentsInChildren<TextMeshPro>()) {
            tmp.text = currentM.ToString() + "m";
        }

        //*****
        //levelHeight = 2;

        background.material = backgroundMaterials[levelID];
        floorRend.material = mountainMaterials[levelID];
        GenerateSegment();


        foreach (VisualEffect vfx in blizzardHolder.GetComponentsInChildren<VisualEffect>()) {
            vfx.SetVector4("c1", blizzardColours[levelID]);
            vfx.SetVector4("c2", blizzardColours[levelID]);
        }
        blizzardHolder.Find("BlizzardFog").GetComponent<Renderer>().material.color = new Color(blizzardColours[levelID].r, blizzardColours[levelID].g, blizzardColours[levelID].b, 0.67f);
        foreach (SphereCollider sc in blizzardHolder.GetComponentsInChildren<SphereCollider>()) {
            sc.GetComponent<Renderer>().material.SetColor("_Colour", blizzardColours[levelID] * 0.8f);
        }
        foreach (CapsuleCollider cap in blizzardHolder.GetComponentsInChildren<CapsuleCollider>()) {
            cap.GetComponent<Renderer>().material.SetColor("_Colour", blizzardColours[levelID]);
        }


        foreach (RawImage RI in tutorialUI) {
            RI.color = new Color(0, 0, 0, 0);
            RI.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        

        if(levelID == 0) {
            foreach (RawImage RI in tutorialUI) {
                RI.color = Color.Lerp(RI.color, new Color(1, 1, 1, (firstTrigger || !player.GetComponent<PlayerLocomotion>().gameStarted) ? 0f : 1f), Time.unscaledDeltaTime * 14f);
            }
        }

        if (player.GetComponent<PlayerLocomotion>().summitDelay <= 0) {
            pinholeRend.transform.localScale = Vector3.one * 0.963f;
            pinholeRend.material.SetFloat("_Scan", Mathf.MoveTowards(pinholeRend.material.GetFloat("_Scan"), 1f, Time.unscaledDeltaTime * 3f));

            deathDelay -= Time.unscaledDeltaTime;
            if (deathDelay < 0f) {
                SceneManager.LoadScene(0);
            }
        } else {
            if (dead) {
                pinholeRend.transform.localScale = Vector3.one * 0.963f;
                pinholeRend.material.SetFloat("_Scan", Mathf.MoveTowards(pinholeRend.material.GetFloat("_Scan"), 1f, Time.unscaledDeltaTime * 3f));

                deathDelay -= Time.unscaledDeltaTime;
                if (deathDelay < 0f) {
                    SceneManager.LoadScene(1);
                }
            }
        }
        

        if (Keyboard.current.gKey.wasPressedThisFrame) {
            //GameObject loadedSegment = GameObject.Find("Seg_1");

            GenerateSegment();

            //GameObject foundSegment = GameObject.Find("Seg_" + currentSegment.ToString());
            //GameObject newSegment = Instantiate(loadedSegment, new Vector3(foundSegment.transform.position.x, foundSegment.transform.position.y + 188.4f,foundSegment.transform.position.z), foundSegment.transform.Find("StartRotation").transform.rotation);
            //newSegment.name = "Seg_" + (currentSegment+1).ToString();
            //currentSegment++;
            //Debug.Log(currentSegment);
        }
        if (Keyboard.current.hKey.wasPressedThisFrame) {
            //GameObject loadedSegment = GameObject.Find("Seg_1");
            
        }
    }
    
    public void PlayerDied() {
        pinholeRend.transform.parent.position = player.position;
        GetComponent<AvalancheSystem>().StopAll();
        dead = true;
    }

    public void FirstTrigger() {
        firstTrigger = true;
    }

    public void GenerateSegment() {
        if (1 == 1) {
            if (currentSegment >= levelHeight) {
                if (!hasFinishedGeneration) {
                    GameObject loadedSegment = allSummitSegments[levelID];//levelID


                    GameObject newSegment = Instantiate(loadedSegment, new Vector3(allSegmentsStart.position.x + 4.57006f, allSegmentsStart.transform.position.y + (188.4f * currentSegment) - 10f, allSegmentsStart.position.z + 4.711397f), allSegmentsStart.rotation);
                    newSegment.transform.Find("Mesh").GetComponent<Renderer>().material = mountainMaterials[levelID];

                    newSegment.name = "Seg_" + (currentSegment + 1).ToString();
                    currentSegment++;

                    finishTrigger.transform.position = new Vector3(finishTrigger.transform.position.x, newSegment.transform.position.y + 21.5f, finishTrigger.transform.position.z);
                    hasFinishedGeneration = true;
                }
            } else {
                if(currentSegment != levelHeight) {
                    currentM += 250;
                    GameObject newThresh = Instantiate(heightThresh, new Vector3(heightThresh.transform.position.x, heightThresh.transform.position.y + (188.4f * (currentSegment+1)), heightThresh.transform.position.z), heightThresh.transform.rotation);

                    foreach (TextMeshPro tmp in newThresh.GetComponentsInChildren<TextMeshPro>()) {
                        tmp.text = currentM.ToString() + "m";
                    }
                }

                int segID = 0;
                if (levelID == 0) {
                    segID = currentSegment;
                } else {
                    segID = Random.Range(1,13);
                }

                //segID = 2;

                
                GameObject loadedSegment = allLevelSegments[segID];

                GameObject newSegment = Instantiate(loadedSegment, new Vector3(allSegmentsStart.position.x, allSegmentsStart.transform.position.y + (188.4f * currentSegment), allSegmentsStart.position.z), allSegmentsStart.rotation);
                newSegment.transform.Find("Mesh").GetComponent<Renderer>().material = mountainMaterials[levelID];

                GameObject startRot = newSegment.transform.Find("StartRotation").transform.gameObject;
                GameObject endRot = newSegment.transform.Find("EndRotation").transform.gameObject;
                //endRot.transform.Rotate(Vector3.up * 180f, Space.World);
                startRot.transform.localPosition += new Vector3(0.015f / 3f, 0, 0.015f / 3f);
                endRot.transform.localPosition += new Vector3(0.015f / 3f, 0, 0.015f / 3f);

                startRot.transform.parent = null;

                newSegment.transform.parent = startRot.transform;
                if (lastEndRot != null) {
                    //lastEndRot.transform.Rotate(Vector3.up * 180f, Space.World);
                    startRot.transform.rotation = lastEndRot.transform.rotation;

                } else {
                    startRot.transform.rotation = allSegmentsStart.rotation;
                }
                endRot.transform.parent = null;

                //lastEndRot = newSegment.transform.Find("EndRotation").gameObject;//EndRot
                lastEndRot = endRot;//EndRot
                newSegment.name = "Seg_" + (currentSegment + 1).ToString();
                currentSegment++;
            }


        }
    }
}
