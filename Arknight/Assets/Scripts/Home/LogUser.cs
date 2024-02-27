using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class LogUser : MonoBehaviour
{
    public static LogUser This;

    public Text VersionLog;
    public Text ResourcesLog;
    public Text DBLog;
    public Text NetLog;
    // Start is called before the first frame update
    private void Awake()
    {
        This = this;

        VersionLog.transform.parent.gameObject.SetActive(false);
        ResourcesLog.transform.parent.gameObject.SetActive(false);
        DBLog.transform.parent.gameObject.SetActive(false);
        NetLog.transform.parent.gameObject.SetActive(false);
    }
    public void Clear()
    {
        VersionLog.transform.parent.gameObject.SetActive(false);
        ResourcesLog.transform.parent.gameObject.SetActive(false);
        DBLog.transform.parent.gameObject.SetActive(false);
        NetLog.transform.parent.gameObject.SetActive(false);
    }
}
