using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour {


    //몬스터에 대해 새로 정의할 클릭버튼
   

    public Text MonsterDisplayer;
    public Text StageDisplayer;
    public Slider MonsterHp;

    public int Game_stage = 1;

    public int startGame_stage = 1;

    public int MonsterHealt;
    public int MonsterMaxHP;

    public int startMonsterHealt = 100;

    public int StartMM = 500;

    public float costPow = 2.25f;

    //몬스터 체력의 증가량
    public float MonsterPow = 1.5f;

    void Start()
    {
        Game_stage = DataController.Instance.getStage();
           
        MonsterHealt = MonsterStagePerHealt();
        MonsterMaxHP = MonsterStagePerHealt();
           

        UpdateUI();
    }

    public void OneClick1()
    {
        MonsterHealt -= DataController.Instance.goldPerClick;
        UpdateUI();
        if(MonsterHealt <= 0)
        {
                //몬스터가 죽을때마다 스테이지당 몬스터 피가 늘어나는 함수
            Game_stage++;
            MonsterHealt = MonsterStagePerHealt();
            MonsterMaxHP = MonsterHealt;
            DataController.Instance.setStage(Game_stage);
            DataController.Instance.gold +=  StartMM * (int)Mathf.Pow(costPow, Game_stage);

            UpdateUI();
        }
    }
        //몬스터가 죽을때마다 스테이지당 몬스터 피가 늘어나는 함수
    public int MonsterStagePerHealt()
    {
        int NomalHealt;
        int TenHealt;
        if (Game_stage % 10 == 0)
        {
            TenHealt = startMonsterHealt * (int)Mathf.Pow(MonsterPow, Game_stage)*2;
             
            return TenHealt;
        }
        else
        {
            NomalHealt = startMonsterHealt * (int)Mathf.Pow(MonsterPow, Game_stage);
           
           return NomalHealt;
        }
    }

    public void UpdateUI()
    {
        MonsterDisplayer.text = "체력 : " + (float)MonsterMaxHP;
        StageDisplayer.text = "현재 스테이지 : " + Game_stage;
        MonsterHp.value = (float)MonsterHealt / (float)MonsterMaxHP;
    }
}
