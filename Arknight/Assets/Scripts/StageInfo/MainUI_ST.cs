using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using UI.Button;
using UI;
using MaterialData;
public class MainUI_ST : MonoBehaviour
{
    public DataHub_ST dataMng;
    public DataHub_Material dataHub_MT;
    public InfoUI_ST infoUI;

    public GameObject subBtmBar;
    public GameObject mainMenu;
    public GameObject subMenu;

    public Transform mainviewer;
    public Transform stageviewer;

    public Image bar;

    public TextMeshProUGUI[] main_menu_txt;

    stage_button_1[] main_stage_btns;
    Text[] stageTxt;
    Icon[] materialIcon;

    StageList curData;
    CanvasScaler canvasScreen;
    RectTransform rect;

    List<StageList> cur_stage_data;
    public void setCanvasScale()
    {
        canvasScreen = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>();
        rect = canvasScreen.transform.GetChild(0).GetComponent<RectTransform>();
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScreen, rect);
    }
    public void Initialized()
    {
        main_stage_btns = new stage_button_1[mainviewer.childCount];
        stageTxt = new Text[stageviewer.childCount];
        materialIcon = new Icon[stageviewer.childCount];

        for (int i = 0; i < mainviewer.childCount; i++)
            main_stage_btns[i] = new stage_button_1(mainviewer.GetChild(i));
       
        for (int i = 0; i < stageviewer.childCount; i++)
        {
            stageTxt[i] = stageviewer.GetChild(i).GetChild(0).GetComponent<Text>();

            Icon tmp = new Icon();
            tmp.This = stageviewer.GetChild(i).GetChild(1).gameObject;
            tmp.frame_raw = stageviewer.GetChild(i).GetChild(1).GetComponent<RawImage>();
            tmp.icon_raw = stageviewer.GetChild(i).GetChild(1).GetChild(0).GetComponent<RawImage>();
            materialIcon[i] = tmp;
        }
        bar.gameObject.SetActive(false);
        SetStart();
        StartCoroutine(DownLoadDropData());
    }
    public void clear_AND_select_menu_txt(int index)
    {
        for (int i = 0; i < main_menu_txt.Length; i++)
            main_menu_txt[i].color = Color.gray;

        main_menu_txt[index].color = new Color(0.86f, 0.86f, 0.86f);
    }
    IEnumerator DownLoadDropData()
    {
        string url = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/DB/NEW/stage_dropdata.xml";
        string ver = StatusChecking.instance.setting.stagedropDB_ver;
        string cur_ver = XML.This.getVersionCode("drop");
        if (ver != cur_ver && ver != string.Empty)
        {
            bar.gameObject.SetActive(true);
            string path = Files.Use.DocumentsPath("Resource/DB");
            yield return StartCoroutine(Download.Use.DownLoad(url, path, "stage_dropdata.xml", bar, null, "NULL"));
            XML.This.setVersionCode("drop", ver);
            yield return new WaitForSeconds(0.1f);
            bar.gameObject.SetActive(false);
        }
    }
    public void SetStart()
    {
        cur_stage_data = dataMng.stages;
        refresh_main_menu();

        mainMenu.SetActive(true);
        subMenu.SetActive(false);
        subBtmBar.SetActive(false);
    }
    public void refresh_main_menu()
    {
        for (int i = 0; i < main_stage_btns.Length; i++)
        {
            if (i >= cur_stage_data.Count)
            {
                main_stage_btns[i]._this.SetActive(false);
                continue;
            }
            if (cur_stage_data == dataMng.stages)
                main_stage_btns[i].Refresh(cur_stage_data[i].name, 1);
            else
                main_stage_btns[i].Refresh(cur_stage_data[i].name, cur_stage_data[i].date_start, cur_stage_data[i].date_exite);
        }
    }
    public void refresh_sub_menu()
    {
        for (int i = 0; i < stageTxt.Length; i++)
        {
            if (i >= curData.data.Count)
            {
                stageTxt[i].transform.parent.gameObject.SetActive(false);
                continue;
            }
            stageTxt[i].text = curData.data[i].name;
            stageTxt[i].transform.parent.gameObject.SetActive(true);

            EliteMaterial tmp = dataHub_MT.get_material_all(curData.data[i].ndrop[0]);
            if (tmp != null)
            {
                materialIcon[i].icon_raw.texture = tmp.icon;
                materialIcon[i].frame_raw.texture = dataHub_MT.GetMaterialFrame(tmp.rare);
                materialIcon[i].This.SetActive(true);
            }
            else
                materialIcon[i].This.SetActive(false);
        }
        mainMenu.SetActive(false);
        subMenu.SetActive(true);
        subBtmBar.SetActive(true);
    }
    public void TR_SetSubMenu(Transform set)
    {
        int index = set.GetSiblingIndex();
        curData = cur_stage_data[index];
        refresh_sub_menu();
    }
   
    public void TR_SetMainMenu()
    {
        if (infoUI.mainCanvas.enabled)
            infoUI.TR_Close();

        cur_stage_data = dataMng.stages;
        refresh_main_menu();
        clear_AND_select_menu_txt(0);

        mainMenu.SetActive(true);
        subMenu.SetActive(false);
        subBtmBar.SetActive(false);
    }
    public void tr_set_event_menu()
    {
        if (infoUI.mainCanvas.enabled)
            infoUI.TR_Close();

        if (dataMng.get_event_stage_data() != null)
            cur_stage_data = dataMng.get_event_stage_data();
        refresh_main_menu();
        clear_AND_select_menu_txt(1);

        mainMenu.SetActive(true);
        subMenu.SetActive(false);
        subBtmBar.SetActive(false);
    }

    public string get_stage_type()
    {
        if (cur_stage_data == dataMng.stages)
            return "nomarl";
        else return "event";
    }

    public bool get_timeout_stage()
    {
        if (curData.code.Equals("AF"))
        {
            System.DateTime cur_date = System.DateTime.Now;
            System.DateTime start_date = System.Convert.ToDateTime(curData.date_start);
            System.DateTime exite_date = System.Convert.ToDateTime(curData.date_exite);

            System.TimeSpan timeCal = start_date - cur_date;

            if (timeCal.Days > 0) return false;
            else if (timeCal.Hours > 0) return false;
            else if (timeCal.Minutes > 0) return false;
            else
            {
                System.TimeSpan timeCal_end = exite_date - cur_date;
                if (timeCal_end.Days > 0) return true;
                else if (timeCal_end.Hours > 0) return true;
                else if (timeCal_end.Minutes > 0) return true;
                else return false;
            }
        }
        else
            return false;
    }
    public void TR_SetStage(Transform set)
    {
        int index = set.GetSiblingIndex();

        infoUI.curData = curData.data[index];
        infoUI.StartUI_Main();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (infoUI.mainCanvas.enabled)
                infoUI.TR_Close();
            else if (subMenu.gameObject.activeSelf)
                TR_SetMainMenu();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
}
