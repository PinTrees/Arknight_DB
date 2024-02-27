using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharactorDataSet;
using UI;
using TMPro;

public class StatusInfra_UIClass_Opinfo : MonoBehaviour
{
    Operater_Data_Hub dataHub;

    Canvas main_canvas;
    OperaterClass curData;
    InfraIcon infra_panel;

    Color[] colors;
    public void Initialized()
    {
        main_canvas = this.GetComponent<Canvas>();
        dataHub = GameObject.FindGameObjectWithTag("DataManager").GetComponent<Operater_Data_Hub>();
        infra_panel = new InfraIcon();
        infra_panel.Initialize(this.transform);

        colors = new Color[9];
        colors[0] = new Color(33 / 255f, 205 / 255f, 203 / 255f);
        colors[1] = new Color(0 / 255f, 87 / 255f, 82 / 255f);
        colors[2] = new Color(140 / 255f, 191 / 255f, 30 / 255f);
        colors[3] = new Color(255 / 255f, 216 / 255f, 0 / 255f);
        colors[4] = new Color(0 / 255f, 116 / 255f, 167 / 255f);
        colors[5] = new Color(227 / 255f, 235 / 255f, 0 / 255f);
        colors[6] = new Color(125 / 255f, 0 / 255f, 34 / 255f);
        colors[7] = new Color(86 / 255f, 86 / 255f, 86 / 255f);
        colors[8] = new Color(221 / 255f, 101 / 255f, 63 / 255f);
    }
    public void ui_refresh_main_panel(OperaterClass set)
    {
        curData = set;

        if (curData.infraData == null)
            return;

        Texture2D[] textureS = new Texture2D[curData.infraData[curData.infraData.Count - 1].id];
        for (int j = 0; j < curData.infraData.Count; j++)
        {
            if (curData.infraData[j].id.Equals(1))
                textureS[0] = dataHub.GetInfraSkillIcon(curData.infraData[j].target);
            else if (curData.infraData[j].id.Equals(2))
                textureS[1] = dataHub.GetInfraSkillIcon(curData.infraData[j].target);
            else if (curData.infraData[j].id.Equals(3))
                textureS[2] = dataHub.GetInfraSkillIcon(curData.infraData[j].target);
        }

        infra_panel.Refresh(curData);
        infra_panel.RefreshGraphic(curData.icon, textureS, Color.white, get_infra_type_by_color(curData.infraData[0].target));
        infra_panel.eliteicon.texture = dataHub.GetEliteIcon(curData.infraData[0].unlock);
        infra_panel._this.SetActive(true);

        main_canvas.enabled = true;
    }
    public void ui_set_main_canvas(bool set)
    {
        main_canvas.enabled = set;
    }
    public void tr_set_infraskill_level()
    {
        InfraSkill tmp = infra_panel.Upgrade(curData);
        infra_panel.eliteicon.texture = dataHub.GetEliteIcon(tmp.unlock);
    }
    public void tr_set_infraskill_index(Transform set)
    {
        int skillNum = set.GetSiblingIndex();
        int index = 0;
        for (int i = 0; i < curData.infraData.Count; i++)
        {
            if (curData.infraData[i].id.Equals(skillNum + 1))
            {
                index = i;
                break;
            }
        }
        InfraSkill tmp = infra_panel.RefreshSkill(curData, skillNum, get_infra_type_by_color(curData.infraData[index].target));
        infra_panel.eliteicon.texture = dataHub.GetEliteIcon(tmp.unlock);
    }
    Color get_infra_type_by_color(string type)
    {
        if (type.Equals("기숙사"))
            return colors[0];
        else if (type.Equals("통제실"))
            return colors[1];
        else if (type.Equals("발전소"))
            return colors[2];
        else if (type.Equals("제조소"))
            return colors[3];
        else if (type.Equals("무역소"))
            return colors[4];
        else if (type.Equals("가공소"))
            return colors[5];
        else if (type.Equals("훈련실"))
            return colors[6];
        else if (type.Equals("인사부"))
            return colors[7];
        else if (type.Equals("응접실"))
            return colors[8];

        return Color.gray;
    }
}
