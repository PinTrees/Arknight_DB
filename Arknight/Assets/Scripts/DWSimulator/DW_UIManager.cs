using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CharactorDataSet;
using UI;
using TMPro;
public class DW_UIManager : MonoBehaviour
{
    public DW_DataHub dataHub;

    [Header("- UI Menu")]
    public Text stack;

    [Header("- UI Main Object")]
    private GameObject[] eventPanel;
    private RawImage[] banner;
    private Text[] date;

    [Header("- UI Popup Object")]
    public GameObject Panel;
    private GameObject[] IconOb;
    private RawImage[] icon;
    private Text[] name;
    private RawImage[] rare;
    private RawImage[] backgraound;
    GameObject[] cur_tag;
    GameObject[] end_tag;
    Image[] load_bar;
    List<Icon_Operator> oper_icons;

    [Header("- UI Canvas")]
    public Canvas oper_canvas;

    public Transform Viewer;
    public Transform eventViewer;
    public Transform oper_viewer;

    public Texture2D noneImg;

    private int curEvent;
    private int curCount;
    CanvasScaler canvasScreen;
    RectTransform rect;
    public void Initialized()
    {
        IconOb = new GameObject[10];
        icon = new RawImage[10];
        name = new Text[10];
        rare = new RawImage[10];
        backgraound = new RawImage[10];

        eventPanel = new GameObject[10];
        banner = new RawImage[10];
        date = new Text[10];
        cur_tag = new GameObject[10];
        end_tag = new GameObject[10];
        load_bar = new Image[10];

        canvasScreen = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>();
        rect = canvasScreen.transform.GetChild(0).GetComponent<RectTransform>();
        CanvasScreen androidProgram = new CanvasScreen();
        androidProgram.setCanvasScreen(XML.This, canvasScreen, rect);

        for (int i = 0; i < Viewer.childCount; i++)
        {
            IconOb[i] = Viewer.GetChild(i).gameObject;
            icon[i] = IconOb[i].transform.GetChild(2).GetComponent<RawImage>();
            name[i] = IconOb[i].transform.GetChild(3).GetComponent<Text>();
            rare[i] = IconOb[i].transform.GetChild(4).GetComponent<RawImage>();
            backgraound[i] = IconOb[i].transform.GetChild(0).GetComponent<RawImage>();
        }
        for (int i = 0; i < 10; i++)
        {
            eventPanel[i] = eventViewer.GetChild(i).gameObject;
            banner[i] = eventViewer.GetChild(i).GetComponent<RawImage>();
            date[i] = eventViewer.GetChild(i).GetChild(4).GetComponent<Text>();
            cur_tag[i] = eventViewer.GetChild(i).GetChild(7).gameObject;
            end_tag[i] = eventViewer.GetChild(i).GetChild(6).gameObject;
            load_bar[i] = eventViewer.GetChild(i).GetChild(8).GetComponent<Image>();
        }

        oper_icons = new List<Icon_Operator>();
        for (int i = 0; i < oper_viewer.childCount; i++)
        {
            Icon_Operator tmp = new Icon_Operator();
            tmp._this = oper_viewer.transform.GetChild(i).gameObject;
            tmp.icon = oper_viewer.transform.GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.frame = oper_viewer.transform.GetChild(i).GetChild(1).GetComponent<Image>();
            tmp.name = oper_viewer.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
            oper_icons.Add(tmp);
        }

        oper_canvas.enabled = false;

        Clear();
        Refresh();
        Refresh_Menu();
        Panel.SetActive(false);

        ClearFile();
    }
    private void OnDestroy()
    {
        for (int i = 0; i < dataHub.eventData.Count; i++)
            Destroy(banner[i].texture);
    }
    void ClearFile()
    {
        string path = Files.Use.DocumentsPath("Resource/EventBanner/");
        string[] _filePaths = System.IO.Directory.GetFiles(path, "*.png",
                                  System.IO.SearchOption.TopDirectoryOnly);
        Debug.Log(_filePaths[0]);

        List<string> filePaths = _filePaths.ToList();
        Debug.Log(filePaths.Count);

        List<EventOffer> data = dataHub.eventData;
        for (int i = 0; i < 10; i++)
        {
            string curPath = Files.Use.DocumentsPath("Resource/EventBanner/" + data[i].fileName);
            filePaths.Remove(curPath);
        }

        string bpath = Files.Use.DocumentsPath("Resource/EventBanner/Banner.png");
        filePaths.Remove(bpath);
        for (int i = 0; i < filePaths.Count; i++)
        {
            System.IO.File.Delete(filePaths[i]);
            Debug.Log("file delete");
        }
    }
    private void Clear()
    {
        for (int i = 0; i < eventPanel.Length; i++)
            eventPanel[i].SetActive(false);
    }
    public void Refresh()
    {
        string event_url = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Resources/Event/History";
        string event_path = StatusChecking.instance.pathForDocumentsFile("Resource/EventBanner");
        System.DateTime curData = System.DateTime.Now;
        System.DateTime StartDate = System.DateTime.Now;

        List<EventOffer> data = dataHub.eventData;
        for (int i = 0; i < data.Count; i++)
        {
            if (i >= 10)
                break;

            cur_tag[i].SetActive(false);
            end_tag[i].SetActive(false);

            if (!data[i].endData.Equals(string.Empty))
                StartDate = System.Convert.ToDateTime(data[i].endData); // end time

            string path = Files.Use.DocumentsPath("Resource/EventBanner");

            date[i].text = data[i].date;
            banner[i].texture = Files.Use.GetPNG(path, data[i].fileName);
            if (banner[i].texture == null
                || banner[i].texture.width < 128)
            {
                string cur_url = string.Format("{0}/{1}", event_url, data[i].fileName);

                StartCoroutine(LogU.Use.SetLog("이벤트 파일을 다운로드중..."));
                StartCoroutine(Download.Use.EventFile(cur_url, event_path, data[i].fileName, load_bar[i], banner[i]));
            }
            eventPanel[i].SetActive(true);

            System.TimeSpan timeCal = StartDate - curData; // 시간차 계산

            if (timeCal.Days < 0)
                end_tag[i].SetActive(true);
            else if (timeCal.Hours < 0)
                end_tag[i].SetActive(true);
            else if (timeCal.Minutes < 0)
                end_tag[i].SetActive(true);
            else
                cur_tag[i].SetActive(true);
        }
    }
    public void Refresh_Menu()
    {
        int add_per;
        if (dataHub.stack >= 50)
        {
            int tmp = (dataHub.stack - 50) / 10;
            add_per = ((tmp + 1) * 2) + 2;
        }
        else
            add_per = 2;

        stack.text = "6성 확률 업 스택 : " + dataHub.stack.ToString() + "  6성 등장 확률 : " + add_per.ToString() + "%";
    }
    public void LoadSceneMain()
    {
        SceneManager.LoadScene("Main");
    }
    public void SetActiveDWPanel(bool set)
    {
        Panel.SetActive(set);
    }
    private IEnumerator SetDWPanel(List<OperaterClass> data)
    {
        Panel.SetActive(true);

        for(int i = 0; i < 10; i++)
        {
            if (i >= data.Count)
            {
                IconOb[i].SetActive(false);
                continue;
            }

            if (data[i].icon != null)
                icon[i].texture = data[i].icon;

            name[i].text = data[i].name;
            rare[i].texture = dataHub.GetRareTexture(data[i].rare);

            if (data[i].rare.Equals("6"))
                backgraound[i].color = new Color(212 / 225f, 122 / 255f, 49 / 255f);
            else if (data[i].rare.Equals("5"))
                backgraound[i].color = new Color(233 / 225f, 180 / 255f, 0 / 255f);
            else if (data[i].rare.Equals("4"))
                backgraound[i].color = new Color(171 / 225f, 71 / 255f, 212 / 255f);
            else if (data[i].rare.Equals("3"))
                backgraound[i].color = new Color(71 / 225f, 110 / 255f, 212 / 255f);

            IconOb[i].SetActive(true);
        }

        Refresh_Menu();
        yield return null;
    }
    public void UIStart_DropTable(Transform set)
    {
        EventOffer eventData = dataHub.eventData[set.GetSiblingIndex()];

        List<OperaterClass> data = new List<OperaterClass>();
        for (int i = 0; i < eventData.rare6_up.Length; i++)
        {
            if (eventData.rare6_up[i].Equals(string.Empty))
                break;
            else
                data.Add(dataHub.GetOperatironData(eventData.rare6_up[i]));
        }
        for (int i = 0; i < eventData.rare6.Length; i++)
        {
            if (eventData.rare6[i].Equals(string.Empty))
                break;
            else
                data.Add(dataHub.GetOperatironData(eventData.rare6[i]));
        }
        for (int i = 0; i < eventData.rare5_up.Length; i++)
        {
            if (eventData.rare5_up[i].Equals(string.Empty))
                break;
            else
                data.Add(dataHub.GetOperatironData(eventData.rare5_up[i]));
        }
        for (int i = 0; i < eventData.rare5.Length; i++)
        {
            if (eventData.rare5[i].Equals(string.Empty))
                break;
            else
                data.Add(dataHub.GetOperatironData(eventData.rare5[i]));
        }
        for (int i = 0; i < eventData.rare5.Length; i++)
        {
            if (eventData.rare5[i].Equals(string.Empty))
                break;
            else
                data.Add(dataHub.GetOperatironData(eventData.rare5[i]));
        }
        for (int i = 0; i < eventData.rare4.Length; i++)
        {
            if (eventData.rare4[i].Equals(string.Empty))
                break;
            else
                data.Add(dataHub.GetOperatironData(eventData.rare4[i]));
        }
        for (int i = 0; i < eventData.rare3.Length; i++)
        {
            if (eventData.rare3[i].Equals(string.Empty))
                break;
            else
                data.Add(dataHub.GetOperatironData(eventData.rare3[i]));
        }
     
        for (int i = 0; i < oper_icons.Count; i++)
        {
            if (i >= data.Count)
            {
                oper_icons[i]._this.SetActive(false);
                continue;
            }
            oper_icons[i].name.text = data[i].name;        // 이름 출력

            oper_icons[i].Refresh(data[i].icon, dataHub.GetRareColor(data[i].rare), data[i].name);
        }

        oper_canvas.enabled = true;
    }
    public void UIExit_DropTable()
    {
        oper_canvas.enabled = false;
    }
    public void TR_DWOperator(Transform set)
    {
        Debug.Log(set.parent.GetSiblingIndex());
        EventOffer eventData = dataHub.eventData[set.parent.GetSiblingIndex()];
        curEvent = set.parent.GetSiblingIndex();

        if (set.GetSiblingIndex().Equals(1))
            curCount = 10;
        else if (set.GetSiblingIndex().Equals(0))
            curCount = 1;
     
        List<OperaterClass> data = new List<OperaterClass>();
        for (int i = 0; i < curCount; i++)
        {
            OperaterClass tmp = dataHub.GetRandomOperator(eventData);
            data.Add(tmp);
        }

        StartCoroutine(SetDWPanel(data));
    }
    public void TR_DWOperator()
    {
        EventOffer eventData = dataHub.eventData[curEvent];

        List<OperaterClass> data = new List<OperaterClass>();
        for (int i = 0; i < curCount; i++)
        {
            OperaterClass tmp = dataHub.GetRandomOperator(eventData);
            data.Add(tmp);
        }

        StartCoroutine(SetDWPanel(data));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Panel.activeSelf)
                Panel.SetActive(false);
            else
                LoadSceneMain();
        }
    }
}
