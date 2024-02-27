using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorCompare_Initialize : MonoBehaviour
{
    public GameObject singleton;
    OperatorCompare_DataHub dataHub;
    OperatorCompare_MainUI mainUI;
    // Start is called before the first frame update
    void Start()
    {
        if (XML.This == null)
            GameObject.Instantiate(singleton);

        dataHub = GameObject.FindWithTag("DataManager").GetComponent<OperatorCompare_DataHub>();
        mainUI = GameObject.FindWithTag("UIManager").GetComponent<OperatorCompare_MainUI>();

        dataHub.Initiailized();
        mainUI.Initialized();
    }
}
