using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using CharactorDataSet;
using UI;
using TMPro;
public class InfraUIManager_OP : MonoBehaviour
{
    [Header("- UI Transform")]
    public Operater_Data_Hub dataHub;
    public Canvas This;

    [Header("- UI Transform")]
    public Transform infraViewer;

    [Header("- UI Button Object")]
    public List<Button> rareBtn;
    public List<Button> classBtn;

    private Color[] colors;
    private List<OperaterClass> data;
    private List<OperaterClass> curData;
    private List<InfraIcon> infraIcons;

    List<string> cur_rare;
    List<string> cur_target;

    Image targetBtnMemory;
    public void Initialized()
    {
        cur_rare = new List<string>();
        cur_target = new List<string>();

        curData = dataHub.operatorData;
        data = dataHub.operatorData;
        infraIcons = new List<InfraIcon>();
        for (int i = 0; i < infraViewer.childCount; i++)
        {
            InfraIcon tmp = new InfraIcon();
            tmp._this = infraViewer.GetChild(i).gameObject;
            tmp.rectTransform = infraViewer.GetChild(i).GetComponent<RectTransform>();
            tmp.skillicons = new RawImage[tmp._this.transform.GetChild(1).childCount];

            tmp.icon = infraViewer.GetChild(i).GetChild(0).GetChild(0).GetComponent<RawImage>();
            tmp.rareicon = infraViewer.GetChild(i).GetChild(0).GetChild(1).GetComponent<Image>();
            tmp.operName = infraViewer.GetChild(i).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();

            tmp.eliteicon = infraViewer.GetChild(i).GetChild(3).GetComponentInChildren<RawImage>();
            tmp.nameImg = infraViewer.GetChild(i).GetChild(2).GetComponent<Image>();

            tmp.name = infraViewer.GetChild(i).GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            tmp.leveltxt = infraViewer.GetChild(i).GetChild(5).GetComponent<Text>();
            tmp.info = infraViewer.GetChild(i).GetChild(6).GetComponent<TextMeshProUGUI>();

            tmp.upBtn = infraViewer.GetChild(i).GetChild(4).gameObject;
            for (int j = 0; j < tmp.skillicons.Length; j++)
                tmp.skillicons[j] = tmp._this.transform.GetChild(1).GetChild(j).GetComponent<RawImage>();

            infraIcons.Add(tmp);
        }

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

        This.enabled = false;
        Refresh(data);
    }
    private void OnDestroy()
    {
        // UI =====================================================
        for (int i = 0; i < rareBtn.Count; i++)
            rareBtn.RemoveAt(0);
        for (int i = 0; i < classBtn.Count; i++)
            classBtn.RemoveAt(0);
        // Data =====================================================
        for (int i = 0; i < data.Count; i++)
        {
            Destroy(data[0].icon);
            data.RemoveAt(0);
        }

        infraViewer = null;
        Debug.Log("[Infra Hub] Scripts Destroy Memory");
    }
    public void Refresh(List<OperaterClass> data, string target="")
    {
        for (int i = 0; i < infraIcons.Count; i++)
        {
            if (i >= data.Count)
            {
                infraIcons[i]._this.SetActive(false);
                continue;
            }
            if (data[i].infraData == null)
            {
                infraIcons[i]._this.SetActive(false);
                continue;
            }
            if (data[i].infraData.Count < 1)
            {
                infraIcons[i]._this.SetActive(false);
                continue;
            }

            Color rare = dataHub.GetRareColor(data[i].rare);

            Texture2D[] textureS = new Texture2D[data[i].infraData[data[i].infraData.Count - 1].id];
            for (int j = 0; j < data[i].infraData.Count; j++)
            {
                if (data[i].infraData[j].id.Equals(1))
                    textureS[0] = dataHub.GetInfraSkillIcon(data[i].infraData[j].target);
                else if (data[i].infraData[j].id.Equals(2))
                    textureS[1] = dataHub.GetInfraSkillIcon(data[i].infraData[j].target);
                else if (data[i].infraData[j].id.Equals(3))
                    textureS[2] = dataHub.GetInfraSkillIcon(data[i].infraData[i].target);
            }

            infraIcons[i].Refresh(data[i]);
            infraIcons[i].RefreshGraphic(data[i].icon, textureS, rare, SetNameColor(data[i].infraData[0].target));
            infraIcons[i].eliteicon.texture = dataHub.GetEliteIcon(data[i].infraData[0].unlock);
            infraIcons[i]._this.SetActive(true);

            if(target != "")
            {
                tr_set_SkillIndex_by_Target(i, target);
            }
        }
    }
    public void TR_SetLevel(Transform target)
    {
        int index = target.GetSiblingIndex();
        InfraSkill tmp = infraIcons[index].Upgrade(curData[index]);
        infraIcons[index].eliteicon.texture = dataHub.GetEliteIcon(tmp.unlock);
    }
    public void TR_SetSkillIndex(Transform set)
    {
        int parent = set.parent.parent.GetSiblingIndex();
        int skillNum = set.GetSiblingIndex();
        int index = 0;
        for (int i = 0; i < curData[parent].infraData.Count; i++)
        {
            if (curData[parent].infraData[i].id.Equals(skillNum + 1))
            {
                index = i;
                break;
            }
        }
        InfraSkill tmp = infraIcons[parent].RefreshSkill(curData[parent], skillNum, SetNameColor(curData[parent].infraData[index].target));
        infraIcons[parent].eliteicon.texture = dataHub.GetEliteIcon(tmp.unlock);
    }
    public void tr_set_SkillIndex_by_Target(int iconindex, string target)
    {
        int parent = iconindex;
        int index = 0;
        for (int i = 0; i < curData[parent].infraData.Count; i++)
        {
            if (curData[parent].infraData[i].target.Equals(target))
            {
                index = i;
                break;
            }
        }
        InfraSkill tmp = infraIcons[parent].RefreshSkill(curData[parent], curData[parent].infraData[index].id - 1, SetNameColor(curData[parent].infraData[index].target));
        infraIcons[parent].eliteicon.texture = dataHub.GetEliteIcon(tmp.unlock);
    }

    public void TR_SelectRare(Image set)
    {
        string rare = (set.transform.GetSiblingIndex() + 1).ToString();

        int idx = cur_rare.FindIndex(delegate (string a) { return a == rare; });
        if (!idx.Equals(-1))
        {
            cur_rare.RemoveAt(idx);
            set.color = Color.gray;
        }
        else
        {
            cur_rare.Add(rare);
            set.color = Color.black;
        }
    }
    public void TR_SelectTarget(Image set)
    {
        int tmp = set.transform.GetSiblingIndex();
        string target = string.Empty;

        if (tmp.Equals(0))
            target = "통제실";
        else if (tmp.Equals(1))
            target = "발전소";
        else if (tmp.Equals(2))
            target = "제조소";
        else if (tmp.Equals(3))
            target = "무역소";
        else if (tmp.Equals(4))
            target = "가공소";
        else if (tmp.Equals(5))
            target = "훈련실";
        else if (tmp.Equals(6))
            target = "기숙사";
        else if (tmp.Equals(7))
            target = "인사부";
        else if (tmp.Equals(8))
            target = "응접실";
        else return;

        int idx = cur_target.FindIndex(delegate (string a) { return a == target; });
        if (!idx.Equals(-1))
        {
            cur_target.RemoveAt(idx);
            set.color = Color.gray;
        }
        else
        {
            cur_target.Add(target);
            set.color = Color.white;
        }
        curData = Search(cur_target);
        if (curData.Count.Equals(0))
            curData = data;

        Refresh(curData);
    }
    public void TR_SelectSingleTarget(Image set)
    {
        int tmp = set.transform.GetSiblingIndex();
        string target = string.Empty;

        if (tmp.Equals(0))
            target = "통제실";
        else if (tmp.Equals(1))
            target = "발전소";
        else if (tmp.Equals(2))
            target = "제조소";
        else if (tmp.Equals(3))
            target = "무역소";
        else if (tmp.Equals(4))
            target = "가공소";
        else if (tmp.Equals(5))
            target = "훈련실";
        else if (tmp.Equals(6))
            target = "기숙사";
        else if (tmp.Equals(7))
            target = "인사부";
        else if (tmp.Equals(8))
            target = "응접실";
        else return;

        int idx = cur_target.FindIndex(delegate (string a) { return a == target; });
        if (!idx.Equals(-1))
        {
            cur_target.Clear();
            set.color = Color.gray;
        }
        else
        {
            cur_target.Clear();
            cur_target.Add(target);
            set.color = Color.white;

            if (set != targetBtnMemory && targetBtnMemory != null)
                targetBtnMemory.color = Color.gray;
        }

        curData = Search(cur_target);
        if (curData.Count.Equals(0))
            curData = data;

        Refresh(curData, target);

        targetBtnMemory = set;
    }
    private Color SetNameColor(string type)
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
    public List<OperaterClass> Search(List<string> target)
    {
        List<OperaterClass> retData = new List<OperaterClass>();

        for(int i = 0; i < target.Count; i++)
        {
            for(int j = 0; j < data.Count; j++)
            {
                List<InfraSkill> curData = data[j].infraData;
                if (curData == null)
                    continue;

                for(int k = 0; k < curData.Count; k++)
                {
                    if(curData[k].target.Equals(target[i]))
                    {
                        OperaterClass tmp = retData.Find(delegate (OperaterClass a) { return a.name == data[j].name; });
                        if (tmp == null)
                            retData.Add(data[j]);
                        else
                            break;
                    }
                }
            }
        }
        return retData;
    }
}
