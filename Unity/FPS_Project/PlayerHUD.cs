using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    #region Variables
    [Header("Components")]
    [SerializeField]
    WeaponAssaultRifle weapon;
    [SerializeField]
    Status status;

    [Header("Ammo UI")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;
    [SerializeField]
    private Image imageBloodScreen;
    [SerializeField]
    private AnimationCurve bloodScreenCurve;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        weapon.onAmmoEvent.AddListener(AmmoHUDUpdate);
        status.onHpEvent.AddListener(HPHUDUpdate);
    }

    private void AmmoHUDUpdate (int currentAmmo, int maxAmmo)
    {
        textAmmo.text = currentAmmo + " / " + maxAmmo; 
    }
    
    private void HPHUDUpdate(int previousHP, int currentHP)
    {
        textHP.text = "HP : " + currentHP;

        if(previousHP - currentHP > 0)
        {
            //중간에 stopCoroutine으로 중단 할 필요가 있는 코루틴의 경우
            //문자열로 작동 시키고 종료 시켜야 한다.
            //이떄 매개변수는 최대 1개까지 넣을 수 있다.
            StopCoroutine("UpdateBloodScreen");
            StartCoroutine("UpdateBloodScreen", 1);
        }
    }
    
    IEnumerator UPdateBloodScreen(float maxViewTime)
        //BloodScreen 이미지의 알파 값이 최대치로 설정되고,
        //maxViewTIme 시간에 걸쳐 0으로 감소한다.
        //감소하는 양은 AnimationCurve 현태로 감소한다.
    {
        float currentTime = 0;
        float precent = 0;
        
        while(precent < 1)
        {
            precent = currentTime / maxViewTime;
            currentTime += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, bloodScreenCurve.Evaluate(precent));
            imageBloodScreen.color = color;


            yield return null;
        }
    }
}
