using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.UI;
using CharactorDataSet;
using System;
using TMPro;

public class ProfileUI_Op : MonoBehaviour
{
    public GameObject loadPanel;
    public Canvas mainCanvas;

    public RectTransform basePanel;
    public RectTransform ctPanel;
    public RectTransform profilPanel;
    public RectTransform imsangPanel;
    public RectTransform[] filePanel;

    TextMeshProUGUI baseInfo;
    TextMeshProUGUI ctInfo;
    TextMeshProUGUI profilInfo;
    TextMeshProUGUI imdangInfo;
    TextMeshProUGUI[] fileInfo;

    OperaterClass curData;
    public void Initialized()
    {
        fileInfo = new TextMeshProUGUI[filePanel.Length];
        baseInfo = basePanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        ctInfo = ctPanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        profilInfo = profilPanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        imdangInfo = imsangPanel.GetChild(1).GetComponent<TextMeshProUGUI>();
      
        for(int i = 0; i < filePanel.Length; i++)
            fileInfo[i] = filePanel[i].GetChild(1).GetComponent<TextMeshProUGUI>();

        mainCanvas.enabled = false;
    }

    public void tr_start_mainUI(OperaterClass set)
    {
        curData = set;
        StartCoroutine(start_MainUI());
    }
    public void tr_exite_mainUI()
    {
        mainCanvas.enabled = false;
    }
    IEnumerator start_MainUI()
    {
        loadPanel.SetActive(true);
        UnityWebRequest request = UnityWebRequest.Get("https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/database/KR/profile/" + curData.en_name + ".xml");
        yield return request.SendWebRequest();
        loadPanel.SetActive(false);
      
        if (request.isNetworkError)
        {
            StartCoroutine(LogU.Use.SetLog("인터넷 연결을 확인해 주세요."));
            yield break;
        }
        else if (request.isHttpError)
        {
            StartCoroutine(LogU.Use.SetLog("개발중 입니다."));
            yield break;
        }

        string downloadData = request.downloadHandler.text;
        Dictionary<string, string> data = XML.This.get_profileData_from_stringFile(downloadData);

        if (data.TryGetValue("base", out string return_baseInfo))
        {
            baseInfo.text = return_baseInfo;
            basePanel.sizeDelta = new Vector2(basePanel.rect.width, baseInfo.preferredHeight + 100);
        }
        if (data.TryGetValue("ComprehensiveTest", out string return_testInfo))
        {
            ctInfo.text = return_testInfo;
            ctPanel.sizeDelta = new Vector2(ctPanel.rect.width, ctInfo.preferredHeight + 100);
        }
        if (data.TryGetValue("profil", out string return_profilInfo))
        {
            profilInfo.text = return_profilInfo;
            profilPanel.sizeDelta = new Vector2(profilPanel.rect.width, profilInfo.preferredHeight + 100);
        }
        if (data.TryGetValue("ClinicalDiagnosisAnalysis", out string return_Analysis))
        {
            imdangInfo.text = return_Analysis;
            imsangPanel.sizeDelta = new Vector2(imsangPanel.rect.width, imdangInfo.preferredHeight + 100);
        }
        for (int i = 0; i < fileInfo.Length; i++)
            if (data.TryGetValue("file" + (i + 1).ToString(), out string return_fileInfo))
            {
                fileInfo[i].text = return_fileInfo;
                filePanel[i].sizeDelta = new Vector2(filePanel[i].rect.width, fileInfo[i].preferredHeight + 100);
            }
        mainCanvas.enabled = true;
    }
}
