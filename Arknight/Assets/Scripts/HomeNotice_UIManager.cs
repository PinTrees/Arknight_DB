using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Setting;
public class HomeNotice_UIManager : MonoBehaviour
{
    public ScrollRect viewer;
    public Text notice;

    public void Initialized()
    {
        DataBaseSetting set = StatusChecking.instance.setting;

        notice.text = string.Format("{0}{0}{0}{0}{1}{0}{0}{0}{0}{0}{0}{0}{0}{0}", System.Environment.NewLine, set.notice);
        viewer.content.anchoredPosition = new Vector2(viewer.content.position.x, 0);
    }

    public void TR_OpenLink()
    {
        Application.OpenURL("https://pinforest.imweb.me/freeboard/?q=YToxOntzOjEyOiJrZXl3b3JkX3R5cGUiO3M6MzoiYWxsIjt9&board=b2019123165f5b7e0347ec&bmode=write&back_url=L2ZyZWVib2FyZA%3D%3D");
    }
}
