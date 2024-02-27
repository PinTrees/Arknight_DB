using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using MaterialData;
using UI;
public class InfoStageIcon
{
    public GameObject ob;
    public Image frame;
    public Text txt_name;
    public Text per;
}

public class EliteMT_InfoUI_MI : MonoBehaviour
{
    public DataHub_Material dataHub;
    public GameObject infoUI;

    public Transform subMtViewer;
    public Transform stageViewer;

    public RawImage icon;
    public Text name;
    public Text info;

    private Text[] stageTxt;
    private Image[] stageBtn;
    private List<Icon> subMtIcon;

    private EliteMaterial curData;
    private List<EliteMaterial> subMaterial;
    public void Initialized()
    {
        infoUI.SetActive(false);
        subMtIcon = new List<Icon>();
        for (int i = 0; i < subMtViewer.childCount; i++) // 하위 조합 재료 아이콘 리스트 생성
        {
            Icon set = new Icon();
            set.This = subMtViewer.transform.GetChild(i).gameObject;
            set.frame_raw = subMtViewer.transform.GetChild(i).GetComponent<RawImage>();
            set.icon_raw = subMtViewer.transform.GetChild(i).GetChild(0).GetComponent<RawImage>();
            set.name_txt = subMtViewer.transform.GetChild(i).GetChild(2).GetComponent<Text>();
            set.txt_count = subMtViewer.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<Text>();
            subMtIcon.Add(set);
        }

        stageTxt = new Text[stageViewer.childCount];
        stageBtn = new Image[stageViewer.childCount];
        for (int i = 0; i < stageViewer.childCount; i++) // 스테이지 아이콘 리스트 생성
        {
            stageBtn[i] = stageViewer.transform.GetChild(i).GetComponent<Image>();
            stageTxt[i] = stageViewer.transform.GetChild(i).GetChild(0).GetComponent<Text>();
        }

        subMaterial = new List<EliteMaterial>();
        Clear();
    }
    public void Clear()
    {
        subMaterial.Clear();
        for (int i = 0; i < subMtIcon.Count; i++)
            subMtIcon[i].This.SetActive(false);
        for (int i = 0; i < stageBtn.Length; i++)
            stageBtn[i].gameObject.SetActive(false);
    }
    public void SetStart(EliteMaterial set) // 창 활성화 트리거
    {
        Clear();
        curData = set;
        icon.texture = curData.icon;
        name.text = curData.name;

        info.text = string.Format("<b>{1}</b>{0}{0}<i>{2}</i>", System.Environment.NewLine, curData.info, curData.record); // System.Environment.NewLine 줄바꿈
        //SetSubMaterialIcon(set.submaterial);
        //SetStageIcon(set.dropstage);

        infoUI.SetActive(true);
        SetSubMaterialIcon(curData.submaterial, curData.subCount);
        SetStageIcon(curData.dropstage, curData.dropPer);
    }
    public void SetSubMaterialIcon(string[] codes, string[] count) // 하위 조합 재료 활성화 함수
    {
        if (codes == null)
            return;
        for(int i = 0; i < codes.Length; i++)
        {
            EliteMaterial tmp = dataHub.get_material_all(codes[i]);
            if (tmp == null)
                continue;
            if (tmp.icon != null)
                subMtIcon[i].icon_raw.texture = tmp.icon;
            subMtIcon[i].frame_raw.texture = dataHub.GetMaterialFrame(tmp.rare);
            subMtIcon[i].name_txt.text = tmp.name;
            subMtIcon[i].txt_count.text = count[i];

            //msub_iconList[i].frame.sprite = datahub.datahub.GetMaterialFrame(int.Parse(tmp.rare));
          
            subMtIcon[i].This.SetActive(true);
            subMaterial.Add(tmp);
        }
    }
    public void SetStageIcon(string[] name, string[] per) // 드랍 스테이지 활성화 함수
    {
        if (name == null)
            return;
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] == string.Empty)
                return;
            stageTxt[i].text = name[i] + " " + per[i];
            if (per[i].Equals("매우 낮음"))
                stageBtn[i].color = new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f);
            if (per[i].Equals("낮음"))
                stageBtn[i].color = new Color(255 / 255.0f, 150 / 255.0f, 0 / 255.0f);
            if (per[i].Equals("보통"))
                stageBtn[i].color = new Color(150 / 255.0f, 0 / 255.0f, 255 / 255.0f);
            if (per[i].Equals("높음"))
                stageBtn[i].color = new Color(0 / 255.0f, 150 / 255.0f, 255 / 255.0f);
            if (per[i].Equals("항상"))
                stageBtn[i].color = new Color(0 / 255.0f, 150 / 255.0f, 0 / 255.0f);
            stageBtn[i].gameObject.SetActive(true);
            //Debug.Log("dropStage" + i.ToString() + ":활성화");
        }
    }
    public void TR_SetSubMaterial(Transform set) // 현재 재료가 필요함을 입력 -> 필요한 재료 출력 함수 호출
    {
        int index = set.GetSiblingIndex();
        TR_SetActive(false);
        SetStart(subMaterial[index]);
        //UpgradeMaterial tmp = datahub.datahub.GetMaterial(name.text);
        //if (datahub.searchUI.SetActive(tmp))
        //{
        //    SetActive(false);
        //    info_iconView.SetActive(false);
        //}
    }
    public void TR_SetActive(bool set)
    {
        infoUI.SetActive(set);
    }
}
