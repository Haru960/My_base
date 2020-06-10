using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
//[System.Serializable]을 이용해 직렬화를 하지 않으면 다른 클래스의 변수로 생성되었을 때
//Inspecter View에 멤버 변수들의 목록이 뜨지 않음.
public class WeaponSetting
    //무기 정보에 대한 구조체 생성
{
    public int maxAmmo; //총 탄수
    public float fireRate; //공격 주기
    public float fireDistance; //공격 사거리
    public bool isAutomaticFire; //자동,반자동
}