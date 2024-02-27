using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MaterialData;
using UI;
public class InfomationMaterial_UIManager : MonoBehaviour
{
    [Header("- Data Scripts")]
    public DataHub_Material dataMng;

    public GameObject infoUI;
    public Transform subView;
    public Transform stageView;

    public Image icon;

    public Text name;
    public Text info;
    public Text record;
    public Text count;

    private List<InfoStageIcon> stageIcon;
    private List<Icon> subMatIcon;
    public void Initialized()
    {
        subMatIcon = new List<Icon>();
        for (int i = 0; i < subView.childCount; i++) // 하위 조합 재료 아이콘 리스트 생성
        {
            Icon set = new Icon();
            set.This = subView.transform.GetChild(i).gameObject;
            set.frame = subView.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            set.icon = subView.transform.GetChild(i).GetChild(1).GetComponent<Image>();
            set.name_txt = subView.transform.GetChild(i).GetChild(3).GetComponent<Text>();
            set.txt_count = subView.transform.GetChild(i).GetChild(4).GetComponent<Text>();
            subMatIcon.Add(set);
        }

        stageIcon = new List<InfoStageIcon>();
        for (int i = 0; i < stageView.childCount; i++) // 스테이지 아이콘 리스트 생성
        {
            InfoStageIcon set = new InfoStageIcon();
            set.ob = stageView.transform.GetChild(i).gameObject;
            set.frame = stageView.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            set.txt_name = stageView.transform.GetChild(i).GetChild(1).GetComponent<Text>();
            set.per = stageView.transform.GetChild(i).GetChild(2).GetComponent<Text>();
            stageIcon.Add(set);
        }
        SetClearAll();
        infoUI.SetActive(false);
    }
    public void SetClearAll() // 창 활성화시 빈 도화지 만들기
    {
        for (int i = 0; i < subMatIcon.Count; i++)
            subMatIcon[i].This.SetActive(false);
        for (int i = 0; i < stageIcon.Count; i++)
            stageIcon[i].ob.gameObject.SetActive(false);
    }
    public void SetActive(MaterialCount set)
    {
        SetClearAll();
        icon.sprite = set.data.icon;
        name.text = set.data.name;
        info.text = set.data.info; // System.Environment.NewLine 줄바꿈
        record.text = set.data.record;
        count.text = set.count.ToString();

        if (set.data.submaterial != null)
            for (int i = 0; i < set.data.submaterial.Count; i++)
            {
                EliteMaterial tmp = dataMng.get_material_all(set.data.submaterial[i].name);
                //subMatIcon[i].icon.sprite = tmp.icon;
                subMatIcon[i].name_txt.text = tmp.name;
                subMatIcon[i].txt_count.text = set.data.submaterial[i].count.ToString();

                //subMatIcon[i].frame.sprite = dataMng.GetMaterialFrame(tmp.rare);

                subMatIcon[i].This.SetActive(true);
            }
        if (set.data.dropstage != null)
            for (int i = 0; i < set.data.dropstage.Count; i++)
            {

                stageIcon[i].txt_name.text = set.data.dropstage[i].name;
                stageIcon[i].per.text = set.data.dropstage[i].per;
                if (set.data.dropstage[i].per == "매우 낮음")
                    stageIcon[i].frame.color = new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f);
                if (set.data.dropstage[i].per == "낮음")
                    stageIcon[i].frame.color = new Color(255 / 255.0f, 150 / 255.0f, 0 / 255.0f);
                if (set.data.dropstage[i].per == "보통")
                    stageIcon[i].frame.color = new Color(150 / 255.0f, 0 / 255.0f, 255 / 255.0f);
                if (set.data.dropstage[i].per == "높음")
                    stageIcon[i].frame.color = new Color(0 / 255.0f, 150 / 255.0f, 255 / 255.0f);
                if (set.data.dropstage[i].per == "항상")
                    stageIcon[i].frame.color = new Color(0 / 255.0f, 150 / 255.0f, 0 / 255.0f);
                stageIcon[i].ob.gameObject.SetActive(true);
            }
        SetActive(true);
    }
    public void SetActive(bool set) { infoUI.SetActive(set); }
}
