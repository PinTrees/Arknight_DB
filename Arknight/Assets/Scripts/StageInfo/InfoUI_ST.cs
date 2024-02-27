using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using UI;
using MaterialData;

public class InfoUI_ST : MonoBehaviour
{
    public DataHub_Material dataMaterial;
    public DataHub_ST dataHub;

    public Canvas mainCanvas;
    Canvas infoCanvas;
    Canvas dropCanvas;
    AddDropUI_ST adddropCanvas;

    public TextMeshProUGUI stageName_txt;
    public Text hardTxt;
    public GameObject ndropUI;
    public GameObject sdropUI;
    public GameObject adropUI;

    public GameObject[] atkUI;
    public Transform[] atkViewer;
    public Transform[] dropViewer;

    public Stage curData;

    public Transform dropCanaseViewerTr;

    public TextMeshProUGUI[] menuTxt;
    public Texture2D furnitureImg;

    public List<RawIcon> ndropicons;
    public List<RawIcon> sdropicons;
    public List<RawIcon> adropicons;

    public List<RawIcon_Op[]> opericons;
    List<IconDropMT> dropMaterailIcons;
    public void Initialized()
    {
        opericons = new List<RawIcon_Op[]>();
        ndropicons = new List<RawIcon>();
        sdropicons = new List<RawIcon>();
        adropicons = new List<RawIcon>();
        dropMaterailIcons = new List<IconDropMT>();

        infoCanvas = mainCanvas.transform.GetChild(2).GetComponent<Canvas>();
        dropCanvas = mainCanvas.transform.GetChild(3).GetComponent<Canvas>();
        adddropCanvas = GameObject.FindGameObjectWithTag("UIManager").GetComponent<AddDropUI_ST>();

        for (int i = 0; i < dropViewer[0].childCount; i++)
        {
            RawIcon tmp = new RawIcon();
            tmp.frame = dropViewer[0].GetChild(i).GetComponent<RawImage>();
            tmp.icon = dropViewer[0].GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.per = dropViewer[0].GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>();
            ndropicons.Add(tmp);
        }
        for (int i = 0; i < dropViewer[1].childCount; i++)
        {
            RawIcon tmp = new RawIcon();
            tmp.frame = dropViewer[1].GetChild(i).GetComponent<RawImage>();
            tmp.icon = dropViewer[1].GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.per = dropViewer[1].GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>();
            sdropicons.Add(tmp);
        }
        for (int i = 0; i < dropViewer[2].childCount; i++)
        {
            RawIcon tmp = new RawIcon();
            tmp.frame = dropViewer[2].GetChild(i).GetComponent<RawImage>();
            tmp.icon = dropViewer[2].GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.per = dropViewer[2].GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>();
            adropicons.Add(tmp);
        }
        for(int i = 0; i < dropCanaseViewerTr.childCount; i++)
        {
            IconDropMT tmp = new IconDropMT();
            tmp.Initiailize(dropCanaseViewerTr.GetChild(i));
            dropMaterailIcons.Add(tmp);
        }
        
        for(int i = 0; i < atkViewer.Length; i++)
        {
            RawIcon_Op[] tmpicons = new RawIcon_Op[atkViewer[i].childCount];
            for (int j = 0; j < atkViewer[i].childCount; j++)
            {
                RawIcon_Op tmp = new RawIcon_Op();
                tmp.icon = atkViewer[i].GetChild(j).GetComponent<RawImage>();
                tmp.elite = atkViewer[i].GetChild(j).GetChild(0).GetChild(0).GetComponent<RawImage>();
                tmp.level = atkViewer[i].GetChild(j).GetChild(0).GetChild(1).GetComponent<Text>();
                tmpicons[j] = tmp;
            }
            opericons.Add(tmpicons);
        }
        adddropCanvas.Initialized();
        mainCanvas.enabled = false;
        Clear();
        ClearMenuTxt();
    }
    public void Clear()
    {
        ndropUI.SetActive(false);
        sdropUI.SetActive(false);
        adropUI.SetActive(false);

        for (int i = 0; i < ndropicons.Count; i++)
            ndropicons[i].frame.gameObject.SetActive(false);
        for (int i = 0; i < sdropicons.Count; i++)
            sdropicons[i].frame.gameObject.SetActive(false);
        for (int i = 0; i < adropicons.Count; i++)
            adropicons[i].frame.gameObject.SetActive(false);

        for (int i = 0; i < atkUI.Length; i++)
            atkUI[i].SetActive(false);

        hardTxt.transform.parent.gameObject.SetActive(false);
    }
    public void ClearMenuTxt()
    {
        menuTxt[0].color = new Color(0.4f, 0.4f, 0.4f);
        menuTxt[1].color = new Color(0.4f, 0.4f, 0.4f);
        menuTxt[2].color = new Color(0.4f, 0.4f, 0.4f);
    }
    public void ClearCanvas()
    {
        infoCanvas.enabled = false;
        dropCanvas.enabled = false;
        adddropCanvas.ExiteUI_Main();
    }

    public void StartUI_Main()
    {
        Clear();
        ClearCanvas();
        menuTxt[0].color = new Color(0.86f, 0.86f, 0.86f);

        stageName_txt.text = curData.name + " 지역 정보";

        if (!curData.hard.Equals(string.Empty))
        {
            hardTxt.text = curData.hard;
            hardTxt.transform.parent.gameObject.SetActive(true);
        }

        if (curData.ndrop[0] != string.Empty)// [0] != string.Empty)
        {
            for (int i = 0; i < ndropicons.Count; i++)
            {
                if(i >= curData.ndrop.Length)
                {
                    ndropicons[i].frame.gameObject.SetActive(false);
                    continue;
                }

                EliteMaterial tmp = dataMaterial.get_material_all(curData.ndrop[i]);
                if (tmp == null)  continue;

                ndropicons[i].frame.texture = dataMaterial.GetMaterialFrame(tmp.rare);
                ndropicons[i].icon.texture = tmp.icon;
                ndropicons[i].per.text = string.Format("{0}%", curData.ndropPer[i]);
                ndropicons[i].per.transform.parent.gameObject.SetActive(true);
                ndropicons[i].frame.gameObject.SetActive(true);
            }
            ndropUI.SetActive(true);
        }
        if (curData.sdrop[0] != string.Empty)// [0] != string.Empty)
        {
            for (int i = 0; i < sdropicons.Count; i++)
            {
                if (i >= curData.sdrop.Length)
                {
                    sdropicons[i].frame.gameObject.SetActive(false);
                    continue;
                }

                EliteMaterial tmp = dataMaterial.get_material_all(curData.sdrop[i]);

                if (tmp == null) continue;

                sdropicons[i].frame.texture = dataMaterial.GetMaterialFrame(tmp.rare);
                sdropicons[i].icon.texture = tmp.icon;
                sdropicons[i].per.text = string.Format("{0}%", curData.sdropPer[i]);
                sdropicons[i].per.transform.parent.gameObject.SetActive(true);

                sdropicons[i].frame.gameObject.SetActive(true);
            }
            sdropUI.SetActive(true);
        }
        if (curData.adrop[0] != string.Empty)
        {
            for (int i = 0; i < adropicons.Count; i++)
            {
                if (i >= curData.adrop.Length)
                {
                    adropicons[i].frame.gameObject.SetActive(false);
                    continue;
                }

                EliteMaterial tmp = dataMaterial.get_material_all(curData.adrop[i]);
                if (tmp == null)
                    continue;
                adropicons[i].frame.texture = dataMaterial.GetMaterialFrame(tmp.rare);
                adropicons[i].icon.texture = tmp.icon;
                adropicons[i].per.text = string.Format("{0}%", curData.adropPer[i]);
                adropicons[i].per.transform.parent.gameObject.SetActive(true);


                adropicons[i].frame.gameObject.SetActive(true);
            }
            adropUI.SetActive(true);
        }

        for(int i = 0; i < atkUI.Length; i++)
        {
            if (i >= curData.atOperator.Count)
            {
                atkUI[i].SetActive(false);
                continue;
            }

            for (int j = 0; j < opericons[i].Length; j++)
            {
                if(j >= curData.atOperator[i].Length)
                {
                    opericons[i][j].icon.gameObject.SetActive(false);
                    continue;
                }

                Texture2D icon = dataHub.GetOperatorIcon(curData.atOperator[i][j]);
                opericons[i][j].icon.texture = icon;
                opericons[i][j].elite.texture = dataHub.GetEliteIcon(curData.atElite[i][j]);
                opericons[i][j].level.text = curData.atLevel[i][j];
                opericons[i][j].icon.gameObject.SetActive(true);
            }
            atkUI[i].SetActive(true);
        }

        mainCanvas.enabled = true;
        infoCanvas.enabled = true;
    }
    public void StartUI_DropData()
    {
        List<string[]> data = XML.This.getStageDropDataFromFile(curData.code);

        if (data == null)
            for (int i = 0; i < dropMaterailIcons.Count; i++)
                dropMaterailIcons[i]._this.SetActive(false);

        else
            for (int i = 0; i < dropMaterailIcons.Count; i++)
            {
                if (i >= data.Count)
                {
                    dropMaterailIcons[i]._this.SetActive(false);
                    continue;
                }
                if(data[i][0].Equals("furniture"))
                {
                    dropMaterailIcons[i].Refresh(dataMaterial.GetMaterialFrame("6"), furnitureImg, data[i][1], data[i][2], 6);
                    continue;
                }
                EliteMaterial curData = dataMaterial.get_material_all(data[i][0]);
                if (curData == null)
                    dropMaterailIcons[i]._this.SetActive(false);
                else   dropMaterailIcons[i].Refresh(dataMaterial.GetMaterialFrame(curData.rare), curData.icon, data[i][1], data[i][2], 6);
            }

        dropCanvas.enabled = true;
    }
    public void ExiteUI_Main()
    {
        ClearCanvas();
        mainCanvas.enabled = false;
    }
    public void TR_SetMenu(int index)
    {
        ClearCanvas();
        ClearMenuTxt();
        menuTxt[index -1].color = new Color(0.86f, 0.86f, 0.86f);

        if (index.Equals(1))
            infoCanvas.enabled = true;
        else if (index.Equals(2))
            StartUI_DropData();
        else if (index.Equals(3))
            adddropCanvas.StartUI_Main(curData);
    }
    public void TR_AtkLink(int index)
    {
        Application.OpenURL(curData.link[index]);
    }
    public void TR_AddDropDataWithFireBaseJson()
    {
        StartCoroutine(LogU.Use.SetLog("준비중 입니다."));
    }
    public void TR_Close()
    {
        ClearCanvas();
        ClearMenuTxt();
        mainCanvas.enabled = false;
    }
}
