using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_ST : MonoBehaviour
{
    public DataHub_ST dataMng;
    public DataHub_Material dataMaterial;
    public MainUI_ST mainUI;
    public InfoUI_ST infoUI;

    public void Start()
    {
        dataMng.Initialized();

        mainUI.setCanvasScale();
        dataMaterial.Initialized();
        mainUI.Initialized();
        infoUI.Initialized();

    }
}
