using System.Collections;
using UnityEngine;
using Setting;

public class Initialize_Home : MonoBehaviour
{
    public GameObject singletone;
    public HomeNotice_UIManager noticeUI;
    public Home_UI_Manager mainUI;
    public QeUI_HO qaUI;
    public PopUp_HM PopUIMng;
    public Tutorial tutorial;

    private DataBaseSetting setting;

    RankingUI_HM rankingUI;
    public void Start()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        StartCoroutine(Initialized());
    }
    IEnumerator Initialized()
    {
        if (StatusChecking.instance == null)
            GameObject.Instantiate(singletone);

        StatusChecking.instance.Initialized();
        setting = new DataBaseSetting();

        if (StatusChecking.instance.InternetNetworkStatus())
        {
            XML.This.Get_VersionInfo(setting);
            StatusChecking.instance.setting = setting;
        }
        else // 인터넷 연결 안됨
        {
            setting.DB_ver = "NULL";
            setting.apk_ver = "NULL";
            setting.notice = "Network Error";
            StatusChecking.instance.setting = setting;

            LogUser.This.NetLog.gameObject.SetActive(true);
        }

        rankingUI = GameObject.FindGameObjectWithTag("UIManager").GetComponent<RankingUI_HM>();
        rankingUI.Initialized();

        PopUIMng.Initialized();
        qaUI.Initialized();
        noticeUI.Initialized();
        mainUI.Initialized();
        mainUI.Refresh();

        tutorial.Initialized();
        yield return null;
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (rankingUI.mainCanvas.enabled)
                rankingUI.mainCanvas.enabled = false;
            else
                Application.Quit();
        }
    }
}
