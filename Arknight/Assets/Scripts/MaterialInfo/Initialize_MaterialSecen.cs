using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_MaterialSecen : MonoBehaviour
{
    public DataHub_Material datahub;
    public EliteMT_InfoUI_MI eliteUI;
    MainUI_MI mainUI;
    public void Start()
    {
        mainUI = GameObject.FindGameObjectWithTag("UIManager").GetComponent<MainUI_MI>();
        mainUI.setCanvasScale();
        datahub.Initialized();
        mainUI.Initialized();
        eliteUI.Initialized();
    }
}
