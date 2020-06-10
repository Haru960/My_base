using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    //Components
    private RotateToMouse rotateToMouse;
    private Movement movement;
    private Status status;
    private WeaponAssaultRifle weapon;

    //Input Key
    private KeyCode keyCodeRun = KeyCode.LeftShift;
    private KeyCode keyCodeJump = KeyCode.Space;
    private KeyCode keyCodeReload = KeyCode.R;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Application.runInBackground = true; // 현재 윈도우가 선택되어 있지 않을 때도 실행하도록 함(default 값은 false)

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //커서를 보이지 않게 커서 위치 고정(화면 밖으로 나가지 않게)

        rotateToMouse = this.GetComponent<RotateToMouse>();
        movement = this.GetComponent<Movement>();
        status = this.GetComponent<Status>();
        weapon = this.GetComponentInChildren<WeaponAssaultRifle>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateAttack();
        UpdateReload();
    }
    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        //마우스를 좌우로 움직였을 때 반응
        //좌 : -1 / 제자리 : 0 / 우 : 1
        float mouseY = Input.GetAxis("Mouse Y");
        //마우스를 위아래로 움직였을 때 반응
        //아래 : -1 / 제자리 : 0 / 우 : 1

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }
    private void UpdateMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(horizontal !=0 || vertical !=0)
        {
            if(Input.GetKey(keyCodeRun))
            {
                movement.MoveSpeed = status.RunSpeed;
                weapon.Animator.SetFloat("movementSpeed", 1.0f);
            }
            else
            {
                movement.MoveSpeed = status.WalkSpeed;
                weapon.Animator.SetFloat("movementSpeed", 0.5f);
            }
            
        }
        else
        {
            weapon.Animator.SetFloat("movementSpeed", 0.0f);
        }
        movement.UpdateMove(horizontal, vertical);
        //UpdateMove가 이동,점프에 영향을 주는데 원래 있던 위치는 이동 키를 누를 때만 반응하는 위치이기 때문에 점프만 단독으로 누를 경우 반응이 없다가 이동하기 시작하면 반응하는
        //bug가 생김.
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(keyCodeJump))
        {
            bool isPossibleJump = movement.Jump();

            if (isPossibleJump)
            {
                weapon.Animator.SetTrigger("onJump");
            }
        }
    }
    private void UpdateAttack()
    {
        if(Input.GetMouseButton(0))
        {
            weapon.StartAttack();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            weapon.StopAttack();
        }
    }
    private void UpdateReload()
    {
        if(Input.GetKeyDown(keyCodeReload))
        {
            weapon.StartReload();
        }
    }
}
