using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MainMenu : MonoBehaviour
{
    int completedLevels = 5;

    public List<Renderer> backgrounds;
    int selectedLevel = 0;

    public List<Transform> levelMenuItems;
    public List<Transform> mainMenuItems;

    public List<Color> savedColours;
    public List<Vector3> savedPositions;
    public List<Vector3> savedPositionsMain;


    bool levelChosen = false;

    public Transform loadingBar;
    public RawImage loadingBarRI;
    public TextMeshProUGUI loadingTMP;
    float loadProgress = 0;

    int menuID = 0;

    int selectedMainMenu = 0;

    float splashDelay = 1.5f;
    public RawImage splashBG;
    public RawImage uon_Logo;
    public Renderer bg_cover;

    bool loadingScene = false;

    int selectedDiff = 3;

    public GameObject creditsPage;
    public GameObject logo;

    void Start()
    {
        //LoadProgress

        savedColours = new List<Color>();
        savedPositions = new List<Vector3>();
        savedPositionsMain = new List<Vector3>();

        for (int i = 0; i < levelMenuItems.Count; i++) {
            savedColours.Add(levelMenuItems[i].Find("Fade").GetComponent<RawImage>().color);
            savedPositions.Add(levelMenuItems[i].Find("Text").transform.position);
        }
        for (int i = 0; i < mainMenuItems.Count; i++) {
            savedPositionsMain.Add(mainMenuItems[i].Find("Text").transform.position);
        }

        if (RuntimeVariables.firstBoot) {
            RuntimeVariables.firstBoot = false;
        } else {
            menuID = 1;
        }
    }

    bool movedStick = false;

    void Update()
    {
        logo.SetActive(menuID == 0);

        bool enableInput = !splashBG.transform.parent.gameObject.activeSelf;
        completedLevels = (int)PlayerPrefs.GetFloat("CompletedLevel");
        //Debug.Log(PlayerPrefs.GetFloat("CompletedLevel"));

        if (!RuntimeVariables.splashShown) {
            if (splashDelay > 0)
                splashDelay -= Time.deltaTime;
            else {
                splashBG.color = Color.Lerp(splashBG.color, new Color(0, 0, 0, 0f), Time.deltaTime * 5f);
                uon_Logo.color = Color.Lerp(splashBG.color, new Color(1, 1, 1, 0f), Time.deltaTime * 5f);

                if (uon_Logo.color.a < 0.005f) {
                    splashBG.transform.parent.gameObject.SetActive(false);
                    RuntimeVariables.splashShown = true;
                }
            }
        } else {
            splashBG.transform.parent.gameObject.SetActive(false);
        }

        float deadzone = 0.2f;
        if(Gamepad.current.leftStick.ReadValue().x < deadzone && Gamepad.current.leftStick.ReadValue().x > -deadzone
            && Gamepad.current.leftStick.ReadValue().y < deadzone && Gamepad.current.leftStick.ReadValue().y > -deadzone) {
            if(enableInput)
                movedStick = false;
            else
                movedStick = true;
        }

        bg_cover.enabled = menuID == 1;

        if(Gamepad.current.leftShoulder.ReadValue() == 1 && Gamepad.current.buttonWest.ReadValue() == 1 && Gamepad.current.buttonNorth.ReadValue() == 1) {
            PlayerPrefs.SetFloat("CompletedLevel", 9);
        }

        if (levelChosen) {
            loadingBarRI.enabled = true;
            Color itemColour = levelMenuItems[selectedLevel].Find("Fade").GetComponent<RawImage>().color;
            loadingBarRI.color = new Color(itemColour.r, itemColour.g, itemColour.b, 1f);
            if (loadProgress < 1) {
                loadProgress += Time.deltaTime * 1.05f;
                loadProgress = Mathf.Lerp(loadProgress, 1f, Time.deltaTime * 1.15f);
            } else {
                loadProgress = 1f;
                if (!loadingScene) {
                    SceneManager.LoadScene(1);
                    loadingScene = true;
                }
            }
            //Debug.LogError(loadingBar.transform.localPosition); //(-960.0, -540.0, 0.0)
            //loadingBar.transform.localPosition = new Vector3(0, 1080 / (loadProgress + 0.001f), 0);
            //Debug.LogError(loadingBar.transform.GetComponent<RectTransform>().localPosition);
            loadingBar.transform.GetComponent<RectTransform>().localPosition = new Vector3(-960.0f, Mathf.Lerp(-540f,540f, (loadProgress + 0.001f)), 0.0f);
            int prog = (int)(loadProgress * 100);
            loadingTMP.text = prog.ToString() + "%";
        } else {
            loadingBarRI.enabled = false;

            if(menuID == 0) {
                creditsPage.SetActive(selectedMainMenu == 5);
                

                if (Gamepad.current.leftStick.ReadValue().y > deadzone && !movedStick && enableInput) {
                    //Up
                    selectedMainMenu--;
                    movedStick = true;
                }
                if (Gamepad.current.leftStick.ReadValue().y < -deadzone && !movedStick && enableInput) {
                    //Down
                    selectedMainMenu++;
                    movedStick = true;
                }

                if (Gamepad.current.buttonSouth.wasPressedThisFrame && enableInput) {
                    if(selectedMainMenu == 0) {
                        menuID = 1;
                        RuntimeVariables.endlessMode = false;
                    }
                    if (selectedMainMenu == 1) {
                        menuID = 1;
                        RuntimeVariables.endlessMode = true;
                    }
                    if (selectedMainMenu == 2) {
                        selectedDiff = 2;
                        RuntimeVariables.difficulty = 1;
                    }
                    if (selectedMainMenu == 3) {
                        selectedDiff = 3;
                        RuntimeVariables.difficulty = 2;
                    }
                    if (selectedMainMenu == 4) {
                        selectedDiff = 4;
                        RuntimeVariables.difficulty = 3;
                    }
                    if (selectedMainMenu == 5) {
                        //Credits
                    }
                    if (selectedMainMenu == 6) {
                        Application.Quit();
                    }
                }
            } else if(menuID == 1) {
                if (Gamepad.current.leftStick.ReadValue().y > deadzone && !movedStick && enableInput) {
                    //Up
                    selectedLevel--;
                    movedStick = true;
                }
                if (Gamepad.current.leftStick.ReadValue().y < -deadzone && !movedStick && enableInput) {
                    //Down
                    selectedLevel++;
                    movedStick = true;
                }

                if (Gamepad.current.buttonSouth.wasPressedThisFrame && enableInput) {
                    bool locked = !(completedLevels - 1 >= selectedLevel) && selectedLevel != 0;
                    levelChosen = !locked;
                    RuntimeVariables.currentLevel = selectedLevel;
                }
                if (Gamepad.current.buttonEast.wasPressedThisFrame && enableInput) {
                    menuID = 0;
                }
            }

            

            if (selectedLevel > backgrounds.Count - 1)
                selectedLevel = backgrounds.Count - 1;
            else if (selectedLevel < 0)
                selectedLevel = 0;

            if (selectedMainMenu > mainMenuItems.Count - 1)
                selectedMainMenu = mainMenuItems.Count - 1;
            else if (selectedMainMenu < 0)
                selectedMainMenu = 0;

            

            for (int i = 0; i < backgrounds.Count; i++) {
                if(completedLevels-1 >= i) {
                    backgrounds[i].material.SetFloat("_Fade", Mathf.Lerp(backgrounds[i].material.GetFloat("_Fade"), (i == selectedLevel && menuID == 1) ? 0f : 1f, Time.deltaTime * 8f));
                } else {
                    backgrounds[i].material.SetFloat("_Fade", Mathf.Lerp(backgrounds[i].material.GetFloat("_Fade"), (i == selectedLevel && menuID == 1) ? 0.65f : 1f, Time.deltaTime * 8f));
                }
            }
        }

        for (int i = 0; i < levelMenuItems.Count; i++) {
            bool locked = !(completedLevels - 1 >= i) && i != 0;
            if (locked && menuID == 1) {
                levelMenuItems[i].Find("Lock").GetComponent<RawImage>().enabled = true;
            } else {
                levelMenuItems[i].Find("Lock").GetComponent<RawImage>().enabled = false;
            }
            if (i == selectedLevel && menuID == 1) {
                levelMenuItems[i].Find("Fade").GetComponent<RawImage>().color = Color.Lerp(levelMenuItems[i].Find("Fade").GetComponent<RawImage>().color, savedColours[i], Time.deltaTime * 7f);
                levelMenuItems[i].Find("Fade").transform.localScale = Vector3.Lerp(levelMenuItems[i].Find("Fade").localScale, new Vector3(levelChosen ? 15f : 2.69f, 1f, 1f), Time.deltaTime * 6f);

                levelMenuItems[i].Find("Text").GetComponent<TextMeshProUGUI>().color = Color.Lerp(levelMenuItems[i].Find("Text").GetComponent<TextMeshProUGUI>().color, new Color(1, 1, 1, locked ? 0f : 1), Time.deltaTime * 7f);
                levelMenuItems[i].Find("Text").transform.position = Vector3.Lerp(levelMenuItems[i].Find("Text").transform.position, savedPositions[i] + new Vector3(150f, 0, 0), Time.deltaTime * 10f);

                levelMenuItems[i].Find("TextBG").GetComponent<TextMeshProUGUI>().color = Color.Lerp(levelMenuItems[i].Find("TextBG").GetComponent<TextMeshProUGUI>().color, new Color(0, 0, 0, locked ? 0f : 0.41f), Time.deltaTime * 7f);
                levelMenuItems[i].Find("TextBG").transform.position = levelMenuItems[i].Find("Text").position + new Vector3(10, -10, 0);
            } else {
                levelMenuItems[i].Find("Fade").GetComponent<RawImage>().color = Color.Lerp(levelMenuItems[i].Find("Fade").GetComponent<RawImage>().color, new Color(0, 0, 0, 0), Time.deltaTime * 7f);
                levelMenuItems[i].Find("Fade").transform.localScale = Vector3.Lerp(levelMenuItems[i].Find("Fade").localScale, new Vector3(2.69f, 0.01f, 0.01f), Time.deltaTime * 6f);

                levelMenuItems[i].Find("Text").GetComponent<TextMeshProUGUI>().color = Color.Lerp(levelMenuItems[i].Find("Text").GetComponent<TextMeshProUGUI>().color, new Color(1, 1, 1, locked ? 0f : 0.1f), Time.deltaTime * 7f);
                if(menuID == 1)
                    levelMenuItems[i].Find("Text").transform.position = Vector3.Lerp(levelMenuItems[i].Find("Text").transform.position, savedPositions[i] + (levelChosen ? new Vector3(-150f,0,0) : Vector3.zero), Time.deltaTime * 10f);
                else
                    levelMenuItems[i].Find("Text").transform.position = Vector3.Lerp(levelMenuItems[i].Find("Text").transform.position, savedPositions[i] + new Vector3(-600,0,0), Time.deltaTime * 10f);

                levelMenuItems[i].Find("TextBG").GetComponent<TextMeshProUGUI>().color = Color.Lerp(levelMenuItems[i].Find("TextBG").GetComponent<TextMeshProUGUI>().color, new Color(0, 0, 0, locked ? 0f : 0.72f ), Time.deltaTime * 7f);
                levelMenuItems[i].Find("TextBG").transform.position = levelMenuItems[i].Find("Text").position + new Vector3(0, 0, 0);

            }
        }
        
        for (int i = 0; i < mainMenuItems.Count; i++) {
            if(i == selectedMainMenu && menuID == 0) {
                mainMenuItems[i].Find("Fade").GetComponent<RawImage>().color = Color.Lerp(mainMenuItems[i].Find("Fade").GetComponent<RawImage>().color, savedColours[i], Time.deltaTime * 7f);
                mainMenuItems[i].Find("Fade").transform.localScale = Vector3.Lerp(mainMenuItems[i].Find("Fade").localScale, new Vector3(levelChosen ? 15f : 2.69f, 1f, 1f), Time.deltaTime * 6f);

                mainMenuItems[i].Find("Text").GetComponent<TextMeshProUGUI>().color = Color.Lerp(mainMenuItems[i].Find("Text").GetComponent<TextMeshProUGUI>().color, new Color(1, 1, 1, 1), Time.deltaTime * 7f);
                mainMenuItems[i].Find("Text").transform.position = Vector3.Lerp(mainMenuItems[i].Find("Text").transform.position, savedPositionsMain[i] + new Vector3(25f, 0, 0), Time.deltaTime * 10f);

                mainMenuItems[i].Find("TextBG").GetComponent<TextMeshProUGUI>().color = Color.Lerp(mainMenuItems[i].Find("TextBG").GetComponent<TextMeshProUGUI>().color, new Color(0, 0, 0, 0.41f), Time.deltaTime * 7f);
                mainMenuItems[i].Find("TextBG").transform.position = mainMenuItems[i].Find("Text").position + new Vector3(10, -10, 0);
            } else {
                mainMenuItems[i].Find("Fade").GetComponent<RawImage>().color = Color.Lerp(mainMenuItems[i].Find("Fade").GetComponent<RawImage>().color, new Color(0, 0, 0, 0), Time.deltaTime * 7f);
                mainMenuItems[i].Find("Fade").transform.localScale = Vector3.Lerp(mainMenuItems[i].Find("Fade").localScale, new Vector3(2.69f, 0.01f, 0.01f), Time.deltaTime * 6f);

                mainMenuItems[i].Find("Text").GetComponent<TextMeshProUGUI>().color = Color.Lerp(mainMenuItems[i].Find("Text").GetComponent<TextMeshProUGUI>().color, new Color(1, 1, 1, i == selectedDiff ? 1f : 0.1f), Time.deltaTime * 7f);
                if (menuID == 0)
                    mainMenuItems[i].Find("Text").transform.position = Vector3.Lerp(mainMenuItems[i].Find("Text").transform.position, savedPositionsMain[i] + (levelChosen ? new Vector3(-150f, 0, 0) : Vector3.zero), Time.deltaTime * 10f);
                else
                    mainMenuItems[i].Find("Text").transform.position = Vector3.Lerp(mainMenuItems[i].Find("Text").transform.position, savedPositionsMain[i] + new Vector3(-600, 0, 0), Time.deltaTime * 10f);

                mainMenuItems[i].Find("TextBG").GetComponent<TextMeshProUGUI>().color = Color.Lerp(mainMenuItems[i].Find("TextBG").GetComponent<TextMeshProUGUI>().color, new Color(0, 0, 0, 0.72f), Time.deltaTime * 7f);
                mainMenuItems[i].Find("TextBG").transform.position = mainMenuItems[i].Find("Text").position + new Vector3(0, 0, 0);
            }
        }
        
    }
}
