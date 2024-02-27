using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using CharactorDataSet;
using UI;

public class OperaterIcon_UI_Manager : MonoBehaviour
{
    public Transform icon_viewport;

    [Header("- Manager Scripts")]
    public Operater_Data_Hub dataMng;

    [Header("- UI Scripts")]
    public OperaterInfo_UIManager main_ui_mng;

    GridLayout gridlayout;
    List<Icon_Operator> icon_list;
    public IEnumerator Initailized()
    {
        gridlayout = icon_viewport.GetComponent<GridLayout>();
        gridlayout.Initialized();
        icon_list = new List<Icon_Operator>();

        for (int i = 0; i < icon_viewport.childCount; i++)
        {
            Icon_Operator tmp = new Icon_Operator();
            tmp._this = icon_viewport.transform.GetChild(i).gameObject;
            tmp.icon = icon_viewport.transform.GetChild(i).GetChild(0).GetComponent<RawImage>();
            tmp.frame = icon_viewport.transform.GetChild(i).GetChild(1).GetComponent<Image>();
            tmp.name = icon_viewport.transform.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
            icon_list.Add(tmp);
        }

        SetClear();
        yield return null;
    }
    public void OnBtn_Icon(Transform set)
    {
        int index = set.GetSiblingIndex();
        main_ui_mng.SetData(icon_list[index].name.text);
        main_ui_mng.TR_SetActive(true);
    }
    public void SetClear()
    {
        for (int i = 0; i < icon_list.Count; i++)
        {
            icon_list[i]._this.gameObject.SetActive(false);
        }
    }
    public void Refresh(List<OperaterClass> set)
    {
        if (dataMng.operatorData == null)
        {
            SetClear();
            return;
        }

        for (int i = 0; i < icon_list.Count; i++)
        {
            if (i >= set.Count)
            {
                icon_list[i]._this.SetActive(false);
                continue;
            }
            OperaterClass tmp = set[i];

            icon_list[i].Refresh(tmp.icon, dataMng.GetRareColor(tmp.rare), tmp.name);
        }
        gridlayout.Refresh(set.Count);
    }
    public void OnDestroy()
    {
        for (int i = 0; i < icon_list.Count; i++)
            icon_list.RemoveAt(0);
        icon_list.Clear();

        icon_viewport = null;
        icon_list = null;
        dataMng = null;

        Debug.Log("[IconUI Scripts] Destroy Memory");
    }
}
