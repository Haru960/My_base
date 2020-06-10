using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour {

    public void DisableBoolAnimator(Animator anim)
    {
        anim.SetBool("IsDisplayed", false);
    }

    public void EnableBoolAnimator(Animator anim)
    {
        anim.SetBool("IsDisplayed", true);
    }

    public void DisableOptionBool(Animator anim)
    {
        anim.SetBool("IsDisplayed2", false);
    }
    public void EnableOptionBool(Animator anim)
    {
        anim.SetBool("IsDisplayed2", true);
    }

    public void NavigateTo(int scene)
    {
        Application.LoadLevel(scene);
    }

    public void ExitGame()
    {
          Application.Quit();
        
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void UnPauseGame()
    {
        Time.timeScale = 1;
    }
    
}
