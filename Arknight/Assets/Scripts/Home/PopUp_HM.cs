using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PopUp_HM : MonoBehaviour
{
    public Home_UI_Manager HomeUIMng;

    public GameObject Alert_Download;
    public Image alert_bar;
    public Text alert_info;
    Coroutine alert_coroutine;

    public void Initialized()
    {
        Alert_Download.SetActive(false);
    }
    public IEnumerator StartUI_AlertDownload()
    {
        yield return alert_coroutine = StartCoroutine(RefreshUI_AlertDownload());
        alert_coroutine = null;
        StartCoroutine(HomeUIMng.UpdateAuto());
        yield return null;
    }
    public IEnumerator RefreshUI_AlertDownload()
    {
        Alert_Download.SetActive(true);
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 100; i++)
        {
            alert_bar.fillAmount = (i + 1) / 100f;
            if (i.Equals(0))
                alert_info.text = "3초 후 자동으로 다운로드를 시작 합니다.";
            else if (i.Equals(33))
                alert_info.text = "2초 후 자동으로 다운로드를 시작 합니다.";
            else if (i.Equals(66))
                alert_info.text = "1초 후 자동으로 다운로드를 시작 합니다.";
            yield return new WaitForSeconds(.03f);
        }
        Alert_Download.SetActive(false);
    }
    public void ExitUI_AlertDownload()
    {
        if (alert_coroutine != null)
            StopCoroutine(alert_coroutine);

        Alert_Download.SetActive(false);
    }
}
