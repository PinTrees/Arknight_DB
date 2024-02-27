using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Networking;
using UnityEngine.UI;
using System.Xml;
public class Initialize_Start : MonoBehaviour
{
    public Text per;
    public Image bar;
    private string versionUrl = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/Version/Version.xml";
    private string cacheURL = "https://raw.githubusercontent.com/PinTrees/Arknight_DB/master/cache/cache.xml";

    void Start()
    {
        AdmobVideoScript.This.Initialized();

        StartCoroutine(Initialized());
    }
    IEnumerator Initialized()
    {
        if (StatusChecking.instance.InternetNetworkStatus())
        {
            UnityWebRequest request = UnityWebRequest.Get(versionUrl);
            request.SendWebRequest();
            while (!request.isDone)
            {
                per.text = string.Format("{0}{1}", Mathf.RoundToInt(request.downloadProgress * 100), "%");
                bar.fillAmount = request.downloadProgress / 1.0f;
                yield return new WaitForSeconds(.01f);
            }
            bar.fillAmount = 1; per.text = "100%";
            Version.Use.versionXML = request.downloadHandler.text;

            UnityWebRequest cacheRequest = UnityWebRequest.Get(cacheURL);
            cacheRequest.SendWebRequest();
            while (!cacheRequest.isDone)
            {
                per.text = string.Format("{0}{1}", Mathf.RoundToInt(cacheRequest.downloadProgress * 100), "%");
                bar.fillAmount = cacheRequest.downloadProgress / 1.0f;
                yield return new WaitForSeconds(.01f);
            }
            bar.fillAmount = 1; per.text = "100%";
            Version.Use.cacheXML = cacheRequest.downloadHandler.text;
        }
        else
        {
            Version.Use.versionXML = string.Empty;
            Version.Use.cacheXML = string.Empty;
        }

        if (!PlayerPrefs.HasKey("Version"))
        {
            PlayerPrefs.SetString("Version", "0,0");
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetString("Version") == "")
        {
            PlayerPrefs.SetString("Version", "0,0");
            PlayerPrefs.Save();
        }

        string[] arr = PlayerPrefs.GetString("Version").Split(',');

        if (PlayerPrefs.HasKey("Tutorial").Equals(false))
        {
            PlayerPrefs.SetString("Tutorial", "0");
            PlayerPrefs.Save();
        }
        else
        {
            if (PlayerPrefs.GetString("Tutorial").Equals("4")) // ver 4
            {
                Debug.Log("리소스 다운 완료");
                SceneManager.LoadScene("Main");
            }
            else
            {
                if (int.Parse(arr[0]) >= 9)
                {
                    // Resources Download
                    string save = string.Format("{0},{1}", "0", arr[1]);
                    PlayerPrefs.SetString("Version", save);
                    PlayerPrefs.Save();
                }
            }
        }

        bool success = Caching.ClearCache();
        if (success)
            Debug.Log("캐시가 삭제되었습니다.");
        else Debug.Log("캐시를 삭제할 수 없습니다.");

        SceneManager.LoadScene("Main");
    }
}
