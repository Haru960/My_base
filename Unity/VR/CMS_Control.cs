using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMS_Control : MonoBehaviour
{
    IntroManager introManager;
    GameManager gameManager;
    int curScene;

    void Awake()
    {
        curScene = SceneManager.GetActiveScene().buildIndex;

        if (curScene == 0) introManager = GetComponent<IntroManager>();
        else if (curScene == 1) gameManager = GetComponent<GameManager>();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        CardboardMagnetSensor.SetEnabled(true);
    }
    void Update()
    {
        if (CardboardMagnetSensor.CheckIfWasClicked() || Input.GetMouseButtonDown(0))
        {
            // Magnet Trigger가 on 처리 되었을 때 처리
            if (curScene == 0) introManager.IsOnMagnet = true;
            else if (curScene == 1) gameManager.IsOnMagnet = true;

            CardboardMagnetSensor.ResetClick();
        }
    }
}
