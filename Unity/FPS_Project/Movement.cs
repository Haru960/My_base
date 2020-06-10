using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
//이 명령이 포함된 스크립트를 게임 오브젝트의 컴포넌트에
//넣으면 해당 컴포넌트도 같이 들어간다(적용된다).

public class Movement : MonoBehaviour
{
    #region variables
    //Components
    private CharacterController controller;
    //CharacterController Conponent는 캐릭터의 이동을 위해 제작된 물리 + 충돌 박스

    //Move
    [SerializeField]
    private float moveSpeed = 2;
    public float MoveSpeed
    {
        set { moveSpeed = value; }
        get { return moveSpeed; }
    }
    private Vector3 moveForce;

    // Jump & Gravity
    [SerializeField]
    private float jumpForce = 8;
    [SerializeField]
    private float gravity = -20;

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        controller = this.GetComponent<CharacterController>();
    }

   
    public void UpdateMove(float horizontal, float vertical)
    {
        //이동방향 = 캐릭터 회전값 * 입력된 키 값
        Vector3 direction = transform.rotation * new Vector3(horizontal, 0, vertical);
        //이동 힘 = 이동 방향 * 속도
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);

        //허공에 떠있다면 중력 만큼 가라앉음.
        //바닥이 아닐 경우에는 Y축 방향에 중력(gravity)이 가해져 아래로 이동하려 함
        if( !controller.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }
        //실제 이동(이동 힘 * Time.deltaTime)
        controller.Move(moveForce * Time.deltaTime);
    }

    public bool Jump() 
        //Jump() 메소드가 호출 되었을 때 만약 바닥을 밟고 있으면 moveForce에 jumpForce만큼 힘을 줘 점프하도록함(y축으로 이동)
        //점프에 성공하면 true 값을 반환하도록함()
    {
        //바닥을 밟고 있으면 점프
        if( controller.isGrounded )
        {
            moveForce.y = jumpForce;

            return true;
        }
        return false;
    }
}
