using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Update Hp Event (Unity Envet 생성)
[System.Serializable]
public class HpEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    #region Variables
    [Header("Walk & Run Speed")]
    [SerializeField]
    private float walkSpeed = 2.0f;
    [SerializeField]
    private float runSpeed = 3.5f;

    public float WalkSpeed { get { return walkSpeed; } }
    public float RunSpeed { get { return runSpeed; } }

    [Header("Health")]
    [SerializeField]
    private int maxHp = 100;
    private int currentHp;

    //Update Hp Envet
    [HideInInspector]
    public HpEvent onHpEvent = new HpEvent();
    #endregion

    void Awake()
    {
        currentHp = maxHp;
    }

    public void  DecreaseHp(int decreaseHp)
    {
        int previousHP = currentHp;
        //체력감소
        if(currentHp - decreaseHp > 0)
        {
            currentHp -= decreaseHp;
        }
        //사망
        else
        {
            currentHp = 0;
            Debug.Log("사망!");
        }

        //onHPEvent에 등록된 모든 객체의 메소드를 호출
        onHpEvent.Invoke(previousHP, currentHp);
    }


}
