using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour {

    public Text itemDisplayer;
    
    
    
    //초당 돈올려주는 아이템 함수
    public string itemName;

    public int level=0;

    [HideInInspector]
    //아이템 가격
    public int currentCost;
    //아이템 시작가격
    public int startCurrentCost = 1;

    //1초에 골드 증가량
    [HideInInspector]
    public int goldPerSec;
    //처음 골드 증가량
    public int startGoldPerSec = 1;

    //아이템 가격 배수
    public float costPow = 1.25f;
    //아이템 업그레이드당 파워 배수
    public float upgradePow = 1.25f;

    //아이템을 구매했었는지 안했었는지 체크하는 변수
    [HideInInspector]
    public bool isPurchased = false;
    
    //대소문자 조심
    void Start()
    {
        DataController.Instance.LoadItemButton(this);

        StartCoroutine("AddGoldLoop");

        UpdateUI();
    }

    public void PurchaseItem()
    {
        if (DataController.Instance.gold >= currentCost)
        {
            isPurchased = true;
            DataController.Instance.gold -= currentCost;
            level++;

            UpdateItem();
            UpdateUI();

            DataController.Instance.SaveItemButton(this);
        }
    }

    //yield 문과 함께쓰는 함수(초당 골드가 증가하는 함수
   IEnumerator AddGoldLoop()
    {
        while (true)
        {
            if (isPurchased)
            {
                DataController.Instance.gold += goldPerSec;
            }
            //1초동안 대기시간이 걸리도록 함
            yield return new WaitForSeconds(1.0f);
        }
    }

    //업그레이드마다 초당골드가 올라가도록하는 함수,가격과함께
    public void UpdateItem()
    {
        goldPerSec = startGoldPerSec * (int)Mathf.Pow(upgradePow, level);
        currentCost = startCurrentCost * (int)Mathf.Pow(costPow,level);
    }
    //UI 업데이트
    public void UpdateUI()
    {
        itemDisplayer.text = itemName + "\nlevel : " + level + "\nCost : " + currentCost + "\nGold Per Sec : " + goldPerSec
            + "\nIsPurchased : " + isPurchased;
    }
   
}
