using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsOnMagnet { get; set; }
    public Box Target { set; get; }

    BoxManager boxManager;
    bool isGameOver;

    void Awake()
    {
        GameObject
        IsOnMagnet = null;
        Target = null;
        boxManager = GameObject.Find("BoxManager").GetComponent<BoxManager>();
        isGameOver = false;
    }
    void Update()
    {
        if (IsOnMagnet)
        {
            if(isGameOver)
            {
                SceneManager.LoadScene("Intro");
                return;
            }
            
            if (Target != null)
            {
                Target.Change_Box_Color();
                CompareBoxs();
            }
            IsOnMagnet = false;
        
        }
    }
    void CompareBoxs()
    {
        Box[] boxs = boxManager.BoxArr;
        UIBox[] uiboxs = boxManager.UIBoxArr;

        int count = 0;
        for(int i = 0; i < boxs.Length; ++i)
        {
            if (boxs[i].BoxColor == uiboxs[i].BoxColor)
                count++;

        }
        Debug.Log("맞은 개수 : " + count);

        if (count == boxs.Length)
            isGameOver = true;
    }
}
