using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    #region Variables
    private float rotateSpeedX = 3;
    private float rotateSpeedY = 5;
    private float limitMinX = -80;
    private float limitMaxX = 50;
    private float eulerAngleX;
    private float eulerAngleY;
    private Quaternion rotation;
    #endregion


    public void UpdateRotate(float mouseX, float mouseY)
        //마우스를 좌,우로 움직이면 오브젝트가 실제로 움직이는 축이 Y축
        //마우스를 위,아래로 움직이면 오브젝트가 실제로 움직이는 축이 X축
        //그렇기 때문에 mouseX 값을eulerAngleY값에 적용하는 것.
        //mouseY는 eulerAngleX)
    {
        //위 아래를 바라보는 카메라의 X축은 최대 혹은 최소 값을 넘지 않도록 함.
        eulerAngleY += mouseX * rotateSpeedX;
        eulerAngleX -= mouseY * rotateSpeedY;
        //마우스를 아래로 내리면 음수지만 게임 내에서 아래를 보려면
        //x축 양의 방향을 이동해야 하기 떄문에 위 같은 수식이 나옴.

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
        //오일러 값을 쿼터니온으로 변경하여 회전 값을 적용한다.
        transform.rotation = rotation;
    }
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
