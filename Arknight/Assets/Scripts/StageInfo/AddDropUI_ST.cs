using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using FireBaseClass;
using MaterialData;
using TMPro;
using UI;

class DrophistoryIcon
{
    public GameObject _this;
    public List<IconMT_3> material;
    public TextMeshProUGUI stageCode;

    public string pushId;
    public void Initialize(Transform target)
    {
        _this = target.gameObject;
        stageCode = target.GetChild(0).GetComponent<TextMeshProUGUI>();

        material = new List<IconMT_3>();
        for (int j = 0; j < target.GetChild(1).childCount; j++)
        {
            IconMT_3 Mtmp = new IconMT_3();
            Mtmp.Initialize(target.GetChild(1).GetChild(j));
            material.Add(Mtmp);
        }

        _this.SetActive(false);
    }
    public void Refresh(string _code, string _pushId)
    {
        pushId = _pushId;
        stageCode.text = _code;
        if (!_this.activeSelf)
            _this.SetActive(true);
    }
}
public class AddDropUI_ST : MonoBehaviour
{
    public MainUI_ST mainUi_mng;

    public Canvas mainCanvas;
    public Canvas dropdataHistory;

    public GameObject lodingPanel;
    public GameObject nomarlPanel;
    public GameObject rarePanel;
    public GameObject addPanel;

    public Transform lucnkyTr;
    public Transform nomarlTr;
    public Transform rareTr;
    public Transform addTr;
    public Transform dropdataTr;

    public TextMeshProUGUI menuTxt;

    IconMT_2 lucnkyItem;
    List<IconMT_2> nomarlItems;
    List<IconMT_2> rareItem;
    List<IconMT_2> addItem;
    List<DrophistoryIcon> dropdataItem;

    Stage curData;
    DataHub_Material dataMaterial;

    List<string[]> limit;
    public void Initialized()
    {
        lodingPanel.SetActive(false);
        mainCanvas.enabled = false;
        dropdataHistory.enabled = false;

        dataMaterial = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataHub_Material>();
        lucnkyItem = new IconMT_2();
        nomarlItems = new List<IconMT_2>();
        rareItem = new List<IconMT_2>();
        addItem = new List<IconMT_2>();
        lucnkyItem.Initiailize(lucnkyTr.GetChild(0));
        for(int i = 0; i  < nomarlTr.childCount; i++)
        {
            IconMT_2 tmp = new IconMT_2();
            nomarlItems.Add(tmp.Initiailize(nomarlTr.GetChild(i)));
        }
        for (int i = 0; i < rareTr.childCount; i++)
        {
            IconMT_2 tmp = new IconMT_2();
            rareItem.Add(tmp.Initiailize(rareTr.GetChild(i)));
        }
        for (int i = 0; i < addTr.childCount; i++)
        {
            IconMT_2 tmp = new IconMT_2();
            addItem.Add(tmp.Initiailize(addTr.GetChild(i)));
        }

        dropdataItem = new List<DrophistoryIcon>();
        for(int i = 0; i  < dropdataTr.childCount; i++)
        {
            DrophistoryIcon Dtmp = new DrophistoryIcon();
            Dtmp.Initialize(dropdataTr.GetChild(i));
            dropdataItem.Add(Dtmp);
        }
        menuTxt.color = new Color(0.4f, 0.4f, 0.4f);
    }
    public void StartUI_Main(Stage set)
    {
        bool flag = false;
        curData = set;

        lucnkyItem.DisableSelect();

        limit = XML.This.get_stage_dropLimit(curData.code);
        for (int i = 0; i < nomarlItems.Count; i++)
        {
            Debug.Log(limit.Count);

            if (i >= curData.ndrop.Length)
            {
                nomarlItems[i]._this.SetActive(false);
                continue;
            }
            if (curData.ndrop[i].Equals("MONEY_L") || curData.ndrop[i].Equals("NULL"))
            {
                nomarlItems[i]._this.SetActive(false);
                continue;
            }
            if (curData.ndrop[i].Equals(string.Empty))
            {
                nomarlItems[i]._this.SetActive(false);
                continue;
            }
            EliteMaterial tmp = dataMaterial.get_material_all(curData.ndrop[i]);
            if(tmp == null)
            {
                nomarlItems[i]._this.SetActive(false);
                continue;
            }

            string[] limitMaterial = null;
            for (int j = 0; j < limit.Count; j++)
                if (curData.ndrop[i].Equals(limit[j][1])) limitMaterial = limit[j];
            
            nomarlItems[i].Refresh(tmp.icon, dataMaterial.GetMaterialFrame(tmp.rare));
            nomarlItems[i].DisableSelect();

            if (limitMaterial != null)
            {
                nomarlItems[i].setCount(int.Parse(limitMaterial[2]));
                nomarlItems[i].DisableSelectMenu();
                Debug.Log(limitMaterial[1]);
            }
            flag = true;
        }
        if (!nomarlPanel.activeSelf.Equals(flag)) nomarlPanel.SetActive(flag);

        flag = false;
        for (int i = 0; i < rareItem.Count; i++)
        {
            if (i >= curData.sdrop.Length || curData.sdrop[i].Equals(string.Empty))
            {
                rareItem[i]._this.SetActive(false);
                continue;
            }
            if(mainUi_mng.get_stage_type().Equals("event"))
            {
                if(curData.sdrop[i].Equals("af_qf"))
                {
                    rareItem[i]._this.SetActive(false);
                    continue;
                }
            }

            EliteMaterial tmp = dataMaterial.get_material_all(curData.sdrop[i]);
            rareItem[i].Refresh(tmp.icon, dataMaterial.GetMaterialFrame(tmp.rare));
            rareItem[i].DisableSelect();
            flag = true;
        }
        if (!rarePanel.activeSelf.Equals(flag)) rarePanel.SetActive(flag);

        flag = false;
        for (int i = 0; i < addItem.Count; i++)
        {
            if (i >= curData.adrop.Length)
            {
                addItem[i]._this.SetActive(false);
                continue;
            }
            if (curData.adrop[i].Equals(string.Empty))
            {
                addItem[i]._this.SetActive(false);
                continue;
            }
            EliteMaterial tmp = dataMaterial.get_material_all(curData.adrop[i]);
            addItem[i].Refresh(tmp.icon, dataMaterial.GetMaterialFrame(tmp.rare));
            addItem[i].DisableSelect();
            flag = true;
        }
        if (!addPanel.activeSelf.Equals(flag)) addPanel.SetActive(flag);

        mainCanvas.enabled = true;
    }
    public void ExiteUI_Main()
    {
        mainCanvas.enabled = false;
    }
    public IEnumerator StartUI_DropHistroy()
    {
        List<StageDropData> dropData = new List<StageDropData>();
        List<string> pushId = new List<string>();

        yield return StartCoroutine(getDropDataWithFireBase(dropData, pushId));

        if(dropData.Count <= 0)
        {
            StartCoroutine(LogU.Use.SetLog("검토중인 데이터가 없습니다."));
            yield break;
        }

        for(int i = 0; i < dropdataItem.Count; i++)
        {
            if(i >= dropData.Count)
            {
                dropdataItem[i]._this.SetActive(false);
                continue;
            }

            string[] material = dropData[i].material.Split(',');
            string[] count = dropData[i].count.Split(',');
            for (int j = 0; j < dropdataItem[i].material.Count; j++)
            {
                if (j >= material.Length || material[j].Equals("null"))
                {
                    dropdataItem[i].material[j]._this.SetActive(false);
                    continue;
                }
                if(material[j].Equals("furniture"))
                {
                    dropdataItem[i].material[j].Refresh(dataMaterial.getMaterialIcon("furniture"), 
                        dataMaterial.GetMaterialFrame("6"), count[j]);
                    continue;
                }
                EliteMaterial tmp = dataMaterial.get_material_all(material[j]);
                dropdataItem[i].material[j].Refresh(tmp.icon, dataMaterial.GetMaterialFrame(tmp.rare), count[j]);
            }

            dropdataItem[i].Refresh(dropData[i].code, pushId[i]);
        }

        menuTxt.color = new Color(0.86f, 0.86f, 0.86f);
        dropdataHistory.enabled = true;
    }
    public void SetUI_DropHistroy()
    {
        if (!dropdataHistory.enabled)
            StartCoroutine(StartUI_DropHistroy());
        else
        {
            dropdataHistory.enabled = false;
            menuTxt.color = new Color(0.4f, 0.4f, 0.4f);
        }
    }

    public void trLuckyItemSelect()
    {
        if (lucnkyItem.getActive())
            lucnkyItem.DisableSelect();
        else
        {
            lucnkyItem.setCount(1);
            lucnkyItem.plus.SetActive(false);
            lucnkyItem.mnus.SetActive(false);
        }
    }
    public void trNomarlItemSelect(Transform tr)
    {
        int index = tr.GetSiblingIndex();

        for (int j = 0; j < limit.Count; j++)
            if (curData.ndrop[index].Equals(limit[j][1])) return;

        if (nomarlItems[index].getActive())
            nomarlItems[index].DisableSelect();
        else nomarlItems[index].setCount(1);
    }
    public void trNomarlItemCount(Transform tr)
    {
        int parent = tr.parent.GetSiblingIndex();
        int index = tr.GetSiblingIndex();
        if (index.Equals(3)) nomarlItems[parent].addCount(1);
        else if (index.Equals(4)) nomarlItems[parent].addCount(-1);
    }
    public void trRareItemSelect(Transform tr)
    {
        int index = tr.GetSiblingIndex();
        if (rareItem[index].getActive())
            rareItem[index].DisableSelect();
        else   rareItem[index].setCount(1);
    }
    public void trRareItemCount(Transform tr)
    {
        int parent = tr.parent.GetSiblingIndex();
        int index = tr.GetSiblingIndex();
        if (index.Equals(3)) rareItem[parent].addCount(1);
        else if (index.Equals(4)) rareItem[parent].addCount(-1);
    }
    public void trAddItemSelect(Transform tr)
    {
        int index = tr.GetSiblingIndex();
        if (addItem[index].getActive())
            addItem[index].DisableSelect();
        else addItem[index].setCount(1);
    }
    public void trAddItemCount(Transform tr)
    {
        int parent = tr.parent.GetSiblingIndex();
        int index = tr.GetSiblingIndex();
        if(index.Equals(3)) addItem[parent].addCount(1);
        else if(index.Equals(4)) addItem[parent].addCount(-1);
    }

    public void trAddDropDataWithFireBase()
    {
        StartCoroutine(setDropDataWithFireBase());
    }
    public void trGetDropDataWithFireBase()
    {
        //StartCoroutine(getDropDataWithFireBase());
    }
    public void tr_delete_drophistory_firebase(Transform target)
    {
        int index = target.GetSiblingIndex();

        StartCoroutine(set_DELETE_dropdata_firebase(dropdataItem[index]));
    }
    IEnumerator getDropDataWithFireBase(List<StageDropData> dropData, List<string> puahId)
    {
        lodingPanel.SetActive(true);

        bool flag = XML.This.GetUserData(FirebaseDataBase.instance.GetUserCache());
        if (!flag)
            StartCoroutine(LogU.Use.SetLog("로그인이 필요한 서비스입니다. 메인화면에서 계정을 생성해 주세요."));
        else
           yield return StartCoroutine(FirebaseDataBase.instance.GetUserDropData(dropData, puahId));

        lodingPanel.SetActive(false);
    }
    IEnumerator set_DELETE_dropdata_firebase(DrophistoryIcon target)
    {
        lodingPanel.SetActive(true);

        bool flag = XML.This.GetUserData(FirebaseDataBase.instance.GetUserCache());
        if (!flag)
            StartCoroutine(LogU.Use.SetLog("로그인이 필요한 서비스입니다. 메인화면에서 계정을 생성해 주세요."));
        else
            yield return StartCoroutine(FirebaseDataBase.instance.set_DELETE_dropdata_firebase(target.pushId));

        StartCoroutine(StartUI_DropHistroy());
        lodingPanel.SetActive(false);
    }

    IEnumerator setDropDataWithFireBase()
    {
        lodingPanel.SetActive(true);

        if (mainUi_mng.get_stage_type().Equals("event"))
            if (!mainUi_mng.get_timeout_stage())
            {
                StartCoroutine(LogU.Use.SetLogUserDelay("이벤트 오픈 기간이 아닙니다.", 1f));
                lodingPanel.SetActive(false);
                yield break;
            }

        string data = string.Empty;
        string count = string.Empty;

        if (lucnkyItem.getActive())
        {
            data += ",furniture";
            count += ",1";
        }
        for (int i = 0; i < nomarlItems.Count; i++)
            if (nomarlItems[i].getActive())
            {
                data += "," + curData.ndrop[i];
                count += "," + nomarlItems[i].getCount();
            }
        for (int i = 0; i < rareItem.Count; i++)
            if (rareItem[i].getActive())
            {
                data += "," + curData.sdrop[i];
                count += "," + rareItem[i].getCount();
            }
        for (int i = 0; i < addItem.Count; i++)
            if (addItem[i].getActive())
            {
                data += "," + curData.adrop[i];
                count += "," + addItem[i].getCount();
            }

        if (data != string.Empty)
            data = data.Remove(0, 1);
        else data = "null";
        if (count != string.Empty)
            count = count.Remove(0, 1);
        else count = "null";

        StageDropData dropData = new StageDropData();

        dropData.code = curData.code;
        dropData.material = data;
        dropData.count = count;

        bool flag = XML.This.GetUserData(FirebaseDataBase.instance.GetUserCache());
        if (!flag)
            StartCoroutine(LogU.Use.SetLog("로그인이 필요한 서비스입니다. 메인화면에서 계정을 생성해 주세요."));
        else
           yield return  StartCoroutine(FirebaseDataBase.instance.PushDropData(dropData));

        StartUI_Main(curData);
        lodingPanel.SetActive(false);
        StartCoroutine(LogU.Use.SetLogUserDelay("정상적으로 데이터베이스에 데이터를 추가했습니다.", 1f));
    }
}
