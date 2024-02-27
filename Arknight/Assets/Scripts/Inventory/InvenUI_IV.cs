using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UI;
using MaterialData;
public class InvenUI_IV : MonoBehaviour
{
    public GameObject UI;
    public GameObject Menu;
    public DataHub_Material dataMng;
    public MainUI_IV mainUI;

    private List<Material_IV> dataInven;

    [Header("- UI Transform")]
    public Transform viewer;
    private List<IconIV> invenIcon;

    private Material_IV curData;
    private int curIndex;
    public void Initialized()
    {
        dataInven = new List<Material_IV>();
        XML.This.Get_InvenMaterial(dataInven, dataMng.eliteMaterialData);

        invenIcon = new List<IconIV>();

        for (int i = 0; i < viewer.childCount; i++) // 아이콘 리스트 생성
        {
            IconIV set = new IconIV();
            set.This = viewer.transform.GetChild(i).gameObject;
            set.frame = viewer.transform.GetChild(i).GetChild(0).GetComponent<RawImage>();
            set.icon = viewer.transform.GetChild(i).GetChild(2).GetComponent<RawImage>();
            set.txt_count = viewer.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<Text>();
            set.temp = new GameObject[1];
            set.temp[0] = viewer.transform.GetChild(i).GetChild(1).gameObject;
            invenIcon.Add(set);
        }

        Clear();
        //SetActive(false);
    }
    private void Clear()
    {
        curIndex = -1;

        UI.SetActive(false);
        Menu.SetActive(false);

        for (int i = 0; i < invenIcon.Count; i++)
        {
            invenIcon[i].This.SetActive(false);
            invenIcon[i].temp[0].SetActive(false);
        }
    }
    public void Refresh()
    {
        for (int i = 0; i < invenIcon.Count; i++)
        {
            if(i >= dataInven.Count)
            {
                invenIcon[i].This.SetActive(false);
                continue;
            }

            invenIcon[i].icon.texture = dataInven[i].data.icon;
            invenIcon[i].frame.texture = dataMng.GetMaterialFrame(dataInven[i].data.rare);
            invenIcon[i].txt_count.text = dataInven[i].inven.ToString();

            if (curData != null && curData == dataInven[i])
                invenIcon[i].temp[0].SetActive(true);
            else invenIcon[i].temp[0].SetActive(false);

            invenIcon[i].This.SetActive(true);
        }
    }
    public void RefreshData()
    {
        for (int i = 0; i < dataInven.Count; i++)
            dataInven[i].inven = 0;

        SaveData();
    }
    public void TR_SetActive(bool set)
    {
        UI.SetActive(set);

        if (set)
        {
            curIndex = -1;
            Refresh();
        }
        else
            TR_SetActiveMenu(false);
    }
    public void TR_Reset()
    {
        RefreshData();
        Refresh();
        mainUI.RefreshData();
        mainUI.Refresh();
    }
    public void TR_MaterialIcon(Transform set)
    {
        if(!curIndex.Equals(-1))
        {
            invenIcon[curIndex].temp[0].SetActive(false);
        }
        curIndex = set.GetSiblingIndex();
        invenIcon[curIndex].temp[0].SetActive(true);

        TR_SetActiveMenu(true);

        curData = dataInven[curIndex];
    }
    public void TR_SetActiveMenu(bool set)
    {
        if (set && Menu.activeSelf.Equals(false))
            Menu.SetActive(set);
        else if (!set && Menu.activeSelf.Equals(true))
            Menu.SetActive(set);

        if (!set && !curIndex.Equals(-1))
        {
            invenIcon[curIndex].temp[0].SetActive(false);
            curData = null;
            curIndex = -1;
        }
    }
    public void TR_SetMaterialCount(int count)
    {
        int curCount = curData.inven;
        curCount = curCount + count;

        if (curCount < 0)
            curData.inven = 0;
        else
            curData.inven = curCount;

        SaveData();
        Refresh();
        mainUI.RefreshData();
        mainUI.Refresh();
    }
    public void SaveData()
    {
        string data = string.Empty;
        for(int i = 0; i < dataInven.Count; i++)
        {
            if (dataInven[i].inven > 0)
                data = string.Format("{0},{1},{2}", data, dataInven[i].data.code, dataInven[i].inven);
        }
        if (!data.Equals(string.Empty))
            data.Remove(0, 1);

        PlayerPrefs.SetString("MatI", data);
        PlayerPrefs.Save();
    }

    public int GetCount(string code)
    {
        Material_IV set = dataInven.Find(delegate (Material_IV a)
        {
            return a.data.code == code;
        });
        return set.inven;
    }
    public MaterialCount GetData(string name)
    {
        //MaterialCount set = dataInven.Find(delegate (MaterialCount a)
        //{
        //    return a.data.name == name;
        //});
        return null;
    }


    public void Trigger_InfomaitionUI(Transform set)
    {
        int index = set.GetSiblingIndex();
        //currData = dataInven[index];
        //infoUI.SetActive(dataInven[index]);
    }
    public void Trigger_InputMaterialCount(InputField set)
    {
        int count = int.Parse(set.text);
        if (count >= 0)
        {
            set.Select(); // 입력필드 비활성화
            set.text = string.Empty;
            curData.count = count;

            //RefreshInventory();
            //xml_Reader.SavePrefs_Invnetory(invenData);
            return;
        }
        set.Select(); // 입력필드 비활성화
        set.text = string.Empty;
    }
}
