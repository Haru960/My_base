using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour {

    private static DataController instance;

    //레퍼런스 참조없이 어디서든 접근 가능하게 만드는 함수
    public static DataController Instance
    {
        get{
            if (instance == null)
            {
                instance = FindObjectOfType<DataController>();

                if (instance == null)
                {
                    GameObject container = new GameObject("DataController");

                    instance = container.AddComponent<DataController>();
                }
            }
            return instance;
        }
    }

    private ItemButton[] itemButtons;

    public long gold
    {
        get
        {
            if(!PlayerPrefs.HasKey("Gold"))
            {
                return 0;
            }
            string tmpGold =  PlayerPrefs.GetString("Gold");
            return long.Parse(tmpGold);
            
        }
        set
        {
            PlayerPrefs.SetString("Gold", value.ToString());
        }
    }

    public int goldPerClick
    {
        get
        {
            return PlayerPrefs.GetInt("GoldPerClick",1);
        }
        set
        {
            PlayerPrefs.SetInt("GoldPerClick", value);
        }
    }

    private int g_stage;



    void Awake()
    {
        //로컬에 저장된 골드량과 클릭당 골드 수급량 저장
        g_stage = PlayerPrefs.GetInt("Stage", 1);

        itemButtons = FindObjectsOfType<ItemButton>();
    }
    //로컬에 게임 스테이지를 가져오고 저장하는 함수
    public void setStage(int newStage)
    {
        g_stage = newStage;
        PlayerPrefs.SetInt("Stage",g_stage);
    }
    public int getStage()
    {
        return g_stage;
    }

    //로컬에 쌓은 골드와 골드 수급량을 저장하는 함수
    

    
    
    //업그레이드 버튼 내용 로드
    public void LoadUpgradeButton(UpgradeButton upgradeButton)
    {
        string key = upgradeButton.upgradeName;

        upgradeButton.level = PlayerPrefs.GetInt(key+"_level",1);
        upgradeButton.goldByUpgrade = PlayerPrefs.GetInt(key + "_goldByUpgrade", upgradeButton.startGoldByUpgrade);
        upgradeButton.currentCost = PlayerPrefs.GetInt(key + "_cost", upgradeButton.startCurrentCost);
    }
    //업그레이드 버튼 내용 저장
    public void SaveUpgradeButton(UpgradeButton upgradeButton)
    {
        string key = upgradeButton.upgradeName;

        PlayerPrefs.SetInt(key + "_level", upgradeButton.level);
        PlayerPrefs.SetInt(key + "_goldByUpgrade", upgradeButton.goldByUpgrade);
        PlayerPrefs.SetInt(key + "_cost", upgradeButton.currentCost);
    }

    //초당 골드 증가 아이템의 저장과 로드
    public void LoadItemButton(ItemButton itemButton)
    {
        string key = itemButton.itemName;

        itemButton.level = PlayerPrefs.GetInt(key + "_level");
        itemButton.currentCost = PlayerPrefs.GetInt(key + "_Cost", itemButton.startCurrentCost);
        itemButton.goldPerSec = PlayerPrefs.GetInt(key + "_goldPerSec");

        if(PlayerPrefs.GetInt(key + "_isPurchased") == 1)
        {
            itemButton.isPurchased = true;
        }
        else
        {
            itemButton.isPurchased = false;
        }
    }

    public void SaveItemButton(ItemButton itemButton)
    {
        string key = itemButton.itemName;

        PlayerPrefs.SetInt(key + "_level",itemButton.level);
        PlayerPrefs.SetInt(key + "_Cost", itemButton.currentCost);
        PlayerPrefs.SetInt(key + "_goldPerSec",itemButton.goldPerSec);



        if (itemButton.isPurchased == true)
        {
            PlayerPrefs.SetInt(key + "_isPurchased", 1);
        }
        else
        {
            PlayerPrefs.SetInt(key + "_isPurchased", 0);
        }
    }

    public int GetGoldPerSec()
    {
        int goldPerSec = 0;
        for(int i = 0; i< itemButtons.Length; i++)
        {
            goldPerSec += itemButtons[i].goldPerSec;
        }
        return goldPerSec;
    }
    public void ResetGold()
    {
        PlayerPrefs.SetInt("Gold", 0);
        PlayerPrefs.DeleteAll();
    }
}

