using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_LC : MonoBehaviour
{
    MainUI_LC mainUI;
    public void Start()
    {
        mainUI = GameObject.FindGameObjectWithTag("UIManager").GetComponent<MainUI_LC>();
        mainUI.setCanvasScale();
    }
}
