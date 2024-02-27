using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class Donate_UIManager : MonoBehaviour
{
    public static Donate_UIManager This;
    public Text count;

    private void Awake()
    {
        This = this;
        Refresh();
    }
    public void Trigger_Ad()
    {
        AdmobVideoScript.This.TR_ShowAds();
    }
    public void Refresh()
    {
        int data;
        if (PlayerPrefs.HasKey("Data").Equals(false))
        {
            PlayerPrefs.SetInt("Data", 0);
            PlayerPrefs.Save();
            data = 0;
        }
        else
        {
            data = PlayerPrefs.GetInt("Data");
        }
        count.text = data.ToString();
    }
}
