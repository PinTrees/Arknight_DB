using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LogU : MonoBehaviour
{
    public GameObject log_panel;
    public Text log;
    static public LogU Use;

    float deaytime = 0.5f;
    void Awake()
    {
        Application.targetFrameRate = 60;
        Use = this;
        log_panel.SetActive(false);
    }
    Coroutine delay_corutine;
    public IEnumerator SetLog(string _log)
    {
        if (delay_corutine != null)
            StopCoroutine(delay_corutine);

        yield return StartCoroutine(UIRefresh_Log(_log));
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(deaytime);
        log_panel.SetActive(false);

        delay_corutine = null;
    }
    IEnumerator UIRefresh_Log(string _log)
    {
        log.text = _log;
        if(log_panel.activeSelf.Equals(false))
            log_panel.SetActive(true);

        yield return new WaitForEndOfFrame();
        delay_corutine = StartCoroutine(Delay());
    }

    public IEnumerator SetLogUserDelay(string _log, float _delay)
    {
        if (delay_corutine != null)
            StopCoroutine(delay_corutine);

        yield return StartCoroutine(RefreshUI_Log(_log, _delay));
    }
    IEnumerator UserDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        log_panel.SetActive(false);

        delay_corutine = null;
    }
    IEnumerator RefreshUI_Log(string _log, float _delay)
    {
        log.text = _log;
        if (log_panel.activeSelf.Equals(false))
            log_panel.SetActive(true);

        yield return new WaitForEndOfFrame();
        delay_corutine = StartCoroutine(UserDelay(_delay));
    }

    //float deltaTime = 0.0f;
    //void Update()
    //{
    //    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    //}
    //void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, w, h * 2 / 100);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = h * 2 / 100;
    //    style.normal.textColor = Color.red;
    //    float msec = deltaTime * 1000.0f;
    //    float fps = 1.0f / deltaTime;
    //    string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    //    GUI.Label(rect, text, style);
    //}
}
