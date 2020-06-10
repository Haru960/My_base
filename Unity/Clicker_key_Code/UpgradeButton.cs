using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour {
    //업그레이드 내용을 화면에 텍스트 띄움
    public Text upgradeDisplayer;
  

    public string upgradeName;

    //퍼블릭 수치들을 유니티 인스펙터에 안보이게해주는 놈
    [HideInInspector]
    //처음 이후 업그레이드 폭넓히는 수
    public int goldByUpgrade;
    //맨처음 업그레이드
    public int startGoldByUpgrade = 2;

    [HideInInspector]
    //업그레이드 진행도
    public int level = 1;

    [HideInInspector]
    //업그레이드에 드는 비용 설정 값
    public int currentCost = 1;
    //처음 업그레이드에 드는 비용 설정 값
    public int startCurrentCost = 1;

    //업그레이드에 배 수가 되는 상수 값
    public float upgradePow = 1.07f;

    public float costPow = 3.14f;



    //얘는 awake 함수보다 한박자 느리게 함 (게임 시작시 자동 실행)
    void Start()
    {
        //업그레이드 데이터 가져오기
        DataController.Instance.LoadUpgradeButton(this);

        UpdateUI();
    }

    //업그레이드
    public void PurchaseUpgrade()
    {
        if (DataController.Instance.gold >= currentCost)
        {
            DataController.Instance.gold -= currentCost;
            level++;
            DataController.Instance.goldPerClick += goldByUpgrade;

            //업글할때마다 업그레이드 값이 올라가게 만듬
            UpdateUpgrade();
            UpdateUI();
            //업그레이드 내용 저장하는부분임 (업그레이드 버튼 눌러 업그레이드 할때마다 발동)
            DataController.Instance.SaveUpgradeButton(this);
        }
    }
    //가격과 업그레이드값을 올려줌
    public void UpdateUpgrade()
    {
        goldByUpgrade = startGoldByUpgrade * (int)Mathf.Pow(upgradePow, level);
        currentCost = startCurrentCost * (int)Mathf.Pow(costPow, level);
    }

    public void UpdateUI()
    {
        upgradeDisplayer.text = upgradeName + "\nCost :" + currentCost + "\nLevel : " + level 
            + "\nNext New GoldPerClick : " + goldByUpgrade;
    }

    
}
