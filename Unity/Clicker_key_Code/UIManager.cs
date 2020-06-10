using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text goldDisplayer;

    public Text goldPerClickDisplayer;

    public Text goldPerSecDisplayer;

    void Update()
    {
        goldPerClickDisplayer.text = "" + DataController.Instance.goldPerClick;
        goldDisplayer.text = "" + DataController.Instance.gold;
        goldPerSecDisplayer.text = "" + DataController.Instance.GetGoldPerSec();
    }
}
