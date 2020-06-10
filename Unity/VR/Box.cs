using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Box : EventTrigger
{
    GameManager gameManager;
    Renderer renderer;
    BOX_COLOR boxColor;

    public BOX_COLOR BoxColor
    {
        get { return boxColor; }
        set
        {
            boxColor = value;

            switch (boxColor)
            {
                case BOX_COLOR.WHITE: renderer.material.color = Color.white; break;
                case BOX_COLOR.MAGENTA: renderer.material.color = Color.magenta; break;
                case BOX_COLOR.RED: renderer.material.color = Color.red; break;
                case BOX_COLOR.BLUE: renderer.material.color = Color.blue; break;
                case BOX_COLOR.GREEN: renderer.material.color = Color.green; break;
            }
        }
    }

    public void Init(Vector3 _pos, BOX_COLOR _color)
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        renderer = GetComponent<Renderer>();
        BoxColor = _color;
        transform.position = _pos;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("Pointer Enter");
        gameManager.Target = this;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("Pointer Exit");
        if (gameManager.Target == this) gameManager.Target = null;
    }

    public void Change_Box_Color()
    {
        if ((int)BoxColor < 4) BoxColor++;
        else BoxColor = 0;
    }
}
