using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Update Ammo Event  (Unity Event 생성)
[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class WeaponAssaultRifle : MonoBehaviour
{
    #region Variables
    //Components
    private Animator animator;
    private AudioSource audioSource;

    //"arms@assult_rifle 1"오브젝트에 Animator가 있고, Animator에 대한 제어는 PlayerController에서 하기 때문에
    //PlayerController에서 접근할 수 있도록 프로퍼티 생성.
    public Animator Animator
    {
        get
        {
            return animator;
        }
    }

    // Inspector Variables
    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; //총구 이펙트
    [SerializeField]
    private GameObject casingPrefab; //탄피 이펙트
    [SerializeField]
    private GameObject impactPrefab; //피격 이펙트

    [Header("SpownPoints")]
    [SerializeField]
    private Transform casingSpawnPoint; //탄피 생성 위치
    [SerializeField]
    private Transform bulletSpawnPoint; //총알 생성 위치

    [Header("Sound")]
    [SerializeField]
    private AudioClip fireSound; //공격 사운드
    [SerializeField]
    private AudioClip reloadSound; //장전 사운드

    [Space]
    [SerializeField]
    private WeaponSetting weaponSetting; //무기 기본 설정

    //Update Ammo Event
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();

    //Attack Data
    private bool isAttack = false;
    private bool isAttackStop = false;

    //Reload Data
    private bool isReload = false;

    //Current Ammo
    private int currentAmmo;
    #endregion

    private void OnEnable()
    {
        //onAmmoEnvet에 등록된 모든 객체의  메소드를 호출
        onAmmoEvent.Invoke(currentAmmo, weaponSetting.maxAmmo);
    }

    void Awake()
    {
        animator = this.GetComponent<Animator>();

        muzzleFlashEffect.SetActive(false);

        audioSource = this.GetComponent<AudioSource>();

        currentAmmo = weaponSetting.maxAmmo;
    }

    public void StartAttack()
    {
        if ( isReload == false && isAttack == false)
        {
            StartCoroutine(TryAttack());
        }
    }
    public void StopAttack()
    {
        isAttackStop = true;
    }

    public void StartReload()
    {
        if(isReload == false)
        {
            StopAttack();
            StartCoroutine(TryReload());
        }
    }

    IEnumerator TryAttack()
    //실제 공격에 대한 로직. 공격에 대한 방식이나 설정이 추가되어도 외부에서 알아야 할 내용이나
    //코드가 수정되는 일이 없도록 함.
    {
        isAttack = true;

        while (!isAttackStop)
        {
            // 뛰는 동작 중에는 공격 불가
            if (animator.GetFloat("movementSpeed") > 0.5f)
            {
                break;
            }

            //현재 탄 수가 0이면 공격 불가
            if(currentAmmo <= 0 )
            {
                StartReload(); //공격 중에 탄 수가 0이 되면 자동으로 재장전을 함.
                break;
            }
            currentAmmo--; //공격으로 1씩 감소

            //onAmmoEvent에 등록된 모든 객체의 메소드를 호출
            onAmmoEvent.Invoke(currentAmmo, weaponSetting.maxAmmo);
            
            animator.ResetTrigger("onJump"); //"onJump" 트리거를 리셋시켜서 bug Fix

             animator.Play("Fire"); //Animation
            StartCoroutine(OnFireEffects());//Fire Effects
            PlaySound(fireSound); //Sound
            SpawnCasing();

            RaycastCalculate(); // Attack Target

            yield return new WaitForSeconds(weaponSetting.fireRate);
        }
        isAttack = false;
        isAttackStop = false;
    }

    IEnumerator OnFireEffects()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.fireRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void SpawnCasing()
    {
        Instantiate(casingPrefab, casingSpawnPoint.position, Random.rotation);
    }

    IEnumerator TryReload()
    {
        isReload = true;

        animator.Play("Reload");
        PlaySound(reloadSound);

        while(true)
        {
            if (audioSource.isPlaying == false && animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
                //재장전 사운드가 종료되고, "Movement"애니메이션을 재생 중이면
                //재장전 애니메이션이 종료되었다고 판단하고 while()종료
            {
                break;
            }
            yield return null;
        }
        isReload = false;
        currentAmmo = weaponSetting.maxAmmo;

        //onAmmoEvent에 등록된 모든 객체의 메소드를 호출
        onAmmoEvent.Invoke(currentAmmo, weaponSetting.maxAmmo);
    }

    private void RaycastCalculate()
    {
        Ray ray;
        RaycastHit hit;

        Vector3 targetPoint = Vector3.zero;


        //화면의 중앙 좌표 (Aim기준으로 Raycast 연산)
        ray = Camera.main.ViewportPointToRay(Vector2.one * 0.5f);
        //ViewportPointToRay(Vector3 position);
        //뷰포트 좌표 : 카메라의 좌 하단 (0,0), 카메라의 우상단 (1,1).

        if (Physics.Raycast(ray, out hit, weaponSetting.fireDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.fireDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.fireDistance, Color.red);
        //Debug.DrawRay(Vector3 start, Vector3 dir, Color color = Color.white);
        //Start로 부터 start+dir까지 color색의 라인을 그린다.(Scene View에서만 보임)

        // 첫 번째 Raycast 연산으로 얻어진 targetPoint를 목표 지점으로 설정하고,
        // 총구를 시작지점으로 하여 Raycast를 연산.
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if(Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.fireDistance))
        {
            //공격에 부딪힌 대상의 이름에 "Enemy"단어가 들어가면 부딪힌 대상을 삭제한다.
            if(hit.transform.name.Contains("Enemy"))
            {
                Destroy(hit.transform.gameObject);
                return;
            }
            //벽에 부딪혔을 때 이펙트
            Instantiate(impactPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.fireDistance, Color.blue);
    }
}
