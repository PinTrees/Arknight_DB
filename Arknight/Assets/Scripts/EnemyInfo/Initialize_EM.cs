using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_EM : MonoBehaviour
{
    public DataHub_EM dataHub;
    public MainUI_EM mainUI;
    public void Start()
    {
        mainUI.setCanvasScale();
        dataHub.Initialized();
        mainUI.Initialized();
    }
}
