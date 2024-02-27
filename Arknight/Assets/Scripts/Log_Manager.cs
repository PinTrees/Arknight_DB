using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Log_Manager : MonoBehaviour
{
    public static Log_Manager instance;

    public Transform viewport_log;
    public GameObject panel_Log;

    private List<LogSetting> log_g_list = new List<LogSetting>();

    public int count = 0;
    public void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Initialized();
    }
    public void Initialized()
    {
        for (int i = 0; i < viewport_log.childCount; i++)
        {
            LogSetting tmp_b = viewport_log.GetChild(i).GetComponent<LogSetting>();
            log_g_list.Add(tmp_b);
        }
        Clear_Log();
    }
    public void Clear_Log()
    {
        for (int i = 0; i < log_g_list.Count; i++)
            log_g_list[i].gameObject.SetActive(false);
    }
    public void Add_Log(string set)
    {
        log_g_list[count++ % 10].GetComponent<LogSetting>().setData(set);
    }
}
