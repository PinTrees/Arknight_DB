using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LogSetting : MonoBehaviour
{
    public Text log;
    public void setData(string _set)
    {
        this.gameObject.SetActive(true);
        log.text = _set;
        StartCoroutine(WaitForIt());
    }
    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
    }
}
