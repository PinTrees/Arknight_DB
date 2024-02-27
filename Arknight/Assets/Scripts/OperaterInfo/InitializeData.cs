using System.Collections;
using UnityEngine;

public class InitializeData : MonoBehaviour
{
    public GameObject singleton;
    public GameObject global;
    public GameObject LoadingUI;

    public Operater_Data_Hub datahub;
    public OperaterIcon_UI_Manager iconUImanager;
    public OperaterInfo_UIManager infoUIMng;
    public InfraUIManager_OP infraUIMng;
    OperaterMainUI_Manager mainUIMng;
    MenuUI_OPIF menuUIMng;
    void Start()
    {
        // Editor only ================
        if (XML.This == null)
            Instantiate(singleton);
        if (FirebaseDataBase.instance == null)
            Instantiate(global);
        // ============================
        mainUIMng = GameObject.FindGameObjectWithTag("UIManager").GetComponent<OperaterMainUI_Manager>();
        mainUIMng.Initialized();
        menuUIMng = GameObject.FindGameObjectWithTag("UIManager").GetComponent<MenuUI_OPIF>();
        menuUIMng.Initialized();
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        StartCoroutine(Initailized());
    }

    IEnumerator Initailized()
    {
        StatusChecking.instance.Initialized();

        LoadingUI.SetActive(true);
        yield return new WaitForEndOfFrame();
        infoUIMng.Initialized();

        yield return StartCoroutine(LogU.Use.SetLog("오퍼레이터 DB 파싱중..."));
        StartCoroutine(datahub.InitailizeData());
        StartCoroutine(iconUImanager.Initailized());
        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(LogU.Use.SetLog("오퍼레이터 아이콘 출력중..."));
        iconUImanager.Refresh(datahub.operatorData);
        infraUIMng.Initialized();
        yield return new WaitForEndOfFrame();

        LoadingUI.SetActive(false);
    }
}
