using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MaterialData;
using UI;

public class MainUI_IV : MonoBehaviour
{
    public GameObject UI;
    public GameObject menuUI;
    public GameObject guidUI;

    [Header("- UI Trnasform")]
    public Transform viewerS;
    public Transform viewerR;

    private List<IconIV> iconS;
    private List<IconIV> iconR;

    private List<Material_IV> dataR;
    private List<Material_IV> dataS;

    //private MaterialCount curData;

    public DataHub_Material dataMng;
    public InfoUI_IV infoUI;
    public InvenUI_IV invenUI;

    public GameObject gaussianblur;
    public RawImage gaussianblurPanel;
    public Material_IV curData;

    CanvasScaler canvasScreen;
    RectTransform rect;
    public void setCanvasScale()
    {
        canvasScreen = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>();
        rect = canvasScreen.transform.GetChild(0).GetComponent<RectTransform>();
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScreen, rect);
    }
    public IEnumerator Initialized()
    {
        dataS = new List<Material_IV>(); 
        dataR = new List<Material_IV>();
        yield return new WaitForEndOfFrame();

        // UI 아이콘 리스트 변수에 할당
        iconS = new List<IconIV>();
        for (int i = 0; i < viewerS.childCount; i++) // 아이콘 리스트 생성
        {
            IconIV set = new IconIV();
            set.This = viewerS.transform.GetChild(i).gameObject;
            set.frame = viewerS.transform.GetChild(i).GetChild(0).GetComponent<RawImage>();
            set.icon = viewerS.transform.GetChild(i).GetChild(1).GetComponent<RawImage>();
            set.name_txt = viewerS.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<Text>();
            set.txt_count = viewerS.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<Text>();
            set.txt_madecount = viewerS.transform.GetChild(i).GetChild(4).GetChild(0).GetComponent<Text>();
            set.required_tag = viewerS.transform.GetChild(i).GetChild(5).gameObject;
            set.end_tag = viewerS.transform.GetChild(i).GetChild(6).gameObject;
            iconS.Add(set);
        }
        iconR = new List<IconIV>();
        for (int i = 0; i < viewerR.childCount; i++) // 아이콘 리스트 생성
        {
            IconIV set = new IconIV();
            set.This = viewerR.transform.GetChild(i).gameObject;
            set.frame = viewerR.transform.GetChild(i).GetChild(0).GetComponent<RawImage>();
            set.icon = viewerR.transform.GetChild(i).GetChild(1).GetComponent<RawImage>();
            set.name_txt = viewerR.transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<Text>();
            set.txt_count = viewerR.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<Text>();
            set.txt_madecount = viewerR.transform.GetChild(i).GetChild(4).GetChild(0).GetComponent<Text>();
            set.required_tag = viewerR.transform.GetChild(i).GetChild(5).gameObject;
            set.end_tag = viewerR.transform.GetChild(i).GetChild(6).gameObject;
            iconR.Add(set);
        }

        Clear();
        //if (dataS.Count != 0)
        //{
        //    RefreshNeedCount();
        //    SetSelectIconView();
        //    SetResultIconView();
        //}
        //UI.SetActive(true);
        XML.This.Get_TargetMaterial(dataS, dataMng.eliteMaterialData);

        RefreshData();
        Refresh();

        gaussianblur.SetActive(false);
        gaussianblurPanel.gameObject.SetActive(false);

        menuUI.SetActive(false);
        yield return null;
    }
    public void GetMaterialCount(List<Material_IV> dataR, List<Material_IV> dataS)
    {
        for(int i = 0; i < dataS.Count; i++)
        {
            if (dataS[i].data.rare.Equals("5"))
            {
                dataS[i].made += invenUI.GetCount(dataS[i].data.code);
                if (dataS[i].made > dataS[i].count) dataS[i].made = dataS[i].count;
            }
            //if (dataS[i].count - dataS[i].inven <= 0)     continue;
            if (dataS[i].data.submaterial[0].Equals(string.Empty) || dataS[i].data.rare != "5")
            {
                string code = dataS[i].data.code;
                Material_IV tmp = dataR.Find(delegate (Material_IV a) { return a.data.code == code; });
                if (tmp == null)
                {
                    tmp = new Material_IV();
                    tmp.data = dataMng.get_material_all(code);
                    tmp.count = dataS[i].count;
                    tmp.inven = invenUI.GetCount(code);
                    tmp.countUse = 0;
                    tmp.made = 0;
                    dataR.Add(tmp);
                }
                else
                    tmp.count += dataS[i].count;
                continue;
            }

            int count = dataS[i].count - dataS[i].made;
            if (count < 0) count = 0;
            for (int j = 0; j < dataS[i].data.submaterial.Length; j++)
            {
                string code = dataS[i].data.submaterial[j];
                Material_IV tmp = dataR.Find(delegate (Material_IV a) { return a.data.code == code; });
                if(tmp == null)
                {
                    tmp = new Material_IV();
                    tmp.data = dataMng.get_material_all(code);
                    tmp.count = int.Parse(dataS[i].data.subCount[j]) * count;
                    tmp.inven = invenUI.GetCount(code);
                    tmp.countUse = 0;
                    tmp.made = 0;
                    dataR.Add(tmp);
                }
                else
                {
                    tmp.count += int.Parse(dataS[i].data.subCount[j]) * count;
                }
            }
        }
    }
    public void RefreshCount_Rare(List<Material_IV> dataR, string rare)
    {
        for (int i = 0; i < dataR.Count; i++)
        {
            if (dataR[i].data.submaterial[0].Equals(string.Empty) || dataR[i].data.rare != rare)
                continue;

            int count = dataR[i].count - invenUI.GetCount(dataR[i].data.code);
            if (count < 0) count = 0;

            for (int j = 0; j < dataR[i].data.submaterial.Length; j++)
            {
                string code = dataR[i].data.submaterial[j];
                Material_IV tmp = dataR.Find(delegate (Material_IV a) { return a.data.code == code; });
                if (tmp == null)
                {
                    tmp = new Material_IV();
                    tmp.data = dataMng.get_material_all(code);
                    tmp.inven = invenUI.GetCount(code);
                    tmp.countUse = 0;
                    tmp.made = 0;
                    tmp.count = (int.Parse(dataR[i].data.subCount[j]) * count);

                    //Debug.Log(dataR[i].data.name + ":" + tmp.count);
                    dataR.Add(tmp);
                }
                else if (count > 0)
                {
                    tmp.count = tmp.count + (int.Parse(dataR[i].data.subCount[j]) * count);
                    //Debug.Log(tmp.count);
                }
            }
        }
    }
    public void RefreshMadeCount_Rare(List<Material_IV> data, string rare)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (!rare.Equals("S"))
            {
                if (data[i].data.submaterial[0].Equals(string.Empty) || data[i].data.rare != rare)
                    continue;
            }
            else if (data[i].data.rare != "5")
            {
                string code = data[i].data.code;
                Material_IV tmp = dataR.Find(delegate (Material_IV a) { return a.data.code == code; });
                int use = (tmp.inven + tmp.made) - tmp.countUse;
                if (use > 0)
                {
                    data[i].made += use;
                    tmp.countUse += use;
                }
                continue;
            }

            int[] minCounts = new int[data[i].data.submaterial.Length];
            int minCount = 0;
            for (int j = 0; j < data[i].data.submaterial.Length; j++)
            {
                int staticCount = int.Parse(data[i].data.subCount[j]);
                string code = data[i].data.submaterial[j];
                Material_IV tmp = dataR.Find(delegate (Material_IV a) { return a.data.code == code; });

                int tmpC = Mathf.FloorToInt(((tmp.inven - tmp.countUse) + tmp.made) / staticCount);
                minCounts[j] = tmpC;

                //Debug.Log(data[i].data.name + ":" + data[i].data.submaterial[j] + " " + tmpC);
            }
            for(int j = 0; j < minCounts.Length; j++)
            {
                if (j.Equals(0))
                    minCount = minCounts[j];
                else if (minCount > minCounts[j])
                    minCount = minCounts[j];
            }
            if (minCount <= 0)
                continue;
            else if (data[i].count - data[i].made - data[i].inven <= 0) 
                minCount = 0;
            else if (minCount > (data[i].count - data[i].inven)) 
                minCount = data[i].count - data[i].inven;
            data[i].made += minCount;

            for (int j = 0; j < data[i].data.submaterial.Length; j++)
            {
                string code = data[i].data.submaterial[j];
                Material_IV tmp = dataR.Find(delegate (Material_IV a) { return a.data.code == code; });
                tmp.countUse += int.Parse(data[i].data.subCount[j]) * minCount;
            }
            //Debug.Log(data[i].data.name + ":" + data[i].count);
        }
    }
    public void Clear() // 아이콘 리스트 전체 비활성화
    {
        for (int i = 0; i < iconS.Count; i++)
            iconS[i].This.SetActive(false);
        for (int i = 0; i < iconR.Count; i++)
            iconR[i].This.SetActive(false);

        guidUI.SetActive(false);
    }
    public void Restart()
    {
        dataS.Clear();
        dataR.Clear();
        RefreshData();
        SaveData();
        Refresh();
    }
    public void RefreshData()
    {
        dataR.Clear();

        for (int i = 0; i < dataS.Count; i++)
        {
            dataS[i].inven = 0;
            dataS[i].made = 0;
        }

        GetMaterialCount(dataR, dataS);
        RefreshCount_Rare(dataR, "5");
        RefreshCount_Rare(dataR, "4");
        RefreshCount_Rare(dataR, "3");
        RefreshCount_Rare(dataR, "2");

        RefreshMadeCount_Rare(dataR, "1");
        RefreshMadeCount_Rare(dataR, "2");
        RefreshMadeCount_Rare(dataR, "3");
        RefreshMadeCount_Rare(dataR, "4");
        RefreshMadeCount_Rare(dataR, "5");
        RefreshMadeCount_Rare(dataS, "S");

        dataS.Sort(delegate (Material_IV a, Material_IV b) // 클래스 리스트 특정 요소값으로 정렬 방법
        {
            if (a.data.id > b.data.id) return 1;
            else if (a.data.id < b.data.id) return -1;
            return 0;
        });
        dataR.Sort(delegate (Material_IV a, Material_IV b) // 클래스 리스트 특정 요소값으로 정렬 방법
        {
            if (a.data.id > b.data.id) return 1;
            else if (a.data.id < b.data.id) return -1;
            return 0;
        });
    }
    public void Refresh()
    {
        for (int i = 0; i < iconS.Count; i++)
        {
            if (i >= dataS.Count)
            {
                iconS[i].This.SetActive(false);
                continue;
            }

            iconS[i].icon.texture = dataS[i].data.icon;
            iconS[i].frame.texture = dataMng.GetMaterialFrame(dataS[i].data.rare);
            iconS[i].name_txt.text = dataS[i].data.name;
            iconS[i].txt_count.text = dataS[i].count.ToString();
            iconS[i].txt_madecount.text = dataS[i].made.ToString();

            int c = dataS[i].count - dataS[i].inven - dataS[i].made;
            if (c <= 0) iconS[i].end_tag.SetActive(true);
            else iconS[i].end_tag.SetActive(false);
            if (dataS[i].data.submaterial[0].Equals(string.Empty)) iconS[i].required_tag.SetActive(true);
            else iconS[i].required_tag.SetActive(false);

            iconS[i].This.SetActive(true);
        }
        for(int i = 0; i < iconR.Count; i++)
        {
            if (i >= dataR.Count)
            {
                iconR[i].This.SetActive(false);
                continue;
            }

            iconR[i].icon.texture = dataR[i].data.icon;
            iconR[i].frame.texture = dataMng.GetMaterialFrame(dataR[i].data.rare);
            iconR[i].name_txt.text = dataR[i].data.name;
            iconR[i].txt_madecount.text = (dataR[i].inven + dataR[i].made).ToString();

            int count = dataR[i].count - dataR[i].inven - dataR[i].made;
            if (count < 0) count = 0;
            iconR[i].txt_count.text = count.ToString();

            if (count <= 0) iconR[i].end_tag.SetActive(true);
            else iconR[i].end_tag.SetActive(false);

            if (dataR[i].data.submaterial[0].Equals(string.Empty)) iconR[i].required_tag.SetActive(true);
            else iconR[i].required_tag.SetActive(false);

            iconR[i].This.SetActive(true);
        }
    }

    public void AddMaterial(string code)
    {
        Material_IV tmp = new Material_IV();
        tmp.data = dataMng.get_material_all(code);
        tmp.inven = invenUI.GetCount(code);
        tmp.made = 0;
        tmp.count = 1;
        dataS.Add(tmp);
    }
    public void DeleteMaterial(string code)
    {
        int index = dataS.FindIndex(delegate (Material_IV a) { return a.data.code == code; });

        if (index.Equals(-1))
            return;
        else
            dataS.RemoveAt(index);
    }
    public void SaveData()
    {
        string data = string.Empty;
        for (int i = 0; i < dataS.Count; i++)
        {
            if (data.Equals(string.Empty))
                data = string.Format("{0},{1}", dataS[i].data.code, dataS[i].count);
            else
                data = string.Format("{0},{1},{2}", data, dataS[i].data.code, dataS[i].count);
        }

        PlayerPrefs.SetString("MatS", data);
        PlayerPrefs.Save();
    }
    public bool FineSelectMaterial(string code)
    {
        Material_IV tmp = dataS.Find(delegate (Material_IV a) { return a.data.code == code; });

        if (tmp == null)
            return false;
        else
            return true;
    }
    public void TR_SetGuidUI(bool set)
    {
        guidUI.SetActive(set);
    }
    // Start UI Corutine =================================================================================
    IEnumerator StartUI_MaterialInfo(int index)
    {
        gaussianblur.SetActive(true);
        yield return new WaitForEndOfFrame();

        gaussianblurPanel.texture = Files.Use.ScreenShot();
        gaussianblurPanel.gameObject.SetActive(true);
        gaussianblur.SetActive(false);
        infoUI.TR_SetStart(dataR[index]);
    }
    // Exite UI Corutine =================================================================================
    public void TR_MaterialIcon(Transform set)
    {
        int index = set.GetSiblingIndex();
        StartCoroutine(StartUI_MaterialInfo(index));
    }
    public void TR_MaterialIconS(Transform set)
    {
        int index = set.GetSiblingIndex();
        curData = dataS[index];
        TR_SetMenuUI(true);
    }
    public void TR_SetMaterialCount(int count)
    {
        int curCount = curData.count;
        curCount = curCount + count;

        if (curCount < 1)
            curData.count = 1;
        else
            curData.count = curCount;

        SaveData();
        RefreshData();
        Refresh();
    }
    public void TR_SetMenuUI(bool set)
    {
        if (set && menuUI.activeSelf.Equals(false))
            menuUI.SetActive(set);
        else if (!set && menuUI.activeSelf.Equals(true))
            menuUI.SetActive(set);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
}
