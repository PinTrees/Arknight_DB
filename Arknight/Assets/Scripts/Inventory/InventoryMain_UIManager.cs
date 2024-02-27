using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMain_UIManager : MonoBehaviour
{
    public GameObject guideUI;

    public void Initialized()
    {
        TriggerSetActiveGuideUI(false);
    }
    public void TriggerSetActiveGuideUI(bool set)
    {
        guideUI.SetActive(set);
    }
}
