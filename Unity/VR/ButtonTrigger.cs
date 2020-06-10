using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonTrigger : EventTrigger
{
    IntroManager introManager;

    void Awake()
    {
        introManager = GameObject.Find("IntroManager").GetComponent<IntroManager>();
    }

    public void Btn_GameStart()
    {
        SceneManager.LoadScene("Game");
    }
    public void Btn_GameExit()
    {
        Application.Quit();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        introManager.Target = this;
        Debug.Log("Pointer Enter");
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (introManager.Target == this) introManager.Target = null;
        Debug.Log("Pointer Exit");
    }
}
