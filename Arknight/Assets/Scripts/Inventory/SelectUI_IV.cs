using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UI;
public class SelectUI_IV : MonoBehaviour
{
    public DataHub_Material dataMng;
    public MainUI_IV mainUI;

    public Transform viewer;

    private List<RawIconIV> icons;

    public void Initialized()
    {
        icons = new List<RawIconIV>();
        for(int i = 0; i < viewer.childCount; i++)
        {
            RawIconIV tmp = new RawIconIV();
            tmp.frame = viewer.GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.icon = viewer.GetChild(i).GetChild(2).GetComponent<RawImage>();
            tmp.selectUI = viewer.GetChild(i).GetChild(1).gameObject;
            icons.Add(tmp);
        }
        TR_SetActive(false);
    }
    public void Refresh()
    {
        for(int i = 0; i < icons.Count; i++)
        {
            if (i >= dataMng.eliteMaterialData.Count - 3)
            {
                icons[i].icon.transform.parent.gameObject.SetActive(false);
                continue;
            }

            icons[i].frame.texture = dataMng.GetMaterialFrame(dataMng.eliteMaterialData[i].rare);
            icons[i].icon.texture = dataMng.eliteMaterialData[i].icon;

            if (mainUI.FineSelectMaterial(dataMng.eliteMaterialData[i].code))
                icons[i].selectUI.SetActive(true);
            else icons[i].selectUI.SetActive(false);
        }
    }
    public void TR_SetMaterial(Transform set)
    {
        int index = set.GetSiblingIndex();
        if (mainUI.FineSelectMaterial(dataMng.eliteMaterialData[index].code))
            mainUI.DeleteMaterial(dataMng.eliteMaterialData[index].code);
        else
            mainUI.AddMaterial(dataMng.eliteMaterialData[index].code);

        mainUI.RefreshData();
        mainUI.SaveData();
        mainUI.Refresh();
        Refresh();
    }
    public void TR_SetActive(bool set)
    {
        if (set)
            Refresh();
        this.gameObject.SetActive(set);
    }
}
