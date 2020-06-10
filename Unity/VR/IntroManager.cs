using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public bool             IsOnMagnet { set; get; }
    public ButtonTrigger Target { set; get; }

    void Awake()
    {
        IsOnMagnet = false;
        Target = null;
    }
    
    void Update()
    {
        if(IsOnMagnet)
        {
            if (Target != null)
            {
                if (Target.name.Equals("Btn_GameStart"))      Target.Btn_GameStart();
                else if (Target.name.Equals("Btn_GameExit")) Target.Btn_GameExit();
            }
            IsOnMagnet = false;
        }
    }
}
