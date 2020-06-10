using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BOX_COLOR { WHITE = 0, MAGENTA, RED, BLUE, GREEN }

public class BoxManager : MonoBehaviour
{
    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject uiBoxPrefab;

    public Box[] BoxArr { private set; get; }
    public UIBox[] UIBoxArr { private set; get; }

    void Awake()
    {
        CreateAllBox(4, 4);
    }
    void CreateAllBox(int _width, int _height)
    {
        BoxArr = new Box[_width * _height];
        UIBoxArr = new UIBox[_width * _height];
        for (int y = 0; y < _height; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                int idx = y * _width + x;
                Vector3 pos = new Vector3(-_width * 0.5f +0.5f + x, _height * 0.5f - 0.5f - y, 0.0f);
                CreateBox(pos, idx);
                CreateUIBox(x, y, idx);
            }
        }
    }
    void CreateBox(Vector3 _pos, int _idx)
    {
        GameObject clone = Instantiate(boxPrefab);

        BOX_COLOR color = (BOX_COLOR)Random.Range(0, 5);

        Box box = clone.GetComponent<Box>();
        box.Init(_pos, color);

        BoxArr[_idx] = box;
    }
    void CreateUIBox(int _x, int _y, int _idx)
    {
        GameObject clone = Instantiate(uiBoxPrefab);

        BOX_COLOR color = (BOX_COLOR)Random.Range(0, 5);

        clone.transform.SetParent(GameObject.Find("Canvas").transform);

        UIBox box = clone.GetComponent<UIBox>();
        box.Init(color, _x, _y);
        UIBoxArr[_idx] = box;
    }
}
