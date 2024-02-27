using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;

using UI;
using MaterialData;

public class MaterialSearch_UIManager : MonoBehaviour
{
    public DataHub_Material dataMng;

    public GameObject UI;
    public GameObject UI_select;

    public Transform tr_selectView;
    public Transform tr_retView;

    public Image icon;
    public Text txtname;
    public Text txtcount;
    public Text txtInfo;
    public Text txtRecord;
    public GameObject Info_topMenu;
    public Transform tr_sub;
    public Transform tr_stage;
    private List<InfoStageIcon> stageList;
    private List<Icon> subList;

    private List<Icon> selectIconList;
    private List<Icon> retIconList;

    private List<MaterialCount> data;
    private List<MaterialCount> retdata;
    private MaterialCount currData;
    public void Initialized()
    {
        data = new List<MaterialCount>();
        retdata = new List<MaterialCount>();

        selectIconList = new List<Icon>();
        for (int i = 0; i < tr_selectView.childCount; i++) // 아이콘 리스트 생성
        {
            Icon set = new Icon();
            set.This = tr_selectView.transform.GetChild(i).gameObject;
            set.frame = tr_selectView.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            set.icon = tr_selectView.transform.GetChild(i).GetChild(1).GetComponent<Image>();
            set.name_txt = tr_selectView.transform.GetChild(i).GetChild(3).GetComponent<Text>();
            set.txt_count = tr_selectView.transform.GetChild(i).GetChild(4).GetComponent<Text>();
            selectIconList.Add(set);
        }
        retIconList = new List<Icon>();
        for (int i = 0; i < tr_retView.childCount; i++) // 아이콘 리스트 생성
        {
            Icon set = new Icon();
            set.This = tr_retView.transform.GetChild(i).gameObject;
            set.frame = tr_retView.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            set.icon = tr_retView.transform.GetChild(i).GetChild(1).GetComponent<Image>();
            set.name_txt = tr_retView.transform.GetChild(i).GetChild(3).GetComponent<Text>();
            set.txt_count = tr_retView.transform.GetChild(i).GetChild(4).GetComponent<Text>();
            set.required_tag = tr_retView.transform.GetChild(i).GetChild(5).gameObject;
            retIconList.Add(set);
        }

        subList = new List<Icon>();
        for (int i = 0; i < tr_sub.childCount; i++) // 하위 조합 재료 아이콘 리스트 생성
        {
            Icon set = new Icon();
            set.This = tr_sub.transform.GetChild(i).gameObject;
            set.frame = tr_sub.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            set.icon = tr_sub.transform.GetChild(i).GetChild(1).GetComponent<Image>();
            set.name_txt = tr_sub.transform.GetChild(i).GetChild(3).GetComponent<Text>();
            set.txt_count = tr_sub.transform.GetChild(i).GetChild(4).GetComponent<Text>();
            subList.Add(set);
        }
        stageList = new List<InfoStageIcon>();
        for (int i = 0; i < tr_stage.childCount; i++) // 스테이지 아이콘 리스트 생성
        {
            InfoStageIcon set = new InfoStageIcon();
            set.ob = tr_stage.transform.GetChild(i).gameObject;
            set.frame = tr_stage.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            set.txt_name = tr_stage.transform.GetChild(i).GetChild(1).GetComponent<Text>();
            set.per = tr_stage.transform.GetChild(i).GetChild(2).GetComponent<Text>();
            stageList.Add(set);
        }
        SetClear(true, true);
        SetClear_Info();
    }
    public void SetClear_Info()
    {
        for (int i = 0; i < subList.Count; i++)
            subList[i].This.gameObject.SetActive(false);
        for (int i = 0; i < stageList.Count; i++)
            stageList[i].ob.gameObject.SetActive(false);
        SetActiveInfo(false);
    }
    public void SetClear(bool select, bool ret)
    {
        if (select)
            for (int i = 0; i < selectIconList.Count; i++)
                selectIconList[i].This.SetActive(false);
        if (ret)
            for (int i = 0; i < retIconList.Count; i++)
                retIconList[i].This.SetActive(false);
    }
    public bool SetActive(UpgradeMaterial set) // 상세창을 통한 활성화 트리거
    {
        if (set.submaterial == null)
            return false;

        retdata.Clear();
        SetActive(true);
        if (set != null)
        {
            MaterialCount tmp = data.Find(delegate (MaterialCount a) { return a.data.name == set.name; });
            if (tmp != null)
                tmp.count++;
            else
            {
                MaterialCount a = new MaterialCount();
                a.data = set;
                a.count = 1;
                data.Add(a);
            }
        }
        SetSelectIconView();
        SetResultIconView();

        return true;
    }
    public void SetActive(bool set) // 상단 메뉴를 통한 활성화 트리거
    {
        UI.SetActive(set);
    }
    public void SetSelectIconView()
    {
        for (int i = 0; i < data.Count; i++)
        {
            selectIconList[i].name = data[i].data.name;
            selectIconList[i].icon.sprite = data[i].data.icon;
            selectIconList[i].name_txt.text = data[i].data.name;
            selectIconList[i].txt_count.text = data[i].count.ToString();

            //selectIconList[i].frame.sprite = dataMng.GetMaterialFrame(int.Parse(data[i].data.rare));

            selectIconList[i].This.SetActive(true);
        }
    }
    public void SetResultIconView() // 재료 검색 결과 아이콘 출력 함수
    {
        for (int i = 0; i < data.Count;)
        {
            List<MaterialCount> tmp = new List<MaterialCount>(); // 임시 결과 리스트 생성

            List<SubMaterial> subData1 = data[i].data.submaterial;
            if (subData1 == null) continue;
            for (int sub1 = 0; sub1 < subData1.Count; sub1++) // 첫 재료 확인
            {
                CheckMaterialCount(tmp, subData1[sub1], data[i], data[i]); // 해당 재료의 하위 재료 목록 계산
            }

            if (int.Parse(data[i].data.rare) > 4)
                CheckMaterial_Rare(tmp, data[i], "4"); // 4성 하위재료 3성 추가
            if (int.Parse(data[i].data.rare) > 3)
                CheckMaterial_Rare(tmp, data[i], "3"); // 3성 하위재료 2성 추가
            if (int.Parse(data[i].data.rare) > 2)
                CheckMaterial_Rare(tmp, data[i], "2"); // 3성 하위재료 1성 추가

            for (int j = 0; j < tmp.Count; j++) // 임시 결과 리스트를 확정 결과 리스트에 반영
            {
                MaterialCount c = retdata.Find(delegate (MaterialCount a) { return a.data.name == tmp[j].data.name; });
                if (c == null)
                    retdata.Add(tmp[j]);
                else c.count = c.count + tmp[j].count;
            }
            i++; // 다음 아이템 검색
        }
        retdata.Sort(delegate (MaterialCount a, MaterialCount b) // 클래스 리스트 특정 요소값으로 정렬 방법
        {
            if (a.data.id > b.data.id) return 1;
            else if (a.data.id < b.data.id) return -1;
            return 0;
        });

        SetIconListView(retIconList, retdata);
    }
    public void SetIconListView(List<Icon> ui, List<MaterialCount> data) // 전달받은 아이콘리스트, 데이터 출력
    {
        for (int i = 0; i < data.Count; i++)
        {
            ui[i].name = data[i].data.name;
            ui[i].icon.sprite = data[i].data.icon;
            ui[i].name_txt.text = data[i].data.name;
            ui[i].txt_count.text = data[i].count.ToString();

            //ui[i].frame.sprite = dataMng.GetMaterialFrame(int.Parse(data[i].data.rare));
  
            if (data[i].data.submaterial == null && ui == retIconList && data[i].data.rare != "1")
                ui[i].required_tag.SetActive(true);
            else ui[i].required_tag.SetActive(false);

            ui[i].This.SetActive(true);
        }
    }
    public void CheckMaterialCount(List<MaterialCount> ret, SubMaterial target, MaterialCount parent, MaterialCount root) // 재료 검색 함수
    {
        EliteMaterial tmp = dataMng.get_material_all(target.name); // 결과 리스트에 없는 재료일경우 반영을 위해 데이터 로딩

        MaterialCount set = ret.Find(delegate (MaterialCount a) {    return a.data.name == tmp.name; }); // 해당 재료가 결과 리스트에 있는지 확인
        if(set != null) // 재료가 있다면
        {
            set.count = target.count * parent.count; // 결과 리스트에 반영
        }
        else // 결과 리스트에 없다면 추가
        {
            set = new MaterialCount();
            //set.data = tmp;
            set.count = target.count * parent.count;
            ret.Add(set);
        }
    }
    public void CheckMaterial_Rare(List<MaterialCount> set, MaterialCount root, string rare) // 특정 레어도 제한 검색 함수
    {
        for (int i = 0; i < set.Count; i++) // 결과 리스트의 특정 레어도 재료만 검색
        {
            if (set[i].data.rare == rare)
            {
                List<SubMaterial> subData1 = set[i].data.submaterial;
                if (subData1 == null) continue; // 해당 재료의 하위 재료가 없다면 다음 재료로
                for (int sub1 = 0; sub1 < subData1.Count; sub1++) // 하위 재료 검색
                {
                    CheckMaterialCount(set, subData1[sub1], set[i], root);
                }
            }
        }
        retdata.Sort(delegate (MaterialCount a, MaterialCount b) // 레어도 검색을 위해 리스트 정렬
        {
            if (a.data.id > b.data.id) return 1;
            else if (a.data.id < b.data.id) return -1;
            return 0;
        });
    }
    
    public void SetActiveInfo(MaterialCount set) // 재료 상세창 + 메뉴 활성화 함수
    {
        SetClear_Info();
        currData = set;
        Info_topMenu.SetActive(true);
        icon.sprite = set.data.icon;
        txtname.text = set.data.name;
        txtcount.text = set.count.ToString();
        txtInfo.text = set.data.info; // System.Environment.NewLine 줄바꿈
        txtRecord.text = set.data.record;
        if (set.data.submaterial != null)
            for (int i = 0; i < set.data.submaterial.Count; i++)
            {
                EliteMaterial tmp = dataMng.get_material_all(set.data.submaterial[i].name);
                //subList[i].icon.sprite = tmp.icon;
                subList[i].name_txt.text = tmp.name;
                subList[i].txt_count.text = set.data.submaterial[i].count.ToString();

                //subList[i].frame.sprite = dataMng.GetMaterialFrame(int.Parse(tmp.rare));

                subList[i].This.SetActive(true);
            }
        if (set.data.dropstage != null)
            for (int i = 0; i < set.data.dropstage.Count; i++)
            {
                stageList[i].txt_name.text = set.data.dropstage[i].name;
                stageList[i].per.text = set.data.dropstage[i].per;
                if (set.data.dropstage[i].per == "매우 낮음")
                    stageList[i].frame.color = new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f);
                if (set.data.dropstage[i].per == "낮음")
                    stageList[i].frame.color = new Color(255 / 255.0f, 150 / 255.0f, 0 / 255.0f);
                if (set.data.dropstage[i].per == "보통")
                    stageList[i].frame.color = new Color(150 / 255.0f, 0 / 255.0f, 255 / 255.0f);
                if (set.data.dropstage[i].per == "높음")
                    stageList[i].frame.color = new Color(0 / 255.0f, 150 / 255.0f, 255 / 255.0f);
                if (set.data.dropstage[i].per == "항상")
                    stageList[i].frame.color = new Color(0 / 255.0f, 150 / 255.0f, 0 / 255.0f);
                stageList[i].ob.gameObject.SetActive(true);
                //Debug.Log("dropStage" + i.ToString() + ":활성화");
            }
        SetActiveInfo(true);
    }
    public void SetActiveInfo_RetIcon(MaterialCount set) // 재료 상세창 + 메뉴 활성화 함수
    {
        SetClear_Info();
        currData = set;
        Info_topMenu.SetActive(false);
        icon.sprite = set.data.icon;
        txtname.text = set.data.name;
        txtcount.text = set.count.ToString();
        txtInfo.text = set.data.info; // System.Environment.NewLine 줄바꿈
        txtRecord.text = set.data.record;
        if (set.data.submaterial != null)
            for (int i = 0; i < set.data.submaterial.Count; i++)
            {
                EliteMaterial tmp = dataMng.get_material_all(set.data.submaterial[i].name);
                //subList[i].icon.sprite = tmp.icon;
                subList[i].name_txt.text = tmp.name;
                subList[i].txt_count.text = set.data.submaterial[i].count.ToString();

                //subList[i].frame.sprite = dataMng.GetMaterialFrame(int.Parse(tmp.rare));

                subList[i].This.SetActive(true);
            }
        if (set.data.dropstage != null)
            for (int i = 0; i < set.data.dropstage.Count; i++)
            {
                stageList[i].txt_name.text = set.data.dropstage[i].name;
                stageList[i].per.text = set.data.dropstage[i].per;
                if (set.data.dropstage[i].per == "매우 낮음")
                    stageList[i].frame.color = new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f);
                if (set.data.dropstage[i].per == "낮음")
                    stageList[i].frame.color = new Color(255 / 255.0f, 150 / 255.0f, 0 / 255.0f);
                if (set.data.dropstage[i].per == "보통")
                    stageList[i].frame.color = new Color(150 / 255.0f, 0 / 255.0f, 255 / 255.0f);
                if (set.data.dropstage[i].per == "높음")
                    stageList[i].frame.color = new Color(0 / 255.0f, 150 / 255.0f, 255 / 255.0f);
                if (set.data.dropstage[i].per == "항상")
                    stageList[i].frame.color = new Color(0 / 255.0f, 150 / 255.0f, 0 / 255.0f);
                stageList[i].ob.gameObject.SetActive(true);
                //Debug.Log("dropStage" + i.ToString() + ":활성화");
            }
        SetActiveInfo(true);
    }
    
    public void SetActiveInfo(bool active) // 재료 상세창 외부 버튼제어 함수 
    {
        UI_select.SetActive(active);
    }
    public void SetSelectMaterialCount(InputField set)
    {
        int count = int.Parse(set.text);
        if (count > 0)
        {
            set.Select(); // 입력필드 비활성화
            set.text = string.Empty;
            currData.count = count;
            SetActiveInfo(currData);

            SetClear(true, true);
            SetSelectIconView();
            retdata.Clear();
            SetResultIconView();
            return;
        }
        set.Select(); // 입력필드 비활성화
        set.text = string.Empty;
    }
    public void SelectMaterialData_Delete()
    {     
        int index = data.IndexOf(currData);
        data.RemoveAt(index);

        SetClear(true, true);
        SetSelectIconView();
        retdata.Clear();
        SetResultIconView();

        SetActiveInfo(false);
    }
    public void ButtonIcon_Trigger(Transform set)
    {
        int index = set.GetSiblingIndex();
        //Debug.Log(index);
        SetActiveInfo(data[index]);
    }
    public void ButtonRetIcon_Trigger(Transform set)
    {
        int index = set.GetSiblingIndex();
        //Debug.Log(index);
        SetActiveInfo_RetIcon(retdata[index]);
    }
    public void Trigger_SaveSelect()
    {
        if (data.Count > 0)
            XML.This.SavePrefs_SelectMaterial(data, retdata);
    }
}
