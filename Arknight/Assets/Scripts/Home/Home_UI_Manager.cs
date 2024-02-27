using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

using CharactorDataSet;
using UnityEngine.UI;

public class Home_UI_Manager : MonoBehaviour
{
    public FileMNG_HM setUI;
    public PopUp_HM PopMng;
    [Header("- UI Load Components")]
    public GameObject loadpanel;

    [Header("- UI Menu Object")]
    public Text MenuTitle;
    public GameObject[] Menu;

    [Header("- UI Panel Popup")]
    public GameObject panel_ads;
    public GameObject dbUpateUI;

    [Header("- UI Button Component")]
    public Image lockBtn;

    [Header("- UI Component")]
    public GameObject notice_new;
    public GameObject loadUI;
    public RawImage banner;

    [Header("- UI Loading Component")]
    public Image bar;
    public Image mainbar;
    public Text mainvalue;
    public Text value;
    public Text filename;

    [Header("- UI Alert Component")]
    public Image[] alert;               // [0] FileMng, [1] Resource, [2] Audio, [3] DB

    private int curMenuIdx;
    private string[] url;
    private string[] uiSet;
    public void Initialized()
    {
        loadUI.SetActive(false);
        setUI.Initialized();
        url = XML.This.Get_UrlData();
        uiSet = XML.This.Get_UISetting();

        panel_ads.SetActive(false);
        ClearMenu();
        TR_SetMenu(int.Parse(uiSet[0]));

        for (int i = 0; i < alert.Length; i++)
            alert[i].gameObject.SetActive(false);
    }
    public void ClearMenu()
    {
        for (int i = 0; i < Menu.Length; i++)
        {
            if (Menu[i] == null)
                continue;
            Menu[i].SetActive(false);
        }
    }
    public void Refresh()
    {
        LogUser.This.Clear();
        notice_new.SetActive(false);

        banner.gameObject.SetActive(false);

        for (int i = 0; i < alert.Length; i++)
            alert[i].gameObject.SetActive(false);

        string new_ver = StatusChecking.instance.setting.apk_ver;
        if (!new_ver.Equals(Application.version))
        {
            LogUser.This.VersionLog.text = new_ver;
            LogUser.This.VersionLog.transform.parent.gameObject.SetActive(true);
            notice_new.SetActive(true);
        }
        else
            LogUser.This.VersionLog.transform.parent.gameObject.SetActive(false);

        // Version Alert - Start ============================================================
        if (StatusChecking.instance.InternetNetworkStatus())
        {
            bool versionFlag = false;
            string[] version = XML.This.Get_VersionCode();
            if (!version[0].Equals(StatusChecking.instance.setting.resources_ver))
            {
                alert[1].gameObject.SetActive(true);
                versionFlag = true;

                StartCoroutine(PopMng.StartUI_AlertDownload());
                return;
            }
            if (!version[1].Equals(StatusChecking.instance.setting.audio_ver))
            {
                alert[2].gameObject.SetActive(true);
                versionFlag = true;
            }
            string new_dbver = StatusChecking.instance.setting.DB_ver;
            string cur_dbver = StatusChecking.instance.user_setting.DB_ver;

            if (!cur_dbver.Equals(new_dbver))
            {
                LogUser.This.DBLog.text = "DB 업데이트";
                LogUser.This.DBLog.transform.parent.gameObject.SetActive(true);
                alert[3].gameObject.SetActive(true);
                versionFlag = true;

                StartCoroutine(PopMng.StartUI_AlertDownload());
                return;
            }
            if (versionFlag)
                alert[0].gameObject.SetActive(true);
        }
        // Version Alert - End ============================================================

        string path = Files.Use.DocumentsPath("Resource/EventBanner");
        Texture2D tmp = Files.Use.GetPNG(path, "Banner.png");
        if (tmp != null)
            banner.texture = Files.Use.GetPNG(path, "Banner.png");

        banner.gameObject.SetActive(true);
    }
    public void Trigger_UpdateBtn()
    {
        Application.OpenURL(StatusChecking.instance.setting.apklink);
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void Trigger_OpenOperatorDataHub()
    {
        SceneManager.LoadScene("operator_info");
    }
    // ==================================================================================
    public void TR_LockMenu()
    {
        PlayerPrefs.SetString("UI", curMenuIdx.ToString());
        PlayerPrefs.Save();

        uiSet = XML.This.Get_UISetting();
        TR_SetMenu(int.Parse(uiSet[0]));
    }
    public void TR_SetMenu(int index)
    {
        curMenuIdx = index;

        if (uiSet[0].Equals(index.ToString()))
            lockBtn.color = new Color(225 / 255f, 225 / 255f, 225 / 255f);
        else
            lockBtn.color = new Color(68/255f, 68/255f, 68/255f);

        if (index.Equals(0))
            MenuTitle.text = "명일방주 도감";
        else if (index.Equals(1))
            MenuTitle.text = "명일방주 계산기";
        else if (index.Equals(2))
            MenuTitle.text = "명일방주 창작";
        else if (index.Equals(3))
            MenuTitle.text = "파일 관리자";
        else if (index.Equals(4))
            MenuTitle.text = "패치 노트";

        if (Menu[index] != null)
        {
            ClearMenu();
            Menu[index].SetActive(true);
        }
    }
    // ==================================================================================
    public IEnumerator UpdateAuto()
    {
        yield return StartCoroutine(Download.Use.DownLoad_DataBase("NEW", bar, filename, value, loadUI));
        yield return StartCoroutine(Download.Use.Resources(StatusChecking.instance.setting.resources_ver, mainbar, bar, filename, value, mainvalue, loadUI));
        Refresh();
    }

    public void TR_ResourcesUpdate()
    {
        StartCoroutine(UpdateResources("NULL"));
    } // New
    public void TR_ResourcesDownAll()
    {
        Log_Manager.instance.Add_Log("리소스 파일 재설치");
        StartCoroutine(UpdateResources("Del"));
    }
    public void TR_Resources_Illust(string type)
    {
        if (type.Equals("ALL"))
            Log_Manager.instance.Add_Log("일러스트 파일 재설치");
        if (type.Equals("Del"))
        {
            Log_Manager.instance.Add_Log("일러스트 파일 삭제");
            StartCoroutine(Files.Use.DeleteFolder(Files.Use.DocumentsPath("Resource/OperatorIllust"), loadUI));
        }
        else
            StartCoroutine(Download.Use.OperatorIllust(type, mainbar, bar, filename, value, mainvalue, loadUI));
    }
    public void TR_Resources_Audio(string type)
    {
        StartCoroutine(UpdateAudio(type));
    }
    IEnumerator UpdateAudio(string type)
    {
        if (type.Equals("ALL"))
            Log_Manager.instance.Add_Log("오디오 파일 재설치");
        if (type.Equals("Del"))
        {
            Log_Manager.instance.Add_Log("오디오 파일 삭제");
            yield return StartCoroutine(Files.Use.DeleteFolders(Files.Use.DocumentsPath("Resource/OperatorAudio"), loadUI));
        }
        else
            yield return StartCoroutine(Download.Use.OperatorAudio(StatusChecking.instance.setting.audio_ver, mainbar, bar, filename, value, mainvalue, loadUI));

        Refresh();
    }
    IEnumerator UpdateDB(string ver)
    {
        if (ver.Equals("OG"))
            yield return StartCoroutine(Download.Use.DownLoad_DataBase("Format_1", bar, filename, value, loadUI));
        else if (ver.Equals("NEW"))
            yield return StartCoroutine(Download.Use.DownLoad_DataBase("NEW", bar, filename, value, loadUI));

        Refresh();
    }
    IEnumerator UpdateResources(string type)
    {
        if (type.Equals("Del"))
            yield return StartCoroutine(Download.Use.ResourcesDel(StatusChecking.instance.setting.resources_ver, mainbar, bar, filename, value, mainvalue, loadUI));
        else
            yield return StartCoroutine(Download.Use.Resources(StatusChecking.instance.setting.resources_ver, mainbar, bar, filename, value, mainvalue, loadUI));

        Refresh();
    }
    public void TR_DBUpdate(string ver)
    {
        StartCoroutine(UpdateDB(ver));
    }  
    public void TR_Corona19()
    {
        Application.OpenURL("https://coronaboard.kr/");
    }
    // ==================================================================================
    public void TR_Pinforest()
    {
        Application.OpenURL(url[0]);
    }
    public void TR_Delete_Illust()
    {
        loadpanel.SetActive(true);
        List<OperaterClass> data = new List<OperaterClass>();
        XML.This.Get_OperatorInfo(data);

        for(int i = 0; i < data.Count; i++)
        {
            string path = Files.Use.DocumentsPath(string.Format("Illust/{0}/{1}", data[i].rare, data[i].en_name));
            StartCoroutine(Files.Use.DeleteFolder(path, loadpanel));
        }
        string f_path = Files.Use.DocumentsPath("Illust/6");
        StartCoroutine(Files.Use.DeleteFolder(f_path, loadpanel));
        f_path = Files.Use.DocumentsPath("Illust/5");
        StartCoroutine(Files.Use.DeleteFolder(f_path, loadpanel));
        f_path = Files.Use.DocumentsPath("Illust/4");
        StartCoroutine(Files.Use.DeleteFolder(f_path, loadpanel));
        f_path = Files.Use.DocumentsPath("Illust/3");
        StartCoroutine(Files.Use.DeleteFolder(f_path, loadpanel));
        f_path = Files.Use.DocumentsPath("Illust/2");
        StartCoroutine(Files.Use.DeleteFolder(f_path, loadpanel));
        f_path = Files.Use.DocumentsPath("Illust/1");
        StartCoroutine(Files.Use.DeleteFolder(f_path, loadpanel));
        f_path = Files.Use.DocumentsPath("Illust");
        StartCoroutine(Files.Use.DeleteFolder(f_path, loadpanel));

        Log_Manager.instance.Add_Log("파일 삭제 완료");
    }
    // ==================================================================================
    public void open_p_Ads()
    {
        panel_ads.SetActive(true);
    }
    public void close_p_Ads()
    {
        panel_ads.SetActive(false);
    }
}
