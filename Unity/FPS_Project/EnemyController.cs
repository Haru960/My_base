using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variables
    //Components
    private Movement movement;
    private Status status;

    //Target (Player)
    private Transform target;
    private string targetName = "Player";
    #endregion

    public void Initialize(Transform target)
    {
        movement = this.GetComponent<Movement>();
        status = this.GetComponent<Status>();

        this.target = target; //target = GameObject.Find("Player").transform;
    }

    void Update()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        Vector3 moveDirection = target.position - transform.position;
        moveDirection.y = 0;

        transform.rotation = Quaternion.LookRotation(moveDirection);

        movement.MoveSpeed = status.RunSpeed;
        movement.UpdateMove(0, 1); //전방으로 이동
        //Quaternion.LookRotation울 이용해 Target의 위치를 항상 바라보고 있기 때문에
        //이동은 전방 (z축의 양의 방향)으로 설정 한다.
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.name.Equals(targetName))
        {
            //플레이어 체력이 낮아지면 사망
            hit.gameObject.GetComponent<Status>().DecreaseHp(5);

            Destroy(gameObject);
        }
    }
}
