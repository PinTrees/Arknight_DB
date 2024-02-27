using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_IV : MonoBehaviour
{
    public DataHub_Material datahubMaterial;
    private InventoryMain_UIManager MainUI;
    public InvenUI_IV invenUI;
    public SelectUI_IV selectUI;
    public MainUI_IV mainUI;
    public InfoUI_IV infoUI;
    public GameObject singleton;
    public void Start()
    {
        if (XML.This == null)
            GameObject.Instantiate(singleton);
        mainUI.setCanvasScale();
        StartCoroutine(Initialized());
    }
    private IEnumerator Initialized()
    {
        datahubMaterial.Initialized();

        invenUI.Initialized();
        StartCoroutine(mainUI.Initialized());
        yield return new WaitForEndOfFrame();
        selectUI.Initialized();
        infoUI.Initialized();

        yield return null;
    }
}
