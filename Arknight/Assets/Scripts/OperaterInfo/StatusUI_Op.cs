using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using MaterialData;
using CharactorDataSet;
using UI;
public class StatusUI_Op : MonoBehaviour
{
    public StatusInfra_UIClass_Opinfo status_infra_uiMng;
    public Canvas mainCanvas;
    Canvas statusCanvas;
    Canvas materialCanvas;

    public GameObject[] skillMenu;
    public RectTransform skillMaterialpanel;
    public Transform skillMaterialViewerTr;
    public RawImage[] skillIcon;
    public Text[] menuTxt;
    public TextMeshProUGUI[] defaultSkillLevelTxt;
    public TextMeshProUGUI[] skillEliteLevelTxt1;
    public TextMeshProUGUI[] skillEliteLevelTxt2;
    public TextMeshProUGUI[] skillEliteLevelTxt3;

    Operater_Data_Hub dataHub;

    List<RawIconMT> skillMaterialicons;
    List<Material_Op> skillMaterialData;
    OperaterClass curOperator;
    List<int> selectLevel;
    List<int>[] selectSkillLevelS;

    public void Initialized()
    {
        selectSkillLevelS = new List<int>[3];
        selectSkillLevelS[0] = new List<int>();
        selectSkillLevelS[1] = new List<int>();
        selectSkillLevelS[2] = new List<int>();
        selectLevel = new List<int>();
        skillMaterialicons = new List<RawIconMT>();
        skillMaterialData = new List<Material_Op>();

        dataHub = GameObject.FindGameObjectWithTag("DataManager").GetComponent<Operater_Data_Hub>();
        statusCanvas = mainCanvas.transform.GetChild(0).GetComponent<Canvas>();
        materialCanvas = mainCanvas.transform.GetChild(1).GetComponent<Canvas>();

        for(int i = 0; i < skillMaterialViewerTr.childCount; i++)
        {
            RawIconMT tmp = new RawIconMT();
            tmp.Initialize(skillMaterialViewerTr.GetChild(i));
            tmp._this.SetActive(false);
            skillMaterialicons.Add(tmp);
        }

        status_infra_uiMng.Initialized();

        ExiteUI_Main();
        Clear();
        ClearText();
        ClearMenuTxt();
    }
    void Clear()
    {
        materialCanvas.enabled = false;
        statusCanvas.enabled = false;
    }
    void ClearText()
    {
        for (int i = 0; i < defaultSkillLevelTxt.Length; i++)
            defaultSkillLevelTxt[i].color = new Color(0.4f, 0.4f, 0.4f);
        for (int i = 0; i < skillEliteLevelTxt1.Length; i++)
            skillEliteLevelTxt1[i].color = new Color(0.4f, 0.4f, 0.4f);
        for (int i = 0; i < skillEliteLevelTxt2.Length; i++)
            skillEliteLevelTxt2[i].color = new Color(0.4f, 0.4f, 0.4f);
        for (int i = 0; i < skillEliteLevelTxt3.Length; i++)
            skillEliteLevelTxt3[i].color = new Color(0.4f, 0.4f, 0.4f);
    }
    void ClearMenuTxt()
    {
        menuTxt[0].color = new Color(0.4f, 0.4f, 0.4f);
        menuTxt[1].color = new Color(0.4f, 0.4f, 0.4f);
    }
    public void StartUI_Main(OperaterClass set)
    {
        ClearMenuTxt();
        ClearText();
        selectLevel.Clear();
        skillMaterialData.Clear();
        selectSkillLevelS[0].Clear();
        selectSkillLevelS[1].Clear();
        selectSkillLevelS[2].Clear();

        curOperator = set;

        if (curOperator.rare != "1" && curOperator.rare != "2") {
            for (int i = 2; i <= 7; i++)
                TR_SetSkillLevelMain(i);
            skillMenu[0].SetActive(true);
        }
        else skillMenu[0].SetActive(false);
        if (curOperator.rare == "3" || curOperator.rare == "1" || curOperator.rare == "2")   skillMenu[1].SetActive(false);
        else skillMenu[1].SetActive(true);

        status_infra_uiMng.ui_refresh_main_panel(set);

        mainCanvas.enabled = true;
        materialCanvas.enabled = false;
        statusCanvas.enabled = true;

        menuTxt[0].color = new Color(0.86f, 0.86f, 0.86f);
    }
    public void StartUI_Materail()
    {
        RefreshUI_SkillMeterial();

        string path = Files.Use.DocumentsPath("Resource/OperatorIcon/" + curOperator.en_name);
        for (int i = 0; i < skillIcon.Length; i++)
        {
            if (i >= curOperator.skillData.Count)
            {
                skillIcon[i].transform.parent.gameObject.SetActive(false);
                continue;
            }
            if (skillIcon[i] != null)
                Destroy(skillIcon[i].texture);
            skillIcon[i].texture = Files.Use.GetPNG(path, "S" + i.ToString() + ".png");
            skillIcon[i].transform.parent.gameObject.SetActive(true);
        }

        materialCanvas.enabled = true;
        status_infra_uiMng.ui_set_main_canvas(false);
    }
    public void ui_refresh_status_canvas()
    {
        statusCanvas.enabled = true;
        status_infra_uiMng.ui_set_main_canvas(true);
    }
    public void ExiteUI_Main()
    {
        Clear();
        mainCanvas.enabled = false;
        status_infra_uiMng.ui_set_main_canvas(false);
    }
    public void RefreshUI_SkillMeterial()
    {
        for (int i = 0; i < skillMaterialicons.Count; i++)
        {
            if (i >= skillMaterialData.Count)     {
                skillMaterialicons[i]._this.SetActive(false);
                continue;
            }
            skillMaterialicons[i].Refresh(skillMaterialData[i].data.icon,
                dataHub.GetMaterialFrame(skillMaterialData[i].data.rare),
                skillMaterialData[i].count.ToString());
        }
        int height = skillMaterialData.Count / 6;
        if (!(skillMaterialData.Count % 6).Equals(0) && skillMaterialData.Count != 0) height++;
        skillMaterialpanel.sizeDelta = new Vector2(skillMaterialpanel.sizeDelta.x, height * 150 + 50);
    }
    void AddSkillMaterail(int level, int index=0)
    {
        string[] material = null; 
        string[] materailCount = null;
        if (index.Equals(0))
        {
            material = curOperator.skillmaterial[level - 1];
            materailCount = curOperator.skillmaterialCount[level - 1];
        }
        else 
        {
            material = curOperator.skillData[index - 1].data[level + 7].submaterial;
            materailCount = curOperator.skillData[index - 1].data[level + 7].subCount;
        }
        for (int i = 0; i < material.Length; i++)
        {
            Material_Op data = skillMaterialData.Find(delegate (Material_Op a) { return a.data.code == material[i]; });

            if (data != null)
                data.count += int.Parse(materailCount[i]);
            else
            {
                Material_Op newData = new Material_Op();
                newData.data = dataHub.GetMaterialData(material[i]);
                if (newData.data == null)
                    return;
                newData.count += int.Parse(materailCount[i]);
                skillMaterialData.Add(newData);
            }
        }
        skillMaterialData.Sort(delegate (Material_Op a, Material_Op b)
        {
            if (a.data.id > b.data.id) return 1;
            else if (a.data.id < b.data.id) return -1;
            return 0;
        });
    }
    void AddSkillMaterail(string[] material, string[] materailCount)
    {
        for (int i = 0; i < material.Length; i++)
        {
            Material_Op data = skillMaterialData.Find(delegate (Material_Op a) { return a.data.code == material[i]; });

            if (data != null)
                data.count += int.Parse(materailCount[i]);
            else
            {
                Material_Op newData = new Material_Op();
                newData.data = dataHub.GetMaterialData(material[i]);
                if (newData.data == null)
                    return;
                newData.count += int.Parse(materailCount[i]);
                skillMaterialData.Add(newData);
            }
        }
        skillMaterialData.Sort(delegate (Material_Op a, Material_Op b)
        {
            if (a.data.id > b.data.id) return 1;
            else if (a.data.id < b.data.id) return -1;
            return 0;
        });
    }
    void RemoveSkillMaterail(int level)
    {
        string[] material = curOperator.skillmaterial[level - 1];
        string[] materailCount = curOperator.skillmaterialCount[level - 1];
        for (int i = 0; i < material.Length; i++)
        {
            Material_Op data = skillMaterialData.Find(delegate (Material_Op a) { return a.data.code == material[i]; });

            if (data != null)
            {
                data.count -= int.Parse(materailCount[i]);
                if (data.count.Equals(0))
                    skillMaterialData.Remove(data);
            }
        }
    }
    void RemoveSkillMaterail(string[] material, string[] materailCount)
    {
        for (int i = 0; i < material.Length; i++)
        {
            Material_Op data = skillMaterialData.Find(delegate (Material_Op a) { return a.data.code == material[i]; });

            if (data != null)
            {
                data.count = data.count - int.Parse(materailCount[i]);
                if (data.count <= 0)
                    skillMaterialData.Remove(data);
            }
        }
    }
    public void TR_SetMenu(int index)
    {
        Clear();
        ClearMenuTxt();
        menuTxt[index].color = new Color(0.86f, 0.86f, 0.86f);
        if (index.Equals(0))
            ui_refresh_status_canvas();
        if (index.Equals(1))
            StartUI_Materail();
    }
    public void TR_SetSkillLevelMain(int index)
    {
        if (curOperator.skillmaterial.Count == 0)
            return;

        int where = selectLevel.IndexOf(index);
        if(where.Equals(-1))
        {
            defaultSkillLevelTxt[index -2].color = new Color(0.86f, 0.86f, 0.86f);
            selectLevel.Add(index);
            AddSkillMaterail(index);
        }
        else
        {
            defaultSkillLevelTxt[index - 2].color = new Color(0.4f, 0.4f, 0.4f);
            selectLevel.RemoveAt(where);
            RemoveSkillMaterail(index);
        }
        RefreshUI_SkillMeterial();
    }
    public void TR_SetSkillEliteLevel_1(int level)
    {
        SetSkillEliteLevel(0, level);
    }
    public void TR_SetSkillEliteLevel_2(int level)
    {
        SetSkillEliteLevel(1, level);
    }
    public void TR_SetSkillEliteLevel_3(int level)
    {
        SetSkillEliteLevel(2, level);
    }
    public void SetSkillEliteLevel(int index, int level)
    {
        TextMeshProUGUI[] levelTxt = null;
        if (index.Equals(0)) levelTxt = skillEliteLevelTxt1;
        else if (index.Equals(1)) levelTxt = skillEliteLevelTxt2;
        else if (index.Equals(2)) levelTxt = skillEliteLevelTxt3;

        int where = selectSkillLevelS[index].IndexOf(level);
        string[] material = curOperator.skillData[index].data[level + 7].submaterial;
        string[] materailCount = curOperator.skillData[index].data[level + 7].subCount;

        if (where.Equals(-1))
        {
            levelTxt[level].color = new Color(0.86f, 0.86f, 0.86f);
            selectSkillLevelS[index].Add(level);
            AddSkillMaterail(material, materailCount);
        }
        else
        {
            levelTxt[level].color = new Color(0.4f, 0.4f, 0.4f);
            selectSkillLevelS[index].RemoveAt(where);
            RemoveSkillMaterail(material, materailCount);
        }
        RefreshUI_SkillMeterial();
    }
    public bool activeSelf()
    {
        return mainCanvas.enabled;
    }
}
