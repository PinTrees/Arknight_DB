using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DW_Initialize : MonoBehaviour
{
    public GameObject singleton;
    public DW_DataHub dataHub;
    public DW_UIManager UIMng;
    public void Start()
    {
        if (XML.This == null)
            GameObject.Instantiate(singleton);

        dataHub.Initialized();
        UIMng.Initialized();
    }
}
