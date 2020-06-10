using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBox : MonoBehaviour
{
    Image image;
    BOX_COLOR boxColor;

    public BOX_COLOR BoxColor
    {
        get { return boxColor; }
        set
        {
            boxColor = value;

            switch (boxColor)
            {
                case BOX_COLOR.WHITE: image.color = Color.white; break;
                case BOX_COLOR.MAGENTA: image.color = Color.magenta; break;
                case BOX_COLOR.RED: image.color = Color.red; break;
                case BOX_COLOR.BLUE: image.color = Color.blue; break;
                case BOX_COLOR.GREEN: image.color = Color.green; break;
            }
        }
    }
    public void Init(BOX_COLOR _color, int _x, int _y)
    {
        image = GetComponent<Image>();
        BoxColor = _color;
        transform.localPosition = new Vector3(40.0f + 9.0f * _x, 40.0f - 9.0f * _y, 0.0f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
