using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class QeUI_HO : MonoBehaviour
{
    public GameObject UI;

    [Header("UI - Menu")]
    public GameObject[] menu;      // [0] main, [1] QA

    [Header("UI - Components")]
    public Text Title;

    public void Initialized()
    {
        Clear();
        UI.SetActive(false);
    }
    public void Clear()
    {
        for (int i = 0; i < menu.Length; i++)
            menu[i].SetActive(false);
    }
    public void TR_SetStart()
    {
        Clear();
        menu[0].SetActive(true);
        Title.text = "문의 메뉴";
        UI.SetActive(true);
    }
    public void TR_Close()
    {
        UI.SetActive(false);
    }
    public void TR_QAMenu()
    {
        Clear();
        menu[1].SetActive(true);
        Title.text = "자주 하는 문의";
    }
    public void TR_Debug()
    {
        Application.OpenURL("https://pinforest.imweb.me/freeboard/?q=YToxOntzOjEyOiJrZXl3b3JkX3R5cGUiO3M6MzoiYWxsIjt9&board=b2019123165f5b7e0347ec&bmode=write&back_url=L2ZyZWVib2FyZA%3D%3D");
    }
    public void TR_GoogleQuestDropData()
    {
        Application.OpenURL("https://forms.gle/oDRRhMoVuPuc61Cr9");
    }
}
