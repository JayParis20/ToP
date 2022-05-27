using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeVariables : MonoBehaviour
{
    public static int currentLevel = 0;
    public static int gameProgress = 0;
    public static bool splashShown = false;

    public static bool endlessMode = false;
    public static int difficulty = 2;

    public static bool firstBoot = true;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.SetFloat("CompletedLevel", 1);

        if (!PlayerPrefs.HasKey("CompletedLevel")) {
            PlayerPrefs.SetFloat("CompletedLevel",1);
        }
        //gameProgress = (int)PlayerPrefs.GetFloat("CompletedLevel");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
