using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UI;
using TMPro;
using CharactorDataSet;
using MaterialData;
public class OpSkillInfo_UIClass : MonoBehaviour
{
    public OpSkillInfo_UIClass This;

    [Header("- Manager Scripts")]
    public Operater_Data_Hub dataMng;

    [Header("- UI GameObject")]
    public GameObject subMat;

    [Header("- UI Transform")]
    public Transform subMatViewer;

    [Header("- UI Components")]
    public RawImage icon;
    public RawImage range;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI info;
    public Text startSp;
    public Text usdeSp;
    public TextMeshProUGUI unlock;
    public TextMeshProUGUI effecttime;

    [Header("- Skill Tag Object")]
    public GameObject spAtk;
    public GameObject spAuto;
    public GameObject spHit;
    public GameObject triggerUse;
    public GameObject triggerAuto;

    public Slider levelslider;

    //private GraphicRaycaster graphicRay;
    private int lv;
    private int curindex;          
    private OperaterClass parent;
    private List<Skill> skills;
    List<IconMT_1> materials;

    private string path;
    private Coroutine levelUI;
    public void Initialized()
    {
        lv = 1;
        curindex = 0;
        materials = new List<IconMT_1>();
        for (int i = 0; i < subMatViewer.childCount; i++) // 아이콘 리스트 생성
        {
            IconMT_1 set = new IconMT_1();
            set.Initiailize(subMatViewer.GetChild(i));
            materials.Add(set);
        }
        Clear();

        //graphicRay = this.transform.GetChild(1).GetComponent<GraphicRaycaster>();
        //graphicRay.enabled = false;
        This.gameObject.SetActive(false);
        subMat.SetActive(false);
    }
    public void Clear()
    {
        spAtk.SetActive(false);
        spAuto.SetActive(false);
        spHit.SetActive(false);

        triggerUse.SetActive(false);
        triggerAuto.SetActive(false);

        for (int i = 0; i < materials.Count; i++)
            materials[i].This.SetActive(false);
    }
    // UI Corutine ================================================================
    public void UIExit_MainCanvas()
    {
        if (levelUI != null)
            StopCoroutine(levelUI);

        //graphicRay.enabled = false;
    }
    public IEnumerator UIStart_MainCanvas(OperaterClass set)
    {
        if (set.skillData.Count.Equals(0))
        {
            This.gameObject.SetActive(false);
            subMat.SetActive(false);
            yield break;
        }

        if (This.gameObject.activeSelf.Equals(false))
            This.gameObject.SetActive(true);

        parent = set;
        skills = set.skillData;
        curindex = 0;
        levelslider.minValue = 1;
        levelslider.maxValue = skills[0].maxlv;
        levelslider.value = levelslider.minValue;
        lv = -1;
        path = Files.Use.DocumentsPath("Resource/OperatorIcon/" + parent.en_name);

        subMat.SetActive(true);
        //graphicRay.enabled = true;

        yield return StartCoroutine(UIRefresh_Index());
        levelUI = StartCoroutine(UIRefresh_Level());

        yield return new WaitForEndOfFrame();
    }
    IEnumerator UIRefresh_Index()
    {
        Skill data = skills[curindex];
        int index = curindex;

        skillName.text = data.name;

        Destroy(icon.texture);
        icon.texture = Files.Use.GetPNG(path, "S" + index.ToString() + ".png");

        if (data.rang_idx.Equals(0))
            range.gameObject.SetActive(false);
        else
        {
            Destroy(range.texture);
            range.texture = Files.Use.GetPNG(path, "S" + index.ToString() + "_R.png");
            range.gameObject.SetActive(true);
        }

        if (data.needLv != string.Empty)
        {
            if (data.needLv.Equals("E0"))
                unlock.text = string.Empty;
            else if (data.needLv.Equals("E1"))
                unlock.text = "1차 정예화 후 해금";
            else if (data.needLv.Equals("E2"))
                unlock.text = "2차 정예화 후 해금";
        }
        if (data != null)
        {
            spAtk.SetActive(false);
            spAuto.SetActive(false);
            spHit.SetActive(false);

            if (data.chargetype.Equals("ATK"))
                spAtk.SetActive(true);
            else if (data.chargetype.Equals("AUTO"))
                spAuto.SetActive(true);
            else if (data.chargetype.Equals("피격"))
                spHit.SetActive(true);

            triggerAuto.SetActive(false);
            triggerUse.SetActive(false);

            if (data.trigger.Equals("AUTO"))
                triggerAuto.SetActive(true);
            else if (data.trigger.Equals("MANU"))
                triggerUse.SetActive(true);
        }
        yield return new WaitForEndOfFrame();

        StartCoroutine(UIRefresh_Level());
    }
    IEnumerator UIRefresh_Level()
    {
        int curLv = ((int)levelslider.value) - 1;

        Skill data = skills[curindex];
        level.text = string.Format("Rank   {0}", curLv + 1);
        startSp.text = string.Format("{0}", data.data[curLv].startSp);
        usdeSp.text = string.Format("{0}", data.data[curLv].sp);

        if (data.data[curLv].efctime.Equals(string.Empty) || data.data[curLv].efctime.Equals("0"))
            effecttime.gameObject.SetActive(false);
        else
        {
            if (data.data[curLv].efctime.Equals("999"))
                effecttime.text = string.Format("{0} {1}", "지속시간", "무한");
            else
                effecttime.text = string.Format("{0} {1}{2}", "지속시간", data.data[curLv].efctime, "초");
            effecttime.gameObject.SetActive(true);
        }

        string tmpInfo = string.Empty;                 // 스킬 설명 출력부
        for (int i = 0; i < data.infom.Length; i++)
        {
            tmpInfo = tmpInfo + data.infom[i];

            if (i < data.infom.Length - 1)
            {
                string now_rdata = data.data[curLv].rdata[i];
                if (now_rdata.IndexOf('*').Equals(0))
                {
                    string now = now_rdata.Substring(1, now_rdata.Length - 1);
                    tmpInfo = string.Format("{0}<color=#F35126>{1}</color>", tmpInfo, now);
                }
                else
                    tmpInfo = string.Format("{0}<color=#34BBFF>{1}</color>", tmpInfo, now_rdata);
            }
        }
        info.text = tmpInfo;

        string[] material;
        string[] count;
        if (curLv + 1 <= 7)
        {
            material = parent.skillmaterial[curLv];
            count = parent.skillmaterialCount[curLv];
        }
        else
        {
            material = data.data[curLv].submaterial;
            count = data.data[curLv].subCount;
        }

        for (int i = 0; i < materials.Count; i++)
        {
            if (i >= material.Length)
            {
                materials[i].This.SetActive(false);
                continue;
            }
            EliteMaterial now = dataMng.GetMaterialData(material[i]);
            if (now == null)
            {
                materials[i].This.SetActive(false);
                continue;
            }
            materials[i].Refresh(now.icon, dataMng.GetMaterialFrame(now.rare), now.name, count[i]);
        }

        lv = curLv + 1;
        yield return new WaitForEndOfFrame();
    }
    // ============================================================================
    public void TR_SetSkillLevel()
    {
        if (levelUI != null)
            StopCoroutine(levelUI);
        levelUI = StartCoroutine(UIRefresh_Level());
    }
    public void TR_SetSkillIndex(bool set)
    {
        if (set)
        {
            if (curindex + 1 >= skills.Count)
                curindex = 0;
            else
                curindex++;
        }
        else
        {
            if (curindex - 1 < 0)
                curindex = skills.Count - 1;
            else
                curindex--;
        }
        StartCoroutine(UIRefresh_Index());
    }
}
