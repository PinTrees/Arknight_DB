using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

using MaterialData;
using UI;
public class MainUI_MI : MonoBehaviour
{
    public DataHub_Material dataMng;
    public EliteMT_InfoUI_MI infoUI;

    public Transform viewer;
    private List<Icon> icons;
    private List<EliteMaterial> data;
    private int curStatus;

    CanvasScaler canvasScreen;
    RectTransform rect;
    public void setCanvasScale()
    {
        canvasScreen = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>();
        rect = canvasScreen.transform.GetChild(0).GetComponent<RectTransform>();
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScreen, rect);
    }
    public void Initialized()
    {
        curStatus = 0;
        icons = new List<Icon>();
        for(int i = 0; i < viewer.childCount; i++)
        {
            Icon tmp = new Icon();
            tmp.This = viewer.transform.GetChild(i).gameObject;
            tmp.frame_raw = viewer.transform.GetChild(i).GetComponent<RawImage>();
            tmp.icon_raw = viewer.transform.GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.name_txt = viewer.transform.GetChild(i).GetChild(2).GetComponent<Text>();
            icons.Add(tmp);
        }
        Refresh(dataMng.eliteMaterialData);
    }
    public void Refresh(List<EliteMaterial> set)
    {
        data = set;
        curStatus = 0;
        for (int i = 0; i < icons.Count; i++)
        {
            if (i >= set.Count || set[i] == null)
            {
                icons[i].This.SetActive(false);
                continue;
            }
            if (set[i].icon != null)
                icons[i].icon_raw.texture = set[i].icon;
            if (icons[i].frame_raw != null)
                icons[i].frame_raw.texture = dataMng.GetMaterialFrame(set[i].rare);
            icons[i].name_txt.text = set[i].name;
            icons[i].This.SetActive(true);
        }
    }
    public void Refresh(List<MaterialChip> set)
    {
        curStatus = 1;
        for (int i = 0; i < icons.Count; i++)
        {
            if (i >= set.Count || set[i] == null)
            {
                icons[i].This.SetActive(false);
                continue;
            }
            if (set[i].icon != null)
                icons[i].icon_raw.texture = set[i].icon;
            if (icons[i].frame_raw != null)
                icons[i].frame_raw.texture = dataMng.GetMaterialFrame(set[i].rare);
            icons[i].name_txt.text = set[i].name;
            icons[i].This.SetActive(true);
        }
    }
    public void TR_SetMaterial(int index)
    {
        if (index.Equals(1))
            Refresh(dataMng.eliteMaterialData);
        else if(index.Equals(2))
            Refresh(dataMng.chipMaterialData);
        else
            Refresh(dataMng.eliteMaterialData);
    }
    public void TR_SetMaterialInfo(Transform set)
    {
        if (!curStatus.Equals(0))
            return;
        int index = set.GetSiblingIndex();
        infoUI.SetStart(data[index]);
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (infoUI.infoUI.activeSelf.Equals(true))
                infoUI.TR_SetActive(false);
            else
            {
                LoadScene("Main");
            }
        }
    }
}
