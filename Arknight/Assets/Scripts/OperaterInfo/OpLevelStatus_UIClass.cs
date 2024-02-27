using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharactorDataSet;
using UnityEngine.UI;
using UI;
using TMPro;
using MaterialData;
public class OpLevelStatus_UIClass : MonoBehaviour
{
    public Operater_Data_Hub dataMng;

    [Header("- UI GameObject")]
    public GameObject subMat_pnl;

    [Header("- UI TrustPanel")]
    public GameObject trustPanel;
    public TextMeshProUGUI trustInfo;

    [Header("- UI Transform")]
    public Transform subMatViewer;

    [Header("- UI Components")]
    public Image rangIcon;
    public Slider levelSlider;
    public Image eliteBtn;
    public TextMeshProUGUI level;
    public TextMeshProUGUI[] status;       //[0] Status, [1] unStatus

    List<IconMT_1> materials;
    //GraphicRaycaster graphic_ray;
    private Sprite[] elite;
    private int lv;
    private int curElite = 0;
    private OperaterClass curData;
    private Coroutine levelUI;
    public void Initialized()
    {
        elite = Resources.LoadAll<Sprite>("UI/Icon/Elite");

        materials = new List<IconMT_1>();
        for (int i = 0; i < subMatViewer.childCount; i++) // 아이콘 리스트 생성
        {
            IconMT_1 set = new IconMT_1();
            set.This = subMatViewer.transform.GetChild(i).gameObject;
            set.frame = subMatViewer.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            set.icon = subMatViewer.transform.GetChild(i).GetChild(1).GetComponent<RawImage>();
            set.name = subMatViewer.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
            set.count = subMatViewer.transform.GetChild(i).GetChild(4).GetComponent<TextMeshProUGUI>();
            materials.Add(set);
        }
        //graphic_ray = this.transform.GetChild(1).GetComponent<GraphicRaycaster>();
        //graphic_ray.enabled = false;
        SetClear();
        subMat_pnl.SetActive(false);
    }
    private void OnDestroy()
    {
        dataMng = null;
        for(int i = 0; i < status.Length; i++)
            Destroy(status[0]);
        for (int i = 0; i < materials.Count; i++)
        {
            Destroy(materials[0].frame);
            Destroy(materials[0].icon);
            Destroy(materials[0].name);
            Destroy(materials[0].count);
            Destroy(materials[0].This);
            materials.RemoveAt(0);
        }
        Destroy(subMat_pnl);
        Destroy(trustPanel);
        Debug.Log("[Level Data Scripts] Destroy Memory");
    }
    public void SetClear()
    {
        for (int i = 0; i < materials.Count; i++)
            materials[i].This.SetActive(false);
        trustPanel.SetActive(false);
    }
    public void UIExit_MainCanvas()
    {
        if (levelUI != null)
            StopCoroutine(levelUI);
        //graphic_ray.enabled = false;
    }
    // UI Corutine ========================================================
    public IEnumerator UIStart_MainCanvas()
    {
        subMat_pnl.SetActive(false);

        curElite = 0;
        levelSlider.minValue = 1;
        levelSlider.maxValue = curData.level_status[curElite].maxlevel;
        levelSlider.value = levelSlider.maxValue;
        lv = -1;

        eliteBtn.sprite = elite[curElite];
        rangIcon.sprite = dataMng.GetRangIcon(curData.eliteData[curElite].rang);

        //graphic_ray.enabled = true;
        levelUI = StartCoroutine(UIRefresh_LevelComponent());
        yield return new WaitForEndOfFrame();
    }
    private IEnumerator UIRefresh_LevelComponent()
    {
        while (true)
        {
            yield return new WaitForSeconds(.05f);
            if (lv.Equals((int)levelSlider.value - 1))
                continue;

            lv = (int)levelSlider.value - 1;
            Level_Info cur = curData.level_status[curElite];
            level.text = (lv + 1).ToString();
            status[0].text = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}", System.Environment.NewLine,
                Mathf.RoundToInt(cur.def.hp + (cur.add_hp * lv)).ToString(), Mathf.RoundToInt(cur.def.atk + (cur.add_atk * lv)).ToString(),
                Mathf.RoundToInt(cur.def.def + (cur.add_def * lv)).ToString(), Mathf.RoundToInt(cur.def.m_def + (cur.add_m_def * lv)).ToString());
            status[1].text = string.Format("{1}{0}{2}{0}{3}명{0}{4}초{0}", System.Environment.NewLine,
                cur.redeploy.ToString(), cur.cost.ToString(), cur.block.ToString(), cur.attack_time.ToString());
            
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator UIRefresh_Material()
    {
        if (curElite == 0)
        {
            subMat_pnl.SetActive(false);
            yield break;
        }
        int index = 0;
        for (int i = 0; i < curData.eliteData[curElite].sub_mat.Length; i += 2)
        {
            materials[index].This.SetActive(false);
            string[] now = curData.eliteData[curElite].sub_mat;
            if (now[i] == "용문폐")
            {
                materials[index].count.text = now[i + 1];
                materials[index++].This.SetActive(true);
            }
            else if (i < 4)
            {
                MaterialChip tg = dataMng.GetMaterialChip(now[i]);
                if (tg == null)
                    continue;
                materials[index++].Refresh(tg.icon, dataMng.GetMaterialFrame(tg.rare), tg.name, now[i + 1]);
            }
            else
            {
                EliteMaterial tg = dataMng.GetMaterialData(now[i]);
                if (tg == null)
                    materials[index++].Refresh(null, dataMng.GetMaterialFrame("1"), now[i], now[i + 1]);
                else
                    materials[index++].Refresh(tg.icon, dataMng.GetMaterialFrame(tg.rare), tg.name, now[i + 1]);
            }
        }
        subMat_pnl.SetActive(true);
    }
    // ============================================================================

    public void Tirgger_SetElite()
    {
        curElite++;
        if (curElite > curData.eliteData.Count - 1)
            curElite = 0;

        levelSlider.maxValue = curData.level_status[curElite].maxlevel;
        levelSlider.value = levelSlider.maxValue;
        lv = -1;

        eliteBtn.sprite = elite[curElite];
        rangIcon.sprite = dataMng.GetRangIcon(curData.eliteData[curElite].rang); // 사정거리 아이콘 출력

        StartCoroutine(UIRefresh_Material());
    }
    public void SetData(OperaterClass set)
    {
        curData = set;
    }
    public void TR_SetTrustPanel(bool set)
    {
        if (set)
        {
            string data = string.Empty;
            for (int i = 0; i < curData.trust.Length; i++)
            {
                if (!curData.trust[i].Equals("0") && i.Equals(0))
                    data = string.Format("최대 HP +{0}", curData.trust[i]);
                if (!curData.trust[i].Equals("0") && i.Equals(1))
                {
                    if (!data.Equals(string.Empty)) data += "     ";
                    data = string.Format("{0}공격력 +{1}", data, curData.trust[i]);
                }
                if (!curData.trust[i].Equals("0") && i.Equals(2))
                {
                    if (!data.Equals(string.Empty)) data += "     ";
                    data = string.Format("{0}방어력 +{1}", data, curData.trust[i]);
                }
                if (!curData.trust[i].Equals("0") && i.Equals(3))
                {
                    if (!data.Equals(string.Empty)) data += "     ";
                    data = string.Format("{0}마법 저항력 +{1}", data, curData.trust[i]);
                }
            }
            trustInfo.text = data;
        }
        if (!trustPanel.activeSelf.Equals(set))
            trustPanel.SetActive(set);
        else
            trustPanel.SetActive(!set);
    }
}
