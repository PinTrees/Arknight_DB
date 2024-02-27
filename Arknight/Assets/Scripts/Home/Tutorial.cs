using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tutorial : MonoBehaviour
{
    public GameObject mainPanel;
    public RawImage mainImg;
    public Image bar;
    public Text value;

    string softkeyUrl = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/tutorial/softkey";
    int softkeyIndex = 0;
    bool loading = false;
    public void Initialized()
    {
        mainImg.enabled = false;
        mainPanel.SetActive(false);
        int progress = XML.This.getTutorialProgress();
        if (progress.Equals(0))
        {
            mainPanel.SetActive(true);
            softkeyIndex = 1;
            RefreshTutorial();
        }
    }
    void RefreshTutorial()
    {
        loading = true;
        StartCoroutine(setTutorialIamge());
        if (softkeyIndex >= 4)
        {
            XML.This.setTutorialProgress(1);
            mainPanel.SetActive(false);
        }
    }
    IEnumerator setTutorialIamge()
    {
        yield return StartCoroutine(Download.Use.getTextureFormWWW(softkeyUrl + "/" + softkeyIndex.ToString() + ".png", mainImg, bar, value));
        mainImg.enabled = true;
        yield return new WaitForSeconds(1f);
        loading = false;
    }
    public void TRsetNext(int _where)
    {
        if (loading)
        {
            Log_Manager.instance.Add_Log("조금만 기다려 주세요");
            return;
        }
        if (_where.Equals(1))
        {
            softkeyIndex++;
            RefreshTutorial();
        }
    }
}
