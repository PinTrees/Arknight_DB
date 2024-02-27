using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CharactorDataSet;
using TMPro;
public class MenuUI_OPIF : MonoBehaviour
{
    public OperaterIcon_UI_Manager iconUIMng;

    [Header("- Mian Menu Component")]
    public Image menuIcon;
    public GameObject menuContents;
    public GameObject opListMenu;
    public TextMeshProUGUI[] severfilterText;

    public int curMenuIndex;
    public void Initialized()
    {
        menuIcon.color = new Color(0.86f, 0.86f, 0.86f);
        menuContents.SetActive(false);
        ExitCurrentMenu();
    }
    public void TR_SetMenu(bool set)
    {
        menuContents.SetActive(set);
        if (!set)
            TR_SelectMenu(curMenuIndex);
    }
    public void ExitCurrentMenu()
    {
        opListMenu.SetActive(false);
        curMenuIndex = 0;
    }
    public void TR_SelectMenu(int index)
    {
        ExitCurrentMenu();
        curMenuIndex = index;
        if (index.Equals(1))
            opListMenu.SetActive(true);
        menuContents.SetActive(false);
    }

    public void TR_SelectSeverFillterMenu(string code)
    {
        for(int i = 0; i < severfilterText.Length; i++)
            severfilterText[i].color = new Color(0.4f, 0.4f, 0.4f);
        int index = -1;
        if (code.Equals("KR")) index = 0;
        else if (code.Equals("EN")) index = 1;
        else if (code.Equals("CN")) index = 2;
        else if (code.Equals("JP")) index = 3;
        severfilterText[index].color = new Color(0.86f, 0.86f, 0.86f);

        List<OperaterClass> data = new List<OperaterClass>(iconUIMng.dataMng.operatorData);
        string[] deleteName = XML.This.getOperatorLiveServerData(code);
        for(int i = 0; i < deleteName.Length; i++)
        {
            OperaterClass target = data.Find( delegate (OperaterClass a) {
                return a.en_name.Equals(deleteName[i]);
            });
            data.Remove(target);
        }
        iconUIMng.Refresh(data);
    }
}
